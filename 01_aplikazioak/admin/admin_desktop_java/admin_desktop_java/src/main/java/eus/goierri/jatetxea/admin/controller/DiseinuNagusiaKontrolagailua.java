package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.JatetxeaKudeaketaAplikazioa;
import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.layout.StackPane;
import javafx.stage.Stage;

import java.io.IOException;

public final class DiseinuNagusiaKontrolagailua {
    private final ApiBezeroa apiClient;
    private final KudeatzaileSaioa session;
    private final JatetxeaKudeaketaAplikazioa application;
    private final Stage stage;

    @FXML private StackPane contentHost;
    @FXML private Label titleLabel;
    @FXML private Label userLabel;
    @FXML private Label serviceStatusLabel;
    @FXML private Button refreshStatusButton;

    public DiseinuNagusiaKontrolagailua() {
        this(null, null, null, null);
    }

    public DiseinuNagusiaKontrolagailua(ApiBezeroa apiClient, KudeatzaileSaioa session, JatetxeaKudeaketaAplikazioa application, Stage stage) {
        this.apiClient = apiClient;
        this.session = session;
        this.application = application;
        this.stage = stage;
    }

    @FXML
    private void initialize() {
        if (session == null) {
            return;
        }
        userLabel.setText(session.getUser().displayName() + "  ·  " + session.getUser().rola);
        serviceStatusLabel.textProperty().bind(session.serviceStatusTextProperty());
        showDashboard();
        refreshServiceStatus();
    }

    @FXML private void showDashboard() { loadScreen("laburpena.fxml", "Laburpena"); }
    @FXML private void showStaff() { loadScreen("langileak.fxml", "Langileak"); }
    @FXML private void showMenu() { loadScreen("menua.fxml", "Menua eta errezetak"); }
    @FXML private void showIngredients() { loadScreen("osagaiak.fxml", "Osagaiak eta inbentarioa"); }
    @FXML private void showTables() { loadScreen("mahaiak.fxml", "Mahaiak"); }

    @FXML
    private void refreshServiceStatus() {
        refreshStatusButton.setDisable(true);
        serviceStatusLabel.getStyleClass().setAll("status-warning");
        session.setServiceState(false, "Zerbitzu-egoera egiaztatzen...");
        EkintzaAsinkronoak.run(
                apiClient::getServiceStatus,
                status -> {
                    boolean writable = !status.active() && status.openTables() == 0;
                    String message = writable
                            ? "Zerbitzua itxita · konfigurazio-aldaketak baimenduta"
                            : "Zerbitzua irekita (" + status.openTables()
                                    + " mahai) · menuaren eta konfigurazio orokorraren aldaketak blokeatuta";
                    session.setServiceState(writable, message);
                    serviceStatusLabel.getStyleClass().setAll(writable ? "status-ok" : "status-warning");
                    refreshStatusButton.setDisable(false);
                },
                error -> {
                    session.setServiceState(false,
                            "Ezin da zerbitzu-egoera egiaztatu. Konfigurazio-aldaketak blokeatuta daude.");
                    serviceStatusLabel.getStyleClass().setAll("status-error");
                    refreshStatusButton.setDisable(false);
                });
    }

    @FXML
    private void signOut() {
        try {
            application.showLogin();
        } catch (IOException ex) {
            serviceStatusLabel.setText("Ezin izan da saioa itxi: " + ex.getMessage());
            serviceStatusLabel.getStyleClass().setAll("status-error");
        }
    }

    private void loadScreen(String fileName, String title) {
        try {
            FXMLLoader loader = new FXMLLoader(DiseinuNagusiaKontrolagailua.class.getResource(
                    "/eus/goierri/jatetxea/admin/fxml/" + fileName));
            loader.setControllerFactory(new KontrolagailuFabrika(apiClient, stage, session));
            Parent page = loader.load();
            contentHost.getChildren().setAll(page);
            titleLabel.setText(title);
        } catch (IOException ex) {
            contentHost.getChildren().setAll(new Label("Ezin izan da pantaila kargatu: " + ex.getMessage()));
        }
    }
}
