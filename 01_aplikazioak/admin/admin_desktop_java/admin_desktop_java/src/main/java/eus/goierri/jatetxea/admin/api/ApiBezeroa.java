package eus.goierri.jatetxea.admin.api;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.google.gson.reflect.TypeToken;
import eus.goierri.jatetxea.admin.model.*;

import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Type;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.List;
import java.util.Map;
import java.util.Properties;

public final class ApiBezeroa {
    private final String baseUrl;
    private final HttpClient httpClient;
    private final Gson gson = new Gson();

    public ApiBezeroa(String baseUrl) {
        this.baseUrl = baseUrl.replaceAll("/+$", "");
        this.httpClient = HttpClient.newBuilder()
                .connectTimeout(Duration.ofSeconds(5))
                .build();
    }

    public static ApiBezeroa fromEnvironment() {
        String envUrl = System.getenv("API_BASE_URL");
        if (envUrl != null && !envUrl.isBlank()) {
            return new ApiBezeroa(envUrl.trim());
        }

        Properties properties = new Properties();
        try (InputStream input = ApiBezeroa.class.getResourceAsStream(
                "/eus/goierri/jatetxea/admin/api.properties")) {
            if (input != null) {
                properties.load(input);
            }
        } catch (IOException ex) {
            throw new IllegalStateException("Ezin izan da API konfigurazioa irakurri.", ex);
        }

        return new ApiBezeroa(properties.getProperty("api.base.url", "http://localhost:5050/api/v1"));
    }

    public String getBaseUrl() {
        return baseUrl;
    }

    public ApiHealth checkHealth() throws IOException, InterruptedException {
        String apiRoot = baseUrl.replaceFirst("(?i)/api/v1/?$", "");
        URI healthUri = URI.create(apiRoot + "/health");
        HttpRequest request = HttpRequest.newBuilder()
                .uri(healthUri)
                .timeout(Duration.ofSeconds(4))
                .header("Accept", "application/json")
                .header("X-App-Id", "kudeaketa_desktop_java")
                .header("X-App-Name", "T1 Jatetxea Kudeaketa Desktop")
                .GET()
                .build();

        HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() != 200 && response.statusCode() != 503) {
            return new ApiHealth(false, false,
                    "REST APIa ez dago helbide honetan (" + response.statusCode() + "): " + healthUri);
        }

        JsonObject health = parseObject(response.body());
        JsonElement database = health.get("database");
        if (database == null || !database.isJsonPrimitive()) {
            return new ApiHealth(false, false,
                    "Helbidea erantzuten ari da, baina ez da jatetxearen REST APIa: " + healthUri);
        }

        boolean databaseConnected = response.statusCode() == 200
                && "connected".equalsIgnoreCase(database.getAsString());
        String exceptionType = health.has("exceptionType") && health.get("exceptionType").isJsonPrimitive()
                ? health.get("exceptionType").getAsString() : null;
        String message = databaseConnected
                ? "API martxan · DB konektatuta"
                : exceptionType == null || exceptionType.isBlank()
                        ? "API martxan · DB deskonektatuta"
                        : "API martxan · DB: " + exceptionType;
        return new ApiHealth(true, databaseConnected, message);
    }

    public KudeatzaileErabiltzailea login(String username, String password) throws IOException, InterruptedException {
        JsonObject response = send("POST", "/langileak/login",
                Map.of("erabiltzailea", username, "pasahitza", password));
        JsonObject data = responseObject(response, "data");
        JsonObject employee = data.has("langilea") && data.get("langilea").isJsonObject()
                ? data.getAsJsonObject("langilea") : null;
        if (employee == null) {
            throw new IOException("APIaren login erantzunak ez du langilearen informaziorik.");
        }

        KudeatzaileErabiltzailea user = gson.fromJson(employee, KudeatzaileErabiltzailea.class);
        if (!user.isManager()) {
            throw new IOException("Kudeatzaile edo administratzaile baimena behar da.");
        }
        return user;
    }

    public <T> List<T> getList(String path, Class<T> itemType) throws IOException, InterruptedException {
        JsonObject response = send("GET", path, null);
        JsonElement list = response.get("dataList");
        if (list == null || !list.isJsonArray()) {
            throw new IOException("APIaren erantzunak ez du zerrenda-daturik: " + path);
        }
        Type listType = TypeToken.getParameterized(List.class, itemType).getType();
        return gson.fromJson(list, listType);
    }

    public <T> T getData(String path, Class<T> dataType) throws IOException, InterruptedException {
        JsonObject response = send("GET", path, null);
        JsonElement data = response.get("data");
        if (data == null || !data.isJsonObject()) {
            throw new IOException("APIaren erantzunak ez du objektu-daturik: " + path);
        }
        return gson.fromJson(data, dataType);
    }

    public void post(String path, Object body) throws IOException, InterruptedException {
        send("POST", path, body);
    }

    public void put(String path, Object body) throws IOException, InterruptedException {
        send("PUT", path, body);
    }

    public void delete(String path) throws IOException, InterruptedException {
        send("DELETE", path, null);
    }

    public List<Faktura> getFakturak(String mota) throws IOException, InterruptedException {
        String path = "/fakturak";
        if (mota != null && !mota.isBlank() && !"Guztiak".equalsIgnoreCase(mota)) {
            path += "?mota=" + mota;
        }
        return getList(path, Faktura.class);
    }

    public FinantzaLaburpena getFinantzaLaburpena() throws IOException, InterruptedException {
        return getData("/fakturak/laburpena", FinantzaLaburpena.class);
    }

    public void sortuFaktura(Object body) throws IOException, InterruptedException {
        post("/fakturak", body);
    }

    public void aldatuFakturaEgoera(int id, String egoera) throws IOException, InterruptedException {
        put("/fakturak/" + id + "/egoera", Map.of("egoera", egoera));
    }

    public ServiceStatus getServiceStatus() throws IOException, InterruptedException {
        JsonObject response = send("GET", "/system/service-status", null);
        JsonObject status = response.has("data") && response.get("data").isJsonObject()
                ? response.getAsJsonObject("data") : response;
        JsonElement activeElement = firstField(status, "serviceActive", "service_active", "martxan");
        JsonElement openTablesElement = firstField(status, "openTables", "open_tables", "mahaiIrekiak");
        if (activeElement == null || openTablesElement == null
                || !activeElement.isJsonPrimitive() || !openTablesElement.isJsonPrimitive()) {
            throw new IOException("Zerbitzu-egoeraren erantzunak serviceActive eta openTables eremuak behar ditu.");
        }
        return new ServiceStatus(activeElement.getAsBoolean(), openTablesElement.getAsInt());
    }

    private JsonObject send(String method, String path, Object body) throws IOException, InterruptedException {
        HttpRequest.Builder request = HttpRequest.newBuilder()
                .uri(URI.create(baseUrl + path))
                .timeout(Duration.ofSeconds(10))
                .header("Accept", "application/json")
                .header("X-App-Id", "kudeaketa_desktop_java")
                .header("X-App-Name", "T1 Jatetxea Kudeaketa Desktop");

        if (body != null) {
            request.header("Content-Type", "application/json");
        }
        switch (method) {
            case "GET" -> request.GET();
            case "POST" -> request.POST(HttpRequest.BodyPublishers.ofString(gson.toJson(body)));
            case "PUT" -> request.PUT(HttpRequest.BodyPublishers.ofString(gson.toJson(body)));
            case "DELETE" -> request.DELETE();
            default -> throw new IllegalArgumentException("HTTP metodoa ez da onartzen: " + method);
        }

        HttpResponse<String> response = httpClient.send(request.build(), HttpResponse.BodyHandlers.ofString());
        JsonObject json;
        try {
            json = parseObject(response.body());
        } catch (IOException ex) {
            if (response.statusCode() >= 200 && response.statusCode() < 300) {
                throw ex;
            }
            String detail = response.body().isBlank() ? response.statusCode() + " erantzuna" : response.body();
            throw new ApiSalbuespena(response.statusCode(), detail);
        }
        if (response.statusCode() < 200 || response.statusCode() >= 300) {
            String message = json.has("message") ? json.get("message").getAsString() : response.body();
            throw new ApiSalbuespena(response.statusCode(), message);
        }
        if (json.has("status") && "error".equalsIgnoreCase(json.get("status").getAsString())) {
            String message = json.has("message") ? json.get("message").getAsString() : "API errorea.";
            throw new IOException(message);
        }
        return json;
    }

    private JsonObject parseObject(String body) throws IOException {
        try {
            JsonElement parsed = JsonParser.parseString(body);
            if (!parsed.isJsonObject()) {
                throw new IOException("APIaren erantzuna ez da JSON objektu bat.");
            }
            return parsed.getAsJsonObject();
        } catch (RuntimeException ex) {
            throw new IOException("APIak JSON erantzun baliogabea bidali du.", ex);
        }
    }

    private JsonObject responseObject(JsonObject response, String name) throws IOException {
        JsonElement value = response.get(name);
        if (value == null || !value.isJsonObject()) {
            throw new IOException("APIaren login erantzunak ez du '" + name + "' objekturik.");
        }
        return value.getAsJsonObject();
    }

    private JsonElement firstField(JsonObject object, String... names) {
        for (String name : names) {
            if (object.has(name)) {
                return object.get(name);
            }
        }
        return null;
    }

    public record ServiceStatus(boolean active, int openTables) {
    }

    public record ApiHealth(boolean apiConnected, boolean databaseConnected, String message) {
    }
}
