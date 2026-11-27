package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.api.ApiKontratua;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Lehengaia;
import eus.goierri.jatetxea.admin.model.LehengaiaAlerta;
import javafx.beans.property.ReadOnlyStringWrapper;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public final class OsagaiakKontrolagailua extends BaseAdminKontrolagailua {
    private final ObservableList<Lehengaia> ingredients = FXCollections.observableArrayList();
    private final ObservableList<String> stockAlerts = FXCollections.observableArrayList();

    @FXML private TableView<Lehengaia> ingredientsTable;
    @FXML private TableColumn<Lehengaia, String> nameColumn;
    @FXML private TableColumn<Lehengaia, String> quantityColumn;
    @FXML private TableColumn<Lehengaia, String> minimumColumn;
    @FXML private TableColumn<Lehengaia, String> unitColumn;
    @FXML private TableColumn<Lehengaia, String> supplierColumn;
    @FXML private TableColumn<Lehengaia, String> alertColumn;
    @FXML private javafx.scene.control.ListView<String> alertList;
    @FXML private Label alertCountLabel;
    @FXML private Label formTitle;
    @FXML private javafx.scene.layout.VBox initialStockBox;
    @FXML private TextField nameField;
    @FXML private javafx.scene.control.ComboBox<String> unitCombo;
    @FXML private TextField stockField;
    @FXML private TextField minimumStockField;
    @FXML private TextField costField;
    @FXML private TextField supplierField;
    @FXML private TextField entryQuantityField;
    @FXML private TextField entryNoteField;
    @FXML private Button createButton;
    @FXML private Button editButton;
    @FXML private Button cancelEditButton;
    @FXML private Button stockEntryButton;
    @FXML private Button deactivateButton;
    @FXML private Label ingredientsMessage;
    private int editingIngredientId;

    public OsagaiakKontrolagailua() {
        this(null);
    }

    public OsagaiakKontrolagailua(ApiKontratua apiClient) {
        super(apiClient, null);
    }

    @Override
    public String getPantailaIzena() {
        return "Osagaiak eta inbentarioa";
    }

    @FXML
    private void initialize() {
        nameColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().izena));
        quantityColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                formatNumber(cell.getValue().unekoStocka)));
        minimumColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                formatNumber(cell.getValue().gutxienekoStocka)));
        unitColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().unitatea));
        supplierColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                cell.getValue().hornitzailea == null ? "—" : cell.getValue().hornitzailea));
        alertColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                cell.getValue().unekoStocka <= cell.getValue().gutxienekoStocka ? "Stock baxua" : ""));
        ingredientsTable.setItems(ingredients);
        alertList.setItems(stockAlerts);
        unitCombo.setItems(FXCollections.observableArrayList("kg", "g", "l", "ml", "unitatea"));
        unitCombo.getSelectionModel().select("kg");
        ingredientsTable.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, newValue) -> {
                    if (editingIngredientId != 0
                            && (newValue == null || newValue.id != editingIngredientId)) {
                        beginNewIngredient();
                    }
                    updateSelectionButtons();
                });
        beginNewIngredient();
        updateSelectionButtons();
        if (apiClient != null) {
            refresh();
        }
    }

    @FXML
    @Override
    public void refresh() {
        ingredientsMessage.setText("Osagaiak kargatzen...");
        EkintzaAsinkronoak.run(() -> new InventoryData(
                        apiClient.getList("/lehengaiak", Lehengaia.class),
                        apiClient.getList("/lehengaiak/alertak", LehengaiaAlerta.class)),
                data -> {
                    ingredients.setAll(data.ingredients());
                    stockAlerts.setAll(data.alerts().stream()
                            .map(alert -> alert.izena + " · erosi "
                                    + formatNumber(alert.proposatutakoErosketa) + " " + alert.unitatea
                                    + (alert.hornitzailea == null || alert.hornitzailea.isBlank()
                                            ? "" : " · " + alert.hornitzailea))
                            .toList());
                    alertCountLabel.setText(data.alerts().size() + " stock-alerta");
                    ingredientsMessage.setText(data.ingredients().size() + " osagai.");
                    updateSelectionButtons();
                },
                error -> ingredientsMessage.setText("Ezin izan dira osagaiak kargatu: " + error.getMessage()));
    }

    @FXML
    private void saveIngredient() {
        String name = nameField.getText().trim();
        String unit = unitCombo.getValue();
        String supplier = supplierField.getText().trim();
        if (name.isBlank() || unit == null) {
            ingredientsMessage.setText("Osagaiaren izena eta unitatea beharrezkoak dira.");
            return;
        }

        final double stock;
        final double minimum;
        final double cost;
        try {
            stock = editingIngredientId == 0
                    ? PantailaLaguntza.parseNonNegative(stockField.getText(), "Hasierako stocka")
                    : 0;
            minimum = PantailaLaguntza.parseNonNegative(minimumStockField.getText(), "Gutxieneko stocka");
            cost = PantailaLaguntza.parseNonNegative(costField.getText(), "Unitateko kostua");
        } catch (IllegalArgumentException ex) {
            ingredientsMessage.setText(ex.getMessage());
            return;
        }

        Map<String, Object> body = new LinkedHashMap<>();
        body.put("izena", name);
        body.put("unitatea", unit);
        body.put("gutxienekoStocka", minimum);
        body.put("kostuUnitarioa", cost);
        body.put("hornitzailea", supplier.isBlank() ? null : supplier);
        int targetIngredientId = editingIngredientId;
        boolean creating = targetIngredientId == 0;
        if (creating) {
            body.put("unekoStocka", stock);
        }
        createButton.setDisable(true);
        editButton.setDisable(true);
        cancelEditButton.setDisable(true);
        ingredientsMessage.setText(creating ? "Osagaia sortzen..." : "Osagaia eguneratzen...");
        EkintzaAsinkronoak.run(() -> {
                    if (creating) {
                        apiClient.post("/lehengaiak", body);
                    } else {
                        apiClient.put("/lehengaiak/" + targetIngredientId, body);
                    }
                    return null;
                },
                ignored -> {
                    beginNewIngredient();
                    ingredientsMessage.setText(creating
                            ? "Osagaia sortu da."
                            : "Osagaiaren datuak eguneratu dira; stocka aldatu gabe mantendu da.");
                    createButton.setDisable(false);
                    refresh();
                },
                error -> {
                    ingredientsMessage.setText("Ezin izan da osagaia gorde: " + error.getMessage());
                    createButton.setDisable(false);
                    cancelEditButton.setDisable(false);
                    updateSelectionButtons();
                });
    }

    @FXML
    private void editIngredient() {
        Lehengaia selected = ingredientsTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            ingredientsMessage.setText("Hautatu editatu nahi duzun osagaia.");
            return;
        }
        editingIngredientId = selected.id;
        nameField.setText(selected.izena);
        unitCombo.setValue(selected.unitatea);
        minimumStockField.setText(Double.toString(selected.gutxienekoStocka));
        costField.setText(Double.toString(selected.kostuUnitarioa));
        supplierField.setText(selected.hornitzailea == null ? "" : selected.hornitzailea);
        initialStockBox.setVisible(false);
        initialStockBox.setManaged(false);
        formTitle.setText("Osagaia editatzen");
        createButton.setText("Gorde aldaketak");
        cancelEditButton.setVisible(true);
        cancelEditButton.setManaged(true);
        ingredientsMessage.setText(selected.izena + " editatzeko prest. Stocka ez da aldatuko.");
    }

    @FXML
    private void cancelEdit() {
        beginNewIngredient();
        ingredientsMessage.setText("Osagai berria sortzeko formularioa prest.");
    }

    @FXML
    private void registerStockEntry() {
        Lehengaia selected = ingredientsTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            ingredientsMessage.setText("Hautatu osagai bat stock-sarrera erregistratzeko.");
            return;
        }
        final double quantity;
        try {
            quantity = PantailaLaguntza.parsePositive(entryQuantityField.getText(), "Sarrerako kantitatea");
        } catch (IllegalArgumentException ex) {
            ingredientsMessage.setText(ex.getMessage());
            return;
        }

        String note = entryNoteField.getText().trim();
        stockEntryButton.setDisable(true);
        ingredientsMessage.setText("Stock-sarrera erregistratzen...");
        EkintzaAsinkronoak.run(() -> {
                    Map<String, Object> body = new LinkedHashMap<>();
                    body.put("lehengaiaId", selected.id);
                    body.put("kantitatea", quantity);
                    body.put("oharrak", note);
                    apiClient.post("/lehengaiak/sarrera", body);
                    return null;
                },
                ignored -> {
                    entryQuantityField.clear();
                    entryNoteField.clear();
                    ingredientsMessage.setText("Stock-sarrera erregistratu da.");
                    stockEntryButton.setDisable(false);
                    refresh();
                },
                error -> {
                    ingredientsMessage.setText("Ezin izan da stock-sarrera erregistratu: " + error.getMessage());
                    stockEntryButton.setDisable(false);
                });
    }

    @FXML
    private void deactivateIngredient() {
        Lehengaia selected = ingredientsTable.getSelectionModel().getSelectedItem();
        if (selected == null || !PantailaLaguntza.confirm(
                "Osagaia desgaitu", selected.izena + " osagaia desgaitu (soft delete)?")) {
            return;
        }
        deactivateButton.setDisable(true);
        EkintzaAsinkronoak.run(() -> {
                    apiClient.delete("/lehengaiak/" + selected.id);
                    return null;
                },
                ignored -> {
                    ingredientsMessage.setText("Osagaia desgaitu da.");
                    refresh();
                },
                error -> {
                    ingredientsMessage.setText("Ezin izan da osagaia desgaitu: " + error.getMessage());
                    updateSelectionButtons();
                });
    }

    private void beginNewIngredient() {
        editingIngredientId = 0;
        nameField.clear();
        unitCombo.getSelectionModel().select("kg");
        stockField.clear();
        minimumStockField.clear();
        costField.clear();
        supplierField.clear();
        initialStockBox.setVisible(true);
        initialStockBox.setManaged(true);
        formTitle.setText("Osagai berria");
        createButton.setText("Osagaia sortu");
        cancelEditButton.setDisable(false);
        cancelEditButton.setVisible(false);
        cancelEditButton.setManaged(false);
    }

    private void updateSelectionButtons() {
        boolean selected = ingredientsTable.getSelectionModel().getSelectedItem() != null;
        stockEntryButton.setDisable(!selected);
        deactivateButton.setDisable(!selected);
        editButton.setDisable(!selected);
    }

    private static String formatNumber(double number) {
        return String.format(java.util.Locale.ROOT, "%.2f", number);
    }

    private record InventoryData(List<Lehengaia> ingredients, List<LehengaiaAlerta> alerts) {
    }
}
