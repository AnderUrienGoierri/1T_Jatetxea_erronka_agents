package eus.goierri.jatetxea.barra.controller;

import eus.goierri.jatetxea.barra.BarraApp;
import eus.goierri.jatetxea.core.controller.BaseEskaeraController;
import eus.goierri.jatetxea.core.model.Eskaera;
import eus.goierri.jatetxea.core.model.EskaeraLerroa;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;

import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;

public class BarraController extends BaseEskaeraController {

    @FXML private TableColumn<Eskaera, Integer> colEdariak;
    @FXML private Label lblEdariKopurua;

    @Override
    protected Class<?> getAppResourceClass() {
        return BarraApp.class;
    }

    @FXML
    public void initialize() {
        hasieratuBaseEskaera();

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
                        if (e.contains("zerbitzat") || e.contains("prest")) {
                            setStyle("-fx-text-fill: #15803d; -fx-font-weight: bold;");
                        } else if (e.contains("martxan") || e.contains("prestatzen")) {
                            setStyle("-fx-text-fill: #1d4ed8; -fx-font-weight: bold;");
                        } else {
                            setStyle("-fx-text-fill: #b45309; -fx-font-weight: bold;");
                        }
                    }
                }
            });
        }

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
                        if (e.contains("zerbitzat") || e.contains("prest") || e.contains("egin")) {
                            setStyle("-fx-text-fill: #15803d; -fx-font-weight: bold;");
                        } else {
                            setStyle("-fx-text-fill: #b45309; -fx-font-weight: bold;");
                        }
                    }
                }
            });
        }

        if (colLerroEkintza != null) {
            colLerroEkintza.setCellFactory(column -> new TableCell<>() {
                private final Button btnZerbitzatu = new Button("Zerbitzatu");

                {
                    btnZerbitzatu.setStyle("-fx-font-size: 11px; -fx-padding: 3 8;");
                    btnZerbitzatu.setOnAction(event -> {
                        EskaeraLerroa lerroa = getTableView().getItems().get(getIndex());
                        aldatuLerroaEgoera(lerroa, "Zerbitzatuta");
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
                        if (egoera.contains("zerbitzat") || egoera.contains("prest") || egoera.contains("egin")) {
                            btnZerbitzatu.setDisable(true);
                            btnZerbitzatu.setText("✓ Zerbitzatuta");
                            btnZerbitzatu.setStyle("-fx-font-size: 11px; -fx-padding: 3 8; -fx-background-color: #e2e8f0; -fx-text-fill: #64748b;");
                        } else {
                            btnZerbitzatu.setDisable(false);
                            btnZerbitzatu.setText("Zerbitzatu");
                            btnZerbitzatu.setStyle("-fx-font-size: 11px; -fx-padding: 3 8; -fx-background-color: #0284c7; -fx-text-fill: white; -fx-font-weight: bold;");
                        }
                        setGraphic(btnZerbitzatu);
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
                    return !amaitua && e.getBarrakoEdariKopurua() > 0;
                })
                .collect(Collectors.toList());
    }

    @Override
    protected List<EskaeraLerroa> iragaziLerroak(List<EskaeraLerroa> guztiak) {
        if (guztiak == null) return Collections.emptyList();
        return guztiak.stream()
                .filter(EskaeraLerroa::isEdaria)
                .collect(Collectors.toList());
    }

    @Override
    protected void erakutsiEskaeraXehetasunak(Eskaera e) {
        super.erakutsiEskaeraXehetasunak(e);
        if (lblEdariKopurua != null) {
            int kop = e != null ? e.getBarrakoEdariKopurua() : 0;
            lblEdariKopurua.setText(kop + " edari barran");
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
                            lblEgoeraMezua.setText("Errorea edariaren egoera aldatzean.");
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
        onEdariakEgindaClick();
    }

    @FXML
    private void onEdariakEgindaClick() {
        Eskaera hautatua = tblEskaerak.getSelectionModel().getSelectedItem();
        if (hautatua == null) {
            erakutsiAlerta("Hautapenik ez", "Hautatu eskaera bat lehenik.");
            return;
        }

        new Thread(() -> {
            try {
                for (EskaeraLerroa l : hautatua.getLerroak()) {
                    if (l.isEdaria()) {
                        api.aldatuLerroEgoera(l.getId(), "Zerbitzatuta");
                    }
                }

                Platform.runLater(() -> {
                    if (lblEgoeraMezua != null) {
                        lblEgoeraMezua.setText("✓ Eskaera #" + hautatua.getId() + " barrako edari guztiak zerbitzatu dira.");
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

    @FXML
    private void onEdariaZaiClick() {
        EskaeraLerroa lerroa = tblLerroak != null ? tblLerroak.getSelectionModel().getSelectedItem() : null;
        if (lerroa == null) {
            erakutsiAlerta("Hautapenik ez", "Hautatu edari bat lehenik.");
            return;
        }
        aldatuLerroaEgoera(lerroa, "Eskatua");
    }

    @FXML
    private void onEdariaMartxanClick() {
        EskaeraLerroa lerroa = tblLerroak != null ? tblLerroak.getSelectionModel().getSelectedItem() : null;
        if (lerroa == null) {
            erakutsiAlerta("Hautapenik ez", "Hautatu edari bat lehenik.");
            return;
        }
        aldatuLerroaEgoera(lerroa, "Prestatua");
    }

    @FXML
    private void onEdariaEgindaClick() {
        EskaeraLerroa lerroa = tblLerroak != null ? tblLerroak.getSelectionModel().getSelectedItem() : null;
        if (lerroa == null) {
            erakutsiAlerta("Hautapenik ez", "Hautatu edari bat lehenik.");
            return;
        }
        aldatuLerroaEgoera(lerroa, "Zerbitzatuta");
    }
}
