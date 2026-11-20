package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Faktura;
import eus.goierri.jatetxea.admin.model.FinantzaLaburpena;
import javafx.beans.property.ReadOnlyStringWrapper;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.layout.VBox;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public final class FakturakKontrolagailua {
    private final ApiBezeroa apiClient;
    private final KudeatzaileSaioa session;

    private final ObservableList<Faktura> masterList = FXCollections.observableArrayList();
    private FilteredList<Faktura> filteredList;

    @FXML private Label totalSalesLabel;
    @FXML private Label salesCountLabel;
    @FXML private Label totalExpensesLabel;
    @FXML private Label expensesCountLabel;
    @FXML private Label netProfitLabel;
    @FXML private Label netProfitSubtitle;
    @FXML private Label vatBalanceLabel;
    @FXML private Label vatDetailLabel;

    @FXML private ComboBox<String> filterTypeCombo;
    @FXML private TextField searchField;
    @FXML private Label invoiceMessage;

    @FXML private TableView<Faktura> invoicesTable;
    @FXML private TableColumn<Faktura, String> numberColumn;
    @FXML private TableColumn<Faktura, String> typeColumn;
    @FXML private TableColumn<Faktura, String> dateColumn;
    @FXML private TableColumn<Faktura, String> entityColumn;
    @FXML private TableColumn<Faktura, String> conceptColumn;
    @FXML private TableColumn<Faktura, String> baseColumn;
    @FXML private TableColumn<Faktura, String> vatColumn;
    @FXML private TableColumn<Faktura, String> totalColumn;
    @FXML private TableColumn<Faktura, String> methodColumn;
    @FXML private TableColumn<Faktura, String> statusColumn;

    // Formularioa faktura berria gehitzeko
    @FXML private VBox formCard;
    @FXML private Label formTitle;
    @FXML private ComboBox<String> formTypeCombo;
    @FXML private ComboBox<String> formSubtypeCombo;
    @FXML private TextField formConceptField;
    @FXML private TextField formEntityField;
    @FXML private TextField formCifField;
    @FXML private TextField formBaseField;
    @FXML private ComboBox<String> formVatPercentCombo;
    @FXML private ComboBox<String> formMethodCombo;
    @FXML private ComboBox<String> formStatusCombo;
    @FXML private TextField formNotesField;
    @FXML private Button saveInvoiceButton;

    public FakturakKontrolagailua() {
        this(null, null);
    }

    public FakturakKontrolagailua(ApiBezeroa apiClient, KudeatzaileSaioa session) {
        this.apiClient = apiClient;
        this.session = session;
    }

    @FXML
    private void initialize() {
        filteredList = new FilteredList<>(masterList, p -> true);

        // Zutabeen lotura
        numberColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getFakturaZenbakia()));
        typeColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                cell.getValue().getMota() + " (" + cell.getValue().getAzpimota() + ")"));
        dateColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getDataLaburra()));
        entityColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getEntitatea()));
        conceptColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getKontzeptua()));
        baseColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getFormatOinarria()));
        vatColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getFormatBez()));
        totalColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getFormatGuztira()));
        methodColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getOrdainketaMetodoa()));
        statusColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().getEgoera()));

        invoicesTable.setItems(filteredList);

        // Iragazki konboa
        filterTypeCombo.getItems().setAll("Guztiak", "Salmenta", "Erosketa");
        filterTypeCombo.setValue("Guztiak");
        filterTypeCombo.valueProperty().addListener((obs, oldVal, newVal) -> applyFilters());
        searchField.textProperty().addListener((obs, oldVal, newVal) -> applyFilters());

        // Formularioaren hasieraketa
        if (formTypeCombo != null) {
            formTypeCombo.getItems().setAll("Salmenta", "Erosketa");
            formTypeCombo.setValue("Erosketa");
            formTypeCombo.valueProperty().addListener((obs, oldVal, newVal) -> updateFormLabels(newVal));
        }
        if (formSubtypeCombo != null) {
            formSubtypeCombo.getItems().setAll("Sinplifikatua", "Osoa");
            formSubtypeCombo.setValue("Osoa");
        }
        if (formVatPercentCombo != null) {
            formVatPercentCombo.getItems().setAll("10%", "21%", "4%", "0%");
            formVatPercentCombo.setValue("10%");
        }
        if (formMethodCombo != null) {
            formMethodCombo.getItems().setAll("Txartela", "Transferentzia", "Eskudirua", "Bizum", "Helbideratua");
            formMethodCombo.setValue("Transferentzia");
        }
        if (formStatusCombo != null) {
            formStatusCombo.getItems().setAll("Ordaindua", "Zain", "Ezeztatua");
            formStatusCombo.setValue("Ordaindua");
        }

        refresh();
    }

    private void updateFormLabels(String mota) {
        if ("Erosketa".equalsIgnoreCase(mota)) {
            if (formEntityField != null) formEntityField.setPromptText("Hornitzailearen izena (Adib. Goierri Harategia)");
            if (formCifField != null) formCifField.setPromptText("Hornitzailearen CIF/NIF");
            if (formSubtypeCombo != null) formSubtypeCombo.setValue("Osoa");
        } else {
            if (formEntityField != null) formEntityField.setPromptText("Bezeroaren izena (edo utzi hutsik)");
            if (formCifField != null) formCifField.setPromptText("Bezeroaren NIF/DNI");
            if (formSubtypeCombo != null) formSubtypeCombo.setValue("Sinplifikatua");
        }
    }

    private void applyFilters() {
        String motaFilter = filterTypeCombo.getValue();
        String searchText = searchField.getText() == null ? "" : searchField.getText().trim().toLowerCase();

        filteredList.setPredicate(f -> {
            if (motaFilter != null && !"Guztiak".equalsIgnoreCase(motaFilter)) {
                if (!motaFilter.equalsIgnoreCase(f.getMota())) return false;
            }
            if (!searchText.isEmpty()) {
                boolean matchNum = f.getFakturaZenbakia().toLowerCase().contains(searchText);
                boolean matchConcept = f.getKontzeptua().toLowerCase().contains(searchText);
                boolean matchEntity = f.getEntitatea().toLowerCase().contains(searchText);
                return matchNum || matchConcept || matchEntity;
            }
            return true;
        });
    }

    @FXML
    public void refresh() {
        if (apiClient == null) return;
        invoiceMessage.setText("Fakturak eta finantza-laburpena kargatzen...");
        invoiceMessage.getStyleClass().setAll("inline-message");

        // 1. Finantza laburpena kargatu
        EkintzaAsinkronoak.run(
                apiClient::getFinantzaLaburpena,
                this::eguneratuFinantzaKpiak,
                error -> {
                    invoiceMessage.setText("Ezin izan da finantza laburpena lortu: " + error.getMessage());
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
                }
        );

        // 2. Fakturen zerrenda kargatu
        EkintzaAsinkronoak.run(
                () -> apiClient.getFakturak(null),
                zerrenda -> {
                    masterList.setAll(zerrenda);
                    applyFilters();
                    invoiceMessage.setText(zerrenda.size() + " faktura eskuragarri (REST API bidez eguneratuta).");
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-ok");
                },
                error -> {
                    invoiceMessage.setText("Errorea fakturak kargatzean: " + error.getMessage());
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
                }
        );
    }

    private void eguneratuFinantzaKpiak(FinantzaLaburpena laburpena) {
        if (laburpena == null) return;

        totalSalesLabel.setText(laburpena.getFormatSalmentak());
        salesCountLabel.setText(laburpena.salmentaKopurua + " salmenta-faktura");

        totalExpensesLabel.setText(laburpena.getFormatGastuak());
        expensesCountLabel.setText(laburpena.gastuKopurua + " hornitzaile-faktura");

        netProfitLabel.setText(laburpena.getFormatMozkina());
        if (laburpena.mozkinGarbia >= 0) {
            netProfitLabel.setStyle("-fx-text-fill: #16a34a;");
            netProfitSubtitle.setText("Mozkin garbi positiboa");
        } else {
            netProfitLabel.setStyle("-fx-text-fill: #dc2626;");
            netProfitSubtitle.setText("Gastuak salmentak baino handiagoak");
        }

        vatBalanceLabel.setText(laburpena.getFormatBezLikidazioa());
        vatDetailLabel.setText("Jasota: " + laburpena.getFormatJasotakoBez() + " | Ordainduta: " + laburpena.getFormatOrdaindutakoBez());
    }

    @FXML
    private void saveInvoice() {
        if (apiClient == null) return;

        String mota = formTypeCombo.getValue();
        String azpimota = formSubtypeCombo.getValue();
        String kontzeptua = formConceptField.getText();
        String entitatea = formEntityField.getText();
        String cif = formCifField.getText();
        String oinarriaStr = formBaseField.getText();
        String bezPercentStr = formVatPercentCombo.getValue().replace("%", "").trim();
        String metodoa = formMethodCombo.getValue();
        String egoera = formStatusCombo.getValue();
        String oharrak = formNotesField.getText();

        if (kontzeptua == null || kontzeptua.isBlank()) {
            invoiceMessage.setText("Kontzeptua zehaztea beharrezkoa da.");
            invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
            return;
        }

        double oinarria;
        try {
            oinarria = Double.parseDouble(oinarriaStr.replace(",", ".").trim());
            if (oinarria < 0) throw new NumberFormatException();
        } catch (Exception ex) {
            invoiceMessage.setText("Oinarri zergagarriaren balio numeriko baliogabea.");
            invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
            return;
        }

        double bezEhunekoa = Double.parseDouble(bezPercentStr);

        Map<String, Object> body = new LinkedHashMap<>();
        body.put("mota", mota);
        body.put("azpimota", azpimota);
        body.put("kontzeptua", kontzeptua.trim());
        if ("Erosketa".equalsIgnoreCase(mota)) {
            body.put("hornitzailea", entitatea != null ? entitatea.trim() : null);
            body.put("hornitzaileCif", cif != null ? cif.trim() : null);
        } else {
            body.put("bezeroIzena", entitatea != null ? entitatea.trim() : "Bezero Arrunta");
            body.put("bezeroNif", cif != null ? cif.trim() : null);
        }
        body.put("oinarriZergagarria", oinarria);
        body.put("bezEhunekoa", bezEhunekoa);
        body.put("ordainketaMetodoa", metodoa);
        body.put("egoera", egoera);
        body.put("oharrak", oharrak != null ? oharrak.trim() : null);

        saveInvoiceButton.setDisable(true);
        invoiceMessage.setText("Faktura erregistratzen...");
        invoiceMessage.getStyleClass().setAll("inline-message");

        EkintzaAsinkronoak.run(
                () -> {
                    apiClient.sortuFaktura(body);
                    return Boolean.TRUE;
                },
                result -> {
                    saveInvoiceButton.setDisable(false);
                    clearForm();
                    invoiceMessage.setText("Faktura berria arrakastaz gorde da!");
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-ok");
                    refresh();
                },
                error -> {
                    saveInvoiceButton.setDisable(false);
                    invoiceMessage.setText("Errorea faktura gordetzean: " + error.getMessage());
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
                }
        );
    }

    @FXML
    private void clearForm() {
        formConceptField.clear();
        formEntityField.clear();
        formCifField.clear();
        formBaseField.clear();
        formNotesField.clear();
    }

    @FXML
    private void markAsPaid() {
        Faktura hautatua = invoicesTable.getSelectionModel().getSelectedItem();
        if (hautatua == null) {
            invoiceMessage.setText("Hautatu faktura bat lehenik.");
            invoiceMessage.getStyleClass().setAll("inline-message", "status-warning");
            return;
        }

        EkintzaAsinkronoak.run(
                () -> {
                    apiClient.aldatuFakturaEgoera(hautatua.getId(), "Ordaindua");
                    return Boolean.TRUE;
                },
                result -> {
                    invoiceMessage.setText("Faktura #" + hautatua.getFakturaZenbakia() + " 'Ordaindua' gisa markatu da.");
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-ok");
                    refresh();
                },
                error -> {
                    invoiceMessage.setText("Errorea egoera eguneratzean: " + error.getMessage());
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
                }
        );
    }

    @FXML
    private void markAsCancelled() {
        Faktura hautatua = invoicesTable.getSelectionModel().getSelectedItem();
        if (hautatua == null) {
            invoiceMessage.setText("Hautatu faktura bat lehenik.");
            invoiceMessage.getStyleClass().setAll("inline-message", "status-warning");
            return;
        }

        EkintzaAsinkronoak.run(
                () -> {
                    apiClient.aldatuFakturaEgoera(hautatua.getId(), "Ezeztatua");
                    return Boolean.TRUE;
                },
                result -> {
                    invoiceMessage.setText("Faktura #" + hautatua.getFakturaZenbakia() + " 'Ezeztatua' gisa markatu da.");
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-warning");
                    refresh();
                },
                error -> {
                    invoiceMessage.setText("Errorea ezeztatzean: " + error.getMessage());
                    invoiceMessage.getStyleClass().setAll("inline-message", "status-error");
                }
        );
    }
}
