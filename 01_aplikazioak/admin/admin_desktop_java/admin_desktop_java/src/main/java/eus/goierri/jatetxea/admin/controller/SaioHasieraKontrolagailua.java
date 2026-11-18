package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.JatetxeaKudeaketaAplikazioa;
import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import javafx.fxml.FXML;
import javafx.animation.Animation;
import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.scene.paint.Color;
import javafx.scene.shape.Circle;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TextField;

import java.io.IOException;
import java.util.concurrent.atomic.AtomicBoolean;
import javafx.util.Duration;

public final class SaioHasieraKontrolagailua {
    private final ApiBezeroa apiClient;
    private final JatetxeaKudeaketaAplikazioa application;

    @FXML private TextField usernameField;
    @FXML private PasswordField pinField;
    @FXML private Button signInButton;
    @FXML private Label errorLabel;
    @FXML private Circle apiStatusDot;
    @FXML private Label apiStatusLabel;
    @FXML private Button refreshApiButton;

    private final AtomicBoolean healthCheckInProgress = new AtomicBoolean();
    private Timeline healthCheckTimeline;

    public SaioHasieraKontrolagailua() {
        this(null, null);
    }

    public SaioHasieraKontrolagailua(ApiBezeroa apiClient, JatetxeaKudeaketaAplikazioa application) {
        this.apiClient = apiClient;
        this.application = application;
    }

    @FXML
    private void initialize() {
        if (apiClient == null) {
            return;
        }
        refreshApiStatus();
        healthCheckTimeline = new Timeline(new KeyFrame(Duration.seconds(10), event -> refreshApiStatus()));
        healthCheckTimeline.setCycleCount(Animation.INDEFINITE);
        healthCheckTimeline.play();
    }

    @FXML
    private void refreshApiStatus() {
        if (apiClient == null || !healthCheckInProgress.compareAndSet(false, true)) {
            return;
        }
        refreshApiButton.setDisable(true);
        apiStatusDot.setFill(Color.web("#d97706"));
        apiStatusLabel.setText("API egiaztatzen...");
        apiStatusLabel.getStyleClass().setAll("api-status-pending");
        EkintzaAsinkronoak.run(apiClient::checkHealth,
                health -> {
                    healthCheckInProgress.set(false);
                    refreshApiButton.setDisable(false);
                    apiStatusDot.setFill(Color.web(health.databaseConnected() ? "#15803d"
                            : health.apiConnected() ? "#d97706" : "#dc2626"));
                    apiStatusLabel.setText(health.message());
                    apiStatusLabel.getStyleClass().setAll(
                            health.databaseConnected() ? "api-status-online"
                                    : health.apiConnected() ? "api-status-degraded" : "api-status-offline");
                },
                error -> {
                    healthCheckInProgress.set(false);
                    refreshApiButton.setDisable(false);
                    apiStatusDot.setFill(Color.web("#dc2626"));
                    String detail = error.getMessage();
                    apiStatusLabel.setText("APIra konektatu ezinik"
                            + (detail == null || detail.isBlank() ? "" : " · " + detail));
                    apiStatusLabel.getStyleClass().setAll("api-status-offline");
                });
    }

    @FXML
    private void signIn() {
        String username = usernameField.getText().trim();
        String pin = pinField.getText();
        if (username.isBlank() || pin.isBlank()) {
            showError("Sartu erabiltzailea eta PINa.");
            return;
        }

        signInButton.setDisable(true);
        errorLabel.setText("Saioa egiaztatzen...");
        errorLabel.getStyleClass().setAll("inline-message");
        EkintzaAsinkronoak.run(
                () -> apiClient.login(username, pin),
                user -> {
                    try {
                        if (healthCheckTimeline != null) {
                            healthCheckTimeline.stop();
                        }
                        application.showWorkspace(new KudeatzaileSaioa(user));
                    } catch (IOException ex) {
                        showError("Ezin izan da kudeaketa-pantaila ireki: " + ex.getMessage());
                        signInButton.setDisable(false);
                    }
                },
                error -> {
                    showError(error.getMessage());
                    signInButton.setDisable(false);
                });
    }

    private void showError(String message) {
        errorLabel.setText(message == null ? "Errore ezezaguna gertatu da." : message);
        errorLabel.getStyleClass().setAll("error-message");
    }
}
