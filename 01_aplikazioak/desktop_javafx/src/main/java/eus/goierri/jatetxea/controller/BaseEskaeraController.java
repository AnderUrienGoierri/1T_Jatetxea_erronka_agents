package eus.goierri.jatetxea.controller;

import eus.goierri.jatetxea.api.ApiZerbitzua;
import eus.goierri.jatetxea.model.Eskaera;
import eus.goierri.jatetxea.model.EskaeraLerroa;
import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.util.Duration;

import java.time.LocalTime;
import java.time.format.DateTimeFormatter;
import java.util.Collections;
import java.util.List;

/**
 * Sukaldea (KDS) eta Barra (BDS) kontroladoreen oinarrizko klase abstraktua.
 * Eskaeren taula, lerroen taula, auto-refresh tenporizadorea eta egoera-aldaketen
 * logika komuna biltzen ditu, kode bikoizketak ezabatzeko.
 */
public abstract class BaseEskaeraController extends BaseController {

    @FXML protected TableView<Eskaera> tblEskaerak;
    @FXML protected TableColumn<Eskaera, Integer> colEskaeraId;
    @FXML protected TableColumn<Eskaera, String> colMahaia;
    @FXML protected TableColumn<Eskaera, String> colLangilea;
    @FXML protected TableColumn<Eskaera, String> colEgoera;
    @FXML protected TableColumn<Eskaera, String> colOrdua;

    @FXML protected TableView<EskaeraLerroa> tblLerroak;
    @FXML protected TableColumn<EskaeraLerroa, String> colLerroPlatera;
    @FXML protected TableColumn<EskaeraLerroa, Integer> colLerroKop;
    @FXML protected TableColumn<EskaeraLerroa, String> colLerroOharrak;
    @FXML protected TableColumn<EskaeraLerroa, String> colLerroEgoera;
    @FXML protected TableColumn<EskaeraLerroa, Void> colLerroEkintza;

    @FXML protected Label lblEskaeraKopurua;
    @FXML protected Label lblHautatutakoEskaera;
    @FXML protected Label lblKonexioa;
    @FXML protected Label lblEgoeraMezua;
    @FXML protected Label lblAzkenFreskatzea;
    @FXML protected CheckBox chkAutoRefresh;

    protected final ApiZerbitzua api = new ApiZerbitzua();
    protected Timeline tenporizadorea;

    /**
     * Zutabe komunak lotu, hautapen-entzulea ezarri eta tenporizadorea martxan jarri.
     */
    protected void hasieratuBaseEskaera() {
        // 1. Eskaeren taularen zutabe komunak lotu
        if (colEskaeraId != null) colEskaeraId.setCellValueFactory(new PropertyValueFactory<>("id"));
        if (colMahaia != null) colMahaia.setCellValueFactory(new PropertyValueFactory<>("mahaiaTestua"));
        if (colLangilea != null) colLangilea.setCellValueFactory(new PropertyValueFactory<>("langileIzena"));
        if (colOrdua != null) colOrdua.setCellValueFactory(new PropertyValueFactory<>("sortzeData"));

        // 2. Lerroen taularen zutabe komunak lotu
        if (colLerroPlatera != null) colLerroPlatera.setCellValueFactory(new PropertyValueFactory<>("platerIzena"));
        if (colLerroKop != null) colLerroKop.setCellValueFactory(new PropertyValueFactory<>("kantitatea"));
        if (colLerroOharrak != null) colLerroOharrak.setCellValueFactory(new PropertyValueFactory<>("oharrak"));

        // 3. Hautapen gertaerak kudeatu
        if (tblEskaerak != null) {
            tblEskaerak.getSelectionModel().selectedItemProperty().addListener((obs, zaharra, berria) -> {
                erakutsiEskaeraXehetasunak(berria);
            });
        }

        // 4. Auto-freskatzea prestatu (10 segundotik behin)
        tenporizadorea = new Timeline(new KeyFrame(Duration.seconds(10), e -> {
            if (chkAutoRefresh != null && chkAutoRefresh.isSelected()) {
                kargatuDatuak();
            }
        }));
        tenporizadorea.setCycleCount(Timeline.INDEFINITE);
        tenporizadorea.play();

        // 5. Langilearen saioa eta rola egiaztatu
        konfiguratuSaioaEtaRola();
    }

    @Override
    protected void garbituBaliabideak() {
        super.garbituBaliabideak();
        if (tenporizadorea != null) {
            tenporizadorea.stop();
        }
    }

    /**
     * Datuak kargatu APItik, osasuna egiaztatu eta taulak eguneratu.
     */
    public void kargatuDatuak() {
        new Thread(() -> {
            ApiZerbitzua.HealthResult health = api.egiaztatuOsasuna();
            if (!health.apiMartxan) {
                Platform.runLater(() -> {
                    lblKonexioa.setText("API konexioa: DESKONEKTATUTA (" + ApiZerbitzua.getBaseUrl() + ")");
                    lblKonexioa.setStyle("-fx-text-fill: #dc2626; -fx-font-weight: bold;");
                    lblEgoeraMezua.setText("Ezin izan da backend zerbitzariarekin konektatu.");
                });
                return;
            }

            if (!health.dbMartxan) {
                Platform.runLater(() -> {
                    lblKonexioa.setText("API KONEKTATUTA | Datu-basea DESKONEKTATUTA!");
                    lblKonexioa.setStyle("-fx-text-fill: #ea580c; -fx-font-weight: bold;");
                    lblEgoeraMezua.setText("APIra konektatu da, baina MySQL datu-basearekin ezin izan da konexioa lortu.");
                });
                return;
            }

            try {
                List<Eskaera> guztiak = api.lortuEskaeraAktiboak();
                List<Eskaera> iragaziak = iragaziEskaerak(guztiak);

                Platform.runLater(() -> {
                    int aurrekoId = tblEskaerak.getSelectionModel().getSelectedItem() != null
                            ? tblEskaerak.getSelectionModel().getSelectedItem().getId()
                            : -1;

                    tblEskaerak.setItems(FXCollections.observableArrayList(iragaziak));
                    if (lblEskaeraKopurua != null) {
                        lblEskaeraKopurua.setText(iragaziak.size() + " eskaera");
                    }

                    lblKonexioa.setText("API konexioa: KONEKTATUTA (" + ApiZerbitzua.getBaseUrl() + ")");
                    lblKonexioa.setStyle("-fx-text-fill: #166534; -fx-font-weight: bold;");
                    if (lblAzkenFreskatzea != null) {
                        lblAzkenFreskatzea.setText("Azken freskatzea: " + LocalTime.now().format(DateTimeFormatter.ofPattern("HH:mm:ss")));
                    }
                    lblEgoeraMezua.setText("Datuak eguneratuta (" + getModuluIzena() + ").");

                    // Hautapena mantendu
                    if (aurrekoId != -1) {
                        for (Eskaera e : iragaziak) {
                            if (e.getId() == aurrekoId) {
                                tblEskaerak.getSelectionModel().select(e);
                                break;
                            }
                        }
                    } else if (!iragaziak.isEmpty()) {
                        tblEskaerak.getSelectionModel().selectFirst();
                    }
                });
            } catch (Exception e) {
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Errorea datuak kargatzean: " + e.getMessage());
                });
            }
        }).start();
    }

    /**
     * Hautatutako eskaeraren xehetasunak (lerroak) erakutsi.
     */
    protected void erakutsiEskaeraXehetasunak(Eskaera eskaera) {
        if (eskaera == null) {
            tblLerroak.setItems(FXCollections.emptyObservableList());
            if (lblHautatutakoEskaera != null) {
                lblHautatutakoEskaera.setText("Hautatutako Eskaera: Ez dago hautatuta");
            }
            eguneratuXehetasunEtiketa(null);
            return;
        }

        if (lblHautatutakoEskaera != null) {
            lblHautatutakoEskaera.setText("Eskaera #" + eskaera.getId() + " - " + eskaera.getMahaiaTestua() + " (" + eskaera.getLangileIzena() + ")");
        }
        List<EskaeraLerroa> lerroak = lortuEskaeraLerroak(eskaera);
        tblLerroak.setItems(FXCollections.observableArrayList(lerroak != null ? lerroak : Collections.emptyList()));
        eguneratuXehetasunEtiketa(eskaera);
    }

    /**
     * Lerro bakar baten egoera backend-ean eguneratu eta interfazea freskatu.
     */
    protected void eguneratuLerroaZuzenean(EskaeraLerroa lerroa, String egoeraBerria) {
        if (lerroa == null) return;

        new Thread(() -> {
            try {
                boolean ok = api.eguneratuLerroEgoera(lerroa.getId(), egoeraBerria);
                Platform.runLater(() -> {
                    if (ok) {
                        lerroa.setEgoera(egoeraBerria);
                        tblLerroak.refresh();
                        tblEskaerak.refresh();
                        lblEgoeraMezua.setText(lerroa.getPlaterIzena() + " -> " + egoeraBerria + " egoerara aldatu da.");
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Errorea egoera aldatzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    @FXML
    protected void onFreskatuClick() {
        kargatuDatuak();
    }

    // ==========================================
    // METODO ABSTRAKTUAK (Azpiklaseek definitzeko)
    // ==========================================

    protected abstract String getModuluIzena();
    protected abstract List<Eskaera> iragaziEskaerak(List<Eskaera> guztiak);
    protected abstract List<EskaeraLerroa> lortuEskaeraLerroak(Eskaera eskaera);
    protected abstract void eguneratuXehetasunEtiketa(Eskaera eskaera);
}
