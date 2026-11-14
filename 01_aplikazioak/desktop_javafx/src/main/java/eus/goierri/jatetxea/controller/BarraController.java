package eus.goierri.jatetxea.controller;

import eus.goierri.jatetxea.PantailaAldatzailea;
import eus.goierri.jatetxea.api.ApiZerbitzua;
import eus.goierri.jatetxea.model.Eskaera;
import eus.goierri.jatetxea.model.EskaeraLerroa;
import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.Node;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.util.Duration;

import java.io.IOException;
import java.time.LocalTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;

/**
 * Barrako BDS kontroladorea (Bar Display System - MVC ereduaren kontroladore geruza).
 * Edariak eta kafeak kudeatzeko diseinatua.
 * Barrako langileak edariak eta kafeak soilik ikusten ditu, eta hauen egoera 'Zain' (edo 'Zerbitzatua') da, inoiz ez 'Sukaldean'.
 */
public class BarraController {

    @FXML private TableView<Eskaera> tblEskaerak;
    @FXML private TableColumn<Eskaera, Integer> colEskaeraId;
    @FXML private TableColumn<Eskaera, String> colMahaia;
    @FXML private TableColumn<Eskaera, String> colLangilea;
    @FXML private TableColumn<Eskaera, String> colEgoera;
    @FXML private TableColumn<Eskaera, Integer> colEdariak;
    @FXML private TableColumn<Eskaera, String> colOrdua;

    @FXML private TableView<EskaeraLerroa> tblLerroak;
    @FXML private TableColumn<EskaeraLerroa, String> colLerroPlatera;
    @FXML private TableColumn<EskaeraLerroa, Integer> colLerroKop;
    @FXML private TableColumn<EskaeraLerroa, String> colLerroOharrak;
    @FXML private TableColumn<EskaeraLerroa, String> colLerroEgoera;
    @FXML private TableColumn<EskaeraLerroa, Void> colLerroEkintza;

    @FXML private Label lblEskaeraKopurua;
    @FXML private Label lblHautatutakoEskaera;
    @FXML private Label lblEdariKopurua;
    @FXML private Label lblKonexioa;
    @FXML private Label lblEgoeraMezua;
    @FXML private Label lblAzkenFreskatzea;
    @FXML private CheckBox chkAutoRefresh;

    private final ApiZerbitzua api = new ApiZerbitzua();
    private Timeline tenporizadorea;

    @FXML
    public void initialize() {
        // 1. Eskaeren taularen zutabeak lotu
        colEskaeraId.setCellValueFactory(new PropertyValueFactory<>("id"));
        colMahaia.setCellValueFactory(new PropertyValueFactory<>("mahaiaTestua"));
        colLangilea.setCellValueFactory(new PropertyValueFactory<>("langileIzena"));
        colEgoera.setCellValueFactory(new PropertyValueFactory<>("barrakoEgoera"));
        colEdariak.setCellValueFactory(new PropertyValueFactory<>("barrakoEdariKopurua"));
        colOrdua.setCellValueFactory(new PropertyValueFactory<>("sortzeData"));

        // Eskaeren egoera zutabearen koloreak estilo bereziz
        colEgoera.setCellFactory(column -> new TableCell<>() {
            @Override
            protected void updateItem(String egoera, boolean empty) {
                super.updateItem(egoera, empty);
                if (empty || egoera == null) {
                    setText(null);
                    setStyle("");
                } else {
                    setText(egoera);
                    String e = egoera.trim().toLowerCase();
                    if (e.contains("zerbitzat")) {
                        setStyle("-fx-text-fill: #64748b; -fx-font-weight: bold;");
                    } else if (e.contains("prestat") || e.contains("egin")) {
                        setStyle("-fx-text-fill: #15803d; -fx-font-weight: bold;");
                    } else if (e.contains("martxan")) {
                        setStyle("-fx-text-fill: #1d4ed8; -fx-font-weight: bold;");
                    } else {
                        setStyle("-fx-text-fill: #b45309; -fx-font-weight: bold;");
                    }
                }
            }
        });

        // 2. Edarien lerroen taularen zutabeak lotu
        colLerroPlatera.setCellValueFactory(new PropertyValueFactory<>("platerIzena"));
        colLerroKop.setCellValueFactory(new PropertyValueFactory<>("kantitatea"));
        colLerroOharrak.setCellValueFactory(new PropertyValueFactory<>("oharrak"));
        colLerroEgoera.setCellValueFactory(new PropertyValueFactory<>("barrakoEgoera"));

        // Edarien egoera zutabearen koloreak (Zain, Prestatua, Zerbitzatua)
        colLerroEgoera.setCellFactory(column -> new TableCell<>() {
            @Override
            protected void updateItem(String egoera, boolean empty) {
                super.updateItem(egoera, empty);
                if (empty || egoera == null) {
                    setText(null);
                    setStyle("");
                } else {
                    setText(egoera);
                    String e = egoera.trim().toLowerCase();
                    if (e.contains("zerbitzat")) {
                        setStyle("-fx-text-fill: #64748b; -fx-font-weight: bold;");
                    } else if (e.contains("prestat") || e.contains("egin")) {
                        setStyle("-fx-text-fill: #15803d; -fx-font-weight: bold;");
                    } else if (e.contains("martxan")) {
                        setStyle("-fx-text-fill: #1d4ed8; -fx-font-weight: bold;");
                    } else {
                        // Zain
                        setStyle("-fx-text-fill: #b45309; -fx-font-weight: bold;");
                    }
                }
            }
        });

        // Ekintza azkarreko botoia lerro bakoitzean ('Zerbitzatu' / 'Zerbitzatuta')
        if (colLerroEkintza != null) {
            colLerroEkintza.setCellFactory(param -> new TableCell<>() {
                private final Button btnEkintza = new Button("Zerbitzatu");
                {
                    btnEkintza.setOnAction(e -> {
                        EskaeraLerroa lerroa = getTableView().getItems().get(getIndex());
                        if (lerroa != null) {
                            eguneratuLerroaZuzenean(lerroa, "Zerbitzatua");
                        }
                    });
                }

                @Override
                protected void updateItem(Void item, boolean empty) {
                    super.updateItem(item, empty);
                    if (empty || getIndex() >= getTableView().getItems().size()) {
                        setGraphic(null);
                    } else {
                        EskaeraLerroa lerroa = getTableView().getItems().get(getIndex());
                        if (lerroa != null) {
                            String egoera = lerroa.getBarrakoEgoera();
                            if ("Zerbitzatua".equalsIgnoreCase(egoera)) {
                                btnEkintza.setText("Zerbitzatuta");
                                btnEkintza.setStyle("-fx-font-size: 11px; -fx-padding: 4 8; -fx-background-color: #f1f5f9; -fx-text-fill: #64748b; -fx-font-weight: bold;");
                                btnEkintza.setDisable(true);
                            } else {
                                btnEkintza.setText("Zerbitzatu");
                                btnEkintza.setStyle("-fx-font-size: 11px; -fx-padding: 4 8; -fx-background-color: #166534; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;");
                                btnEkintza.setDisable(false);
                            }
                        }
                        setGraphic(btnEkintza);
                    }
                }
            });
        }

        // Klike bikoitzarekin ere edaria 'Zerbitzatua' moduan markatu daiteke azkar
        tblLerroak.setRowFactory(tv -> {
            TableRow<EskaeraLerroa> row = new TableRow<>();
            row.setOnMouseClicked(event -> {
                if (event.getClickCount() == 2 && (!row.isEmpty())) {
                    EskaeraLerroa rowData = row.getItem();
                    if (rowData != null) {
                        String berria = "Zerbitzatua".equalsIgnoreCase(rowData.getBarrakoEgoera()) ? "Eskatua" : "Zerbitzatua";
                        eguneratuLerroaZuzenean(rowData, berria);
                    }
                }
            });
            return row;
        });

        // 3. Hautapen gertaerak kudeatu
        tblEskaerak.getSelectionModel().selectedItemProperty().addListener((obs, zaharra, berria) -> {
            erakutsiEskaeraXehetasunak(berria);
        });

        // 4. Datuak kargatu
        kargatuDatuak();

        // 5. Auto-freskatzea prestatu (10 segundotik behin)
        tenporizadorea = new Timeline(new KeyFrame(Duration.seconds(10), e -> {
            if (chkAutoRefresh != null && chkAutoRefresh.isSelected()) {
                kargatuDatuak();
            }
        }));
        tenporizadorea.setCycleCount(Timeline.INDEFINITE);
        tenporizadorea.play();
    }

    public void kargatuDatuak() {
        new Thread(() -> {
            boolean apiOk = api.isApiMartxan();
            if (!apiOk) {
                Platform.runLater(() -> {
                    lblKonexioa.setText("API konexioa: DESKONEKTATUTA (http://localhost:5000)");
                    lblKonexioa.setStyle("-fx-text-fill: #dc2626; -fx-font-weight: bold;");
                    lblEgoeraMezua.setText("Ezin izan da backend zerbitzariarekin konektatu.");
                });
                return;
            }

            try {
                List<Eskaera> eskaerak = api.lortuEskaeraAktiboak();
                List<Eskaera> barrakoEskaerak = new ArrayList<>();
                for (Eskaera e : eskaerak) {
                    // Edariak dituzten eskaera aktiboak (oraindik kobratu edo itxi gabe daudenak)
                    if (e.getBarrakoEdariKopurua() > 0 && !"Kobratua".equalsIgnoreCase(e.getEgoera())) {
                        barrakoEskaerak.add(e);
                    }
                }

                Platform.runLater(() -> {
                    int aurrekoId = tblEskaerak.getSelectionModel().getSelectedItem() != null 
                            ? tblEskaerak.getSelectionModel().getSelectedItem().getId() 
                            : -1;

                    tblEskaerak.setItems(FXCollections.observableArrayList(barrakoEskaerak));
                    lblEskaeraKopurua.setText(barrakoEskaerak.size() + " eskaera");

                    lblKonexioa.setText("API konexioa: KONEKTATUTA (http://localhost:5000)");
                    lblKonexioa.setStyle("-fx-text-fill: #166534; -fx-font-weight: bold;");
                    lblAzkenFreskatzea.setText("Azken freskatzea: " + LocalTime.now().format(DateTimeFormatter.ofPattern("HH:mm:ss")));
                    lblEgoeraMezua.setText("Datuak eguneratuta (Barrako edariak eta kafeak).");

                    // Hautapena mantendu
                    if (aurrekoId != -1) {
                        for (Eskaera e : barrakoEskaerak) {
                            if (e.getId() == aurrekoId) {
                                tblEskaerak.getSelectionModel().select(e);
                                break;
                            }
                        }
                    } else if (!barrakoEskaerak.isEmpty()) {
                        tblEskaerak.getSelectionModel().selectFirst();
                    }
                });
            } catch (Exception e) {
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Errorea datuak kargatzean: " + e.getMessage());
                });
            }
        }).start();
    }

    private void erakutsiEskaeraXehetasunak(Eskaera eskaera) {
        if (eskaera == null) {
            tblLerroak.setItems(FXCollections.emptyObservableList());
            lblHautatutakoEskaera.setText("Hautatutako Eskaera: Ez dago hautatuta");
            lblEdariKopurua.setText("0 edari");
            return;
        }

        lblHautatutakoEskaera.setText("Eskaera #" + eskaera.getId() + " - " + eskaera.getMahaiaTestua() + " (" + eskaera.getLangileIzena() + ")");
        tblLerroak.setItems(FXCollections.observableArrayList(eskaera.getBarrakoLerroak()));
        lblEdariKopurua.setText(eskaera.getBarrakoEdariKopurua() + " edari");
    }

    @FXML
    private void onFreskatuClick() {
        kargatuDatuak();
    }

    @FXML
    private void onEdariaZaiClick() {
        eguneratuHautatutakoLerroa("Eskatua");
    }

    @FXML
    private void onEdariaMartxanClick() {
        eguneratuHautatutakoLerroa("Prestatua");
    }

    @FXML
    private void onEdariaEgindaClick() {
        eguneratuHautatutakoLerroa("Zerbitzatua");
    }

    private void eguneratuLerroaZuzenean(EskaeraLerroa lerroa, String egoeraBerria) {
        if (lerroa == null) return;

        new Thread(() -> {
            try {
                boolean ok = api.eguneratuLerroEgoera(lerroa.getId(), egoeraBerria);
                Platform.runLater(() -> {
                    if (ok) {
                        lerroa.setEgoera(egoeraBerria);
                        tblLerroak.refresh();
                        tblEskaerak.refresh();
                        lblEgoeraMezua.setText(lerroa.getPlaterIzena() + " -> " + lerroa.getBarrakoEgoera() + " egoerara aldatu da.");
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Errorea egoera aldatzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    private void eguneratuHautatutakoLerroa(String egoeraBerria) {
        EskaeraLerroa lerroa = tblLerroak.getSelectionModel().getSelectedItem();
        if (lerroa == null) {
            erakutsiAlerta("Aukeratu edaria", "Mesedez, aukeratu edari lerro bat egoera aldatzeko.");
            return;
        }
        eguneratuLerroaZuzenean(lerroa, egoeraBerria);
    }

    @FXML
    private void onEskaeraEgindaClick() {
        Eskaera eskaera = tblEskaerak.getSelectionModel().getSelectedItem();
        if (eskaera == null) {
            erakutsiAlerta("Aukeratu eskaera", "Mesedez, aukeratu eskaera bat ezkerreko zerrendatik.");
            return;
        }

        new Thread(() -> {
            try {
                for (EskaeraLerroa l : eskaera.getBarrakoLerroak()) {
                    api.eguneratuLerroEgoera(l.getId(), "Zerbitzatua");
                    l.setEgoera("Zerbitzatua");
                }
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Eskaera #" + eskaera.getId() + " barrako edari guztiak zerbitzatu dira.");
                    kargatuDatuak();
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Errorea edariak eguneratzean: " + ex.getMessage());
                });
            }
        }).start();
    }

    @FXML
    private void onSukaldeaClick(ActionEvent event) {
        try {
            if (tenporizadorea != null) {
                tenporizadorea.stop();
            }
            Node iturria = (Node) event.getSource();
            PantailaAldatzailea.aldatu(iturria, "sukaldea-view.fxml");
        } catch (IOException e) {
            erakutsiAlerta("Errorea pantaila aldatzean", "Ezin izan da Sukaldea pantaila kargatu: " + e.getMessage());
        }
    }

    @FXML
    private void onBiltegiaClick(ActionEvent event) {
        try {
            if (tenporizadorea != null) {
                tenporizadorea.stop();
            }
            Node iturria = (Node) event.getSource();
            PantailaAldatzailea.aldatu(iturria, "biltegia-view.fxml");
        } catch (IOException e) {
            erakutsiAlerta("Errorea pantaila aldatzean", "Ezin izan da Biltegia pantaila kargatu: " + e.getMessage());
        }
    }

    private void erakutsiAlerta(String izenburua, String mezua) {
        Alert alerta = new Alert(Alert.AlertType.INFORMATION);
        alerta.setTitle(izenburua);
        alerta.setHeaderText(null);
        alerta.setContentText(mezua);
        alerta.showAndWait();
    }
}
