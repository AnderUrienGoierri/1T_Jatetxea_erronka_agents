package eus.goierri.jatetxea.controller;

import eus.goierri.jatetxea.model.Eskaera;
import eus.goierri.jatetxea.model.EskaeraLerroa;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;

import java.util.Collections;
import java.util.List;

/**
 * Barrako BDS kontroladorea (Bar Display System - MVC kontroladore geruza).
 * Edariak eta kafeak bistaratzen eta kudeatzen ditu.
 */
public class BarraController extends BaseEskaeraController {

    @FXML private TableColumn<Eskaera, Integer> colEdariak;
    @FXML private Label lblEdariKopurua;

    @FXML
    public void initialize() {
        // 1. Oinarrizko zutabeak eta tenporizadorea hasieratu
        hasieratuBaseEskaera();

        // 2. Barrako zutabe espezifikoak lotu
        if (colEdariak != null) {
            colEdariak.setCellValueFactory(new PropertyValueFactory<>("barrakoEdariKopurua"));
        }
        if (colEgoera != null) {
            colEgoera.setCellValueFactory(new PropertyValueFactory<>("barrakoEgoera"));
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
        }

        // 3. Edarien lerroen egoera zutabea
        if (colLerroEgoera != null) {
            colLerroEgoera.setCellValueFactory(new PropertyValueFactory<>("barrakoEgoera"));
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
                            setStyle("-fx-text-fill: #b45309; -fx-font-weight: bold;");
                        }
                    }
                }
            });
        }

        // 4. Ekintza azkarreko botoia lerro bakoitzean ('Zerbitzatu')
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

        // 5. Klike bikoitzarekin edaria 'Zerbitzatua' moduan markatu daiteke azkar
        if (tblLerroak != null) {
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
        }

        // 6. Hasierako datuak kargatu
        kargatuDatuak();
    }

    @Override
    protected String getModuluIzena() {
        return "Barrako edariak eta kafeak";
    }

    @Override
    protected List<Eskaera> iragaziEskaerak(List<Eskaera> guztiak) {
        if (guztiak == null) return Collections.emptyList();
        return guztiak.stream()
                .filter(e -> e.getBarrakoEdariKopurua() > 0 && !"Kobratua".equalsIgnoreCase(e.getEgoera()))
                .toList();
    }

    @Override
    protected List<EskaeraLerroa> lortuEskaeraLerroak(Eskaera eskaera) {
        return eskaera != null ? eskaera.getBarrakoLerroak() : Collections.emptyList();
    }

    @Override
    protected void eguneratuXehetasunEtiketa(Eskaera eskaera) {
        if (lblEdariKopurua != null) {
            lblEdariKopurua.setText(eskaera != null ? eskaera.getBarrakoEdariKopurua() + " edari" : "0 edari");
        }
    }

    // ==========================================
    // BARRAKO BOTOI ETA EKINTZAK
    // ==========================================

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
}
