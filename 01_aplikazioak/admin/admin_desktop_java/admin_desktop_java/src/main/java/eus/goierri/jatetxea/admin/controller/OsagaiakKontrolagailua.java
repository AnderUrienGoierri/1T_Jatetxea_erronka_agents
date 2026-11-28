package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.api.ApiKontratua;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Hornitzailea;
import eus.goierri.jatetxea.admin.model.Lehengaia;
import eus.goierri.jatetxea.admin.model.LehengaiaAlerta;
import javafx.beans.property.ReadOnlyStringWrapper;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;
import javafx.scene.layout.VBox;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

public final class OsagaiakKontrolagailua extends BaseAdminKontrolagailua {
    private final ObservableList<Lehengaia> ingredients = FXCollections.observableArrayList();
    private final ObservableList<Hornitzailea> suppliers = FXCollections.observableArrayList();
    private final ObservableList<Lehengaia> supplierIngredients = FXCollections.observableArrayList();
    private final ObservableList<String> stockAlerts = FXCollections.observableArrayList();

    @FXML private TableView<Lehengaia> ingredientsTable;
    @FXML private TableColumn<Lehengaia, String> nameColumn;
    @FXML private TableColumn<Lehengaia, String> quantityColumn;
    @FXML private TableColumn<Lehengaia, String> minimumColumn;
    @FXML private TableColumn<Lehengaia, String> unitColumn;
    @FXML private TableColumn<Lehengaia, String> supplierColumn;
    @FXML private TableColumn<Lehengaia, String> alertColumn;
    @FXML private ListView<String> alertList;
    @FXML private Label alertCountLabel;
    @FXML private Label formTitle;
    @FXML private VBox initialStockBox;
    @FXML private TextField nameField;
    @FXML private ComboBox<String> unitCombo;
    @FXML private TextField stockField;
    @FXML private TextField minimumStockField;
    @FXML private TextField costField;
    @FXML private ComboBox<Hornitzailea> supplierCombo;

    @FXML private ComboBox<Hornitzailea> entrySupplierCombo;
    @FXML private Label entrySupplierInfoLabel;
    @FXML private ComboBox<Lehengaia> entryIngredientCombo;
    @FXML private Label entryQuantityLabel;
    @FXML private TextField entryQuantityField;
    @FXML private ComboBox<String> entryMethodCombo;
    @FXML private TextField entryAlbaranField;
    @FXML private TextField entryNoteField;
    @FXML private Label entryCostDetailLabel;
    @FXML private Label entryTotalCostLabel;

    @FXML private Button createButton;
    @FXML private Button editButton;
    @FXML private Button cancelEditButton;
    @FXML private Button stockEntryButton;
    @FXML private Button deactivateButton;
    @FXML private Label ingredientsMessage;
    private int editingIngredientId;
    private boolean syncingEntrySelection;

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

        if (supplierCombo != null) {
            supplierCombo.setItems(suppliers);
        }

        if (entryMethodCombo != null) {
            entryMethodCombo.setItems(FXCollections.observableArrayList("Transferentzia", "Txartela", "Eskudirua"));
            entryMethodCombo.setValue("Transferentzia");
        }

        if (entrySupplierCombo != null) {
            entrySupplierCombo.setItems(suppliers);
            entrySupplierCombo.valueProperty().addListener((obs, oldVal, newVal) -> {
                if (!syncingEntrySelection) {
                    updateEntryProductsForSupplier(newVal, null);
                }
            });
        }

        if (entryIngredientCombo != null) {
            entryIngredientCombo.setItems(supplierIngredients);
            entryIngredientCombo.valueProperty().addListener((obs, oldVal, newVal) -> {
                updateEntryCostPreview();
                updateSelectionButtons();
            });
        }

        if (entryQuantityField != null) {
            entryQuantityField.textProperty().addListener((obs, oldVal, newVal) -> updateEntryCostPreview());
        }

        ingredientsTable.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, newValue) -> {
                    if (editingIngredientId != 0
                            && (newValue == null || newValue.id != editingIngredientId)) {
                        beginNewIngredient();
                    }
                    if (newValue != null) {
                        syncEntryWithIngredient(newValue);
                    }
                    updateSelectionButtons();
                });

        beginNewIngredient();
        updateEntryCostPreview();
        updateSelectionButtons();
        if (apiClient != null) {
            refresh();
        }
    }

    @FXML
    @Override
    public void refresh() {
        if (apiClient == null) return;
        Integer prevSupplierId = entrySupplierCombo != null && entrySupplierCombo.getValue() != null
                ? entrySupplierCombo.getValue().id : null;
        Integer prevIngredientId = entryIngredientCombo != null && entryIngredientCombo.getValue() != null
                ? entryIngredientCombo.getValue().id : null;

        ingredientsMessage.setText("Osagaiak eta hornitzaileak kargatzen...");
        EkintzaAsinkronoak.run(() -> new InventoryData(
                        apiClient.getList("/hornitzaileak", Hornitzailea.class),
                        apiClient.getList("/lehengaiak", Lehengaia.class),
                        apiClient.getList("/lehengaiak/alertak", LehengaiaAlerta.class)),
                data -> {
                    suppliers.setAll(data.suppliers());
                    ingredients.setAll(data.ingredients());
                    stockAlerts.setAll(data.alerts().stream()
                            .map(alert -> alert.izena + " · erosi "
                                    + formatNumber(alert.proposatutakoErosketa) + " " + alert.unitatea
                                    + (alert.hornitzailea == null || alert.hornitzailea.isBlank()
                                            ? "" : " · " + alert.hornitzailea))
                            .toList());
                    alertCountLabel.setText(data.alerts().size() + " stock-alerta");
                    ingredientsMessage.setText(data.ingredients().size() + " osagai · " + data.suppliers().size() + " hornitzaile.");

                    if (entrySupplierCombo != null && !suppliers.isEmpty()) {
                        Hornitzailea chosen = suppliers.stream()
                                .filter(h -> prevSupplierId != null && h.id == prevSupplierId)
                                .findFirst()
                                .orElse(suppliers.get(0));
                        syncingEntrySelection = true;
                        entrySupplierCombo.setValue(chosen);
                        syncingEntrySelection = false;
                        updateEntryProductsForSupplier(chosen, prevIngredientId);
                    } else if (entryIngredientCombo != null) {
                        supplierIngredients.setAll(ingredients);
                        if (!supplierIngredients.isEmpty()) {
                            entryIngredientCombo.setValue(supplierIngredients.get(0));
                        }
                        updateEntryCostPreview();
                    }
                    updateSelectionButtons();
                },
                error -> ingredientsMessage.setText("Ezin izan dira osagaiak kargatu: " + error.getMessage()));
    }

    private Hornitzailea findSupplierForIngredient(Lehengaia ingredient) {
        if (ingredient.hornitzaileaId != null) {
            for (Hornitzailea h : suppliers) {
                if (h.id == ingredient.hornitzaileaId) return h;
            }
        }
        if (ingredient.hornitzailea != null && !ingredient.hornitzailea.isBlank()) {
            for (Hornitzailea h : suppliers) {
                if (ingredient.hornitzailea.equalsIgnoreCase(h.izena)) return h;
            }
        }
        return null;
    }

    private void syncEntryWithIngredient(Lehengaia ingredient) {
        if (entrySupplierCombo == null || entryIngredientCombo == null) return;
        Hornitzailea matched = findSupplierForIngredient(ingredient);
        if (matched != null) {
            syncingEntrySelection = true;
            entrySupplierCombo.setValue(matched);
            syncingEntrySelection = false;
            updateEntryProductsForSupplier(matched, ingredient.id);
        } else {
            entryIngredientCombo.setValue(ingredient);
            updateEntryCostPreview();
        }
    }

    private void updateEntryProductsForSupplier(Hornitzailea supplier, Integer selectIngredientId) {
        if (supplier == null) {
            supplierIngredients.setAll(ingredients);
            if (entrySupplierInfoLabel != null) {
                entrySupplierInfoLabel.setText("Aukeratu hornitzaile bat bere produktuak ikusteko");
            }
        } else {
            if (entrySupplierInfoLabel != null) {
                entrySupplierInfoLabel.setText(String.format(Locale.ROOT,
                        "CIF: %s · Mota: %s · BEZ: %.0f%%",
                        supplier.cif, supplier.produktuMota, supplier.bezEhunekoa));
            }
            List<Lehengaia> filtered = ingredients.stream()
                    .filter(l -> (l.hornitzaileaId != null && l.hornitzaileaId == supplier.id)
                            || (l.hornitzailea != null && l.hornitzailea.equalsIgnoreCase(supplier.izena)))
                    .toList();
            supplierIngredients.setAll(filtered);
        }

        if (entryIngredientCombo != null) {
            if (supplierIngredients.isEmpty()) {
                entryIngredientCombo.setValue(null);
            } else {
                Lehengaia chosen = supplierIngredients.stream()
                        .filter(l -> selectIngredientId != null && l.id == selectIngredientId)
                        .findFirst()
                        .orElse(supplierIngredients.get(0));
                entryIngredientCombo.setValue(chosen);
            }
        }
        updateEntryCostPreview();
        updateSelectionButtons();
    }

    private void updateEntryCostPreview() {
        Lehengaia selected = entryIngredientCombo != null ? entryIngredientCombo.getValue() : null;
        Hornitzailea supplier = entrySupplierCombo != null ? entrySupplierCombo.getValue() : null;

        if (entryQuantityLabel != null) {
            entryQuantityLabel.setText(selected != null
                    ? String.format(Locale.ROOT, "3. Kantitatea (%s · %.2f €)", selected.unitatea, selected.kostuUnitarioa)
                    : "3. Kantitatea");
        }

        double quantity = 0.0;
        if (entryQuantityField != null && entryQuantityField.getText() != null && !entryQuantityField.getText().isBlank()) {
            try {
                double parsed = Double.parseDouble(entryQuantityField.getText().trim().replace(',', '.'));
                if (parsed > 0) quantity = parsed;
            } catch (NumberFormatException ignored) {
            }
        }

        double unitCost = selected != null ? selected.kostuUnitarioa : 0.0;
        double vatRate = supplier != null
                ? supplier.bezEhunekoa
                : (selected != null && selected.bezEhunekoa > 0 ? selected.bezEhunekoa : 10.0);
        double base = Math.round(quantity * unitCost * 100.0) / 100.0;
        double vat = Math.round(base * (vatRate / 100.0) * 100.0) / 100.0;
        double total = base + vat;

        if (entryCostDetailLabel != null) {
            entryCostDetailLabel.setText(String.format(Locale.ROOT,
                    "Oinarria: %.2f € · BEZ (%.0f%%): %.2f €", base, vatRate, vat));
        }
        if (entryTotalCostLabel != null) {
            entryTotalCostLabel.setText(String.format(Locale.ROOT,
                    "GUZTIRA ORDAINTZEKO: %.2f €", total));
        }
    }

    @FXML
    private void saveIngredient() {
        String name = nameField.getText().trim();
        String unit = unitCombo.getValue();
        Hornitzailea selectedSupplier = supplierCombo != null ? supplierCombo.getValue() : null;
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
        if (selectedSupplier != null) {
            body.put("hornitzaileaId", selectedSupplier.id);
            body.put("hornitzailea", selectedSupplier.izena);
        }
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
        if (supplierCombo != null) {
            supplierCombo.setValue(findSupplierForIngredient(selected));
        }
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
        Hornitzailea supplier = entrySupplierCombo != null ? entrySupplierCombo.getValue() : null;
        Lehengaia selected = entryIngredientCombo != null ? entryIngredientCombo.getValue() : null;
        if (selected == null) {
            selected = ingredientsTable.getSelectionModel().getSelectedItem();
        }
        if (selected == null) {
            ingredientsMessage.setText("Hautatu hornitzaile bat eta osagai bat albarana erregistratzeko.");
            return;
        }
        final double quantity;
        try {
            quantity = PantailaLaguntza.parsePositive(entryQuantityField.getText(), "Sarrerako kantitatea");
        } catch (IllegalArgumentException ex) {
            ingredientsMessage.setText(ex.getMessage());
            return;
        }

        String method = entryMethodCombo != null && entryMethodCombo.getValue() != null
                ? entryMethodCombo.getValue() : "Transferentzia";
        String albaran = entryAlbaranField != null ? entryAlbaranField.getText().trim() : "";
        String note = entryNoteField != null ? entryNoteField.getText().trim() : "";

        double vatRate = supplier != null
                ? supplier.bezEhunekoa
                : (selected.bezEhunekoa > 0 ? selected.bezEhunekoa : 10.0);
        double base = Math.round(quantity * selected.kostuUnitarioa * 100.0) / 100.0;
        double vat = Math.round(base * (vatRate / 100.0) * 100.0) / 100.0;
        double total = base + vat;
        String supplierName = supplier != null ? supplier.izena : (selected.hornitzailea != null ? selected.hornitzailea : "Hornitzailea");

        String confirmText = String.format(Locale.ROOT,
                "Hornitzailea: %s%nProduktua: %s (+%.2f %s)%nOinarria: %.2f € · BEZ (%.0f%%): %.2f €%nGUZTIRA ORDAINTZEKO: %.2f € (%s)%n%nStocka igoko da eta Erosketa-faktura kontabilitatean gordeko da.",
                supplierName, selected.izena, quantity, selected.unitatea, base, vatRate, vat, total, method);
        if (!PantailaLaguntza.confirm("Ordainketa simulazioa", confirmText)) {
            return;
        }

        final Lehengaia targetIngredient = selected;
        final Integer targetSupplierId = supplier != null ? supplier.id : selected.hornitzaileaId;
        stockEntryButton.setDisable(true);
        ingredientsMessage.setText("Albarana eta erosketa-faktura erregistratzen...");
        EkintzaAsinkronoak.run(() -> {
                    Map<String, Object> body = new LinkedHashMap<>();
                    if (targetSupplierId != null) {
                        body.put("hornitzaileaId", targetSupplierId);
                    }
                    body.put("lehengaiaId", targetIngredient.id);
                    body.put("kantitatea", quantity);
                    if (!albaran.isBlank()) {
                        body.put("albaranZenbakia", albaran);
                    }
                    body.put("ordainketaMetodoa", method);
                    body.put("sortuFaktura", true);
                    if (!note.isBlank()) {
                        body.put("oharrak", note);
                    }
                    apiClient.post("/lehengaiak/sarrera", body);
                    return null;
                },
                ignored -> {
                    entryQuantityField.clear();
                    if (entryAlbaranField != null) entryAlbaranField.clear();
                    if (entryNoteField != null) entryNoteField.clear();
                    ingredientsMessage.setText(String.format(Locale.ROOT,
                            "✓ +%.2f %s %s sartu dira eta %.2f €-ko erosketa-faktura kontabilitatean erregistratu da (%s).",
                            quantity, targetIngredient.unitatea, targetIngredient.izena, total, method));
                    stockEntryButton.setDisable(false);
                    refresh();
                },
                error -> {
                    ingredientsMessage.setText("Ezin izan da albarana erregistratu: " + error.getMessage());
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
        if (supplierCombo != null) {
            supplierCombo.setValue(null);
        }
        initialStockBox.setVisible(true);
        initialStockBox.setManaged(true);
        formTitle.setText("Osagai berria");
        createButton.setText("Osagaia sortu");
        cancelEditButton.setDisable(false);
        cancelEditButton.setVisible(false);
        cancelEditButton.setManaged(false);
    }

    private void updateSelectionButtons() {
        boolean tableSelected = ingredientsTable.getSelectionModel().getSelectedItem() != null;
        boolean entrySelected = (entryIngredientCombo != null && entryIngredientCombo.getValue() != null) || tableSelected;
        stockEntryButton.setDisable(!entrySelected);
        deactivateButton.setDisable(!tableSelected);
        editButton.setDisable(!tableSelected);
    }

    private static String formatNumber(double number) {
        return String.format(Locale.ROOT, "%.2f", number);
    }

    private record InventoryData(List<Hornitzailea> suppliers, List<Lehengaia> ingredients, List<LehengaiaAlerta> alerts) {
    }
}
