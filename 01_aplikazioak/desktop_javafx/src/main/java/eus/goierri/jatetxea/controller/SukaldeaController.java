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
 * Sukaldeko KDS kontroladorea (Kitchen Display System - MVC kontroladore geruza).
 * Sukaldeko platerak bistaratzen eta kudeatzen ditu.
 */
public class SukaldeaController extends BaseEskaeraController {

    @FXML private TableColumn<Eskaera, Integer> colPlaterak;
    @FXML private Label lblPlaterKopurua;

    @FXML
    public void initialize() {
        // 1. Oinarrizko zutabeak eta tenporizadorea hasieratu
        hasieratuBaseEskaera();

        // 2. Sukaldeko zutabe espezifikoa lotu
        if (colPlaterak != null) {
            colPlaterak.setCellValueFactory(new PropertyValueFactory<>("sukaldekoPlaterKopurua"));
        }
        if (colEgoera != null) {
            colEgoera.setCellValueFactory(new PropertyValueFactory<>("egoera"));
        }

        // 3. Egoera zutabearen koloreak
        if (colLerroEgoera != null) {
            colLerroEgoera.setCellValueFactory(new PropertyValueFactory<>("egoera"));
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
                        if (e.contains("prestat") || e.contains("egin")) {
                            setStyle("-fx-text-fill: #15803d; -fx-font-weight: bold;");
                        } else if (e.contains("zerbitzat")) {
                            setStyle("-fx-text-fill: #64748b; -fx-font-weight: bold;");
                        } else if (e.contains("sukalde") || e.contains("martxan")) {
                            setStyle("-fx-text-fill: #1d4ed8; -fx-font-weight: bold;");
                        } else {
                            setStyle("-fx-text-fill: #b45309; -fx-font-weight: bold;");
                        }
                    }
                }
            });
        }

        // 4. Ekintza azkarreko botoia lerro bakoitzean ('Eginda jarri')
        if (colLerroEkintza != null) {
            colLerroEkintza.setCellFactory(param -> new TableCell<>() {
                private final Button btnEginda = new Button("Eginda jarri");
                {
                    btnEginda.setOnAction(e -> {
                        EskaeraLerroa lerroa = getTableView().getItems().get(getIndex());
                        if (lerroa != null) {
                            eguneratuLerroaZuzenean(lerroa, "Prestatua");
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
                            String egoera = lerroa.getEgoera() != null ? lerroa.getEgoera() : "";
                            if ("Prestatua".equalsIgnoreCase(egoera) || "Eginda".equalsIgnoreCase(egoera)) {
                                btnEginda.setText("Eginda dago");
                                btnEginda.setStyle("-fx-font-size: 11px; -fx-padding: 4 8; -fx-background-color: #dcfce7; -fx-text-fill: #15803d; -fx-font-weight: bold;");
                                btnEginda.setDisable(true);
                            } else if ("Zerbitzatua".equalsIgnoreCase(egoera)) {
                                btnEginda.setText("Zerbitzatuta");
                                btnEginda.setStyle("-fx-font-size: 11px; -fx-padding: 4 8; -fx-background-color: #f1f5f9; -fx-text-fill: #64748b; -fx-font-weight: bold;");
                                btnEginda.setDisable(true);
                            } else {
                                btnEginda.setText("Eginda jarri");
                                btnEginda.setStyle("-fx-font-size: 11px; -fx-padding: 4 8; -fx-background-color: #166534; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;");
                                btnEginda.setDisable(false);
                            }
                        }
                        setGraphic(btnEginda);
                    }
                }
            });
        }

        // 5. Klike bikoitzarekin platera 'Prestatua' moduan markatu daiteke azkar
        if (tblLerroak != null) {
            tblLerroak.setRowFactory(tv -> {
                TableRow<EskaeraLerroa> row = new TableRow<>();
                row.setOnMouseClicked(event -> {
                    if (event.getClickCount() == 2 && (!row.isEmpty())) {
                        EskaeraLerroa rowData = row.getItem();
                        if (rowData != null && !"Zerbitzatua".equalsIgnoreCase(rowData.getEgoera())) {
                            String berria = "Prestatua".equalsIgnoreCase(rowData.getEgoera()) ? "Sukaldean" : "Prestatua";
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
        return "Sukaldeko platerak";
    }

    @Override
    protected List<Eskaera> iragaziEskaerak(List<Eskaera> guztiak) {
        if (guztiak == null) return Collections.emptyList();
        return guztiak.stream()
                .filter(e -> e.getSukaldekoPlaterKopurua() > 0 && !"Zerbitzatua".equalsIgnoreCase(e.getEgoera()))
                .toList();
    }

    @Override
    protected List<EskaeraLerroa> lortuEskaeraLerroak(Eskaera eskaera) {
        return eskaera != null ? eskaera.getSukaldekoLerroak() : Collections.emptyList();
    }

    @Override
    protected void eguneratuXehetasunEtiketa(Eskaera eskaera) {
        if (lblPlaterKopurua != null) {
            lblPlaterKopurua.setText(eskaera != null ? eskaera.getSukaldekoPlaterKopurua() + " plater" : "0 plater");
        }
    }

    // ==========================================
    // SUKALDEKO BOTOI ETA EKINTZAK
    // ==========================================

    @FXML
    private void onPlateraZaiClick() {
        eguneratuHautatutakoLerroa("Eskatua");
    }

    @FXML
    private void onPlateraMartxanClick() {
        eguneratuHautatutakoLerroa("Sukaldean");
    }

    @FXML
    private void onPlateraEgindaClick() {
        eguneratuHautatutakoLerroa("Prestatua");
    }

    private void eguneratuHautatutakoLerroa(String egoeraBerria) {
        EskaeraLerroa lerroa = tblLerroak.getSelectionModel().getSelectedItem();
        if (lerroa == null) {
            erakutsiAlerta("Aukeratu platera", "Mesedez, aukeratu plater lerro bat egoera aldatzeko.");
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
                for (EskaeraLerroa l : eskaera.getSukaldekoLerroak()) {
                    api.eguneratuLerroEgoera(l.getId(), "Prestatua");
                }
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Eskaera #" + eskaera.getId() + " sukaldeko plater guztiak prestatu dira.");
                    kargatuDatuak();
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    lblEgoeraMezua.setText("Errorea platerak eguneratzean: " + ex.getMessage());
                });
            }
        }).start();
    }
}
