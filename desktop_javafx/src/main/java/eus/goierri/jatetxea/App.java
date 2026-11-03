package eus.goierri.jatetxea;

import javafx.application.Application;
import javafx.scene.Scene;
import javafx.scene.control.Label;
import javafx.scene.layout.StackPane;
import javafx.stage.Stage;

/**
 * Sukaldeko KDS eta Biltegiko JavaFX Interfazea (MVC)
 */
public class App extends Application {

    @Override
    public void start(Stage stage) {
        var label = new Label("Jatetxea - Sukaldeko KDS & Biltegi Kudeaketa (JavaFX)");
        var scene = new Scene(new StackPane(label), 900, 600);
        stage.setTitle("Jatetxea - KDS & Stock");
        stage.setScene(scene);
        stage.show();
    }

    public static void main(String[] args) {
        launch();
    }
}
