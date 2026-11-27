package eus.goierri.jatetxea.admin.controller;

import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.net.URL;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.FutureTask;
import java.util.concurrent.TimeUnit;

import static org.junit.jupiter.api.Assertions.assertNotNull;

class ManagerScreensFxmlTest {
    @BeforeAll
    static void startJavaFx() throws InterruptedException {
        CountDownLatch started = new CountDownLatch(1);
        Platform.startup(started::countDown);
        if (!started.await(10, TimeUnit.SECONDS)) {
            throw new IllegalStateException("JavaFX plataforma ez da garaiz abiatu.");
        }
    }

    @AfterAll
    static void stopJavaFx() {
        Platform.exit();
    }

    @Test
    void managerScreensLoadWithTheirControllers() throws Exception {
        for (String screen : new String[] {
                "saio-hasiera.fxml", "diseinu-nagusia.fxml", "laburpena.fxml",
                "langileak.fxml", "menua.fxml", "osagaiak.fxml", "mahaiak.fxml", "fakturak.fxml", "txata.fxml"
        }) {
            URL resource = getClass().getResource(
                    "/eus/goierri/jatetxea/admin/fxml/" + screen);
            assertNotNull(resource, screen + " baliabidea aurkitu behar da");

            FutureTask<Parent> loadTask = new FutureTask<>(() ->
                    new FXMLLoader(resource).load());
            Platform.runLater(loadTask);
            assertNotNull(loadTask.get(10, TimeUnit.SECONDS), screen + " kargatu behar da");
        }
    }
}
