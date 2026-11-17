package eus.goierri.jatetxea.api;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.google.gson.reflect.TypeToken;
import eus.goierri.jatetxea.model.Eskaera;
import eus.goierri.jatetxea.model.Lehengaia;
import eus.goierri.jatetxea.model.Platera;

import java.io.IOException;
import java.lang.reflect.Type;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Jatetxearen REST APIarekin (backend_api) komunikatzeko zerbitzua.
 * Sukaldeko KDS, Barrako BDS eta Biltegiko stock kudeaketarako deialdiak eskaintzen ditu.
 */
public class ApiZerbitzua {
    private static final String BASE_URL = hasieratuBaseUrl();
    private final HttpClient bezeroa;
    private final Gson gson;

    public static String getBaseUrl() {
        return BASE_URL;
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
                .connectTimeout(Duration.ofSeconds(4))
                .build();
        this.gson = new Gson();
    }

    private String getHttpJson(String bidea) throws IOException, InterruptedException {
        HttpRequest eskaera = HttpRequest.newBuilder()
                .uri(URI.create(BASE_URL + bidea))
                .timeout(Duration.ofSeconds(5))
                .header("Accept", "application/json")
                .header("X-App-Id", "javafx_kds")
                .header("X-App-Name", "JavaFX Sukaldea KDS")
                .header("User-Agent", "Jatetxea-JavaFX-KDS/1.0 (Java 21)")
                .GET()
                .build();

        HttpResponse<String> erantzuna = bezeroa.send(eskaera, HttpResponse.BodyHandlers.ofString());

        if (erantzuna.statusCode() < 200 || erantzuna.statusCode() >= 300) {
            throw new IOException("APIak errore kodea itzuli du: " + erantzuna.statusCode());
        }

        return erantzuna.body();
    }

    private String sendJson(String bidea, String metodoa, String jsonGorputza) throws IOException, InterruptedException {
        HttpRequest.Builder builder = HttpRequest.newBuilder()
                .uri(URI.create(BASE_URL + bidea))
                .timeout(Duration.ofSeconds(5))
                .header("Content-Type", "application/json")
                .header("Accept", "application/json")
                .header("X-App-Id", "javafx_kds")
                .header("X-App-Name", "JavaFX Sukaldea KDS")
                .header("User-Agent", "Jatetxea-JavaFX-KDS/1.0 (Java 21)");

        if ("POST".equalsIgnoreCase(metodoa)) {
            builder.POST(HttpRequest.BodyPublishers.ofString(jsonGorputza));
        } else if ("PUT".equalsIgnoreCase(metodoa)) {
            builder.PUT(HttpRequest.BodyPublishers.ofString(jsonGorputza));
        }

        HttpResponse<String> erantzuna = bezeroa.send(builder.build(), HttpResponse.BodyHandlers.ofString());

        if (erantzuna.statusCode() < 200 || erantzuna.statusCode() >= 300) {
            throw new IOException("APIak errore kodea itzuli du: " + erantzuna.statusCode() + " -> " + erantzuna.body());
        }

        return erantzuna.body();
    }

    public static class HealthResult {
        public final boolean apiMartxan;
        public final boolean dbMartxan;
        public final String mezua;

        public HealthResult(boolean apiMartxan, boolean dbMartxan, String mezua) {
            this.apiMartxan = apiMartxan;
            this.dbMartxan = dbMartxan;
            this.mezua = mezua;
        }
    }

    public HealthResult egiaztatuOsasuna() {
        String[] urls = new String[] {
            BASE_URL.replaceAll("/api/v1/?$", "/health"),
            "http://localhost:5050/health"
        };

        for (String u : urls) {
            try {
                HttpRequest req = HttpRequest.newBuilder()
                        .uri(URI.create(u))
                        .timeout(Duration.ofSeconds(2))
                        .header("Accept", "application/json")
                        .header("X-App-Id", "javafx_kds")
                        .header("X-App-Name", "JavaFX Sukaldea KDS")
                        .header("User-Agent", "Jatetxea-JavaFX-KDS/1.0 (Java 21)")
                        .GET()
                        .build();
                HttpResponse<String> res = bezeroa.send(req, HttpResponse.BodyHandlers.ofString());
                if (res.statusCode() == 200) {
                    return new HealthResult(true, true, "API eta Datu-basea konektatuta daude.");
                } else if (res.statusCode() == 503) {
                    return new HealthResult(true, false, "APIra konektatuta dago, baina MySQL datu-basearekin ezin izan da konexioa lortu.");
                }
            } catch (Exception ignored) { }
        }

        return new HealthResult(false, false, "Ezin izan da REST APIarekin konektatu.");
    }

    public boolean isApiMartxan() {
        return egiaztatuOsasuna().apiMartxan;
    }

    // ==========================================
    // 1. SUKALDEKO METODOAK (KDS)
    // ==========================================

    public List<Eskaera> lortuEskaeraAktiboak() throws IOException, InterruptedException {
        String json = getHttpJson("/eskaerak");
        Type type = new TypeToken<ApiErantzuna<Eskaera>>() {}.getType();
        ApiErantzuna<Eskaera> erantzuna = gson.fromJson(json, type);

        List<Eskaera> zerrenda = erantzuna != null && erantzuna.getDataList() != null 
                ? erantzuna.getDataList() 
                : Collections.emptyList();

        List<Eskaera> aktiboak = new ArrayList<>();
        for (Eskaera e : zerrenda) {
            if (!"Kobratua".equalsIgnoreCase(e.getEgoera()) && !"Ezeztatua".equalsIgnoreCase(e.getEgoera())) {
                try {
                    Eskaera xehea = lortuEskaeraXehetasunak(e.getId());
                    aktiboak.add(xehea != null ? xehea : e);
                } catch (Exception ex) {
                    aktiboak.add(e);
                }
            }
        }
        return aktiboak;
    }

    public Eskaera lortuEskaeraXehetasunak(int eskaeraId) throws IOException, InterruptedException {
        String json = getHttpJson("/eskaerak/" + eskaeraId);
        Type type = new TypeToken<ApiErantzuna<Eskaera>>() {}.getType();
        ApiErantzuna<Eskaera> erantzuna = gson.fromJson(json, type);
        return erantzuna != null ? erantzuna.getData() : null;
    }

    public boolean eguneratuLerroEgoera(int lerroId, String egoeraBerria) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("egoera", egoeraBerria);
        sendJson("/eskaerak/lerroak/" + lerroId + "/egoera", "PUT", json.toString());
        return true;
    }

    public boolean eguneratuEskaeraEgoera(int eskaeraId, String egoeraBerria) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("egoera", egoeraBerria);
        sendJson("/eskaerak/" + eskaeraId + "/egoera", "PUT", json.toString());
        return true;
    }

    // ==========================================
    // 2. BILTEGIKO METODOAK (STOCK)
    // ==========================================

    public List<Lehengaia> lortuLehengaiak() throws IOException, InterruptedException {
        String json = getHttpJson("/lehengaiak");
        Type type = new TypeToken<ApiErantzuna<Lehengaia>>() {}.getType();
        ApiErantzuna<Lehengaia> erantzuna = gson.fromJson(json, type);
        return erantzuna != null && erantzuna.getDataList() != null ? erantzuna.getDataList() : Collections.emptyList();
    }

    public List<Lehengaia> lortuStockAlertak() throws IOException, InterruptedException {
        String json = getHttpJson("/lehengaiak/alertak");
        Type type = new TypeToken<ApiErantzuna<Lehengaia>>() {}.getType();
        ApiErantzuna<Lehengaia> erantzuna = gson.fromJson(json, type);
        return erantzuna != null && erantzuna.getDataList() != null ? erantzuna.getDataList() : Collections.emptyList();
    }

    public boolean erregistratuStockSarrera(int lehengaiaId, double kantitatea, String faktura, String oharrak) throws IOException, InterruptedException {
        JsonObject json = new JsonObject();
        json.addProperty("lehengaiaId", lehengaiaId);
        json.addProperty("kantitatea", kantitatea);
        json.addProperty("fakturaZenbakia", faktura != null ? faktura : "ALBARAN-" + System.currentTimeMillis());
        json.addProperty("oharrak", oharrak != null ? oharrak : "Sarrera biltegitik");

        sendJson("/lehengaiak/sarrera", "POST", json.toString());
        return true;
    }

    public List<Platera> lortuPlaterak() throws IOException, InterruptedException {
        String json = getHttpJson("/platerak");
        Type type = new TypeToken<ApiErantzuna<Platera>>() {}.getType();
        ApiErantzuna<Platera> erantzuna = gson.fromJson(json, type);
        return erantzuna != null && erantzuna.getDataList() != null ? erantzuna.getDataList() : Collections.emptyList();
    }
}
