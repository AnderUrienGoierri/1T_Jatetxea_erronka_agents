package eus.goierri.jatetxea.core.api;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.google.gson.reflect.TypeToken;
import eus.goierri.jatetxea.core.model.Eskaera;
import eus.goierri.jatetxea.core.model.Langilea;
import eus.goierri.jatetxea.core.model.Lehengaia;
import eus.goierri.jatetxea.core.model.Platera;
import eus.goierri.jatetxea.core.session.Sesioa;

import java.io.IOException;
import java.lang.reflect.Type;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.Collections;
import java.util.List;

public class ApiZerbitzua {
    private static final String BASE_URL = hasieratuBaseUrl();
    private static String appId = null;

    private final HttpClient bezeroa;
    private final Gson gson;

    public static String getBaseUrl() {
        return BASE_URL;
    }

    public static void setAppId(String id) {
        appId = id;
    }

    public static String getAppId() {
        if (appId != null && !appId.isBlank()) return appId;
        String prop = System.getProperty("app.id");
        if (prop != null && !prop.isBlank()) return prop;
        String cmd = System.getProperty("sun.java.command");
        if (cmd != null) {
            String lower = cmd.toLowerCase();
            if (lower.contains("sukaldea")) return "desktop_sukaldea_kds";
            if (lower.contains("barra")) return "desktop_barra_bds";
            if (lower.contains("biltegi")) return "desktop_biltegia_stock";
            if (lower.contains("admin")) return "desktop_admin_kudeaketa";
        }
        return "desktop_javafx";
    }

    private static String hasieratuBaseUrl() {
        String envUrl = System.getenv("API_BASE_URL");
        if (envUrl != null && !envUrl.isBlank()) {
            return envUrl.trim().replaceAll("/+$", "");
        }
        try (var is = ApiZerbitzua.class.getClassLoader().getResourceAsStream("api.properties")) {
            if (is != null) {
                var props = new java.util.Properties();
                props.load(is);
                String propUrl = props.getProperty("api.base.url");
                if (propUrl != null && !propUrl.isBlank()) {
                    return propUrl.trim().replaceAll("/+$", "");
                }
            }
        } catch (Exception ignored) {
        }
        return "http://localhost:5050/api/v1";
    }

    public ApiZerbitzua() {
        this.bezeroa = HttpClient.newBuilder()
                .followRedirects(HttpClient.Redirect.NORMAL)
                .connectTimeout(Duration.ofSeconds(4))
                .build();
        this.gson = new Gson();
    }

    public static class HealthResult {
        public final boolean apiMartxan;
        public final String mezua;

        public HealthResult(boolean apiMartxan, String mezua) {
            this.apiMartxan = apiMartxan;
            this.mezua = mezua;
        }
    }

    private HttpRequest.Builder sortuRequestBuilder(String subpath) {
        HttpRequest.Builder builder = HttpRequest.newBuilder()
                .uri(URI.create(BASE_URL + subpath))
                .header("X-App-Id", getAppId())
                .timeout(Duration.ofSeconds(5));

        if (Sesioa.getUnekoLangilea() != null) {
            builder.header("X-Langilea", Sesioa.getUnekoLangilea().getIzena());
            builder.header("X-Erabiltzailea", Sesioa.getUnekoLangilea().getErabiltzailea());
            builder.header("X-Rola", Sesioa.getUnekoLangilea().getRola());
        }
        return builder;
    }

    private <T> List<T> parseList(String jsonStr, Type type) {
        if (jsonStr == null || jsonStr.isBlank()) {
            return Collections.emptyList();
        }
        try {
            var element = com.google.gson.JsonParser.parseString(jsonStr);
            if (element.isJsonArray()) {
                return gson.fromJson(element, type);
            }
            if (element.isJsonObject()) {
                var obj = element.getAsJsonObject();
                if (obj.has("dataList") && obj.get("dataList").isJsonArray()) {
                    return gson.fromJson(obj.get("dataList"), type);
                }
                if (obj.has("data") && obj.get("data").isJsonArray()) {
                    return gson.fromJson(obj.get("data"), type);
                }
            }
        } catch (Exception e) {
            System.err.println("[ApiZerbitzua] Errorea JSON zerrenda parseatzean: " + e.getMessage());
        }
        return Collections.emptyList();
    }

    public HealthResult egiaztatuOsasuna() {
        try {
            HttpRequest request = sortuRequestBuilder("/osasuna")
                    .timeout(Duration.ofSeconds(3))
                    .GET()
                    .build();
            HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
            if (response.statusCode() == 200) {
                return new HealthResult(true, "APIa konektatuta (200 OK)");
            }

            // Erreserbako proba: /health endpoint orokorra
            HttpRequest fallbackRequest = sortuRequestBuilder("/health")
                    .timeout(Duration.ofSeconds(3))
                    .GET()
                    .build();
            HttpResponse<String> fallbackResponse = bezeroa.send(fallbackRequest, HttpResponse.BodyHandlers.ofString());
            if (fallbackResponse.statusCode() == 200) {
                return new HealthResult(true, "APIa konektatuta (200 OK)");
            }

            return new HealthResult(false, "APIak egoera kode okerra itzuli du: " + response.statusCode());
        } catch (Exception e) {
            return new HealthResult(false, "Ezin da APIarekin konektatu (" + e.getClass().getSimpleName() + ")");
        }
    }

    public List<Langilea> lortuLangileak() throws IOException, InterruptedException {
        HttpRequest request = sortuRequestBuilder("/langileak")
                .timeout(Duration.ofSeconds(5))
                .GET()
                .build();
        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() != 200) {
            return Collections.emptyList();
        }
        Type zerrendaMota = new TypeToken<List<Langilea>>() {}.getType();
        return parseList(response.body(), zerrendaMota);
    }

    public LoginErantzuna saioaHasi(String erabiltzailea, String pin) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("erabiltzailea", erabiltzailea);
        json.addProperty("pasahitza", pin);
        json.addProperty("pin", pin);

        HttpRequest request = sortuRequestBuilder("/langileak/login")
                .timeout(Duration.ofSeconds(5))
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(json.toString()))
                .build();

        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() == 200) {
            JsonObject root = gson.fromJson(response.body(), JsonObject.class);
            JsonObject res = root;
            if (root.has("data") && root.get("data").isJsonObject()) {
                res = root.getAsJsonObject("data");
            }
            boolean arrakasta = res.has("arrakasta") && res.get("arrakasta").getAsBoolean();
            String mezua = res.has("mezua") ? res.get("mezua").getAsString() : (root.has("message") ? root.get("message").getAsString() : "Ongi etorri");
            Langilea l = null;
            if (res.has("langilea") && !res.get("langilea").isJsonNull()) {
                l = gson.fromJson(res.get("langilea"), Langilea.class);
            }
            return new LoginErantzuna(arrakasta, mezua, l);
        } else if (response.statusCode() == 401) {
            return new LoginErantzuna(false, "PIN edo erabiltzaile okerra.", null);
        } else {
            return new LoginErantzuna(false, "Errorea saioa hastean (" + response.statusCode() + ")", null);
        }
    }

    public List<Eskaera> lortuEskaeraAktiboak() throws IOException, InterruptedException {
        HttpRequest request = sortuRequestBuilder("/eskaerak")
                .timeout(Duration.ofSeconds(5))
                .GET()
                .build();
        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() != 200) {
            return Collections.emptyList();
        }
        Type listType = new TypeToken<List<Eskaera>>() {}.getType();
        return parseList(response.body(), listType);
    }

    public boolean aldatuEskaeraEgoera(int eskaeraId, String egoeraBerria) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("egoera", egoeraBerria);

        HttpRequest request = sortuRequestBuilder("/eskaerak/" + eskaeraId + "/egoera")
                .timeout(Duration.ofSeconds(5))
                .header("Content-Type", "application/json")
                .PUT(HttpRequest.BodyPublishers.ofString(json.toString()))
                .build();

        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        return response.statusCode() == 200 || response.statusCode() == 204;
    }

    public boolean aldatuLerroEgoera(int lerroId, String egoeraBerria) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("egoera", egoeraBerria);

        HttpRequest request = sortuRequestBuilder("/eskaera-lerroak/" + lerroId + "/egoera")
                .timeout(Duration.ofSeconds(5))
                .header("Content-Type", "application/json")
                .PUT(HttpRequest.BodyPublishers.ofString(json.toString()))
                .build();

        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        return response.statusCode() == 200 || response.statusCode() == 204;
    }

    public List<Lehengaia> lortuLehengaiak() throws IOException, InterruptedException {
        HttpRequest request = sortuRequestBuilder("/lehengaiak")
                .timeout(Duration.ofSeconds(5))
                .GET()
                .build();
        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() != 200) {
            return Collections.emptyList();
        }
        Type listType = new TypeToken<List<Lehengaia>>() {}.getType();
        return parseList(response.body(), listType);
    }

    public boolean gehituStockSarrera(int lehengaiaId, double kantitatea, String albarana, String oharrak) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("lehengaiaId", lehengaiaId);
        json.addProperty("kantitatea", kantitatea);
        json.addProperty("albarana", albarana);
        json.addProperty("oharrak", oharrak);

        HttpRequest request = sortuRequestBuilder("/lehengaiak/" + lehengaiaId + "/stock")
                .timeout(Duration.ofSeconds(5))
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(json.toString()))
                .build();

        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        return response.statusCode() == 200 || response.statusCode() == 201;
    }

    public List<Platera> lortuPlaterak() throws IOException, InterruptedException {
        HttpRequest request = sortuRequestBuilder("/platerak")
                .timeout(Duration.ofSeconds(5))
                .GET()
                .build();
        HttpResponse<String> response = bezeroa.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() != 200) {
            return Collections.emptyList();
        }
        Type listType = new TypeToken<List<Platera>>() {}.getType();
        return parseList(response.body(), listType);
    }
}
