package eus.goierri.jatetxea.core.controller;

import eus.goierri.jatetxea.core.api.ApiZerbitzua;
import eus.goierri.jatetxea.core.api.LoginErantzuna;
import eus.goierri.jatetxea.core.model.Langilea;
import eus.goierri.jatetxea.core.session.Sesioa;
import eus.goierri.jatetxea.core.util.PantailaAldatzailea;
import javafx.application.Platform;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.geometry.Pos;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.ScrollPane;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyEvent;
import javafx.scene.layout.HBox;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.VBox;
import javafx.scene.shape.Circle;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

public abstract class BaseLoginController {

    @FXML protected HBox pnlAvatars;
    @FXML protected ScrollPane scrollAvatars;
    @FXML protected Label lblHautatuaInfo;
    @FXML protected Label lblErrorea;
    @FXML protected Label lblApiEgoera;
    @FXML protected Circle dotApiStatus;
    @FXML protected Button btnLogin;

    @FXML protected Circle dotPin1;
    @FXML protected Circle dotPin2;
    @FXML protected Circle dotPin3;
    @FXML protected Circle dotPin4;

    protected final ApiZerbitzua api = new ApiZerbitzua();
    protected final List<Langilea> baimendutakoLangileak = new ArrayList<>();
    protected final List<VBox> avatarCardNodes = new ArrayList<>();
    protected Langilea aukeratutakoLangilea = null;
    protected final StringBuilder pinBuffer = new StringBuilder();

    protected abstract boolean baimenduRola(String rola);
    protected abstract String getMainFxml();
    protected abstract Class<?> getAppResourceClass();

    @FXML
    public void initialize() {
        freskatuPinDots();
        kargatuLangileak();

        Platform.runLater(() -> {
            if (btnLogin != null && btnLogin.getScene() != null) {
                btnLogin.getScene().addEventFilter(KeyEvent.KEY_PRESSED, this::onKeyPressed);
            }
        });
    }

    private void onKeyPressed(KeyEvent event) {
        if (event.getCode().isDigitKey()) {
            gehituPinDigitoa(event.getText());
            event.consume();
        } else if (event.getCode() == KeyCode.BACK_SPACE) {
            onPinAtzeraClick();
            event.consume();
        } else if (event.getCode() == KeyCode.ESCAPE) {
            onPinGarbituClick();
            event.consume();
        } else if (event.getCode() == KeyCode.ENTER) {
            onLoginClick();
            event.consume();
        }
    }

    private void kargatuLangileak() {
        if (lblApiEgoera != null) lblApiEgoera.setText("API: Egiaztatzen...");
        if (dotApiStatus != null) dotApiStatus.setStyle("-fx-fill: #d97706;");

        new Thread(() -> {
            var health = api.egiaztatuOsasuna();
            if (!health.apiMartxan) {
                Platform.runLater(() -> {
                    if (lblApiEgoera != null) lblApiEgoera.setText("API: Deskonektatuta (" + ApiZerbitzua.getBaseUrl() + ")");
                    if (dotApiStatus != null) dotApiStatus.setStyle("-fx-fill: #dc2626;");
                    if (lblErrorea != null) lblErrorea.setText("Ezin izan da REST APIarekin konektatu.");
                });
                return;
            }

            try {
                List<Langilea> guztiak = api.lortuLangileak();
                List<Langilea> iragaziak = guztiak.stream()
                        .filter(l -> baimenduRola(l.getRola()))
                        .sorted(Comparator.comparing(Langilea::getIzena))
                        .collect(Collectors.toList());

                Platform.runLater(() -> {
                    if (lblApiEgoera != null) lblApiEgoera.setText("API: Konektatuta (" + ApiZerbitzua.getBaseUrl() + ")");
                    if (dotApiStatus != null) dotApiStatus.setStyle("-fx-fill: #16a34a;");

                    baimendutakoLangileak.clear();
                    baimendutakoLangileak.addAll(iragaziak);

                    eraikiAvatarrak();

                    if (!baimendutakoLangileak.isEmpty()) {
                        hautatuLangilea(baimendutakoLangileak.get(0));
                    }
                });

            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblApiEgoera != null) lblApiEgoera.setText("API: Errorea");
                    if (dotApiStatus != null) dotApiStatus.setStyle("-fx-fill: #dc2626;");
                    if (lblErrorea != null) lblErrorea.setText("Errorea langileak kargatzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    private void eraikiAvatarrak() {
        if (pnlAvatars == null) return;
        pnlAvatars.getChildren().clear();
        avatarCardNodes.clear();

        for (Langilea l : baimendutakoLangileak) {
            VBox card = new VBox(4);
            card.setAlignment(Pos.CENTER);
            card.setPrefWidth(96);
            card.setPrefHeight(102);
            card.setStyle("-fx-background-color: #ffffff; -fx-background-radius: 8; -fx-cursor: hand; -fx-padding: 6;");

            StackPane avatarCircle = new StackPane();
            Circle circleBg = new Circle(24);
            circleBg.setStyle("-fx-fill: #f1f5f9; -fx-stroke: #cbd5e1; -fx-stroke-width: 1.5;");

            Label lblInitials = new Label(l.getInitialak());
            lblInitials.setStyle("-fx-font-size: 13px; -fx-font-weight: bold; -fx-text-fill: #475569;");
            avatarCircle.getChildren().addAll(circleBg, lblInitials);

            String shortName = l.getIzena();
            if (l.getErabiltzailea().equalsIgnoreCase("anderu")) shortName = "Ander U.";
            else if (l.getErabiltzailea().equalsIgnoreCase("ander_m_zuz")) shortName = "Ander M.";

            Label lblName = new Label(shortName);
            lblName.setStyle("-fx-font-size: 11px; -fx-font-weight: bold; -fx-text-fill: #334155;");

            Label lblRole = new Label(l.getRola());
            lblRole.setStyle("-fx-font-size: 9px; -fx-text-fill: #64748b;");

            card.getChildren().addAll(avatarCircle, lblName, lblRole);
            card.setOnMouseClicked(e -> hautatuLangilea(l));

            pnlAvatars.getChildren().add(card);
            avatarCardNodes.add(card);
        }
    }

    protected void hautatuLangilea(Langilea l) {
        this.aukeratutakoLangilea = l;
        pinBuffer.setLength(0);
        freskatuPinDots();
        if (lblErrorea != null) lblErrorea.setText("");

        for (int i = 0; i < baimendutakoLangileak.size(); i++) {
            VBox card = avatarCardNodes.get(i);
            boolean hautatuaDa = baimendutakoLangileak.get(i).getId() == l.getId();
            if (hautatuaDa) {
                card.setStyle("-fx-background-color: #fdf2f4; -fx-background-radius: 8; -fx-border-color: #881337; -fx-border-radius: 8; -fx-border-width: 2; -fx-cursor: hand; -fx-padding: 5;");
            } else {
                card.setStyle("-fx-background-color: #ffffff; -fx-background-radius: 8; -fx-cursor: hand; -fx-padding: 6;");
            }
        }

        if (lblHautatuaInfo != null) {
            lblHautatuaInfo.setText(l.getIzenOsoa() + " (" + l.getRola() + ")");
        }
    }

    @FXML
    protected void onPinBotoiaClick(ActionEvent event) {
        Button btn = (Button) event.getSource();
        String balioa = btn.getText().trim();
        gehituPinDigitoa(balioa);
    }

    private void gehituPinDigitoa(String digitoa) {
        if (pinBuffer.length() < 4 && digitoa.matches("\\d")) {
            pinBuffer.append(digitoa);
            freskatuPinDots();
            if (lblErrorea != null) lblErrorea.setText("");
            if (pinBuffer.length() == 4) {
                onLoginClick();
            }
        }
    }

    @FXML
    protected void onPinAtzeraClick() {
        if (pinBuffer.length() > 0) {
            pinBuffer.deleteCharAt(pinBuffer.length() - 1);
            freskatuPinDots();
        }
    }

    @FXML
    protected void onPinGarbituClick() {
        pinBuffer.setLength(0);
        freskatuPinDots();
        if (lblErrorea != null) lblErrorea.setText("");
    }

    private void freskatuPinDots() {
        int len = pinBuffer.length();
        setStyleDot(dotPin1, len >= 1);
        setStyleDot(dotPin2, len >= 2);
        setStyleDot(dotPin3, len >= 3);
        setStyleDot(dotPin4, len >= 4);
    }

    private void setStyleDot(Circle dot, boolean betea) {
        if (dot == null) return;
        if (betea) {
            dot.setStyle("-fx-fill: #881337; -fx-stroke: #881337; -fx-stroke-width: 2;");
        } else {
            dot.setStyle("-fx-fill: #ffffff; -fx-stroke: #cbd5e1; -fx-stroke-width: 2;");
        }
    }

    @FXML
    protected void onLoginClick() {
        if (aukeratutakoLangilea == null) {
            if (lblErrorea != null) lblErrorea.setText("Hautatu langile bat lehenik.");
            return;
        }

        if (pinBuffer.length() == 0) {
            if (lblErrorea != null) lblErrorea.setText("Sartu zure 4 digituko PIN propioa.");
            return;
        }

        String pin = pinBuffer.toString();
        if (btnLogin != null) {
            btnLogin.setDisable(true);
            btnLogin.setText("EGIAZTATZEN...");
        }
        if (lblErrorea != null) lblErrorea.setText("");

        new Thread(() -> {
            try {
                LoginErantzuna res = api.saioaHasi(aukeratutakoLangilea.getErabiltzailea(), pin);

                Platform.runLater(() -> {
                    if (btnLogin != null) {
                        btnLogin.setDisable(false);
                        btnLogin.setText("HASI SAIOA");
                    }

                    if (res != null && res.isArrakasta() && res.getLangilea() != null) {
                        Langilea l = res.getLangilea();
                        Sesioa.setUnekoLangilea(l);

                        try {
                            PantailaAldatzailea.aldatu(btnLogin, getMainFxml(), getAppResourceClass());
                        } catch (IOException e) {
                            if (lblErrorea != null) lblErrorea.setText("Errorea pantaila kargatzean: " + e.getMessage());
                        }

                    } else {
                        String msg = res != null ? res.getMezua() : "PIN okerra.";
                        if (lblErrorea != null) lblErrorea.setText(msg);
                        pinBuffer.setLength(0);
                        freskatuPinDots();
                    }
                });

            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (btnLogin != null) {
                        btnLogin.setDisable(false);
                        btnLogin.setText("HASI SAIOA");
                    }
                    if (lblErrorea != null) lblErrorea.setText("Errorea saioa hastean: " + ex.getMessage());
                    pinBuffer.setLength(0);
                    freskatuPinDots();
                });
            }
        }).start();
    }
}
