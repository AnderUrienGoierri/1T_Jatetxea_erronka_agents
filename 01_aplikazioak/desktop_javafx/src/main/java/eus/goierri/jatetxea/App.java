package eus.goierri.jatetxea;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;

/**
 * Sukaldeko KDS, Barrako BDS eta Biltegiko JavaFX Interfaze Nagusia (MVC).
 */
public class App extends Application {

    @Override
    public void start(Stage stage) throws IOException {
        FXMLLoader loader = new FXMLLoader(App.class.getResource("login-view.fxml"));
        Parent root = loader.load();
        Scene scene = new Scene(root, 1180, 750);

        stage.setTitle("T1 Jatetxea - Saioa Hasi (Sukaldea, Barra eta Biltegia)");
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
