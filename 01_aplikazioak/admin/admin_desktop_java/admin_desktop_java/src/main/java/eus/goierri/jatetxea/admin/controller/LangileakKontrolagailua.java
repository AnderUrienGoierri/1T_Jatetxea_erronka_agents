package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.model.Langilea;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.beans.property.ReadOnlyObjectWrapper;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;

import java.util.LinkedHashMap;
import java.util.Map;

public final class LangileakKontrolagailua {
    private final ApiBezeroa apiClient;
    private final KudeatzaileSaioa session;
    private final ObservableList<Langilea> employees = FXCollections.observableArrayList();

    @FXML private TableView<Langilea> staffTable;
    @FXML private TableColumn<Langilea, Integer> idColumn;
    @FXML private TableColumn<Langilea, String> nameColumn;
    @FXML private TableColumn<Langilea, String> usernameColumn;
    @FXML private TableColumn<Langilea, String> roleColumn;
    @FXML private TableColumn<Langilea, String> activeColumn;
    @FXML private TextField nameField;
    @FXML private TextField surnameField;
    @FXML private TextField usernameField;
    @FXML private TextField nationalIdField;
    @FXML private TextField emailField;
    @FXML private PasswordField pinField;
    @FXML private ComboBox<String> roleCombo;
    @FXML private Label formTitle;
    @FXML private Button createButton;
    @FXML private Button editButton;
    @FXML private Button cancelEditButton;
    @FXML private Button deactivateButton;
    @FXML private Label staffMessage;
    private int editingEmployeeId;

    public LangileakKontrolagailua() {
        this(null, null);
    }

    public LangileakKontrolagailua(ApiBezeroa apiClient) {
        this(apiClient, null);
    }

    public LangileakKontrolagailua(ApiBezeroa apiClient, KudeatzaileSaioa session) {
        this.apiClient = apiClient;
        this.session = session;
    }

    @FXML
    private void initialize() {
        idColumn.setCellValueFactory(cell -> new ReadOnlyObjectWrapper<>(cell.getValue().id));
        nameColumn.setCellValueFactory(cell -> new javafx.beans.property.ReadOnlyStringWrapper(cell.getValue().izena));
        usernameColumn.setCellValueFactory(
                cell -> new javafx.beans.property.ReadOnlyStringWrapper(cell.getValue().erabiltzailea));
        roleColumn.setCellValueFactory(cell -> new javafx.beans.property.ReadOnlyStringWrapper(cell.getValue().rola));
        activeColumn.setCellValueFactory(cell -> new javafx.beans.property.ReadOnlyStringWrapper(
                cell.getValue().aktibo ? "Bai" : "Ez"));
        staffTable.setItems(employees);
        roleCombo.setItems(FXCollections.observableArrayList(
                "Zerbitzaria", "Sukaldaria", "Barrakoa", "Biltegikoa",
                "Kudeatzailea", "AdminIT", "Admin"));
        roleCombo.getSelectionModel().selectFirst();
        staffTable.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, selected) -> updateSelectionButtons());
        beginNewStaff();
        updateSelectionButtons();
        if (apiClient != null) {
            refresh();
        }
    }

    @FXML
    private void refresh() {
        staffMessage.setText("Langileak kargatzen...");
        EkintzaAsinkronoak.run(() -> apiClient.getList("/langileak", Langilea.class),
                data -> {
                    employees.setAll(data);
                    staffMessage.setText(data.size() + " langile.");
                },
                error -> staffMessage.setText("Ezin izan dira langileak kargatu: " + error.getMessage()));
    }

    @FXML
    private void saveStaff() {
        String name = nameField.getText().trim();
        String surname = surnameField.getText().trim();
        String username = usernameField.getText().trim();
        String nationalId = nationalIdField.getText().trim();
        String email = emailField.getText().trim();
        String pin = pinField.getText();
        String role = roleCombo.getValue();
        if (name.isBlank() || surname.isBlank() || username.isBlank() || nationalId.isBlank()
                || email.isBlank() || role == null) {
            staffMessage.setText("Bete izena, abizenak, erabiltzailea, NANa, emaila eta rola.");
            return;
        }
        if (!email.matches("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$")) {
            staffMessage.setText("Email helbidea ez da baliozkoa.");
            return;
        }
        if ((editingEmployeeId == 0 && pin.isBlank())
                || (!pin.isBlank() && pin.length() < 4)) {
            staffMessage.setText("PINak gutxienez 4 karaktere izan behar ditu.");
            return;
        }

        int targetEmployeeId = editingEmployeeId;
        boolean creating = targetEmployeeId == 0;
        createButton.setDisable(true);
        editButton.setDisable(true);
        cancelEditButton.setDisable(true);
        staffMessage.setText(creating ? "Langilea sortzen..." : "Langilea eguneratzen...");
        EkintzaAsinkronoak.run(() -> {
                    Map<String, Object> body = new LinkedHashMap<>();
                    body.put("izena", name);
                    body.put("abizena", surname);
                    body.put("erabiltzailea", username);
                    body.put("nanDni", nationalId);
                    body.put("emaila", email);
                    body.put("rola", role);
                    if (!pin.isBlank()) {
                        body.put("pasahitza", pin);
                    }
                    if (creating) {
                        apiClient.post("/langileak", body);
                    } else {
                        apiClient.put("/langileak/" + targetEmployeeId, body);
                    }
                    return null;
                },
                ignored -> {
                    beginNewStaff();
                    staffMessage.setText(creating ? "Langilea sortu da." : "Langilearen datuak eguneratu dira.");
                    createButton.setDisable(false);
                    refresh();
                },
                error -> {
                    staffMessage.setText("Ezin izan da langilea gorde: " + error.getMessage());
                    createButton.setDisable(false);
                    cancelEditButton.setDisable(false);
                    updateSelectionButtons();
                });
    }

    @FXML
    private void editStaff() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            staffMessage.setText("Hautatu editatu nahi duzun langilea.");
            return;
        }
        editingEmployeeId = selected.id;
        nameField.setText(selected.izena);
        surnameField.setText(selected.abizena);
        usernameField.setText(selected.erabiltzailea);
        nationalIdField.setText(selected.nanDni);
        emailField.setText(selected.emaila);
        pinField.clear();
        roleCombo.setValue(selected.rola);
        formTitle.setText("Langilea editatzen");
        createButton.setText("Gorde aldaketak");
        pinField.setPromptText("Utzi hutsik PINa mantentzeko");
        cancelEditButton.setVisible(true);
        cancelEditButton.setManaged(true);
        staffMessage.setText(selected.izena + " editatzeko prest.");
    }

    @FXML
    private void cancelEdit() {
        beginNewStaff();
        staffMessage.setText("Langile berria sortzeko formularioa prest.");
    }

    @FXML
    private void deactivateStaff() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            staffMessage.setText("Hautatu desgaitu nahi duzun langilea.");
            return;
        }
        if (session != null && selected.id == session.getUser().id) {
            staffMessage.setText("Ezin duzu zure erabiltzailea desgaitu saioa irekita dagoen bitartean.");
            return;
        }
        if (!PantailaLaguntza.confirm("Langilea desgaitu",
                selected.izena + " langilea desgaitu egingo da eta saioa hasteko baimena galduko du.")) {
            return;
        }

        deactivateButton.setDisable(true);
        staffMessage.setText("Langilea desgaitzen...");
        EkintzaAsinkronoak.run(() -> {
                    apiClient.delete("/langileak/" + selected.id);
                    return null;
                },
                ignored -> {
                    staffMessage.setText("Langilea desgaitu da.");
                    refresh();
                },
                error -> {
                    staffMessage.setText("Ezin izan da langilea desgaitu: " + error.getMessage());
                    updateSelectionButtons();
                });
    }

    private void updateSelectionButtons() {
        boolean selected = staffTable.getSelectionModel().getSelectedItem() != null;
        editButton.setDisable(session == null || !selected);
        deactivateButton.setDisable(session == null || !selected);
    }

    private void beginNewStaff() {
        editingEmployeeId = 0;
        nameField.clear();
        surnameField.clear();
        usernameField.clear();
        nationalIdField.clear();
        emailField.clear();
        pinField.clear();
        pinField.setPromptText("Gutxienez 4 karaktere");
        roleCombo.getSelectionModel().selectFirst();
        formTitle.setText("Langile berria");
        createButton.setText("Langilea sortu");
        cancelEditButton.setVisible(false);
        cancelEditButton.setManaged(false);
        cancelEditButton.setDisable(false);
    }
}
