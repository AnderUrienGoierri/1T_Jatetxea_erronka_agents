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
 * Sukaldeko KDS eta Biltegiko stock kudeaketarako deialdiak eskaintzen ditu.
 */
public class ApiZerbitzua {
    private static final String BASE_URL = "http://localhost:5000/api/v1";
    private final HttpClient bezeroa;
    private final Gson gson;

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
                .header("Accept", "application/json");

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

    public boolean isApiMartxan() {
        try {
            HttpRequest req = HttpRequest.newBuilder()
                    .uri(URI.create("http://localhost:5000/health"))
                    .timeout(Duration.ofSeconds(2))
                    .GET()
                    .build();
            HttpResponse<String> res = bezeroa.send(req, HttpResponse.BodyHandlers.ofString());
            return res.statusCode() == 200;
        } catch (Exception e) {
            return false;
        }
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
