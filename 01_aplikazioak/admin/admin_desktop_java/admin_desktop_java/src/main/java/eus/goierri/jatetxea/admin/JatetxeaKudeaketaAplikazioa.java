package eus.goierri.jatetxea.admin;

import atlantafx.base.theme.PrimerLight;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.controller.KontrolagailuFabrika;
import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;

public final class JatetxeaKudeaketaAplikazioa extends Application {
    private ApiBezeroa apiClient;
    private Stage stage;

    @Override
    public void start(Stage primaryStage) throws IOException {
        stage = primaryStage;
        stage.setUserData(this);
        apiClient = ApiBezeroa.fromEnvironment();
        Application.setUserAgentStylesheet(new PrimerLight().getUserAgentStylesheet());
        stage.setTitle("T1 Jatetxea | Kudeaketa");
        stage.setMinWidth(1080);
        stage.setMinHeight(720);
        showLogin();
        stage.setFullScreen(true);
        stage.show();
    }

    public void showLogin() throws IOException {
        loadScene("/eus/goierri/jatetxea/admin/fxml/saio-hasiera.fxml", null, 1180, 760);
    }

    public void showWorkspace(KudeatzaileSaioa session) throws IOException {
        loadScene("/eus/goierri/jatetxea/admin/fxml/diseinu-nagusia.fxml", session, 1440, 900);
    }

    private void loadScene(String resource, KudeatzaileSaioa session, double width, double height) throws IOException {
        FXMLLoader loader = new FXMLLoader(JatetxeaKudeaketaAplikazioa.class.getResource(resource));
        loader.setControllerFactory(new KontrolagailuFabrika(apiClient, stage, session));
        Parent root = loader.load();
        Scene scene = new Scene(root, width, height);
        stage.setScene(scene);
        stage.setFullScreen(true);
    }
}
