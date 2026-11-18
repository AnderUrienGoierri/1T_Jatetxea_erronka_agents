package eus.goierri.jatetxea.admin.api;

import eus.goierri.jatetxea.admin.model.PlaterEskandalloa;
import org.junit.jupiter.api.Test;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ApiClientHealthTest {
    @Test
    void identifiesTheApiAndConnectedDatabase() throws Exception {
        ApiBezeroa.ApiHealth health = checkHealth(200,
                "{\"status\":\"Healthy\",\"database\":\"Connected\",\"engine\":\"MySQL\"}");

        assertTrue(health.apiConnected());
        assertTrue(health.databaseConnected());
        assertEquals("API martxan · DB konektatuta", health.message());
    }

    @Test
    void distinguishesAnAvailableApiFromAnUnavailableDatabase() throws Exception {
        ApiBezeroa.ApiHealth health = checkHealth(503,
                "{\"status\":\"Unhealthy\",\"database\":\"Disconnected\","
                        + "\"exceptionType\":\"MySqlException\"}");

        assertTrue(health.apiConnected());
        assertFalse(health.databaseConnected());
        assertEquals("API martxan · DB: MySqlException", health.message());
    }

    @Test
    void showsDatabaseDisconnectedWhenExceptionTypeIsUnavailable() throws Exception {
        ApiBezeroa.ApiHealth health = checkHealth(503,
                "{\"status\":\"Unhealthy\",\"database\":\"Disconnected\"}");

        assertTrue(health.apiConnected());
        assertFalse(health.databaseConnected());
        assertEquals("API martxan · DB deskonektatuta", health.message());
    }

    @Test
    void rejectsAnotherServiceRespondingAtTheConfiguredAddress() throws Exception {
        ApiBezeroa.ApiHealth health = checkHealth(200,
                "{\"status\":\"OK\",\"app\":\"Admin IT Zentrala\"}");

        assertFalse(health.apiConnected());
        assertFalse(health.databaseConnected());
    }

    @Test
    void extractsTypedDataFromAnApiResponse() throws Exception {
        String body = "{\"status\":\"success\",\"data\":{\"plateraId\":7,\"prezioa\":10.25}}";
        try (ServerSocket server = new ServerSocket(0);
             ExecutorService executor = Executors.newSingleThreadExecutor()) {
            Future<?> response = executor.submit(() -> respondOnce(server, 200, body));
            ApiBezeroa client = new ApiBezeroa("http://127.0.0.1:" + server.getLocalPort() + "/api/v1");

            PlaterEskandalloa recipe = client.getData("/platerak/7/eskandalloa", PlaterEskandalloa.class);

            assertEquals(7, recipe.plateraId);
            assertEquals(10.25, recipe.prezioa);
            response.get(5, TimeUnit.SECONDS);
        }
    }

    private ApiBezeroa.ApiHealth checkHealth(int status, String body) throws Exception {
        try (ServerSocket server = new ServerSocket(0);
             ExecutorService executor = Executors.newSingleThreadExecutor()) {
            Future<?> response = executor.submit(() -> respondOnce(server, status, body));
            ApiBezeroa client = new ApiBezeroa("http://127.0.0.1:" + server.getLocalPort() + "/api/v1");
            ApiBezeroa.ApiHealth health = client.checkHealth();
            response.get(5, TimeUnit.SECONDS);
            return health;
        }
    }

    private void respondOnce(ServerSocket server, int status, String body) {
        try (Socket socket = server.accept();
             BufferedReader reader = new BufferedReader(
                     new InputStreamReader(socket.getInputStream(), StandardCharsets.US_ASCII))) {
            while (true) {
                String line = reader.readLine();
                if (line == null || line.isEmpty()) {
                    break;
                }
            }
            byte[] content = body.getBytes(StandardCharsets.UTF_8);
            OutputStream output = socket.getOutputStream();
            output.write(("HTTP/1.1 " + status + " Test\r\n"
                    + "Content-Type: application/json; charset=utf-8\r\n"
                    + "Content-Length: " + content.length + "\r\n"
                    + "Connection: close\r\n\r\n").getBytes(StandardCharsets.US_ASCII));
            output.write(content);
            output.flush();
        } catch (Exception ex) {
            throw new IllegalStateException(ex);
        }
    }

}
