package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiKontratua;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Langilea;
import javafx.beans.property.ReadOnlyObjectWrapper;
import javafx.beans.property.ReadOnlyStringWrapper;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
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

public final class LangileakKontrolagailua extends BaseAdminKontrolagailua {
    private final ObservableList<Langilea> employees = FXCollections.observableArrayList();

    @FXML private TableView<Langilea> staffTable;
    @FXML private TableColumn<Langilea, Integer> idColumn;
    @FXML private TableColumn<Langilea, String> nameColumn;
    @FXML private TableColumn<Langilea, String> usernameColumn;
    @FXML private TableColumn<Langilea, String> roleColumn;
    @FXML private TableColumn<Langilea, String> activeColumn;
    @FXML private TableColumn<Langilea, String> chatColumn;
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
    @FXML private Button toggleChatButton;
    @FXML private Button toggleActiveButton;
    @FXML private Button deactivateButton;
    @FXML private Label staffMessage;
    private int editingEmployeeId;

    public LangileakKontrolagailua() {
        this(null, null);
    }

    public LangileakKontrolagailua(ApiKontratua apiClient) {
        this(apiClient, null);
    }

    public LangileakKontrolagailua(ApiKontratua apiClient, KudeatzaileSaioa session) {
        super(apiClient, session);
    }

    @Override
    public String getPantailaIzena() {
        return "Langileak";
    }

    @FXML
    private void initialize() {
        idColumn.setCellValueFactory(cell -> new ReadOnlyObjectWrapper<>(cell.getValue().id));
        nameColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().izena));
        usernameColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().erabiltzailea));
        roleColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(cell.getValue().rola));
        activeColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                cell.getValue().aktibo ? "Bai" : "Ez"));
        chatColumn.setCellValueFactory(cell -> new ReadOnlyStringWrapper(
                cell.getValue().txataAktibo ? "Aktibatuta" : "Desaktibatuta"));
        staffTable.setItems(employees);
        roleCombo.setItems(FXCollections.observableArrayList(
                "Zerbitzaria", "Sukaldaria", "Barrakoa", "Biltegikoa",
                "Kudeatzailea", "AdminIT", "Admin"));
        roleCombo.getSelectionModel().selectFirst();
        staffTable.getSelectionModel().selectedItemProperty().addListener(
                (observable, oldValue, selected) -> updateSelectionButtons());
        beginNewStaff();
        updateSelectionButtons();
        if (apiPrestDago()) {
            refresh();
        }
    }

    @FXML
    @Override
    public void refresh() {
        if (!apiPrestDago()) {
            return;
        }
        ezarriMezua(staffMessage, "Langileak kargatzen...", null);
        EkintzaAsinkronoak.run(() -> apiClient.getList("/langileak", Langilea.class),
                data -> {
                    employees.setAll(data);
                    ezarriMezua(staffMessage, data.size() + " langile.", null);
                    updateSelectionButtons();
                },
                error -> ezarriMezua(staffMessage, "Ezin izan dira langileak kargatu: " + error.getMessage(), "status-error"));
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
            ezarriMezua(staffMessage, "Bete izena, abizenak, erabiltzailea, NANa, emaila eta rola.", "status-warning");
            return;
        }
        if (!email.matches("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$")) {
            ezarriMezua(staffMessage, "Email helbidea ez da baliozkoa.", "status-warning");
            return;
        }
        if ((editingEmployeeId == 0 && pin.isBlank())
                || (!pin.isBlank() && pin.length() < 4)) {
            ezarriMezua(staffMessage, "PINak gutxienez 4 karaktere izan behar ditu.", "status-warning");
            return;
        }

        int targetEmployeeId = editingEmployeeId;
        boolean creating = targetEmployeeId == 0;
        createButton.setDisable(true);
        editButton.setDisable(true);
        cancelEditButton.setDisable(true);
        ezarriMezua(staffMessage, creating ? "Langilea sortzen..." : "Langilea eguneratzen...", null);
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
                    ezarriMezua(staffMessage, creating ? "Langilea sortu da." : "Langilearen datuak eguneratu dira.", "status-ok");
                    createButton.setDisable(false);
                    refresh();
                },
                error -> {
                    ezarriMezua(staffMessage, "Ezin izan da langilea gorde: " + error.getMessage(), "status-error");
                    createButton.setDisable(false);
                    cancelEditButton.setDisable(false);
                    updateSelectionButtons();
                });
    }

    @FXML
    private void editStaff() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            ezarriMezua(staffMessage, "Hautatu editatu nahi duzun langilea.", "status-warning");
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
        ezarriMezua(staffMessage, selected.izena + " editatzeko prest.", null);
    }

    @FXML
    private void cancelEdit() {
        beginNewStaff();
        ezarriMezua(staffMessage, "Langile berria sortzeko formularioa prest.", null);
    }

    @FXML
    private void toggleStaffChat() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            ezarriMezua(staffMessage, "Hautatu langile bat txata aktibatu edo desaktibatzeko.", "status-warning");
            return;
        }
        boolean egoeraBerria = !selected.txataAktibo;
        toggleChatButton.setDisable(true);
        ezarriMezua(staffMessage, egoeraBerria ? "Txata aktibatzen..." : "Txata desaktibatzen...", null);
        EkintzaAsinkronoak.run(() -> {
                    apiClient.eguneratuLangileTxata(selected.id, egoeraBerria);
                    return null;
                },
                ignored -> {
                    ezarriMezua(staffMessage, selected.izena + " langilearen txata "
                            + (egoeraBerria ? "aktibatu" : "desaktibatu") + " da.", "status-ok");
                    refresh();
                },
                error -> {
                    ezarriMezua(staffMessage, "Ezin izan da txat-egoera aldatu: " + error.getMessage(), "status-error");
                    updateSelectionButtons();
                });
    }

    @FXML
    private void toggleStaffActive() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            ezarriMezua(staffMessage, "Hautatu langile bat aktibatu edo desaktibatzeko.", "status-warning");
            return;
        }
        if (saioaAktiboDago() && selected.id == session.getUser().id && selected.aktibo) {
            ezarriMezua(staffMessage, "Ezin duzu zure erabiltzailea desaktibatu saioa irekita dagoen bitartean.", "status-warning");
            return;
        }
        boolean egoeraBerria = !selected.aktibo;
        toggleActiveButton.setDisable(true);
        ezarriMezua(staffMessage, egoeraBerria ? "Langilea aktibatzen..." : "Langilea desaktibatzen...", null);
        EkintzaAsinkronoak.run(() -> {
                    apiClient.eguneratuLangileAktibo(selected.id, egoeraBerria);
                    return null;
                },
                ignored -> {
                    ezarriMezua(staffMessage, selected.izena + " langilea "
                            + (egoeraBerria ? "aktibatu da (TPV login baimenduta)." : "desaktibatu da (TPV login blokeatuta)."),
                            "status-ok");
                    refresh();
                },
                error -> {
                    ezarriMezua(staffMessage, "Ezin izan da aktibo-egoera aldatu: " + error.getMessage(), "status-error");
                    updateSelectionButtons();
                });
    }

    @FXML
    private void deactivateStaff() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        if (selected == null) {
            ezarriMezua(staffMessage, "Hautatu desgaitu nahi duzun langilea.", "status-warning");
            return;
        }
        if (saioaAktiboDago() && selected.id == session.getUser().id) {
            ezarriMezua(staffMessage, "Ezin duzu zure erabiltzailea desgaitu saioa irekita dagoen bitartean.", "status-warning");
            return;
        }
        if (!PantailaLaguntza.confirm("Langilea desgaitu",
                selected.izena + " langilea desgaitu egingo da eta saioa hasteko baimena galduko du.")) {
            return;
        }

        deactivateButton.setDisable(true);
        ezarriMezua(staffMessage, "Langilea desgaitzen...", null);
        EkintzaAsinkronoak.run(() -> {
                    apiClient.delete("/langileak/" + selected.id);
                    return null;
                },
                ignored -> {
                    ezarriMezua(staffMessage, "Langilea desgaitu da.", "status-ok");
                    refresh();
                },
                error -> {
                    ezarriMezua(staffMessage, "Ezin izan da langilea desgaitu: " + error.getMessage(), "status-error");
                    updateSelectionButtons();
                });
    }

    private void updateSelectionButtons() {
        Langilea selected = staffTable.getSelectionModel().getSelectedItem();
        boolean hasSelection = selected != null;
        boolean enabled = session != null && hasSelection;
        editButton.setDisable(!enabled);
        toggleChatButton.setDisable(!enabled);
        toggleActiveButton.setDisable(!enabled);
        deactivateButton.setDisable(!enabled);
        if (hasSelection) {
            toggleChatButton.setText(selected.txataAktibo ? "Txata desaktibatu" : "Txata aktibatu");
            toggleActiveButton.setText(selected.aktibo ? "Desaktibatu (TPV)" : "Aktibatu (TPV)");
        } else {
            toggleChatButton.setText("Txata aktibatu / desaktibatu");
            toggleActiveButton.setText("Aktibatu / Desaktibatu");
        }
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
