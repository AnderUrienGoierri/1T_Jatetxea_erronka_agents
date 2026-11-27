package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiKontratua;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.TxatEgoera;
import eus.goierri.jatetxea.admin.model.TxatMezua;
import javafx.animation.Animation;
import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.control.TextField;
import javafx.util.Duration;

import java.util.List;

/**
 * Gerente aplikazioko barne-txat kontroladorea (BaseAdminKontrolagailua klase abstraktuaren azpiklasea).
 * Gela bidezko mezuak denbora errealean sinkronizatzen ditu (2 segundoro) eta txata_aktibo baimena betearazten du.
 */
public final class TxataKontrolagailua extends BaseAdminKontrolagailua {
    private final ObservableList<TxatMezua> messages = FXCollections.observableArrayList();
    private Timeline autoRefreshTimeline;
    private boolean loading;
    private boolean chatAllowed = true;

    @FXML private ComboBox<String> roomCombo;
    @FXML private ListView<TxatMezua> messagesList;
    @FXML private TextField messageField;
    @FXML private Button sendButton;
    @FXML private Label chatStatusLabel;
    @FXML private Label chatMessage;

    public TxataKontrolagailua() {
        this(null, null);
    }

    public TxataKontrolagailua(ApiKontratua apiClient, KudeatzaileSaioa session) {
        super(apiClient, session);
    }

    @Override
    public String getPantailaIzena() {
        return "Barne Txata";
    }

    @FXML
    private void initialize() {
        roomCombo.setItems(FXCollections.observableArrayList("Orokorra", "Jangela", "Sukaldea", "Barra"));
        roomCombo.getSelectionModel().selectFirst();
        roomCombo.valueProperty().addListener((obs, oldVal, newVal) -> {
            if (apiPrestDago()) {
                refresh();
            }
        });

        messagesList.setItems(messages);
        messagesList.setCellFactory(list -> new ListCell<>() {
            @Override
            protected void updateItem(TxatMezua item, boolean empty) {
                super.updateItem(item, empty);
                if (empty || item == null) {
                    setText(null);
                    setStyle("");
                    return;
                }
                setText(item.toString());
                boolean isOwn = saioaAktiboDago() && item.langileaId == session.getUser().id;
                setStyle(isOwn
                        ? "-fx-font-weight: bold; -fx-text-fill: #881337; -fx-padding: 6 8 6 8;"
                        : "-fx-text-fill: #1e293b; -fx-padding: 6 8 6 8;");
            }
        });

        if (apiPrestDago()) {
            refresh();
            autoRefreshTimeline = new Timeline(new KeyFrame(Duration.seconds(2.5), e -> refreshSilent()));
            autoRefreshTimeline.setCycleCount(Animation.INDEFINITE);
            autoRefreshTimeline.play();
        }
    }

    @Override
    public void garbitu() {
        if (autoRefreshTimeline != null) {
            autoRefreshTimeline.stop();
        }
    }

    @FXML
    @Override
    public void refresh() {
        if (!apiPrestDago()) {
            return;
        }
        ezarriMezua(chatMessage, "Mezuak kargatzen...", null);
        loadMessages(false);
    }

    private void refreshSilent() {
        if (!apiPrestDago() || loading) {
            return;
        }
        loadMessages(true);
    }

    private void loadMessages(boolean silent) {
        if (loading) {
            return;
        }
        loading = true;
        String room = roomCombo.getValue() == null ? "Orokorra" : roomCombo.getValue();
        Integer userId = saioaAktiboDago() ? session.getUser().id : null;

        EkintzaAsinkronoak.run(() -> {
                    TxatEgoera egoera = userId != null ? apiClient.getTxatEgoera(userId) : null;
                    if (egoera != null && !egoera.baimenduta) {
                        return new ChatLoadResult(false, List.of(), "Txata desaktibatuta daukazu.");
                    }
                    List<TxatMezua> list = apiClient.getTxatMezuak(room, userId);
                    return new ChatLoadResult(true, list, null);
                },
                result -> {
                    loading = false;
                    chatAllowed = result.allowed();
                    updateInputState();
                    if (!result.allowed()) {
                        messages.clear();
                        chatStatusLabel.setText("Txata DESAKTIBATUTA");
                        chatStatusLabel.getStyleClass().setAll("status-error");
                        ezarriMezua(chatMessage, result.reason(), "status-error");
                        return;
                    }
                    boolean changed = messages.size() != result.messages().size()
                            || (!messages.isEmpty() && !result.messages().isEmpty()
                            && messages.get(messages.size() - 1).id != result.messages().get(result.messages().size() - 1).id);
                    if (changed || !silent) {
                        messages.setAll(result.messages());
                        if (!messages.isEmpty()) {
                            messagesList.scrollTo(messages.size() - 1);
                        }
                    }
                    chatStatusLabel.setText("Txata AKTIBATUTA · " + room);
                    chatStatusLabel.getStyleClass().setAll("status-ok");
                    if (!silent) {
                        ezarriMezua(chatMessage, result.messages().size() + " mezu '" + room + "' gelan.", "status-ok");
                    }
                },
                error -> {
                    loading = false;
                    chatStatusLabel.setText("Konexio errorea");
                    chatStatusLabel.getStyleClass().setAll("status-error");
                    ezarriMezua(chatMessage, "Ezin izan dira mezuak kargatu: " + error.getMessage(), "status-error");
                });
    }

    @FXML
    private void sendMessage() {
        if (!apiPrestDago() || !saioaAktiboDago()) {
            return;
        }
        if (!chatAllowed) {
            ezarriMezua(chatMessage, "Txata desaktibatuta daukazu eta ezin duzu mezurik bidali.", "status-error");
            return;
        }
        String text = messageField.getText() == null ? "" : messageField.getText().trim();
        if (text.isBlank()) {
            ezarriMezua(chatMessage, "Idatzi mezu bat bidali aurretik.", "status-warning");
            return;
        }

        String room = roomCombo.getValue() == null ? "Orokorra" : roomCombo.getValue();
        int userId = session.getUser().id;
        sendButton.setDisable(true);
        ezarriMezua(chatMessage, "Mezua bidaltzen...", null);

        EkintzaAsinkronoak.run(() -> {
                    apiClient.bidaliTxatMezua(userId, room, text);
                    return null;
                },
                ignored -> {
                    messageField.clear();
                    sendButton.setDisable(false);
                    ezarriMezua(chatMessage, "Mezua bidali da.", "status-ok");
                    loadMessages(false);
                },
                error -> {
                    sendButton.setDisable(false);
                    ezarriMezua(chatMessage, "Ezin izan da mezua bidali: " + error.getMessage(), "status-error");
                });
    }

    private void updateInputState() {
        boolean disabled = !chatAllowed || !saioaAktiboDago();
        messageField.setDisable(disabled);
        sendButton.setDisable(disabled);
    }

    private record ChatLoadResult(boolean allowed, List<TxatMezua> messages, String reason) {
    }
}
