package eus.goierri.jatetxea.core.controller;

import eus.goierri.jatetxea.core.api.ApiZerbitzua;
import eus.goierri.jatetxea.core.model.Eskaera;
import eus.goierri.jatetxea.core.model.EskaeraLerroa;
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

    protected void hasieratuBaseEskaera() {
        if (colEskaeraId != null) colEskaeraId.setCellValueFactory(new PropertyValueFactory<>("id"));
        if (colMahaia != null) colMahaia.setCellValueFactory(new PropertyValueFactory<>("mahaiaTestua"));
        if (colLangilea != null) colLangilea.setCellValueFactory(new PropertyValueFactory<>("langileIzena"));
        if (colOrdua != null) colOrdua.setCellValueFactory(new PropertyValueFactory<>("sortzeData"));

        if (colLerroPlatera != null) colLerroPlatera.setCellValueFactory(new PropertyValueFactory<>("platerIzena"));
        if (colLerroKop != null) colLerroKop.setCellValueFactory(new PropertyValueFactory<>("kantitatea"));
        if (colLerroOharrak != null) colLerroOharrak.setCellValueFactory(new PropertyValueFactory<>("oharrak"));

        if (tblEskaerak != null) {
            tblEskaerak.getSelectionModel().selectedItemProperty().addListener((obs, zaharra, berria) -> {
                erakutsiEskaeraXehetasunak(berria);
            });
        }

        tenporizadorea = new Timeline(new KeyFrame(Duration.seconds(10), e -> {
            if (chkAutoRefresh != null && chkAutoRefresh.isSelected()) {
                kargatuDatuak();
            }
        }));
        tenporizadorea.setCycleCount(Timeline.INDEFINITE);
        tenporizadorea.play();

        konfiguratuSaioaEtaRola();
    }

    @Override
    protected void garbituBaliabideak() {
        super.garbituBaliabideak();
        if (tenporizadorea != null) {
            tenporizadorea.stop();
        }
    }

    public void kargatuDatuak() {
        new Thread(() -> {
            ApiZerbitzua.HealthResult health = api.egiaztatuOsasuna();
            if (!health.apiMartxan) {
                Platform.runLater(() -> {
                    if (lblKonexioa != null) lblKonexioa.setText("DESKONEKTATUTA");
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("APIa ez dago erabilgarri (" + ApiZerbitzua.getBaseUrl() + ")");
                });
                return;
            }

            try {
                List<Eskaera> zerrenda = api.lortuEskaeraAktiboak();
                List<Eskaera> iragaziak = iragaziEskaerak(zerrenda);

                Platform.runLater(() -> {
                    if (lblKonexioa != null) lblKonexioa.setText("KONEKTATUTA (" + ApiZerbitzua.getBaseUrl() + ")");
                    if (tblEskaerak != null) {
                        Eskaera hautatua = tblEskaerak.getSelectionModel().getSelectedItem();
                        int hautatuId = hautatua != null ? hautatua.getId() : -1;

                        tblEskaerak.setItems(FXCollections.observableArrayList(iragaziak));

                        if (hautatuId != -1) {
                            for (Eskaera e : iragaziak) {
                                if (e.getId() == hautatuId) {
                                    tblEskaerak.getSelectionModel().select(e);
                                    break;
                                }
                            }
                        } else if (!iragaziak.isEmpty()) {
                            tblEskaerak.getSelectionModel().selectFirst();
                        }
                    }

                    if (lblEskaeraKopurua != null) {
                        lblEskaeraKopurua.setText(iragaziak.size() + " eskaera");
                    }
                    if (lblAzkenFreskatzea != null) {
                        lblAzkenFreskatzea.setText("Azken eguneratzea: " + LocalTime.now().format(DateTimeFormatter.ofPattern("HH:mm:ss")));
                    }
                });

            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea: " + ex.getMessage());
                });
            }
        }).start();
    }

    protected abstract List<Eskaera> iragaziEskaerak(List<Eskaera> guztiak);

    protected void erakutsiEskaeraXehetasunak(Eskaera e) {
        if (e == null) {
            if (lblHautatutakoEskaera != null) lblHautatutakoEskaera.setText("Ez da eskaerarik hautatu");
            if (tblLerroak != null) tblLerroak.setItems(FXCollections.observableArrayList());
            return;
        }

        if (lblHautatutakoEskaera != null) {
            lblHautatutakoEskaera.setText("Eskaera #" + e.getId() + " - " + e.getMahaiaTestua() + " (" + e.getLangileIzena() + ")");
        }

        List<EskaeraLerroa> lerroak = iragaziLerroak(e.getLerroak());
        if (tblLerroak != null) {
            tblLerroak.setItems(FXCollections.observableArrayList(lerroak));
        }
    }

    protected List<EskaeraLerroa> iragaziLerroak(List<EskaeraLerroa> guztiak) {
        return guztiak != null ? guztiak : Collections.emptyList();
    }

    @FXML
    protected void onFreskatuClick() {
        kargatuDatuak();
    }
}
