package eus.goierri.jatetxea.biltegia.controller;

import eus.goierri.jatetxea.biltegia.BiltegiaApp;
import eus.goierri.jatetxea.core.api.ApiZerbitzua;
import eus.goierri.jatetxea.core.controller.BaseController;
import eus.goierri.jatetxea.core.model.Lehengaia;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;

import java.util.List;

public class BiltegiaController extends BaseController {

    @FXML private TableView<Lehengaia> tblLehengaiak;
    @FXML private TableColumn<Lehengaia, Integer> colId;
    @FXML private TableColumn<Lehengaia, String> colIzena;
    @FXML private TableColumn<Lehengaia, String> colUnekoStocka;
    @FXML private TableColumn<Lehengaia, String> colGutxienekoa;
    @FXML private TableColumn<Lehengaia, String> colKostua;
    @FXML private TableColumn<Lehengaia, String> colHornitzailea;
    @FXML private TableColumn<Lehengaia, String> colEgoera;

    @FXML private Label lblKpiGuztira;
    @FXML private Label lblKpiAlertak;
    @FXML private Label lblKpiAgortuta;
    @FXML private Label lblTaulaKopurua;
    @FXML private Label lblIturria;
    @FXML private Label lblEgoeraMezua;

    @FXML private TextField txtBilaketa;
    @FXML private CheckBox chkAlertakSoilik;

    @FXML private ComboBox<Lehengaia> cmbLehengaia;
    @FXML private TextField txtKantitatea;
    @FXML private TextField txtAlbarana;
    @FXML private TextField txtOharrak;

    private final ApiZerbitzua api = new ApiZerbitzua();
    private final ObservableList<Lehengaia> datuZerrenda = FXCollections.observableArrayList();
    private FilteredList<Lehengaia> iragazitakoDatuak;

    @Override
    protected Class<?> getAppResourceClass() {
        return BiltegiaApp.class;
    }

    @FXML
    public void initialize() {
        colId.setCellValueFactory(new PropertyValueFactory<>("id"));
        colIzena.setCellValueFactory(new PropertyValueFactory<>("izena"));
        colUnekoStocka.setCellValueFactory(new PropertyValueFactory<>("stockFormateatua"));
        colGutxienekoa.setCellValueFactory(new PropertyValueFactory<>("gutxienekoFormateatua"));
        colKostua.setCellValueFactory(new PropertyValueFactory<>("kostuaFormateatua"));
        colHornitzailea.setCellValueFactory(new PropertyValueFactory<>("hornitzailea"));
        colEgoera.setCellValueFactory(new PropertyValueFactory<>("egoera"));

        colEgoera.setCellFactory(column -> new TableCell<>() {
            @Override
            protected void updateItem(String egoera, boolean empty) {
                super.updateItem(egoera, empty);
                if (empty || egoera == null) {
                    setText(null);
                    setStyle("");
                } else {
                    setText(egoera);
                    if (egoera.contains("AGORTUTA")) {
                        setStyle("-fx-text-fill: #dc2626; -fx-font-weight: bold;");
                    } else if (egoera.contains("ALERTA")) {
                        setStyle("-fx-text-fill: #d97706; -fx-font-weight: bold;");
                    } else {
                        setStyle("-fx-text-fill: #16a34a; -fx-font-weight: bold;");
                    }
                }
            }
        });

        iragazitakoDatuak = new FilteredList<>(datuZerrenda, p -> true);
        tblLehengaiak.setItems(iragazitakoDatuak);

        if (txtBilaketa != null) {
            txtBilaketa.textProperty().addListener((obs, zaharra, berria) -> aplikatuIragazkia());
        }
        if (chkAlertakSoilik != null) {
            chkAlertakSoilik.selectedProperty().addListener((obs, zaharra, berria) -> aplikatuIragazkia());
        }

        konfiguratuSaioaEtaRola();
        kargatuDatuak();
    }

    public void kargatuDatuak() {
        new Thread(() -> {
            try {
                List<Lehengaia> lehengaiak = api.lortuLehengaiak();
                Platform.runLater(() -> {
                    datuZerrenda.clear();
                    datuZerrenda.addAll(lehengaiak);
                    if (cmbLehengaia != null) {
                        cmbLehengaia.setItems(datuZerrenda);
                    }
                    eguneratuKpiak();
                    if (lblIturria != null) {
                        lblIturria.setText("Iturria: REST API (" + ApiZerbitzua.getBaseUrl() + "/lehengaiak)");
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea datuak kargatzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    private void aplikatuIragazkia() {
        String testua = txtBilaketa != null && txtBilaketa.getText() != null ? txtBilaketa.getText().toLowerCase().trim() : "";
        boolean alertakSoilik = chkAlertakSoilik != null && chkAlertakSoilik.isSelected();

        iragazitakoDatuak.setPredicate(l -> {
            if (l == null) return false;
            if (alertakSoilik && !l.isAlertaDauka()) return false;
            if (testua.isEmpty()) return true;
            return l.getIzena().toLowerCase().contains(testua) ||
                   l.getHornitzailea().toLowerCase().contains(testua);
        });

        if (lblTaulaKopurua != null) {
            lblTaulaKopurua.setText(iragazitakoDatuak.size() + " lehengai bistaratuta");
        }
    }

    private void eguneratuKpiak() {
        int guztira = datuZerrenda.size();
        long alertak = datuZerrenda.stream().filter(Lehengaia::isAlertaDauka).count();
        long agortuta = datuZerrenda.stream().filter(l -> l.getStock() <= 0).count();

        if (lblKpiGuztira != null) lblKpiGuztira.setText(String.valueOf(guztira));
        if (lblKpiAlertak != null) lblKpiAlertak.setText(String.valueOf(alertak));
        if (lblKpiAgortuta != null) lblKpiAgortuta.setText(String.valueOf(agortuta));
        if (lblTaulaKopurua != null) lblTaulaKopurua.setText(iragazitakoDatuak.size() + " lehengai bistaratuta");
    }

    @FXML
    private void onFreskatuClick() {
        kargatuDatuak();
    }

    @FXML
    private void onKargatuDatuakClick() {
        kargatuDatuak();
    }

    @FXML
    private void onSarreraErregistratuClick() {
        onGordeSarreraClick();
    }

    @FXML
    private void onGarbituIragazkiakClick() {
        if (txtBilaketa != null) txtBilaketa.clear();
        if (chkAlertakSoilik != null) chkAlertakSoilik.setSelected(false);
    }

    @FXML
    private void onGordeSarreraClick() {
        Lehengaia hautatua = cmbLehengaia != null ? cmbLehengaia.getValue() : null;
        if (hautatua == null) {
            erakutsiAlerta("Baliozkotze errorea", "Hautatu lehengai bat sarrera erregistratzeko.");
            return;
        }

        String kantTestua = txtKantitatea != null ? txtKantitatea.getText().trim() : "";
        double kantitatea;
        try {
            kantitatea = Double.parseDouble(kantTestua.replace(',', '.'));
            if (kantitatea <= 0) throw new NumberFormatException();
        } catch (NumberFormatException e) {
            erakutsiAlerta("Baliozkotze errorea", "Sartu baliozko zenbaki positibo bat kantitatean.");
            return;
        }

        String albarana = txtAlbarana != null ? txtAlbarana.getText().trim() : "";
        String oharrak = txtOharrak != null ? txtOharrak.getText().trim() : "";

        new Thread(() -> {
            try {
                boolean ok = api.gehituStockSarrera(hautatua.getId(), kantitatea, albarana, oharrak);
                Platform.runLater(() -> {
                    if (ok) {
                        if (lblEgoeraMezua != null) {
                            lblEgoeraMezua.setText("✓ " + kantitatea + " " + hautatua.getUnitatea() + " gehitu dira " + hautatua.getIzena() + " lehengaian.");
                        }
                        if (txtKantitatea != null) txtKantitatea.clear();
                        if (txtAlbarana != null) txtAlbarana.clear();
                        if (txtOharrak != null) txtOharrak.clear();
                        kargatuDatuak();
                    } else {
                        if (lblEgoeraMezua != null) {
                            lblEgoeraMezua.setText("Errorea stock sarrera erregistratzean.");
                        }
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea: " + ex.getMessage());
                });
            }
        }).start();
    }
}
