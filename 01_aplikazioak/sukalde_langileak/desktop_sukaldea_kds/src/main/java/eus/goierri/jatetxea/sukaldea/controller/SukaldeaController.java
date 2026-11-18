package eus.goierri.jatetxea.sukaldea.controller;

import eus.goierri.jatetxea.core.controller.BaseEskaeraController;
import eus.goierri.jatetxea.core.model.Eskaera;
import eus.goierri.jatetxea.core.model.EskaeraLerroa;
import eus.goierri.jatetxea.sukaldea.SukaldeaApp;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;

import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;

public class SukaldeaController extends BaseEskaeraController {

    @FXML private TableColumn<Eskaera, Integer> colPlaterak;
    @FXML private Label lblPlaterKopurua;

    @Override
    protected Class<?> getAppResourceClass() {
        return SukaldeaApp.class;
    }

    @FXML
    public void initialize() {
        hasieratuBaseEskaera();

        if (colPlaterak != null) {
            colPlaterak.setCellValueFactory(new PropertyValueFactory<>("sukaldekoPlaterKopurua"));
        }
        if (colEgoera != null) {
            colEgoera.setCellValueFactory(new PropertyValueFactory<>("egoera"));
        }

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

        if (colLerroEkintza != null) {
            colLerroEkintza.setCellFactory(column -> new TableCell<>() {
                private final Button btnEginda = new Button("Eginda");

                {
                    btnEginda.getStyleClass().add("btn-success");
                    btnEginda.setStyle("-fx-font-size: 11px; -fx-padding: 3 8;");
                    btnEginda.setOnAction(event -> {
                        EskaeraLerroa lerroa = getTableView().getItems().get(getIndex());
                        aldatuLerroaEgoera(lerroa, "Prestatuta");
                    });
                }

                @Override
                protected void updateItem(Void item, boolean empty) {
                    super.updateItem(item, empty);
                    if (empty) {
                        setGraphic(null);
                    } else {
                        EskaeraLerroa lerroa = getTableView().getItems().get(getIndex());
                        String egoera = lerroa.getEgoera() != null ? lerroa.getEgoera().toLowerCase() : "";
                        if (egoera.contains("prestat") || egoera.contains("zerbitzat") || egoera.contains("egin")) {
                            btnEginda.setDisable(true);
                            btnEginda.setText("✓ Prest");
                            btnEginda.setStyle("-fx-font-size: 11px; -fx-padding: 3 8; -fx-background-color: #e2e8f0; -fx-text-fill: #64748b;");
                        } else {
                            btnEginda.setDisable(false);
                            btnEginda.setText("Prestatu");
                            btnEginda.setStyle("-fx-font-size: 11px; -fx-padding: 3 8; -fx-background-color: #15803d; -fx-text-fill: white;");
                        }
                        setGraphic(btnEginda);
                    }
                }
            });
        }

        kargatuDatuak();
    }

    @Override
    protected List<Eskaera> iragaziEskaerak(List<Eskaera> guztiak) {
        if (guztiak == null) return Collections.emptyList();
        return guztiak.stream()
                .filter(e -> {
                    String egoera = e.getEgoera() != null ? e.getEgoera().toLowerCase() : "";
                    boolean amaitua = egoera.contains("amaitu") || egoera.contains("kobratu") || egoera.contains("bertan_behera");
                    return !amaitua && e.getSukaldekoPlaterKopurua() > 0;
                })
                .collect(Collectors.toList());
    }

    @Override
    protected List<EskaeraLerroa> iragaziLerroak(List<EskaeraLerroa> guztiak) {
        if (guztiak == null) return Collections.emptyList();
        return guztiak.stream()
                .filter(l -> !l.isEdaria())
                .collect(Collectors.toList());
    }

    @Override
    protected void erakutsiEskaeraXehetasunak(Eskaera e) {
        super.erakutsiEskaeraXehetasunak(e);
        if (lblPlaterKopurua != null) {
            int kop = e != null ? e.getSukaldekoPlaterKopurua() : 0;
            lblPlaterKopurua.setText(kop + " plater sukaldean");
        }
    }

    private void aldatuLerroaEgoera(EskaeraLerroa lerroa, String egoeraBerria) {
        if (lerroa == null) return;
        new Thread(() -> {
            try {
                boolean ok = api.aldatuLerroEgoera(lerroa.getId(), egoeraBerria);
                Platform.runLater(() -> {
                    if (ok) {
                        lerroa.setEgoera(egoeraBerria);
                        tblLerroak.refresh();
                        if (lblEgoeraMezua != null) {
                            lblEgoeraMezua.setText("✓ " + lerroa.getPlaterIzena() + " -> " + egoeraBerria);
                        }
                    } else {
                        if (lblEgoeraMezua != null) {
                            lblEgoeraMezua.setText("Errorea plateraren egoera aldatzean.");
                        }
                    }
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea: " + ex.getMessage());
                });
            }
        }).start();
    }

    @FXML
    private void onEskaeraEgindaClick() {
        Eskaera hautatua = tblEskaerak.getSelectionModel().getSelectedItem();
        if (hautatua == null) {
            erakutsiAlerta("Hautapenik ez", "Hautatu eskaera bat lehenik.");
            return;
        }

        new Thread(() -> {
            try {
                for (EskaeraLerroa l : hautatua.getLerroak()) {
                    if (!l.isEdaria()) {
                        api.aldatuLerroEgoera(l.getId(), "Prestatuta");
                    }
                }
                api.aldatuEskaeraEgoera(hautatua.getId(), "Prestatuta");

                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) {
                        lblEgoeraMezua.setText("✓ Eskaera #" + hautatua.getId() + " sukaldean prestatuta markatu da.");
                    }
                    kargatuDatuak();
                });
            } catch (Exception ex) {
                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) lblEgoeraMezua.setText("Errorea: " + ex.getMessage());
                });
            }
        }).start();
    }
}
