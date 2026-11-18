package eus.goierri.jatetxea.core.controller;

import eus.goierri.jatetxea.core.model.Langilea;
import eus.goierri.jatetxea.core.session.Sesioa;
import eus.goierri.jatetxea.core.util.PantailaAldatzailea;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.Node;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.Label;

import java.io.IOException;

public abstract class BaseController {

    @FXML protected Label lblErabiltzaileaInfo;

    protected abstract Class<?> getAppResourceClass();

    protected void konfiguratuSaioaEtaRola() {
        if (Sesioa.isSaioaHasita()) {
            Langilea unekoa = Sesioa.getUnekoLangilea();
            if (lblErabiltzaileaInfo != null) {
                lblErabiltzaileaInfo.setText(unekoa.getIzenOsoa() + " (" + unekoa.getRola() + ")");
            }
        }
    }

    @FXML
    protected void onSaioaItxiClick(ActionEvent event) {
        garbituBaliabideak();
        Sesioa.itxiSaioa();
        aldatuPantaila(event, "login-view.fxml", "Saio-hasiera");
    }

    protected void garbituBaliabideak() {
    }

    protected void aldatuPantaila(ActionEvent event, String fxmlIzena, String izenburua) {
        try {
            Node iturria = (Node) event.getSource();
            PantailaAldatzailea.aldatu(iturria, fxmlIzena, getAppResourceClass());
        } catch (IOException e) {
            erakutsiAlerta("Errorea", "Ezin izan da pantaila kargatu: " + e.getMessage());
        }
    }

    protected void erakutsiAlerta(String izenburua, String mezua) {
        Alert alerta = new Alert(Alert.AlertType.INFORMATION);
        alerta.setTitle(izenburua);
        alerta.setHeaderText(null);
        alerta.setContentText(mezua);
        alerta.showAndWait();
    }
}
