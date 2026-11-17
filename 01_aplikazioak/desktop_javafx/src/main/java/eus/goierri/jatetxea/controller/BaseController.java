package eus.goierri.jatetxea.controller;

import eus.goierri.jatetxea.PantailaAldatzailea;
import eus.goierri.jatetxea.Sesioa;
import eus.goierri.jatetxea.model.Langilea;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.Node;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.Label;

import java.io.IOException;

/**
 * Kontroladore guztien oinarrizko klase abstraktua (MVC ereduaren kontroladore komuna).
 * Goiko nabigazio-barra, rol baimenak eta jakinarazpen/alerta metodoak zentralizatzen ditu.
 */
public abstract class BaseController {

    @FXML protected Button btnSukaldeaNav;
    @FXML protected Button btnBarraNav;
    @FXML protected Button btnBiltegiaNav;
    @FXML protected Label lblErabiltzaileaInfo;

    /**
     * Uneko langilearen saioa eta rola kargatu, eta nabigazio botoiak egokitu.
     */
    protected void konfiguratuSaioaEtaRola() {
        if (Sesioa.isSaioaHasita()) {
            Langilea unekoa = Sesioa.getUnekoLangilea();
            if (lblErabiltzaileaInfo != null) {
                lblErabiltzaileaInfo.setText(unekoa.getIzenOsoa() + " (" + unekoa.getRola() + ")");
            }
            if (!Sesioa.isAdminEdoKudeatzailea()) {
                if (btnSukaldeaNav != null) {
                    btnSukaldeaNav.setVisible(Sesioa.isSukaldaria());
                    btnSukaldeaNav.setManaged(Sesioa.isSukaldaria());
                }
                if (btnBarraNav != null) {
                    btnBarraNav.setVisible(Sesioa.isBarrakoa());
                    btnBarraNav.setManaged(Sesioa.isBarrakoa());
                }
                if (btnBiltegiaNav != null) {
                    btnBiltegiaNav.setVisible(Sesioa.isBiltegikoa());
                    btnBiltegiaNav.setManaged(Sesioa.isBiltegikoa());
                }
            }
        }
    }

    @FXML
    protected void onSukaldeaClick(ActionEvent event) {
        if (!Sesioa.isAdminEdoKudeatzailea() && !Sesioa.isSukaldaria()) {
            erakutsiAlerta("Baimenik ez", "Ez daukazu baimenik Sukaldea modulura sartzeko.");
            return;
        }
        garbituBaliabideak();
        aldatuPantaila(event, "sukaldea-view.fxml", "Sukaldea");
    }

    @FXML
    protected void onBarraClick(ActionEvent event) {
        if (!Sesioa.isAdminEdoKudeatzailea() && !Sesioa.isBarrakoa()) {
            erakutsiAlerta("Baimenik ez", "Ez daukazu baimenik Barra modulura sartzeko.");
            return;
        }
        garbituBaliabideak();
        aldatuPantaila(event, "barra-view.fxml", "Barra");
    }

    @FXML
    protected void onBiltegiaClick(ActionEvent event) {
        if (!Sesioa.isAdminEdoKudeatzailea() && !Sesioa.isBiltegikoa()) {
            erakutsiAlerta("Baimenik ez", "Ez daukazu baimenik Biltegia modulura sartzeko.");
            return;
        }
        garbituBaliabideak();
        aldatuPantaila(event, "biltegia-view.fxml", "Biltegia");
    }

    @FXML
    protected void onSaioaItxiClick(ActionEvent event) {
        garbituBaliabideak();
        Sesioa.itxiSaioa();
        aldatuPantaila(event, "login-view.fxml", "Saio-hasiera");
    }

    /**
     * Pantaila aldatzean dei daiteke baliabideak (adibidez tenporizadoreak) gelditzeko.
     */
    protected void garbituBaliabideak() {
        // Azpiklaseek behar izanez gero gainidazteko
    }

    protected void aldatuPantaila(ActionEvent event, String fxmlIzena, String izenburua) {
        try {
            Node iturria = (Node) event.getSource();
            PantailaAldatzailea.aldatu(iturria, fxmlIzena);
        } catch (IOException e) {
            erakutsiAlerta("Errorea pantaila aldatzean", "Ezin izan da " + izenburua + " pantaila kargatu: " + e.getMessage());
        }
    }

    protected void erakutsiAlerta(String izenburua, String mezua) {
        Alert alerta = new Alert(Alert.AlertType.WARNING);
        alerta.setTitle(izenburua);
        alerta.setHeaderText(null);
        alerta.setContentText(mezua);
        alerta.showAndWait();
    }

    protected void erakutsiInformazioa(String izenburua, String mezua) {
        Alert alerta = new Alert(Alert.AlertType.INFORMATION);
        alerta.setTitle(izenburua);
        alerta.setHeaderText(null);
        alerta.setContentText(mezua);
        alerta.showAndWait();
    }
}
