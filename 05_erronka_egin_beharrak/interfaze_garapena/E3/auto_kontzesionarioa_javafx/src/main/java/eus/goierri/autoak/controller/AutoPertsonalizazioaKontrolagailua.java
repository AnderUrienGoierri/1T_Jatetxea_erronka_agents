package eus.goierri.autoak.controller;

import eus.goierri.autoak.AutoAplikazioa;
import eus.goierri.autoak.model.AplikazioEgoera;
import eus.goierri.autoak.model.AutoKatalogoa;
import eus.goierri.autoak.model.AutoKonfigurazioa;
import eus.goierri.autoak.model.Autoa;
import javafx.fxml.FXML;
import javafx.geometry.Pos;
import javafx.scene.control.Accordion;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.CheckBox;
import javafx.scene.control.Label;
import javafx.scene.control.RadioButton;
import javafx.scene.control.TitledPane;
import javafx.scene.control.ToggleButton;
import javafx.scene.control.ToggleGroup;
import javafx.scene.effect.DropShadow;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.FlowPane;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.shape.Circle;
import javafx.scene.shape.Rectangle;

import java.io.IOException;
import java.io.InputStream;
import java.util.LinkedHashMap;
import java.util.Map;

/**
 * 2. Interfazea: Autoaren xehetasunak eta pertsonalizazio-panela (MVC Controller).
 * Accordion bidezko aukeraketa antolatua, zuzeneko kolore/akabera aurrebista,
 * azken prezioaren kalkulu automatikoa eta egoera galdu gabe 1. pantailara itzultzea.
 */
public final class AutoPertsonalizazioaKontrolagailua {
    private final AutoAplikazioa aplikazioa;
    private final AplikazioEgoera egoera;
    private AutoKonfigurazioa konfigurazioa;

    @FXML private Label carTitleLabel;
    @FXML private Label carSubtitleLabel;
    @FXML private Label viewModeLabel;
    @FXML private ToggleButton viewExteriorBtn;
    @FXML private ToggleButton viewInteriorBtn;
    @FXML private ToggleButton viewDimensionsBtn;

    @FXML private Rectangle colorBackdropRect;
    @FXML private ImageView carImageView;
    @FXML private Label visualOverlayLabel;
    @FXML private Circle exteriorSwatchCircle;
    @FXML private Label exteriorPreviewLabel;
    @FXML private Circle interiorSwatchCircle;
    @FXML private Label interiorPreviewLabel;

    @FXML private Label specPowerLabel;
    @FXML private Label specConsumptionLabel;
    @FXML private Label specDimensionsLabel;
    @FXML private Label specTrunkLabel;
    @FXML private Label descriptionLabel;
    @FXML private Label useCasesLabel;

    @FXML private Accordion configAccordion;
    @FXML private TitledPane enginePane;
    @FXML private TitledPane colorPane;
    @FXML private TitledPane interiorPane;
    @FXML private TitledPane extrasPane;

    @FXML private VBox engineOptionsBox;
    @FXML private FlowPane colorOptionsPane;
    @FXML private VBox interiorOptionsBox;
    @FXML private VBox extrasOptionsBox;

    @FXML private Label priceBreakdownLabel;
    @FXML private Label finalPriceLabel;

    private final Map<Autoa.KoloreAukera, Button> colorButtons = new LinkedHashMap<>();

    public AutoPertsonalizazioaKontrolagailua() {
        this(null, new AplikazioEgoera(AutoKatalogoa.sortuKatalogoa()));
    }

    public AutoPertsonalizazioaKontrolagailua(AutoAplikazioa aplikazioa, AplikazioEgoera egoera) {
        this.aplikazioa = aplikazioa;
        this.egoera = egoera;
    }

    @FXML
    private void initialize() {
        Autoa autoa = egoera.getHautatutakoAutoa();
        if (autoa == null && !egoera.getKatalogoa().isEmpty()) {
            autoa = egoera.getKatalogoa().get(0);
            egoera.setHautatutakoAutoa(autoa);
        }
        if (autoa == null) {
            return;
        }
        this.konfigurazioa = egoera.lortuKonfigurazioa(autoa);

        ToggleGroup viewGroup = new ToggleGroup();
        viewExteriorBtn.setToggleGroup(viewGroup);
        viewInteriorBtn.setToggleGroup(viewGroup);
        viewDimensionsBtn.setToggleGroup(viewGroup);
        viewExteriorBtn.setSelected(true);

        populateStaticInfo(autoa);
        buildEngineOptions(autoa);
        buildColorOptions(autoa);
        buildInteriorOptions(autoa);
        buildExtrasOptions(autoa);

        configAccordion.setExpandedPane(enginePane);
        refreshLiveSummary();
    }

    private void populateStaticInfo(Autoa autoa) {
        carTitleLabel.setText(autoa.getIzenOsoa());
        carSubtitleLabel.setText(autoa.karrozeria() + "  ·  " + autoa.erregaia()
                + "  ·  " + autoa.plazak() + " plaza  ·  " + autoa.ateak() + " ate");
        specDimensionsLabel.setText(autoa.neurriak());
        specTrunkLabel.setText(autoa.maletategiaL() + " L");
        descriptionLabel.setText(autoa.deskribapena());
        useCasesLabel.setText("Erabilera gomendatuak: " + String.join(", ", autoa.erabilerak()));

        InputStream imgStream = getClass().getResourceAsStream("/eus/goierri/autoak/img/" + autoa.irudiFitxategia());
        if (imgStream != null) {
            carImageView.setImage(new Image(imgStream));
        }
    }

    private void buildEngineOptions(Autoa autoa) {
        engineOptionsBox.getChildren().clear();
        ToggleGroup group = new ToggleGroup();
        for (Autoa.MotorBertsioa m : autoa.motorrak()) {
            RadioButton rb = new RadioButton(m.formatLaburra());
            rb.setWrapText(true);
            rb.setToggleGroup(group);
            rb.setSelected(m.equals(konfigurazioa.getMotorra()));
            rb.setOnAction(e -> {
                konfigurazioa.setMotorra(m);
                refreshLiveSummary();
            });
            engineOptionsBox.getChildren().add(rb);
        }
    }

    private void buildColorOptions(Autoa autoa) {
        colorOptionsPane.getChildren().clear();
        colorButtons.clear();
        for (Autoa.KoloreAukera k : autoa.koloreak()) {
            Circle dot = new Circle(10, Color.web(k.hexKodea()));
            dot.setStroke(Color.web("#475569"));
            String plus = k.prezioGehigarria() == 0 ? "Barne" : "+" + AutoKonfigurazioa.formatEuro(k.prezioGehigarria());
            Label lbl = new Label(k.izena() + "\n" + plus);
            HBox content = new HBox(8, dot, lbl);
            content.setAlignment(Pos.CENTER_LEFT);

            Button btn = new Button();
            btn.setGraphic(content);
            btn.setPrefWidth(178);
            btn.setOnAction(e -> {
                konfigurazioa.setKolorea(k);
                refreshLiveSummary();
            });
            colorButtons.put(k, btn);
            colorOptionsPane.getChildren().add(btn);
        }
    }

    private void buildInteriorOptions(Autoa autoa) {
        interiorOptionsBox.getChildren().clear();
        ToggleGroup group = new ToggleGroup();
        for (Autoa.AkaberaAukera a : autoa.akaberak()) {
            String plus = a.prezioGehigarria() == 0 ? "Barne" : "+" + AutoKonfigurazioa.formatEuro(a.prezioGehigarria());
            RadioButton rb = new RadioButton(a.izena() + " [" + plus + "]\n" + a.deskribapena());
            rb.setWrapText(true);
            rb.setToggleGroup(group);
            rb.setSelected(a.equals(konfigurazioa.getAkabera()));
            rb.setOnAction(e -> {
                konfigurazioa.setAkabera(a);
                refreshLiveSummary();
            });
            interiorOptionsBox.getChildren().add(rb);
        }
    }

    private void buildExtrasOptions(Autoa autoa) {
        extrasOptionsBox.getChildren().clear();
        for (Autoa.EkipamenduGehigarria g : autoa.gehigarriak()) {
            CheckBox cb = new CheckBox(g.izena() + "  (+" + AutoKonfigurazioa.formatEuro(g.prezioa()) + ")");
            cb.setStyle("-fx-font-weight: bold;");
            cb.setSelected(konfigurazioa.getGehigarriIdak().contains(g.id()));
            Label desc = new Label(g.deskribapena());
            desc.setWrapText(true);
            desc.getStyleClass().add("muted-text");

            cb.setOnAction(e -> {
                if (cb.isSelected()) {
                    konfigurazioa.getGehigarriIdak().add(g.id());
                } else {
                    konfigurazioa.getGehigarriIdak().remove(g.id());
                }
                refreshLiveSummary();
            });

            VBox item = new VBox(2, cb, desc);
            extrasOptionsBox.getChildren().add(item);
        }
    }

    private void refreshLiveSummary() {
        Autoa autoa = konfigurazioa.getAutoa();
        Autoa.MotorBertsioa m = konfigurazioa.getMotorra();
        Autoa.KoloreAukera k = konfigurazioa.getKolorea();
        Autoa.AkaberaAukera a = konfigurazioa.getAkabera();

        // Ezaugarri teknikoak motorraren arabera eguneratu
        specPowerLabel.setText(m.potentziaCv() + " CV");
        specConsumptionLabel.setText(m.kontsumoa());

        // Kolore eta akabera aurrebista eguneratu
        Color extColor = Color.web(k.hexKodea());
        exteriorSwatchCircle.setFill(extColor);
        exteriorPreviewLabel.setText("Kanpoko kolorea: " + k.izena());

        Color intColor = Color.web(a.hexKodea());
        interiorSwatchCircle.setFill(intColor);
        interiorPreviewLabel.setText("Barruko akabera: " + a.izena());

        colorBackdropRect.setStroke(extColor);
        colorBackdropRect.setStrokeWidth(4.0);
        carImageView.setEffect(new DropShadow(22, extColor.deriveColor(0, 1, 1, 0.45)));

        for (var entry : colorButtons.entrySet()) {
            boolean selected = entry.getKey().equals(k);
            entry.getValue().getStyleClass().setAll(selected ? "color-swatch-selected" : "color-swatch-button");
        }

        // TitledPane izenburuetan hautatutako laburpena jarri (ezkutatuta daudenean ere argi ikusteko!)
        enginePane.setText("1. Bertsioa eta motorra  ·  " + m.potentziaCv() + " CV");
        colorPane.setText("2. Kanpoko kolorea  ·  " + k.izena());
        interiorPane.setText("3. Barruko akabera  ·  " + a.izena());
        extrasPane.setText("4. Ekipamendu gehigarria  ·  " + konfigurazioa.getGehigarriIdak().size() + " hautatuta");

        // Prezio-banaketa eta AZKEN PREZIOA
        int aukerakGuztira = m.prezioGehigarria() + k.prezioGehigarria()
                + a.prezioGehigarria() + konfigurazioa.getGehigarrienPrezioa();
        priceBreakdownLabel.setText("Oinarria: " + AutoKonfigurazioa.formatEuro(autoa.oinarrizkoPrezioa())
                + "  +  Aukerak (" + konfigurazioa.getGehigarriIdak().size() + " gehigarri): "
                + AutoKonfigurazioa.formatEuro(aukerakGuztira));
        finalPriceLabel.setText(AutoKonfigurazioa.formatEuro(konfigurazioa.kalkulatuAzkenPrezioa()));

        updateVisualModeOverlay();
    }

    @FXML
    private void selectViewMode() {
        updateVisualModeOverlay();
    }

    private void updateVisualModeOverlay() {
        if (konfigurazioa == null) return;
        Autoa autoa = konfigurazioa.getAutoa();
        if (viewInteriorBtn.isSelected()) {
            viewModeLabel.setText("Barrualdeko akabera-aurrebista");
            colorBackdropRect.setFill(Color.web(konfigurazioa.getAkabera().hexKodea()).deriveColor(0, 0.35, 1.35, 0.25));
            visualOverlayLabel.setText("BARRUKO AKABERA: " + konfigurazioa.getAkabera().izena()
                    + "\n" + konfigurazioa.getAkabera().deskribapena()
                    + "\nHautatutako gehigarriak: " + konfigurazioa.getGehigarriIdak().size());
            visualOverlayLabel.setVisible(true);
            visualOverlayLabel.setManaged(true);
        } else if (viewDimensionsBtn.isSelected()) {
            viewModeLabel.setText("Neurriak eta espazioaren fitxa");
            colorBackdropRect.setFill(Color.web("#eff6ff"));
            visualOverlayLabel.setText("NEURRIAK: " + autoa.neurriak()
                    + "\nMALETATEGIA: " + autoa.maletategiaL() + " litro"
                    + "  ·  " + autoa.plazak() + " plaza  ·  " + autoa.ateak() + " ate");
            visualOverlayLabel.setVisible(true);
            visualOverlayLabel.setManaged(true);
        } else {
            viewModeLabel.setText("Kanpoaldeko aurrebista (" + konfigurazioa.getKolorea().izena() + ")");
            colorBackdropRect.setFill(Color.web("#f8fafc"));
            visualOverlayLabel.setVisible(false);
            visualOverlayLabel.setManaged(false);
        }
    }

    @FXML
    private void resetConfiguration() {
        konfigurazioa.berrezarri();
        Autoa autoa = konfigurazioa.getAutoa();
        buildEngineOptions(autoa);
        buildColorOptions(autoa);
        buildInteriorOptions(autoa);
        buildExtrasOptions(autoa);
        refreshLiveSummary();
    }

    @FXML
    private void goBackToCatalog() {
        if (aplikazioa == null) return;
        try {
            aplikazioa.erakutsiHasierakoIragazketa();
        } catch (IOException ex) {
            throw new IllegalStateException("Ezin izan da 1. pantailara itzuli.", ex);
        }
    }

    @FXML
    private void showOfferSummary() {
        Autoa autoa = konfigurazioa.getAutoa();
        StringBuilder sb = new StringBuilder();
        sb.append("Autoa: ").append(autoa.getIzenOsoa()).append(" (")
                .append(AutoKonfigurazioa.formatEuro(autoa.oinarrizkoPrezioa())).append(")\n");
        sb.append("Motorra: ").append(konfigurazioa.getMotorra().formatLaburra()).append("\n");
        sb.append("Kanpoko kolorea: ").append(konfigurazioa.getKolorea().izena()).append(" (+")
                .append(AutoKonfigurazioa.formatEuro(konfigurazioa.getKolorea().prezioGehigarria())).append(")\n");
        sb.append("Barruko akabera: ").append(konfigurazioa.getAkabera().izena()).append(" (+")
                .append(AutoKonfigurazioa.formatEuro(konfigurazioa.getAkabera().prezioGehigarria())).append(")\n");
        sb.append("Ekipamendu gehigarria: ");
        if (konfigurazioa.getGehigarriIdak().isEmpty()) {
            sb.append("Gehigarririk gabe\n");
        } else {
            sb.append("\n");
            for (Autoa.EkipamenduGehigarria g : autoa.gehigarriak()) {
                if (konfigurazioa.getGehigarriIdak().contains(g.id())) {
                    sb.append("  • ").append(g.izena()).append(" (+")
                            .append(AutoKonfigurazioa.formatEuro(g.prezioa())).append(")\n");
                }
            }
        }
        sb.append("\nAZKEN PREZIOA: ").append(AutoKonfigurazioa.formatEuro(konfigurazioa.kalkulatuAzkenPrezioa()));

        Alert alert = new Alert(Alert.AlertType.INFORMATION);
        alert.setTitle("Bezeroarentzako eskaintza-laburpena");
        alert.setHeaderText(autoa.getIzenOsoa() + " — Konfigurazio osoa");
        alert.setContentText(sb.toString());
        alert.showAndWait();
    }
}
