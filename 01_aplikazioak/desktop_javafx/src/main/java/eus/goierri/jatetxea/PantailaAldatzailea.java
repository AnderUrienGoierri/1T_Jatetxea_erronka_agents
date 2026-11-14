package eus.goierri.jatetxea;

import javafx.fxml.FXMLLoader;
import javafx.scene.Node;
import javafx.scene.Parent;
import java.io.IOException;

/**
 * Uneko leihoaren (Scene) edukia beste FXML batekin ordezkatzen du.
 * Erronkako MVC egitura jarraituz.
 */
public class PantailaAldatzailea {
    public static void aldatu(Node edozeinNodo, String fxmlIzena) throws IOException {
        FXMLLoader loader = new FXMLLoader(App.class.getResource(fxmlIzena));
        Parent root = loader.load();
        edozeinNodo.getScene().setRoot(root);
    }
}
