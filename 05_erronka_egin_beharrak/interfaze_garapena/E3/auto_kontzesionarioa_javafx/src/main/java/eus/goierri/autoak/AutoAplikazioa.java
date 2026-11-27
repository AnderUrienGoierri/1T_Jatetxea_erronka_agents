package eus.goierri.autoak;

import eus.goierri.autoak.controller.AutoPertsonalizazioaKontrolagailua;
import eus.goierri.autoak.controller.HasierakoIragazketaKontrolagailua;
import eus.goierri.autoak.model.AplikazioEgoera;
import eus.goierri.autoak.model.AutoKatalogoa;
import eus.goierri.autoak.model.Autoa;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;

/**
 * Goierri Motors — E3 Ariketa Praktikoa (JavaFX 21 + Scene Builder).
 * Bi interfazeak (1. Hasierako iragazketa <-> 2. Autoaren pertsonalizazioa)
 * egoera galdu gabe kudeatzen dituen aplikazio nagusia.
 */
public final class AutoAplikazioa extends Application {
    private Stage stage;
    private AplikazioEgoera egoera;

    @Override
    public void start(Stage primaryStage) throws IOException {
        this.stage = primaryStage;
        this.egoera = new AplikazioEgoera(AutoKatalogoa.sortuKatalogoa());

        stage.setTitle("Goierri Motors · Auto Aholkulari Interaktiboa (E3)");
        stage.setMinWidth(1000);
        stage.setMinHeight(680);

        erakutsiHasierakoIragazketa();
        stage.show();
        Platform.runLater(() -> stage.setMaximized(true));
    }

    public void erakutsiHasierakoIragazketa() throws IOException {
        kargatuEszena("/eus/goierri/autoak/fxml/hasierako-iragazketa.fxml");
    }

    public void erakutsiPertsonalizazioa(Autoa autoa) throws IOException {
        egoera.setHautatutakoAutoa(autoa);
        kargatuEszena("/eus/goierri/autoak/fxml/auto-pertsonalizazioa.fxml");
    }

    private void kargatuEszena(String fxmlPath) throws IOException {
        FXMLLoader loader = new FXMLLoader(AutoAplikazioa.class.getResource(fxmlPath));
        loader.setControllerFactory(type -> {
            if (type == HasierakoIragazketaKontrolagailua.class) {
                return new HasierakoIragazketaKontrolagailua(this, egoera);
            }
            if (type == AutoPertsonalizazioaKontrolagailua.class) {
                return new AutoPertsonalizazioaKontrolagailua(this, egoera);
            }
            try {
                return type.getDeclaredConstructor().newInstance();
            } catch (Exception ex) {
                throw new IllegalStateException("Ezin izan da kontroladorea sortu: " + type.getName(), ex);
            }
        });

        Parent root = loader.load();
        if (stage.getScene() == null) {
            stage.setScene(new Scene(root, 1360, 860));
        } else {
            stage.getScene().setRoot(root);
        }
    }
}
