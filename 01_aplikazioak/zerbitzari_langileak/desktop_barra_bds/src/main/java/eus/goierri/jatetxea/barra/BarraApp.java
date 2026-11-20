package eus.goierri.jatetxea.barra;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;

public class BarraApp extends Application {

    @Override
    public void start(Stage stage) throws IOException {
        eus.goierri.jatetxea.core.api.ApiZerbitzua.setAppId("desktop_barra_bds");
        FXMLLoader loader = new FXMLLoader(BarraApp.class.getResource("login-view.fxml"));
        Parent root = loader.load();
        Scene scene = new Scene(root, 1180, 750);

        stage.setTitle("1T Jatetxea - Barrako BDS Pantaila (Bar Display System)");
        stage.setMinWidth(960);
        stage.setMinHeight(620);
        stage.setScene(scene);
        stage.show();
        stage.toFront();
        stage.requestFocus();
    }

    public static void main(String[] args) {
        launch(args);
    }
}
