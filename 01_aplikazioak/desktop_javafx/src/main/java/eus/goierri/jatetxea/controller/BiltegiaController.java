package eus.goierri.jatetxea.controller;

import eus.goierri.jatetxea.api.ApiZerbitzua;
import eus.goierri.jatetxea.model.Lehengaia;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;

import java.util.List;

/**
 * Biltegiko stock kudeaketarako kontroladorea (MVC ereduaren kontroladore geruza).
 * REST APIaren bidez lehengaiak bistaratzen ditu eta horniduren stock sarrerak erregistratzen ditu.
 */
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

    @FXML
    public void initialize() {
        // 1. Zutabeak modeloaren propietateekin lotu
        colId.setCellValueFactory(new PropertyValueFactory<>("id"));
        colIzena.setCellValueFactory(new PropertyValueFactory<>("izena"));
        colUnekoStocka.setCellValueFactory(new PropertyValueFactory<>("stockFormateatua"));
        colGutxienekoa.setCellValueFactory(new PropertyValueFactory<>("gutxienekoFormateatua"));
        colKostua.setCellValueFactory(new PropertyValueFactory<>("kostuaFormateatua"));
        colHornitzailea.setCellValueFactory(new PropertyValueFactory<>("hornitzailea"));
        colEgoera.setCellValueFactory(new PropertyValueFactory<>("egoera"));

        // Egoera zutabearen koloreak egokitu (Alerta gorriz/laranjaz, Egokia berdez)
        colEgoera.setCellFactory(column -> new TableCell<>() {
            @Override
            protected void updateItem(String egoera, boolean empty) {
                super.updateItem(egoera, empty);
                getStyleClass().removeAll("status-agortuta", "status-baxua", "status-egokia");
                if (empty || egoera == null) {
                    setText(null);
                } else {
                    setText(egoera);
                    if ("AGORTUTA".equals(egoera)) {
                        getStyleClass().add("status-agortuta");
                    } else if ("STOCK BAXUA".equals(egoera)) {
                        getStyleClass().add("status-baxua");
                    } else {
                        getStyleClass().add("status-egokia");
                    }
                }
            }
        });

        // 2. Iragazkiak prestatu
        iragazitakoDatuak = new FilteredList<>(datuZerrenda, p -> true);
        tblLehengaiak.setItems(iragazitakoDatuak);

        txtBilaketa.textProperty().addListener((obs, zaharra, berria) -> eguneratuIragazkia());
        chkAlertakSoilik.selectedProperty().addListener((obs, zaharra, berria) -> eguneratuIragazkia());

        // 3. Datuak kargatu APItik
        kargatuDatuakAPItik();

        // 4. Langilearen saioa eta rolen araberako nabigazioa
        konfiguratuSaioaEtaRola();
    }

    private void eguneratuIragazkia() {
        String testua = txtBilaketa.getText() != null ? txtBilaketa.getText().toLowerCase().trim() : "";
        boolean alertakSoilik = chkAlertakSoilik.isSelected();

        iragazitakoDatuak.setPredicate(l -> {
            if (l == null) return false;

            boolean batDatorTestua = testua.isEmpty() ||
                    l.getIzena().toLowerCase().contains(testua) ||
                    l.getHornitzailea().toLowerCase().contains(testua);

            boolean batDatorAlerta = !alertakSoilik || l.isAlerta();

            return batDatorTestua && batDatorAlerta;
        });

        lblTaulaKopurua.setText(iragazitakoDatuak.size() + " produktu");
    }

    @FXML
    private void onKargatuDatuakClick() {
        kargatuDatuakAPItik();
    }

    public void kargatuDatuakAPItik() {
        new Thread(() -> {
            try {
                List<Lehengaia> zerrenda = api.lortuLehengaiak();
                Platform.runLater(() -> {
                    eguneratuDatuZerrenda(zerrenda);
                    lblIturria.setText("Iturria: REST API (" + ApiZerbitzua.getBaseUrl() + "/lehengaiak)");
                    lblIturria.setStyle("-fx-text-fill: #166534; -fx-font-weight: bold;");
                    lblEgoeraMezua.setText("APItik " + zerrenda.size() + " lehengai kargatu dira.");
                });
            } catch (Exception e) {
                Platform.runLater(() -> {
                    lblIturria.setText("Iturria: API DESKONEKTATUTA");
                    lblIturria.setStyle("-fx-text-fill: #dc2626; -fx-font-weight: bold;");
                    lblEgoeraMezua.setText("Errorea APItik kargatzean: " + e.getMessage());
                });
            }
        }).start();
    }

    private void eguneratuDatuZerrenda(List<Lehengaia> zerrenda) {
        datuZerrenda.setAll(zerrenda);
        cmbLehengaia.setItems(FXCollections.observableArrayList(zerrenda));
        if (!zerrenda.isEmpty()) {
            cmbLehengaia.getSelectionModel().selectFirst();
        }

        // KPIak kalkulatu
        int guztira = zerrenda.size();
        long alertak = zerrenda.stream().filter(Lehengaia::isAlerta).count();
        long agortuta = zerrenda.stream().filter(l -> l.getUnekoStocka() <= 0).count();

        lblKpiGuztira.setText(String.valueOf(guztira));
        lblKpiAlertak.setText(String.valueOf(alertak));
        lblKpiAgortuta.setText(String.valueOf(agortuta));
        lblTaulaKopurua.setText(zerrenda.size() + " produktu");
    }

    @FXML
    private void onSarreraErregistratuClick() {
        Lehengaia aukeratua = cmbLehengaia.getSelectionModel().getSelectedItem();
        if (aukeratua == null) {
            erakutsiAlerta("Errorea", "Mesedez, aukeratu lehengai bat.");
            return;
        }

        String kantitateaStr = txtKantitatea.getText();
        if (kantitateaStr == null || kantitateaStr.trim().isEmpty()) {
            erakutsiAlerta("Errorea", "Mesedez, idatzi sartu nahi den kantitatea.");
            return;
        }

        double kantitatea;
        try {
            kantitatea = Double.parseDouble(kantitateaStr.trim().replace(',', '.'));
            if (kantitatea <= 0) {
                erakutsiAlerta("Errorea", "Kantitateak 0 baino handiagoa izan behar du.");
                return;
            }
        } catch (NumberFormatException e) {
            erakutsiAlerta("Errorea", "Kantitate zenbaki baliogabea.");
            return;
        }

        String faktura = txtAlbarana.getText();
        String oharrak = txtOharrak.getText();

        new Thread(() -> {
            try {
                boolean ok = api.erregistratuStockSarrera(aukeratua.getId(), kantitatea, faktura, oharrak);
                Platform.runLater(() -> {
                    if (ok) {
                        erakutsiInformazioa("Stocka Eguneratuta", aukeratua.getIzena() + " lehengaiari +" + kantitatea + " " + aukeratua.getUnitatea() + " gehitu zaizkio.");
                        txtKantitatea.clear();
                        txtAlbarana.clear();
                        txtOharrak.clear();
                        kargatuDatuakAPItik();
                    }
                });
            } catch (Exception e) {
                Platform.runLater(() -> {
                    erakutsiAlerta("Errorea", "Ezin izan da stock sarrera erregistratu: " + e.getMessage());
                });
            }
        }).start();
    }
}
