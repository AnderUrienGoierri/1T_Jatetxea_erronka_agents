package eus.goierri.jatetxea.core.util;

import javafx.fxml.FXMLLoader;
import javafx.scene.Node;
import javafx.scene.Parent;
import javafx.scene.Scene;

import java.io.IOException;
import java.net.URL;

public class PantailaAldatzailea {
    public static void aldatu(Node edozeinNodo, String fxmlIzena, Class<?> resourceClass) throws IOException {
        URL url = resourceClass.getResource(fxmlIzena);
        if (url == null) {
            String path = "/" + resourceClass.getPackageName().replace('.', '/') + "/" + fxmlIzena;
            url = resourceClass.getResource(path);
        }
        if (url == null && resourceClass.getClassLoader() != null) {
            String relPath = resourceClass.getPackageName().replace('.', '/') + "/" + fxmlIzena;
            url = resourceClass.getClassLoader().getResource(relPath);
        }
        if (url == null && resourceClass.getClassLoader() != null) {
            url = resourceClass.getClassLoader().getResource(fxmlIzena);
        }
        if (url == null && Thread.currentThread().getContextClassLoader() != null) {
            String relPath = resourceClass.getPackageName().replace('.', '/') + "/" + fxmlIzena;
            url = Thread.currentThread().getContextClassLoader().getResource(relPath);
        }
        if (url == null) {
            throw new IOException("Ez da aurkitu FXML fitxategia: " + fxmlIzena + " (" + resourceClass.getName() + ")");
        }

        Scene scene = edozeinNodo.getScene();
        if (scene == null) {
            throw new IllegalStateException("Nodoak ez du Scenerik esleituta.");
        }

        FXMLLoader loader = new FXMLLoader(url);
        Parent root = loader.load();
        scene.setRoot(root);
    }
}
