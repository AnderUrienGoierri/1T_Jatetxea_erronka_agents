package eus.goierri.jatetxea.controller;

import eus.goierri.jatetxea.PantailaAldatzailea;
import eus.goierri.jatetxea.Sesioa;
import eus.goierri.jatetxea.api.ApiZerbitzua;
import eus.goierri.jatetxea.api.LoginErantzuna;
import eus.goierri.jatetxea.model.Langilea;
import javafx.application.Platform;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.geometry.Pos;
import javafx.scene.Node;
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

/**
 * Sukaldea, Barra eta Biltegirako JavaFX Saio-Hasiera Kontroladorea (PIN bidezkoa).
 */
public class LoginController {

    @FXML private HBox pnlAvatars;
    @FXML private ScrollPane scrollAvatars;
    @FXML private Label lblHautatuaInfo;
    @FXML private Label lblErrorea;
    @FXML private Label lblApiEgoera;
    @FXML private Circle dotApiStatus;
    @FXML private Button btnLogin;

    @FXML private Circle dotPin1;
    @FXML private Circle dotPin2;
    @FXML private Circle dotPin3;
    @FXML private Circle dotPin4;

    private final ApiZerbitzua api = new ApiZerbitzua();
    private final List<Langilea> baimendutakoLangileak = new ArrayList<>();
    private final List<VBox> avatarCardNodes = new ArrayList<>();
    private Langilea aukeratutakoLangilea = null;
    private final StringBuilder pinBuffer = new StringBuilder();

    @FXML
    public void initialize() {
        freskatuPinDots();
        kargatuLangileak();

        // Teklatu fisikoaren bidez zenbakiak idatzi ahal izateko
        Platform.runLater(() -> {
            if (btnLogin.getScene() != null) {
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
        lblApiEgoera.setText("API: Egiaztatzen...");
        dotApiStatus.setStyle("-fx-fill: #d97706;");

        new Thread(() -> {
            var health = api.egiaztatuOsasuna();
            if (!health.apiMartxan) {
                Platform.runLater(() -> {
                    lblApiEgoera.setText("API: Deskonektatuta (http://localhost:5050)");
                    dotApiStatus.setStyle("-fx-fill: #dc2626;");
                    lblErrorea.setText("Ezin izan da REST APIarekin konektatu (http://localhost:5050).");
                });
                return;
            }

            try {
                List<Langilea> guztiak = api.lortuLangileak();

                // Baimendutako rolak soilik iragazi: Sukaldaria, Barrakoa, Biltegikoa, AdminIT, Admin, Kudeatzailea
                // Sukaldea, Barra eta Biltegirako aplikazioa denez, jantokiko zerbitzariak (Mikel, Maite, Unai) kanpo geratzen dira
                List<Langilea> iragaziak = guztiak.stream()
                        .filter(l -> {
                            String r = l.getRola().trim().toLowerCase();
                            return r.contains("sukaldari") ||
                                   r.contains("barra") ||
                                   r.contains("biltegi") ||
                                   r.contains("admin") ||
                                   r.contains("it") ||
                                   r.contains("kudeatzaile");
                        })
                        .sorted(Comparator.comparingInt(this::getRolaLehentasuna)
                                .thenComparing(Langilea::getIzena))
                        .collect(Collectors.toList());

                Platform.runLater(() -> {
                    lblApiEgoera.setText("API: Konektatuta (" + ApiZerbitzua.getBaseUrl() + ")");
                    dotApiStatus.setStyle("-fx-fill: #16a34a;");

                    baimendutakoLangileak.clear();
                    baimendutakoLangileak.addAll(iragaziak);

                    eraikiAvatarrak();

                    if (!baimendutakoLangileak.isEmpty()) {
                        hautatuLangilea(baimendutakoLangileak.get(0));
                    }
                });

            } catch (Exception ex) {
                Platform.runLater(() -> {
                    lblApiEgoera.setText("API: Errorea");
                    dotApiStatus.setStyle("-fx-fill: #dc2626;");
                    lblErrorea.setText("Errorea langileak kargatzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    private int getRolaLehentasuna(Langilea l) {
        String r = l.getRola().trim().toLowerCase();
        if (r.contains("sukaldari")) return 1;
        if (r.contains("barra")) return 2;
        if (r.contains("biltegi")) return 3;
        if (r.contains("admin") || r.contains("it")) return 4;
        if (r.contains("kudeatzaile")) return 5;
        return 99;
    }

    private void eraikiAvatarrak() {
        pnlAvatars.getChildren().clear();
        avatarCardNodes.clear();

        for (Langilea l : baimendutakoLangileak) {
            VBox card = new VBox(4);
            card.setAlignment(Pos.CENTER);
            card.setPrefWidth(96);
            card.setPrefHeight(102);
            card.setStyle("-fx-background-color: #ffffff; -fx-background-radius: 8; -fx-cursor: hand; -fx-padding: 6;");

            // Borobila initialekin
            StackPane avatarCircle = new StackPane();
            Circle circleBg = new Circle(24);
            circleBg.setStyle("-fx-fill: #f1f5f9; -fx-stroke: #cbd5e1; -fx-stroke-width: 1.5;");

            Label lblInitials = new Label(l.getInitialak());
            lblInitials.setStyle("-fx-font-size: 13px; -fx-font-weight: bold; -fx-text-fill: #475569;");
            avatarCircle.getChildren().addAll(circleBg, lblInitials);

            // Izena
            String shortName = l.getIzena();
            if (l.getErabiltzailea().equalsIgnoreCase("anderu")) {
                shortName = "Ander U.";
            } else if (l.getErabiltzailea().equalsIgnoreCase("ander_m_zuz")) {
                shortName = "Ander M.";
            }
            Label lblName = new Label(shortName);
            lblName.setStyle("-fx-font-size: 11px; -fx-font-weight: bold; -fx-text-fill: #334155;");

            // Rola etiketa
            String roleTag = switch (l.getRola()) {
                case "Sukaldaria" -> "Sukaldea";
                case "Barrakoa" -> "Barra";
                case "Biltegikoa" -> "Biltegia";
                case "AdminIT", "Admin" -> "Admin IT";
                case "Kudeatzailea" -> "Zuzendari";
                default -> l.getRola();
            };
            Label lblRole = new Label(roleTag);
            lblRole.setStyle("-fx-font-size: 9.5px; -fx-text-fill: #64748b;");

            card.getChildren().addAll(avatarCircle, lblName, lblRole);

            card.setOnMouseClicked(e -> hautatuLangilea(l));

            pnlAvatars.getChildren().add(card);
            avatarCardNodes.add(card);
        }
    }

    private void hautatuLangilea(Langilea l) {
        this.aukeratutakoLangilea = l;
        this.pinBuffer.setLength(0);
        freskatuPinDots();
        lblErrorea.setText("");

        lblHautatuaInfo.setText("Hautatua: " + l.getIzenOsoa() + " (" + l.getRola() + ")");

        for (int i = 0; i < baimendutakoLangileak.size(); i++) {
            Langilea current = baimendutakoLangileak.get(i);
            VBox card = avatarCardNodes.get(i);

            boolean isSelected = current.getId() == l.getId();
            StackPane avatarCircle = (StackPane) card.getChildren().get(0);
            Circle circleBg = (Circle) avatarCircle.getChildren().get(0);
            Label lblInitials = (Label) avatarCircle.getChildren().get(1);
            Label lblName = (Label) card.getChildren().get(1);
            Label lblRole = (Label) card.getChildren().get(2);

            if (isSelected) {
                card.setStyle("-fx-background-color: #eff6ff; -fx-background-radius: 8; -fx-cursor: hand; -fx-padding: 6;");
                circleBg.setStyle("-fx-fill: #eff6ff; -fx-stroke: #2563eb; -fx-stroke-width: 2.8;");
                lblInitials.setStyle("-fx-font-size: 13px; -fx-font-weight: bold; -fx-text-fill: #2563eb;");
                lblName.setStyle("-fx-font-size: 11px; -fx-font-weight: bold; -fx-text-fill: #1e293b;");
                lblRole.setStyle("-fx-font-size: 9.5px; -fx-font-weight: bold; -fx-text-fill: #2563eb;");
            } else {
                card.setStyle("-fx-background-color: #ffffff; -fx-background-radius: 8; -fx-cursor: hand; -fx-padding: 6;");
                circleBg.setStyle("-fx-fill: #f1f5f9; -fx-stroke: #cbd5e1; -fx-stroke-width: 1.5;");
                lblInitials.setStyle("-fx-font-size: 13px; -fx-font-weight: bold; -fx-text-fill: #475569;");
                lblName.setStyle("-fx-font-size: 11px; -fx-font-weight: normal; -fx-text-fill: #64748b;");
                lblRole.setStyle("-fx-font-size: 9.5px; -fx-font-weight: normal; -fx-text-fill: #94a3b8;");
            }
        }
    }

    // ==========================================
    // KEYPAD ETA PIN KUDEAKETA
    // ==========================================

    @FXML
    private void onKeyClick(ActionEvent event) {
        if (event.getSource() instanceof Button btn) {
            String text = btn.getText();
            gehituPinDigitoa(text);
        }
    }

    private void gehituPinDigitoa(String digitoa) {
        if (aukeratutakoLangilea == null) {
            lblErrorea.setText("Mesedez, hautatu langile bat lehenik.");
            return;
        }

        if (pinBuffer.length() < 4) {
            pinBuffer.append(digitoa);
            freskatuPinDots();
            lblErrorea.setText("");

            if (pinBuffer.length() == 4) {
                // Automatikoki saioa hasi 4 digituak sartzean
                Platform.runLater(this::onLoginClick);
            }
        }
    }

    @FXML
    private void onPinGarbituClick() {
        pinBuffer.setLength(0);
        freskatuPinDots();
        lblErrorea.setText("");
    }

    @FXML
    private void onPinAtzeraClick() {
        if (pinBuffer.length() > 0) {
            pinBuffer.deleteCharAt(pinBuffer.length() - 1);
            freskatuPinDots();
            lblErrorea.setText("");
        }
    }

    private void freskatuPinDots() {
        int len = pinBuffer.length();
        eguneratuDot(dotPin1, len >= 1);
        eguneratuDot(dotPin2, len >= 2);
        eguneratuDot(dotPin3, len >= 3);
        eguneratuDot(dotPin4, len >= 4);
    }

    private void eguneratuDot(Circle dot, boolean betea) {
        if (betea) {
            dot.setStyle("-fx-fill: #334155; -fx-stroke: #1e293b; -fx-stroke-width: 1.5;");
        } else {
            dot.setStyle("-fx-fill: transparent; -fx-stroke: #94a3b8; -fx-stroke-width: 1.5;");
        }
    }

    // ==========================================
    // LOGIN EXEKUZIOA ETA BIDERATZEA
    // ==========================================

    @FXML
    private void onLoginClick() {
        if (aukeratutakoLangilea == null) {
            lblErrorea.setText("Hautatu langile bat lehenik.");
            return;
        }

        if (pinBuffer.length() == 0) {
            lblErrorea.setText("Sartu zure 4 digituko PIN propioa.");
            return;
        }

        String pin = pinBuffer.toString();
        btnLogin.setDisable(true);
        btnLogin.setText("EGIAZTATZEN...");
        lblErrorea.setText("");

        new Thread(() -> {
            try {
                LoginErantzuna res = api.saioaHasi(aukeratutakoLangilea.getErabiltzailea(), pin);

                Platform.runLater(() -> {
                    btnLogin.setDisable(false);
                    btnLogin.setText("HASI SAIOA");

                    if (res != null && res.isArrakasta() && res.getLangilea() != null) {
                        Langilea l = res.getLangilea();
                        Sesioa.setUnekoLangilea(l);

                        // Bideratu langilearen rola arabera:
                        // - Sukaldaria -> sukaldea-view.fxml (KDS)
                        // - Barrakoa -> barra-view.fxml (BDS)
                        // - Biltegikoa -> biltegia-view.fxml (Stocka)
                        // - AdminIT / Admin / Kudeatzailea -> sukaldea-view.fxml (guztietarako sarbidearekin)
                        String helburuaFxml;
                        String rola = l.getRola().trim().toLowerCase();

                        if (rola.contains("sukaldari")) {
                            helburuaFxml = "sukaldea-view.fxml";
                        } else if (rola.contains("barra")) {
                            helburuaFxml = "barra-view.fxml";
                        } else if (rola.contains("biltegi")) {
                            helburuaFxml = "biltegia-view.fxml";
                        } else {
                            // Admin IT edo Zuzendaritza
                            helburuaFxml = "sukaldea-view.fxml";
                        }

                        try {
                            PantailaAldatzailea.aldatu(btnLogin, helburuaFxml);
                        } catch (IOException e) {
                            lblErrorea.setText("Errorea pantaila kargatzean: " + e.getMessage());
                        }

                    } else {
                        String msg = res != null ? res.getMezua() : "PIN okerra.";
                        lblErrorea.setText(msg);
                        pinBuffer.setLength(0);
                        freskatuPinDots();
                    }
                });

            } catch (Exception ex) {
                Platform.runLater(() -> {
                    btnLogin.setDisable(false);
                    btnLogin.setText("HASI SAIOA");
                    lblErrorea.setText("Errorea saioa hastean: " + ex.getMessage());
                    pinBuffer.setLength(0);
                    freskatuPinDots();
                });
            }
        }).start();
    }
}
