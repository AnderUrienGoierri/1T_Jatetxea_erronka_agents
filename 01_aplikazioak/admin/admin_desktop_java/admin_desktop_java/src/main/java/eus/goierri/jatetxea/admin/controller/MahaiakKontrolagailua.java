package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Mahaia;
import javafx.beans.property.ReadOnlyStringWrapper;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;

import java.io.IOException;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public final class MahaiakKontrolagailua {
    private final ApiBezeroa apiClient;
    private final KudeatzaileSaioa session;
    private final ObservableList<Mahaia> tables = FXCollections.observableArrayList();
    private boolean busy;

    @FXML private TableView<Mahaia> tablesTable;
    @FXML private TableColumn<Mahaia, String> numberColumn;
    @FXML private TableColumn<Mahaia, String> capacityColumn;
    @FXML private TableColumn<Mahaia, String> areaColumn;
    @FXML private TableColumn<Mahaia, String> stateColumn;
    @FXML private TextField numberField;
    @FXML private TextField capacityField;
    @FXML private ComboBox<String> areaCombo;
    @FXML private ComboBox<String> stateCombo;
    @FXML private Button createButton;
    @FXML private Button editTableButton;
    @FXML private Button cancelEditButton;
    @FXML private Button updateStateButton;
    @FXML private Button deactivateButton;
    @FXML private Label tablesMessage;
    private int editingTableId;

    public MahaiakKontrolagailua() {
        this(null, null);
    }

    public MahaiakKontrolagailua(ApiBezeroa apiClient, KudeatzaileSaioa session) {
        this.apiClient = apiClient;
        this.session = session;
    }

    @FXML
    private void initialize() {
        numberColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                Integer.toString(cell.getValue().zenbakia)));
        capacityColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                Integer.toString(cell.getValue().edukiera)));
        areaColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().kokapena));
        stateColumn.setCellValueFactory(cell -> {
            Mahaia table = cell.getValue();
            String state = table.zerbitzuan && "Libre".equals(table.egoera) ? "Zerbitzuan" : table.egoera;
            return new ReadOnlyStringWrapper(state + (table.erreserbaAktiboa ? " · Erreserba" : ""));
        });
        tablesTable.setItems(tables);
        stateCombo.setItems(FXCollections.observableArrayList(
                "Libre", "Okupatuta", "Erreserbatuta", "Garbitzen"));
        areaCombo.setItems(FXCollections.observableArrayList(
                "Barran", "Barra", "Jantoki Nagusia", "Terraza", "VIP"));
        areaCombo.getSelectionModel().select("Jantoki Nagusia");
        beginNewTable();
        tablesTable.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, selected) -> {
                    if (editingTableId != 0 && (selected == null || selected.id != editingTableId)) {
                        beginNewTable();
                    }
                    if (selected != null) {
                        stateCombo.setValue(selected.egoera);
                    }
                    updateWriteButtons();
                });
        if (session != null) {
            session.configurationWritableProperty().addListener(
                    (observable, oldValue, newValue) -> updateWriteButtons());
            updateWriteButtons();
        } else {
            createButton.setDisable(true);
            updateStateButton.setDisable(true);
            deactivateButton.setDisable(true);
        }
        if (apiClient != null) {
            refresh();
        }
    }

    @FXML
    private void refresh() {
        tablesMessage.setText("Mahaiak kargatzen...");
        EkintzaAsinkronoak.run(() -> apiClient.getList("/mahaiak", Mahaia.class),
                data -> {
                    tables.setAll(data);
                    tablesMessage.setText(data.size() + " mahai.");
                    updateWriteButtons();
                },
                error -> tablesMessage.setText("Ezin izan dira mahaiak kargatu: " + error.getMessage()));
    }

    @FXML
    private void createTable() {
        if (editingTableId != 0) {
            return;
        }
        final int number;
        final int capacity;
        try {
            number = Integer.parseInt(numberField.getText().trim());
            capacity = Integer.parseInt(capacityField.getText().trim());
            if (number < 1 || capacity < 1 || capacity > 100) {
                throw new NumberFormatException();
            }
        } catch (NumberFormatException ex) {
            tablesMessage.setText("Mahai-zenbakia eta edukiera zenbaki positiboak izan behar dira.");
            return;
        }
        String area = areaCombo.getValue();
        if (area == null) {
            tablesMessage.setText("Mahaiaren eremua beharrezkoa da.");
            return;
        }
        setBusy(true);
        EkintzaAsinkronoak.run(() -> {
                    verifyServiceClosed();
                    apiClient.post("/mahaiak", Map.of(
                            "zenbakia", number,
                            "edukiera", capacity,
                            "kokapena", area));
                    return null;
                },
                ignored -> {
                    session.setServiceState(true, "Zerbitzua itxita · konfigurazio-aldaketak baimenduta");
                    numberField.clear();
                    capacityField.clear();
                    areaCombo.getSelectionModel().select("Jantoki Nagusia");
                    tablesMessage.setText("Mahaia sortu da.");
                    setBusy(false);
                    refresh();
                },
                error -> {
                    session.setServiceState(false, "Mahai-aldaketa huts egin du; egiaztatu zerbitzu-egoera.");
                    tablesMessage.setText("Ezin izan da mahaia sortu: " + error.getMessage());
                    setBusy(false);
                });
    }

    @FXML
    private void editTable() {
        if (editingTableId != 0) {
            saveTableDetails();
            return;
        }
        Mahaia selected = tablesTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            tablesMessage.setText("Hautatu editatu nahi duzun mahaia.");
            return;
        }
        if (selected.zerbitzuan || selected.erreserbaAktiboa || !"Libre".equals(selected.egoera)) {
            tablesMessage.setText("Mahaia ezin da editatu: libre egon behar du eta ez du erreserba aktiborik izan behar.");
            return;
        }
        editingTableId = selected.id;
        numberField.setText(Integer.toString(selected.zenbakia));
        capacityField.setText(Integer.toString(selected.edukiera));
        areaCombo.setValue(selected.kokapena);
        editTableButton.setText("Gorde mahaiaren aldaketak");
        cancelEditButton.setVisible(true);
        cancelEditButton.setManaged(true);
        tablesMessage.setText(selected.zenbakia + ". mahaia editatzeko prest.");
        updateWriteButtons();
    }

    private void saveTableDetails() {
        Mahaia selected = tablesTable.getSelectionModel().getSelectedItem();
        if (selected == null || selected.id != editingTableId) {
            tablesMessage.setText("Hautatu editatzen ari zaren mahaia berriro.");
            return;
        }
        final int number;
        final int capacity;
        try {
            number = Integer.parseInt(numberField.getText().trim());
            capacity = Integer.parseInt(capacityField.getText().trim());
            if (number < 1 || number > 1000 || capacity < 1 || capacity > 100) {
                throw new NumberFormatException();
            }
        } catch (NumberFormatException ex) {
            tablesMessage.setText("Mahai-zenbakia (1-1000) eta edukiera (1-100) baliozkoak izan behar dira.");
            return;
        }
        String area = areaCombo.getValue();
        if (area == null) {
            tablesMessage.setText("Mahaiaren eremua beharrezkoa da.");
            return;
        }

        int targetTableId = editingTableId;
        setBusy(true);
        tablesMessage.setText("Mahaiaren datuak eguneratzen...");
        EkintzaAsinkronoak.run(() -> {
                    apiClient.put("/mahaiak/" + targetTableId, Map.of(
                            "zenbakia", number,
                            "edukiera", capacity,
                            "kokapena", area));
                    return null;
                },
                ignored -> {
                    beginNewTable();
                    tablesMessage.setText("Mahaiaren datuak eguneratu dira.");
                    setBusy(false);
                    refresh();
                },
                error -> {
                    tablesMessage.setText("Ezin izan dira mahaiaren datuak eguneratu: " + error.getMessage());
                    setBusy(false);
                });
    }

    @FXML
    private void cancelEdit() {
        beginNewTable();
        tablesMessage.setText("Mahai berria sortzeko formularioa prest.");
    }

    @FXML
    private void updateTableState() {
        Mahaia selected = tablesTable.getSelectionModel().getSelectedItem();
        String state = stateCombo.getValue();
        if (selected == null || state == null) {
            tablesMessage.setText("Hautatu mahaia eta egoera berria.");
            return;
        }
        setBusy(true);
        EkintzaAsinkronoak.run(() -> {
                    verifyServiceClosed();
                    apiClient.put("/mahaiak/" + selected.id + "/egoera", Map.of("egoera", state));
                    return null;
                },
                ignored -> {
                    session.setServiceState(true, "Zerbitzua itxita · konfigurazio-aldaketak baimenduta");
                    tablesMessage.setText("Mahaiaren egoera eguneratu da.");
                    setBusy(false);
                    refresh();
                },
                error -> {
                    session.setServiceState(false, "Mahai-aldaketa huts egin du; egiaztatu zerbitzu-egoera.");
                    tablesMessage.setText("Ezin izan da egoera eguneratu: " + error.getMessage());
                    setBusy(false);
                });
    }

    @FXML
    private void deactivateTable() {
        Mahaia selected = tablesTable.getSelectionModel().getSelectedItem();
        if (selected == null || !PantailaLaguntza.confirm(
                "Mahaia desgaitu", selected.zenbakia + ". mahaia desgaitu (soft delete)?")) {
            return;
        }
        setBusy(true);
        EkintzaAsinkronoak.run(() -> {
                    verifyServiceClosed();
                    apiClient.delete("/mahaiak/" + selected.id);
                    return null;
                },
                ignored -> {
                    session.setServiceState(true, "Zerbitzua itxita · konfigurazio-aldaketak baimenduta");
                    tablesMessage.setText("Mahaia desgaitu da.");
                    setBusy(false);
                    refresh();
                },
                error -> {
                    session.setServiceState(false, "Mahai-aldaketa huts egin du; egiaztatu zerbitzu-egoera.");
                    tablesMessage.setText("Ezin izan da mahaia desgaitu: " + error.getMessage());
                    setBusy(false);
                });
    }

    private void verifyServiceClosed() throws IOException, InterruptedException {
        ApiBezeroa.ServiceStatus status = apiClient.getServiceStatus();
        if (status.active() || status.openTables() > 0) {
            throw new IOException("Zerbitzua irekita dago (" + status.openTables()
                    + " mahai). Mahaien aldaketak blokeatuta daude.");
        }
    }

    private void setBusy(boolean value) {
        busy = value;
        updateWriteButtons();
    }

    private void updateWriteButtons() {
        if (session == null) {
            createButton.setDisable(true);
            editTableButton.setDisable(true);
            cancelEditButton.setDisable(true);
            updateStateButton.setDisable(true);
            deactivateButton.setDisable(true);
            return;
        }
        boolean blocked = busy || !session.isConfigurationWritable();
        boolean selected = tablesTable.getSelectionModel().getSelectedItem() != null;
        Mahaia selectedTable = tablesTable.getSelectionModel().getSelectedItem();
        boolean editAllowed = selectedTable != null && !selectedTable.zerbitzuan
                && !selectedTable.erreserbaAktiboa && "Libre".equals(selectedTable.egoera);
        createButton.setDisable(blocked || editingTableId != 0);
        editTableButton.setDisable(busy || !selected || (editingTableId == 0 && !editAllowed));
        editTableButton.setText(editingTableId == 0 ? "Hautatutakoa editatu" : "Gorde mahaiaren aldaketak");
        cancelEditButton.setDisable(busy);
        updateStateButton.setDisable(blocked || !selected);
        deactivateButton.setDisable(blocked || !selected);
    }

    private void beginNewTable() {
        editingTableId = 0;
        numberField.clear();
        capacityField.clear();
        areaCombo.getSelectionModel().select("Jantoki Nagusia");
        editTableButton.setText("Hautatutakoa editatu");
        cancelEditButton.setVisible(false);
        cancelEditButton.setManaged(false);
        updateWriteButtons();
    }
}
