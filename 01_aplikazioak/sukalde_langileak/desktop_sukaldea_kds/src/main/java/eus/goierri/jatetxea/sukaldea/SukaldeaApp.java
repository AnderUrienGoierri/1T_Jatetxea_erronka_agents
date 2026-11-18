package eus.goierri.jatetxea.sukaldea;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;

public class SukaldeaApp extends Application {

    @Override
    public void start(Stage stage) throws IOException {
        FXMLLoader loader = new FXMLLoader(SukaldeaApp.class.getResource("login-view.fxml"));
        Parent root = loader.load();
        Scene scene = new Scene(root, 1180, 750);

        stage.setTitle("1T Jatetxea - Sukaldeko KDS Pantaila (Kitchen Display System)");
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
