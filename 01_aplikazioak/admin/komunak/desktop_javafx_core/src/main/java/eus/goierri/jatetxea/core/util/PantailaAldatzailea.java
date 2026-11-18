package eus.goierri.jatetxea.core.util;

import javafx.fxml.FXMLLoader;
import javafx.scene.Node;
import javafx.scene.Parent;

import java.io.IOException;

public class PantailaAldatzailea {
    public static void aldatu(Node edozeinNodo, String fxmlIzena, Class<?> resourceClass) throws IOException {
        FXMLLoader loader = new FXMLLoader(resourceClass.getResource(fxmlIzena));
        Parent root = loader.load();
        edozeinNodo.getScene().setRoot(root);
    }
}
