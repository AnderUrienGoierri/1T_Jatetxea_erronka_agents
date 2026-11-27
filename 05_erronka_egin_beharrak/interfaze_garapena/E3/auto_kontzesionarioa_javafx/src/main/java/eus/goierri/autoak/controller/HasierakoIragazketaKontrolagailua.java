package eus.goierri.autoak.controller;

import eus.goierri.autoak.AutoAplikazioa;
import eus.goierri.autoak.model.AplikazioEgoera;
import eus.goierri.autoak.model.AutoKatalogoa;
import eus.goierri.autoak.model.AutoKonfigurazioa;
import eus.goierri.autoak.model.Autoa;
import eus.goierri.autoak.model.IragazkiEgoera;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.geometry.Pos;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.control.ToggleButton;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.FlowPane;
import javafx.scene.layout.HBox;
import javafx.scene.layout.Priority;
import javafx.scene.layout.Region;
import javafx.scene.layout.VBox;

import java.io.IOException;
import java.io.InputStream;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/**
 * 1. Interfazea: Hasierako iragazketa (MVC Controller).
 * Iragazki guztiak edozein ordenatan aplikatu, banaka kendu eta denbora errealean auto-sorta eguneratzen du.
 */
public final class HasierakoIragazketaKontrolagailua {
    private final AutoAplikazioa aplikazioa;
    private final AplikazioEgoera egoera;
    private boolean syncingUi;

    @FXML private ToggleButton useHiriaToggle;
    @FXML private ToggleButton useFamiliaToggle;
    @FXML private ToggleButton useBidaiakToggle;
    @FXML private ToggleButton useLanaToggle;

    @FXML private ToggleButton fuelGasolinaToggle;
    @FXML private ToggleButton fuelDieselaToggle;
    @FXML private ToggleButton fuelHibridoaToggle;
    @FXML private ToggleButton fuelElektrikoaToggle;

    @FXML private Label budgetRangeLabel;
    @FXML private Label minPriceLabel;
    @FXML private Label maxPriceLabel;
    @FXML private Slider minPriceSlider;
    @FXML private Slider maxPriceSlider;

    @FXML private ComboBox<String> seatsCombo;
    @FXML private ComboBox<String> doorsCombo;
    @FXML private ComboBox<String> bodyCombo;

    @FXML private FlowPane brandsPane;
    @FXML private FlowPane activeFiltersPane;
    @FXML private FlowPane carsFlowPane;
    @FXML private Label resultCountLabel;

    private final Map<String, Button> brandButtons = new LinkedHashMap<>();

    public HasierakoIragazketaKontrolagailua() {
        this(null, new AplikazioEgoera(AutoKatalogoa.sortuKatalogoa()));
    }

    public HasierakoIragazketaKontrolagailua(AutoAplikazioa aplikazioa, AplikazioEgoera egoera) {
        this.aplikazioa = aplikazioa;
        this.egoera = egoera;
    }

    @FXML
    private void initialize() {
        seatsCombo.setItems(FXCollections.observableArrayList("Guztiak", "Gutxienez 4 plaza", "Gutxienez 5 plaza", "7 plaza"));
        doorsCombo.setItems(FXCollections.observableArrayList("Guztiak", "4 ate", "5 ate"));
        bodyCombo.setItems(FXCollections.observableArrayList("Guztiak", "SUV", "Trinkoa", "Berlina", "Familiarra", "Furgoneta"));

        buildBrandButtons();
        restoreControlsFromState();

        minPriceSlider.valueProperty().addListener((obs, oldVal, newVal) -> {
            if (syncingUi) return;
            int min = roundThousand(newVal.doubleValue());
            if (min > roundThousand(maxPriceSlider.getValue())) {
                syncingUi = true;
                maxPriceSlider.setValue(min);
                syncingUi = false;
            }
            onFiltersChanged();
        });

        maxPriceSlider.valueProperty().addListener((obs, oldVal, newVal) -> {
            if (syncingUi) return;
            int max = roundThousand(newVal.doubleValue());
            if (max < roundThousand(minPriceSlider.getValue())) {
                syncingUi = true;
                minPriceSlider.setValue(max);
                syncingUi = false;
            }
            onFiltersChanged();
        });

        seatsCombo.valueProperty().addListener((obs, o, n) -> { if (!syncingUi) onFiltersChanged(); });
        doorsCombo.valueProperty().addListener((obs, o, n) -> { if (!syncingUi) onFiltersChanged(); });
        bodyCombo.valueProperty().addListener((obs, o, n) -> { if (!syncingUi) onFiltersChanged(); });

        refreshResults();
    }

    private void buildBrandButtons() {
        brandsPane.getChildren().clear();
        brandButtons.clear();
        for (String brand : AutoKatalogoa.lortuMarkak(egoera.getKatalogoa())) {
            Button btn = new Button(brand);
            btn.getStyleClass().setAll("brand-neutral");
            btn.setOnAction(e -> cycleBrandState(brand));
            brandButtons.put(brand, btn);
            brandsPane.getChildren().add(btn);
        }
    }

    private void cycleBrandState(String brand) {
        IragazkiEgoera f = egoera.getIragazkiak();
        if (f.getMarkaNahiak().contains(brand)) {
            f.getMarkaNahiak().remove(brand);
            f.getMarkaBaztertuak().add(brand);
        } else if (f.getMarkaBaztertuak().contains(brand)) {
            f.getMarkaBaztertuak().remove(brand);
        } else {
            f.getMarkaNahiak().add(brand);
        }
        updateBrandButtonsStyle();
        refreshResults();
    }

    private void updateBrandButtonsStyle() {
        IragazkiEgoera f = egoera.getIragazkiak();
        for (var entry : brandButtons.entrySet()) {
            String brand = entry.getKey();
            Button btn = entry.getValue();
            if (f.getMarkaNahiak().contains(brand)) {
                btn.setText("✓ " + brand);
                btn.getStyleClass().setAll("brand-include");
            } else if (f.getMarkaBaztertuak().contains(brand)) {
                btn.setText("✕ " + brand);
                btn.getStyleClass().setAll("brand-exclude");
            } else {
                btn.setText(brand);
                btn.getStyleClass().setAll("brand-neutral");
            }
        }
    }

    private void restoreControlsFromState() {
        syncingUi = true;
        IragazkiEgoera f = egoera.getIragazkiak();

        useHiriaToggle.setSelected(f.getErabilerak().contains("Hiria"));
        useFamiliaToggle.setSelected(f.getErabilerak().contains("Familia"));
        useBidaiakToggle.setSelected(f.getErabilerak().contains("Bidaiak"));
        useLanaToggle.setSelected(f.getErabilerak().contains("Lana"));

        fuelGasolinaToggle.setSelected(f.getErregaiak().contains("Gasolina"));
        fuelDieselaToggle.setSelected(f.getErregaiak().contains("Diesela"));
        fuelHibridoaToggle.setSelected(f.getErregaiak().contains("Hibridoa"));
        fuelElektrikoaToggle.setSelected(f.getErregaiak().contains("Elektrikoa"));

        minPriceSlider.setValue(f.getAurrekontuMin());
        maxPriceSlider.setValue(f.getAurrekontuMax());
        updateBudgetLabels(f.getAurrekontuMin(), f.getAurrekontuMax());

        seatsCombo.setValue(f.getGutxienekoPlazak() == null ? "Guztiak"
                : f.getGutxienekoPlazak() == 7 ? "7 plaza" : "Gutxienez " + f.getGutxienekoPlazak() + " plaza");
        doorsCombo.setValue(f.getAteak() == null ? "Guztiak" : f.getAteak() + " ate");
        bodyCombo.setValue(f.getKarrozeria());

        updateBrandButtonsStyle();
        syncingUi = false;
    }

    @FXML
    private void onFiltersChanged() {
        if (syncingUi) return;
        IragazkiEgoera f = egoera.getIragazkiak();

        f.getErabilerak().clear();
        if (useHiriaToggle.isSelected()) f.getErabilerak().add("Hiria");
        if (useFamiliaToggle.isSelected()) f.getErabilerak().add("Familia");
        if (useBidaiakToggle.isSelected()) f.getErabilerak().add("Bidaiak");
        if (useLanaToggle.isSelected()) f.getErabilerak().add("Lana");

        f.getErregaiak().clear();
        if (fuelGasolinaToggle.isSelected()) f.getErregaiak().add("Gasolina");
        if (fuelDieselaToggle.isSelected()) f.getErregaiak().add("Diesela");
        if (fuelHibridoaToggle.isSelected()) f.getErregaiak().add("Hibridoa");
        if (fuelElektrikoaToggle.isSelected()) f.getErregaiak().add("Elektrikoa");

        int min = roundThousand(minPriceSlider.getValue());
        int max = roundThousand(maxPriceSlider.getValue());
        f.setAurrekontuMin(min);
        f.setAurrekontuMax(max);
        updateBudgetLabels(min, max);

        String seatsVal = seatsCombo.getValue();
        f.setGutxienekoPlazak(switch (seatsVal) {
            case "Gutxienez 4 plaza" -> 4;
            case "Gutxienez 5 plaza" -> 5;
            case "7 plaza" -> 7;
            case null, default -> null;
        });

        String doorsVal = doorsCombo.getValue();
        f.setAteak(switch (doorsVal) {
            case "4 ate" -> 4;
            case "5 ate" -> 5;
            case null, default -> null;
        });

        f.setKarrozeria(bodyCombo.getValue());

        refreshResults();
    }

    @FXML
    private void resetAllFilters() {
        egoera.getIragazkiak().garbituGuztia();
        restoreControlsFromState();
        refreshResults();
    }

    private void updateBudgetLabels(int min, int max) {
        String minTxt = AutoKonfigurazioa.formatEuro(min);
        String maxTxt = AutoKonfigurazioa.formatEuro(max);
        budgetRangeLabel.setText(minTxt + " – " + maxTxt);
        minPriceLabel.setText("Gutxienekoa: " + minTxt);
        maxPriceLabel.setText("Gehienekoa: " + maxTxt);
    }

    private void refreshResults() {
        renderActiveChips();

        List<Autoa> filtered = egoera.getKatalogoa().stream()
                .filter(egoera.getIragazkiak()::betetzenDu)
                .toList();

        resultCountLabel.setText(filtered.size() + " / " + egoera.getKatalogoa().size() + " auto");
        carsFlowPane.getChildren().clear();

        if (filtered.isEmpty()) {
            VBox emptyBox = new VBox(12);
            emptyBox.getStyleClass().add("card");
            emptyBox.setPrefWidth(520);
            emptyBox.setAlignment(Pos.CENTER_LEFT);
            Label title = new Label("Ez dago baldintza hauek betetzen dituen autorik.");
            title.getStyleClass().add("section-heading");
            Label hint = new Label("Probatu aurrekontua zabaltzen, marka-iragazki bat kentzen edo hutsetik hasten.");
            hint.setWrapText(true);
            hint.getStyleClass().add("muted-text");
            Button resetBtn = new Button("Iragazki guztiak garbitu");
            resetBtn.getStyleClass().add("primary-button");
            resetBtn.setOnAction(e -> resetAllFilters());
            emptyBox.getChildren().addAll(title, hint, resetBtn);
            carsFlowPane.getChildren().add(emptyBox);
            return;
        }

        for (Autoa autoa : filtered) {
            carsFlowPane.getChildren().add(createCarCard(autoa));
        }
    }

    private void renderActiveChips() {
        activeFiltersPane.getChildren().clear();
        IragazkiEgoera f = egoera.getIragazkiak();

        if (f.aktiboKopurua() == 0) {
            Label none = new Label("Iragazki aktiborik ez — Katalogo osoa ikusgai");
            none.getStyleClass().add("muted-text");
            activeFiltersPane.getChildren().add(none);
            return;
        }

        for (String u : List.copyOf(f.getErabilerak())) {
            activeFiltersPane.getChildren().add(makeChip("Erabilera: " + u, () -> {
                f.getErabilerak().remove(u);
                restoreControlsFromState();
                refreshResults();
            }));
        }
        for (String fuel : List.copyOf(f.getErregaiak())) {
            activeFiltersPane.getChildren().add(makeChip("Erregaia: " + fuel, () -> {
                f.getErregaiak().remove(fuel);
                restoreControlsFromState();
                refreshResults();
            }));
        }
        if (f.getAurrekontuMin() > IragazkiEgoera.AURREKONTU_MIN_DEFAULT
                || f.getAurrekontuMax() < IragazkiEgoera.AURREKONTU_MAX_DEFAULT) {
            activeFiltersPane.getChildren().add(makeChip(
                    "Aurrekontua: " + AutoKonfigurazioa.formatEuro(f.getAurrekontuMin())
                            + " – " + AutoKonfigurazioa.formatEuro(f.getAurrekontuMax()),
                    () -> {
                        f.setAurrekontuMin(IragazkiEgoera.AURREKONTU_MIN_DEFAULT);
                        f.setAurrekontuMax(IragazkiEgoera.AURREKONTU_MAX_DEFAULT);
                        restoreControlsFromState();
                        refreshResults();
                    }));
        }
        if (f.getGutxienekoPlazak() != null) {
            activeFiltersPane.getChildren().add(makeChip("Plazak: ≥" + f.getGutxienekoPlazak(), () -> {
                f.setGutxienekoPlazak(null);
                restoreControlsFromState();
                refreshResults();
            }));
        }
        if (f.getAteak() != null) {
            activeFiltersPane.getChildren().add(makeChip("Ateak: " + f.getAteak(), () -> {
                f.setAteak(null);
                restoreControlsFromState();
                refreshResults();
            }));
        }
        if (!"Guztiak".equalsIgnoreCase(f.getKarrozeria())) {
            activeFiltersPane.getChildren().add(makeChip("Karrozeria: " + f.getKarrozeria(), () -> {
                f.setKarrozeria("Guztiak");
                restoreControlsFromState();
                refreshResults();
            }));
        }
        for (String b : List.copyOf(f.getMarkaNahiak())) {
            activeFiltersPane.getChildren().add(makeChip("✓ Marka: " + b, () -> {
                f.getMarkaNahiak().remove(b);
                restoreControlsFromState();
                refreshResults();
            }));
        }
        for (String b : List.copyOf(f.getMarkaBaztertuak())) {
            activeFiltersPane.getChildren().add(makeChip("✕ Baztertuta: " + b, () -> {
                f.getMarkaBaztertuak().remove(b);
                restoreControlsFromState();
                refreshResults();
            }));
        }
    }

    private Button makeChip(String text, Runnable onRemove) {
        Button chip = new Button(text + "  ✕");
        chip.getStyleClass().add("active-chip");
        chip.setOnAction(e -> onRemove.run());
        return chip;
    }

    private VBox createCarCard(Autoa autoa) {
        VBox card = new VBox(10);
        card.setPrefWidth(300);
        card.getStyleClass().add("car-card");

        HBox topBadges = new HBox(6);
        Label fuelBadge = new Label(autoa.erregaia());
        fuelBadge.getStyleClass().add("badge-fuel");
        Label bodyBadge = new Label(autoa.karrozeria());
        bodyBadge.getStyleClass().add("badge-body");
        topBadges.getChildren().addAll(fuelBadge, bodyBadge);

        if (egoera.baduPertsonalizazioa(autoa)) {
            Label cfgBadge = new Label("Pertsonalizatua");
            cfgBadge.getStyleClass().add("badge-configured");
            topBadges.getChildren().add(cfgBadge);
        }

        ImageView imgView = new ImageView();
        imgView.setFitWidth(260);
        imgView.setFitHeight(120);
        imgView.setPreserveRatio(true);
        InputStream imgStream = getClass().getResourceAsStream("/eus/goierri/autoak/img/" + autoa.irudiFitxategia());
        if (imgStream != null) {
            imgView.setImage(new Image(imgStream));
        }

        Label title = new Label(autoa.getIzenOsoa());
        title.getStyleClass().add("car-title");

        Label specs = new Label(autoa.getMotorLehenetsia().potentziaCv() + " CV  ·  "
                + autoa.getMotorLehenetsia().kontsumoa() + "  ·  "
                + autoa.plazak() + " plaza · " + autoa.ateak() + " ate");
        specs.getStyleClass().add("muted-text");

        Label uses = new Label("Erabilera: " + String.join(", ", autoa.erabilerak()));
        uses.getStyleClass().add("muted-text");

        AutoKonfigurazioa cfg = egoera.lortuKonfigurazioa(autoa);
        int erakutsiPrezioa = cfg.isPertsonalizatua() ? cfg.kalkulatuAzkenPrezioa() : autoa.oinarrizkoPrezioa();
        String prezioEtiketa = cfg.isPertsonalizatua() ? "Zure konfigurazioa" : "Oinarrizko prezioa";

        VBox priceBox = new VBox(1);
        Label priceCaption = new Label(prezioEtiketa);
        priceCaption.getStyleClass().add("muted-text");
        Label priceLabel = new Label(AutoKonfigurazioa.formatEuro(erakutsiPrezioa));
        priceLabel.getStyleClass().add("car-price");
        priceBox.getChildren().addAll(priceCaption, priceLabel);

        Region spacer = new Region();
        HBox.setHgrow(spacer, Priority.ALWAYS);

        Button selectBtn = new Button(cfg.isPertsonalizatua() ? "Jarraitu →" : "Pertsonalizatu →");
        selectBtn.getStyleClass().add("primary-button");
        selectBtn.setOnAction(e -> openCarCustomization(autoa));

        HBox footer = new HBox(10, priceBox, spacer, selectBtn);
        footer.setAlignment(Pos.CENTER_LEFT);

        card.getChildren().addAll(topBadges, imgView, title, specs, uses, footer);
        return card;
    }

    private void openCarCustomization(Autoa autoa) {
        if (aplikazioa == null) return;
        try {
            aplikazioa.erakutsiPertsonalizazioa(autoa);
        } catch (IOException ex) {
            throw new IllegalStateException("Ezin izan da pertsonalizazio pantaila ireki.", ex);
        }
    }

    private static int roundThousand(double value) {
        return (int) (Math.round(value / 1000.0) * 1000);
    }
}
