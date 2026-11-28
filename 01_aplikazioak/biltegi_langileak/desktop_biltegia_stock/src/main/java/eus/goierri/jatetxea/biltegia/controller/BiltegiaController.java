package eus.goierri.jatetxea.biltegia.controller;

import eus.goierri.jatetxea.biltegia.BiltegiaApp;
import eus.goierri.jatetxea.core.api.ApiZerbitzua;
import eus.goierri.jatetxea.core.controller.BaseController;
import eus.goierri.jatetxea.core.model.AlbaranaSarreraErantzuna;
import eus.goierri.jatetxea.core.model.Hornitzailea;
import eus.goierri.jatetxea.core.model.Lehengaia;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;

import java.util.List;
import java.util.Locale;
import java.util.Optional;

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

    @FXML private ComboBox<Hornitzailea> cmbHornitzailea;
    @FXML private Label lblHornitzaileInfo;
    @FXML private ComboBox<Lehengaia> cmbLehengaia;
    @FXML private Label lblKantitateaLabel;
    @FXML private TextField txtKantitatea;
    @FXML private ComboBox<String> cmbOrdainketaMetodoa;
    @FXML private TextField txtAlbarana;
    @FXML private TextField txtOharrak;
    @FXML private Label lblKostuOinarria;
    @FXML private Label lblKostuBeza;
    @FXML private Label lblKostuGuztira;
    @FXML private Button btnGordeSarrera;

    private final ApiZerbitzua api = new ApiZerbitzua();
    private final ObservableList<Lehengaia> datuZerrenda = FXCollections.observableArrayList();
    private final ObservableList<Hornitzailea> hornitzaileZerrenda = FXCollections.observableArrayList();
    private final ObservableList<Lehengaia> hornitzailearenLehengaiak = FXCollections.observableArrayList();
    private FilteredList<Lehengaia> iragazitakoDatuak;
    private boolean sinkronizatzen = false;

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

        if (cmbOrdainketaMetodoa != null) {
            cmbOrdainketaMetodoa.setItems(FXCollections.observableArrayList("Transferentzia", "Txartela", "Eskudirua"));
            cmbOrdainketaMetodoa.setValue("Transferentzia");
        }

        if (cmbHornitzailea != null) {
            cmbHornitzailea.setItems(hornitzaileZerrenda);
            cmbHornitzailea.valueProperty().addListener((obs, zaharra, berria) -> {
                if (!sinkronizatzen) {
                    eguneratuHornitzailearenProduktuak(berria, null);
                }
            });
        }

        if (cmbLehengaia != null) {
            cmbLehengaia.setItems(hornitzailearenLehengaiak);
            cmbLehengaia.valueProperty().addListener((obs, zaharra, berria) -> eguneratuOrdainketaAurrebista());
        }

        if (txtKantitatea != null) {
            txtKantitatea.textProperty().addListener((obs, zaharra, berria) -> eguneratuOrdainketaAurrebista());
        }

        // Taulan lehengai bat klikatzean, bere hornitzailea eta lehengaia automatikoki hautatu
        tblLehengaiak.getSelectionModel().selectedItemProperty().addListener((obs, zaharra, hautatua) -> {
            if (hautatua != null && cmbHornitzailea != null) {
                Hornitzailea dagokionHornitzailea = bilatuHornitzailea(hautatua);
                if (dagokionHornitzailea != null) {
                    sinkronizatzen = true;
                    cmbHornitzailea.setValue(dagokionHornitzailea);
                    sinkronizatzen = false;
                    eguneratuHornitzailearenProduktuak(dagokionHornitzailea, hautatua.getId());
                }
            }
        });

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
        Integer aurrekoHornitzaileId = cmbHornitzailea != null && cmbHornitzailea.getValue() != null
                ? cmbHornitzailea.getValue().getId() : null;
        Integer aurrekoLehengaiId = cmbLehengaia != null && cmbLehengaia.getValue() != null
                ? cmbLehengaia.getValue().getId() : null;

        new Thread(() -> {
            try {
                List<Hornitzailea> hornitzaileak = api.lortuHornitzaileak();
                List<Lehengaia> lehengaiak = api.lortuLehengaiak();
                Platform.runLater(() -> {
                    hornitzaileZerrenda.setAll(hornitzaileak);
                    datuZerrenda.setAll(lehengaiak);

                    if (cmbHornitzailea != null && !hornitzaileZerrenda.isEmpty()) {
                        Hornitzailea hautatzekoa = hornitzaileZerrenda.stream()
                                .filter(h -> aurrekoHornitzaileId != null && h.getId() == aurrekoHornitzaileId)
                                .findFirst()
                                .orElse(hornitzaileZerrenda.get(0));
                        sinkronizatzen = true;
                        cmbHornitzailea.setValue(hautatzekoa);
                        sinkronizatzen = false;
                        eguneratuHornitzailearenProduktuak(hautatzekoa, aurrekoLehengaiId);
                    } else if (cmbLehengaia != null) {
                        hornitzailearenLehengaiak.setAll(datuZerrenda);
                        if (!hornitzailearenLehengaiak.isEmpty()) {
                            cmbLehengaia.setValue(hornitzailearenLehengaiak.get(0));
                        }
                        eguneratuOrdainketaAurrebista();
                    }

                    eguneratuKpiak();
                    if (lblIturria != null) {
                        lblIturria.setText("Iturria: REST API (" + ApiZerbitzua.getBaseUrl() + ")");
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea datuak kargatzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    private Hornitzailea bilatuHornitzailea(Lehengaia lehengaia) {
        if (lehengaia.getHornitzaileaId() != null) {
            for (Hornitzailea h : hornitzaileZerrenda) {
                if (h.getId() == lehengaia.getHornitzaileaId()) return h;
            }
        }
        String izena = lehengaia.getHornitzailea();
        if (!izena.isBlank()) {
            for (Hornitzailea h : hornitzaileZerrenda) {
                if (h.getIzena().equalsIgnoreCase(izena)) return h;
            }
        }
        return null;
    }

    private void eguneratuHornitzailearenProduktuak(Hornitzailea hornitzailea, Integer hautatuLehengaiId) {
        if (hornitzailea == null) {
            hornitzailearenLehengaiak.setAll(datuZerrenda);
            if (lblHornitzaileInfo != null) lblHornitzaileInfo.setText("Hautatu hornitzaile bat bere produktuak ikusteko");
        } else {
            if (lblHornitzaileInfo != null) {
                lblHornitzaileInfo.setText(String.format(Locale.US, "CIF: %s · Mota: %s · BEZ: %.0f%%",
                        hornitzailea.getCif(), hornitzailea.getProduktuMota(), hornitzailea.getBezEhunekoa()));
            }
            List<Lehengaia> iragazita = datuZerrenda.stream()
                    .filter(l -> (l.getHornitzaileaId() != null && l.getHornitzaileaId() == hornitzailea.getId())
                            || l.getHornitzailea().equalsIgnoreCase(hornitzailea.getIzena()))
                    .toList();
            hornitzailearenLehengaiak.setAll(iragazita);
        }

        if (cmbLehengaia != null) {
            if (hornitzailearenLehengaiak.isEmpty()) {
                cmbLehengaia.setValue(null);
            } else {
                Lehengaia hautatzekoa = hornitzailearenLehengaiak.stream()
                        .filter(l -> hautatuLehengaiId != null && l.getId() == hautatuLehengaiId)
                        .findFirst()
                        .orElse(hornitzailearenLehengaiak.get(0));
                cmbLehengaia.setValue(hautatzekoa);
            }
        }
        eguneratuOrdainketaAurrebista();
    }

    private void eguneratuOrdainketaAurrebista() {
        Lehengaia hautatua = cmbLehengaia != null ? cmbLehengaia.getValue() : null;
        Hornitzailea hornitzailea = cmbHornitzailea != null ? cmbHornitzailea.getValue() : null;

        if (lblKantitateaLabel != null) {
            lblKantitateaLabel.setText(hautatua != null
                    ? "3. Kantitatea (" + hautatua.getUnitatea() + " · " + hautatua.getKostuaFormateatua() + "):"
                    : "3. Kantitatea:");
        }

        double kantitatea = 0.0;
        if (txtKantitatea != null && txtKantitatea.getText() != null && !txtKantitatea.getText().isBlank()) {
            try {
                double val = Double.parseDouble(txtKantitatea.getText().trim().replace(',', '.'));
                if (val > 0) kantitatea = val;
            } catch (NumberFormatException ignored) {
            }
        }

        double kostuUnit = hautatua != null ? hautatua.getKostua() : 0.0;
        double bezEhunekoa = hornitzailea != null
                ? hornitzailea.getBezEhunekoa()
                : (hautatua != null ? hautatua.getBezEhunekoa() : 10.0);
        double oinarria = Math.round(kantitatea * kostuUnit * 100.0) / 100.0;
        double beza = Math.round(oinarria * (bezEhunekoa / 100.0) * 100.0) / 100.0;
        double guztira = oinarria + beza;

        if (lblKostuOinarria != null) {
            lblKostuOinarria.setText(String.format(Locale.US, "Oinarri zergagarria: %.2f €", oinarria));
        }
        if (lblKostuBeza != null) {
            lblKostuBeza.setText(String.format(Locale.US, "BEZ (%.0f%%): %.2f €", bezEhunekoa, beza));
        }
        if (lblKostuGuztira != null) {
            lblKostuGuztira.setText(String.format(Locale.US, "GUZTIRA ORDAINTZEKO: %.2f €", guztira));
        }
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
        Hornitzailea hornitzailea = cmbHornitzailea != null ? cmbHornitzailea.getValue() : null;
        Lehengaia hautatua = cmbLehengaia != null ? cmbLehengaia.getValue() : null;
        if (hautatua == null) {
            erakutsiAlerta("Baliozkotze errorea", "Hautatu hornitzaile bat eta lehengai bat sarrera erregistratzeko.");
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

        String metodoa = cmbOrdainketaMetodoa != null && cmbOrdainketaMetodoa.getValue() != null
                ? cmbOrdainketaMetodoa.getValue() : "Transferentzia";
        String albarana = txtAlbarana != null ? txtAlbarana.getText().trim() : "";
        String oharrak = txtOharrak != null ? txtOharrak.getText().trim() : "";

        double bezEhunekoa = hornitzailea != null ? hornitzailea.getBezEhunekoa() : hautatua.getBezEhunekoa();
        double oinarria = Math.round(kantitatea * hautatua.getKostua() * 100.0) / 100.0;
        double beza = Math.round(oinarria * (bezEhunekoa / 100.0) * 100.0) / 100.0;
        double guztira = oinarria + beza;

        Alert baieztapena = new Alert(Alert.AlertType.CONFIRMATION);
        baieztapena.setTitle("Hornitzailearen Ordainketa Simulazioa");
        baieztapena.setHeaderText("Baieztatu albaran sarrera eta ordainketa?");
        baieztapena.setContentText(String.format(Locale.US,
                "Hornitzailea: %s%nProduktua: %s (+%.2f %s)%nOinarria: %.2f € · BEZ (%.0f%%): %.2f €%nGUZTIRA ORDAINTZEKO: %.2f € (%s)",
                hornitzailea != null ? hornitzailea.getIzena() : hautatua.getHornitzailea(),
                hautatua.getIzena(), kantitatea, hautatua.getUnitatea(),
                oinarria, bezEhunekoa, beza, guztira, metodoa));
        Optional<ButtonType> erantzuna = baieztapena.showAndWait();
        if (erantzuna.isEmpty() || erantzuna.get() != ButtonType.OK) {
            return;
        }

        if (btnGordeSarrera != null) btnGordeSarrera.setDisable(true);
        if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Albarana eta erosketa-faktura erregistratzen...");

        Integer hornitzaileaId = hornitzailea != null ? hornitzailea.getId() : hautatua.getHornitzaileaId();

        new Thread(() -> {
            try {
                AlbaranaSarreraErantzuna res = api.erregistratuAlbaranaSarrera(
                        hornitzaileaId, hautatua.getId(), kantitatea, albarana, metodoa, oharrak);
                Platform.runLater(() -> {
                    if (btnGordeSarrera != null) btnGordeSarrera.setDisable(false);
                    if (res.isArrakasta()) {
                        String laburpena = String.format(Locale.US,
                                "✓ +%.2f %s %s · Faktura: %s (%.2f € %s)",
                                kantitatea, hautatua.getUnitatea(), hautatua.getIzena(),
                                res.getFakturaZenbakia(), res.getGuztira(), res.getOrdainketaMetodoa());
                        if (lblEgoeraMezua != null) {
                            lblEgoeraMezua.setText(laburpena);
                        }
                        if (txtKantitatea != null) txtKantitatea.clear();
                        if (txtAlbarana != null) txtAlbarana.clear();
                        if (txtOharrak != null) txtOharrak.clear();
                        kargatuDatuak();

                        Alert info = new Alert(Alert.AlertType.INFORMATION);
                        info.setTitle("Sarrera eta Ordainketa Burututa");
                        info.setHeaderText("Albarana ondo erregistratu da!");
                        info.setContentText(String.format(Locale.US,
                                "Albarana: %s%nHornitzailea: %s%nProduktua: %s%nStock berria: %.2f %s%nErosketa-faktura: %s%nOrdaindutakoa: %.2f € (%s)",
                                res.getAlbaranZenbakia(), res.getHornitzailea(), res.getIzena(),
                                res.getUnekoStockBerria(), res.getUnitatea(),
                                res.getFakturaZenbakia(), res.getGuztira(), res.getOrdainketaMetodoa()));
                        info.show();
                    } else {
                        if (lblEgoeraMezua != null) {
                            lblEgoeraMezua.setText("Errorea: " + res.getMezua());
                        }
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (btnGordeSarrera != null) btnGordeSarrera.setDisable(false);
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea: " + ex.getMessage());
                });
            }
        }).start();
    }
}
