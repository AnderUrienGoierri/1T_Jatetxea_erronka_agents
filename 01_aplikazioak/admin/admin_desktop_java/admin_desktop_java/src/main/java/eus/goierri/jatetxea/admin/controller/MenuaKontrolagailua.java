package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Lehengaia;
import eus.goierri.jatetxea.admin.model.Platera;
import eus.goierri.jatetxea.admin.model.PlaterEskandalloa;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.beans.property.ReadOnlyDoubleWrapper;
import javafx.beans.property.ReadOnlyObjectWrapper;
import javafx.beans.property.ReadOnlyStringWrapper;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;

import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public final class MenuaKontrolagailua {
    private final ApiBezeroa apiClient;
    private final KudeatzaileSaioa session;
    private final ObservableList<Platera> menuItems = FXCollections.observableArrayList();
    private final ObservableList<RecipeComponent> recipeComponents = FXCollections.observableArrayList();
    private final ObservableList<Lehengaia> ingredients = FXCollections.observableArrayList();

    @FXML private TableView<Platera> menuTable;
    @FXML private TableColumn<Platera, Integer> idColumn;
    @FXML private TableColumn<Platera, String> nameColumn;
    @FXML private TableColumn<Platera, String> categoryColumn;
    @FXML private TableColumn<Platera, Number> priceColumn;
    @FXML private TableColumn<Platera, String> statusColumn;
    @FXML private Label formTitle;
    @FXML private TextField nameField;
    @FXML private TextField descriptionField;
    @FXML private ComboBox<String> categoryCombo;
    @FXML private TextField priceField;
    @FXML private TextField allergensField;
    @FXML private ComboBox<Lehengaia> ingredientCombo;
    @FXML private TextField recipeQuantityField;
    @FXML private ListView<RecipeComponent> recipeList;
    @FXML private Button createButton;
    @FXML private Button editButton;
    @FXML private Button cancelEditButton;
    @FXML private Button deactivateButton;
    @FXML private Button removeRecipeIngredientButton;
    @FXML private Label menuMessage;
    private int editingMenuItemId;
    private boolean busy;

    public MenuaKontrolagailua() {
        this(null, null);
    }

    public MenuaKontrolagailua(ApiBezeroa apiClient, KudeatzaileSaioa session) {
        this.apiClient = apiClient;
        this.session = session;
    }

    @FXML
    private void initialize() {
        idColumn.setCellValueFactory(cell -> new ReadOnlyObjectWrapper<>(cell.getValue().id));
        nameColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().izena));
        categoryColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().kategoria));
        priceColumn.setCellValueFactory(cell -> new ReadOnlyDoubleWrapper(cell.getValue().prezioa));
        statusColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                cell.getValue().aktibo ? "Bai" : "Ez"));
        menuTable.setItems(menuItems);
        menuTable.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, newValue) -> {
                    if (newValue != null) {
                        if (editingMenuItemId != newValue.id) {
                            loadMenuItem(newValue);
                        }
                    } else if (editingMenuItemId != 0) {
                        beginNewMenuItem();
                    }
                    updateWriteButtons();
                });
        categoryCombo.setItems(FXCollections.observableArrayList(
                "Sarrerakoa", "Lehena", "Bigarrena", "Postrea", "Edaria"));
        categoryCombo.getSelectionModel().selectFirst();
        ingredientCombo.setItems(ingredients);
        ingredientCombo.valueProperty().addListener(
                (observable, oldValue, newValue) -> updateWriteButtons());
        recipeList.setItems(recipeComponents);
        recipeList.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, newValue) -> {
                    if (newValue != null) {
                        for (Lehengaia ing : ingredients) {
                            if (ing.id == newValue.ingredientId()) {
                                ingredientCombo.setValue(ing);
                                break;
                            }
                        }
                        recipeQuantityField.setText(Double.toString(newValue.quantity()));
                    }
                    updateWriteButtons();
                });
        beginNewMenuItem();
        if (session != null) {
            session.configurationWritableProperty().addListener(
                    (observable, oldValue, newValue) -> updateWriteButtons());
            updateWriteButtons();
        } else {
            createButton.setDisable(true);
            deactivateButton.setDisable(true);
        }
        if (apiClient != null) {
            refresh();
        }
    }

    @FXML
    private void refresh() {
        menuMessage.setText("Menua eta osagaiak kargatzen...");
        EkintzaAsinkronoak.run(() -> new MenuData(
                        apiClient.getList("/platerak", Platera.class),
                        apiClient.getList("/lehengaiak", Lehengaia.class)),
                data -> {
                    menuItems.setAll(data.menuItems());
                    ingredients.setAll(data.ingredients());
                    menuMessage.setText(data.menuItems().size() + " plater · "
                            + data.ingredients().size() + " osagai eskuragarri.");
                    updateWriteButtons();
                },
                error -> menuMessage.setText("Ezin izan dira menu-datuak kargatu: " + error.getMessage()));
    }

    @FXML
    private void addRecipeIngredient() {
        Lehengaia ingredient = ingredientCombo.getValue();
        if (ingredient == null) {
            menuMessage.setText("Aukeratu errezetaren osagai bat.");
            return;
        }
        try {
            double quantity = PantailaLaguntza.parsePositive(recipeQuantityField.getText(), "Kantitatea");
            recipeComponents.removeIf(component -> component.ingredientId() == ingredient.id);
            recipeComponents.add(new RecipeComponent(
                    ingredient.id, quantity, ingredient.unitatea, ingredient.izena));
            recipeQuantityField.clear();
            menuMessage.setText(ingredient.izena + " (" + quantity + " " + ingredient.unitatea + ") errezetan ezarri da.");
            updateWriteButtons();
        } catch (IllegalArgumentException ex) {
            menuMessage.setText(ex.getMessage());
        }
    }

    @FXML
    private void saveMenuItem() {
        String name = nameField.getText().trim();
        if (name.isBlank() || categoryCombo.getValue() == null) {
            menuMessage.setText("Plateraren izena eta kategoria beharrezkoak dira.");
            return;
        }
        final double price;
        try {
            price = PantailaLaguntza.parseNonNegative(priceField.getText(), "Salmenta-prezioa");
        } catch (IllegalArgumentException ex) {
            menuMessage.setText(ex.getMessage());
            return;
        }

        Map<String, Object> body = new LinkedHashMap<>();
        body.put("izena", name);
        body.put("deskribapena", descriptionField.getText().trim());
        body.put("kategoria", categoryCombo.getValue());
        body.put("prezioa", price);
        body.put("alergenoak", allergensField.getText().trim());
        List<Map<String, Object>> components = new ArrayList<>();
        for (RecipeComponent component : recipeComponents) {
            components.add(Map.of(
                    "lehengaiaId", component.ingredientId(),
                    "kantitatea", component.quantity(),
                    "unitatea", component.unit()));
        }
        body.put("osagaiak", components);

        int targetMenuItemId = editingMenuItemId;
        boolean creating = targetMenuItemId == 0;
        busy = true;
        updateWriteButtons();
        menuMessage.setText(creating
                ? "Zerbitzu-egoera egiaztatzen eta platera sortzen..."
                : "Zerbitzu-egoera egiaztatzen eta errezeta eguneratzen...");
        EkintzaAsinkronoak.run(() -> {
                    verifyServiceClosed();
                    if (creating) {
                        apiClient.post("/platerak", body);
                    } else {
                        apiClient.put("/platerak/" + targetMenuItemId, body);
                    }
                    return null;
                },
                ignored -> {
                    session.setServiceState(true, "Zerbitzua itxita · konfigurazio-aldaketak baimenduta");
                    busy = false;
                    beginNewMenuItem();
                    menuMessage.setText(creating
                            ? "Platera eta errezeta sortu dira."
                            : "Plateraren datuak eta errezeta eguneratu dira.");
                    refresh();
                },
                error -> {
                    session.setServiceState(false, "Menuaren aldaketa huts egin du; egiaztatu zerbitzu-egoera.");
                    busy = false;
                    menuMessage.setText("Ezin izan da platera gorde: " + error.getMessage());
                    updateWriteButtons();
                });
    }

    @FXML
    private void editMenuItem() {
        Platera selected = menuTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            menuMessage.setText("Hautatu editatu nahi duzun platera.");
            return;
        }
        loadMenuItem(selected);
    }

    private void loadMenuItem(Platera selected) {
        if (selected == null) {
            beginNewMenuItem();
            return;
        }
        busy = true;
        updateWriteButtons();
        menuMessage.setText("Plateraren errezeta kargatzen...");
        EkintzaAsinkronoak.run(() -> apiClient.getData(
                        "/platerak/" + selected.id + "/eskandalloa", PlaterEskandalloa.class),
                recipe -> {
                    editingMenuItemId = selected.id;
                    nameField.setText(selected.izena);
                    descriptionField.setText(selected.deskribapena == null ? "" : selected.deskribapena);
                    categoryCombo.setValue(selected.kategoria);
                    priceField.setText(Double.toString(selected.prezioa));
                    allergensField.setText(selected.alergenoak == null ? "" : selected.alergenoak);
                    recipeComponents.clear();
                    if (recipe != null && recipe.osagaiak != null) {
                        for (PlaterEskandalloa.RecipeIngredient ingredient : recipe.osagaiak) {
                            recipeComponents.add(new RecipeComponent(
                                    ingredient.lehengaiaId, ingredient.kantitatea,
                                    ingredient.unitatea, ingredient.lehengaiaIzena));
                        }
                    }
                    formTitle.setText("Platera editatzen");
                    createButton.setText("Gorde platera eta errezeta");
                    cancelEditButton.setVisible(true);
                    cancelEditButton.setManaged(true);
                    busy = false;
                    menuMessage.setText(selected.izena + " eta bere errezeta editatzeko prest.");
                    updateWriteButtons();
                },
                error -> {
                    busy = false;
                    menuMessage.setText("Ezin izan da errezeta kargatu: " + error.getMessage());
                    updateWriteButtons();
                });
    }

    @FXML
    private void cancelEdit() {
        menuTable.getSelectionModel().clearSelection();
        beginNewMenuItem();
        menuMessage.setText("Plater berria sortzeko formularioa prest.");
    }

    @FXML
    private void removeRecipeIngredient() {
        RecipeComponent selected = recipeList.getSelectionModel().getSelectedItem();
        if (selected != null) {
            recipeComponents.remove(selected);
            menuMessage.setText(selected.name() + " errezetatik kendu da.");
            recipeQuantityField.clear();
            updateWriteButtons();
            return;
        }
        Lehengaia comboIng = ingredientCombo.getValue();
        if (comboIng != null) {
            boolean removed = recipeComponents.removeIf(c -> c.ingredientId() == comboIng.id);
            if (removed) {
                menuMessage.setText(comboIng.izena + " errezetatik kendu da.");
                recipeQuantityField.clear();
                updateWriteButtons();
            } else {
                menuMessage.setText("Osagai hori ez dago errezetan.");
            }
        }
    }

    @FXML
    private void deactivateMenuItem() {
        Platera selected = menuTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            menuMessage.setText("Hautatu desgaitu nahi duzun platera.");
            return;
        }
        if (!PantailaLaguntza.confirm("Platera desgaitu", selected.izena + " menutik kenduko da (soft delete).")) {
            return;
        }

        createButton.setDisable(true);
        deactivateButton.setDisable(true);
        EkintzaAsinkronoak.run(() -> {
                    verifyServiceClosed();
                    apiClient.delete("/platerak/" + selected.id);
                    return null;
                },
                ignored -> {
                    session.setServiceState(true, "Zerbitzua itxita · konfigurazio-aldaketak baimenduta");
                    menuMessage.setText("Platera desgaitu da.");
                    updateWriteButtons();
                    refresh();
                },
                error -> {
                    session.setServiceState(false, "Menuaren aldaketa huts egin du; egiaztatu zerbitzu-egoera.");
                    menuMessage.setText("Ezin izan da platera desgaitu: " + error.getMessage());
                    updateWriteButtons();
                });
    }

    private void verifyServiceClosed() throws IOException, InterruptedException {
        ApiBezeroa.ServiceStatus status = apiClient.getServiceStatus();
        if (status.active() || status.openTables() > 0) {
            throw new IOException("Zerbitzua irekita dago (" + status.openTables()
                    + " mahai). Menuaren aldaketak blokeatuta daude.");
        }
    }

    private void updateWriteButtons() {
        menuTable.setDisable(busy);
        boolean recipeSelected = recipeList != null
                && recipeList.getSelectionModel().getSelectedItem() != null;
        boolean comboMatch = ingredientCombo != null
                && ingredientCombo.getValue() != null
                && recipeComponents.stream().anyMatch(c -> c.ingredientId() == ingredientCombo.getValue().id);
        removeRecipeIngredientButton.setDisable(busy || (!recipeSelected && !comboMatch));
        if (session == null) {
            createButton.setDisable(true);
            editButton.setDisable(true);
            cancelEditButton.setDisable(true);
            deactivateButton.setDisable(true);
            return;
        }
        boolean disabled = busy || !session.isConfigurationWritable();
        createButton.setDisable(disabled);
        editButton.setDisable(disabled || menuTable.getSelectionModel().getSelectedItem() == null);
        cancelEditButton.setDisable(busy);
        deactivateButton.setDisable(disabled || menuTable.getSelectionModel().getSelectedItem() == null);
    }

    private void beginNewMenuItem() {
        editingMenuItemId = 0;
        nameField.clear();
        descriptionField.clear();
        categoryCombo.getSelectionModel().selectFirst();
        priceField.clear();
        allergensField.clear();
        recipeComponents.clear();
        formTitle.setText("Plater berria");
        createButton.setText("Platera eta errezeta sortu");
        cancelEditButton.setVisible(false);
        cancelEditButton.setManaged(false);
        updateWriteButtons();
    }

    private record MenuData(List<Platera> menuItems, List<Lehengaia> ingredients) {
    }

    public record RecipeComponent(int ingredientId, double quantity, String unit, String name) {
        @Override
        public String toString() {
            return name + " · " + quantity + " " + unit;
        }
    }
}
