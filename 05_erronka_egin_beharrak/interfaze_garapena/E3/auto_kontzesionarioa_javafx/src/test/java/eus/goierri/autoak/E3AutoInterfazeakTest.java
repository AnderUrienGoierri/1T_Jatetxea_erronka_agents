package eus.goierri.autoak;

import eus.goierri.autoak.controller.HasierakoIragazketaKontrolagailua;
import eus.goierri.autoak.controller.AutoPertsonalizazioaKontrolagailua;
import eus.goierri.autoak.model.AplikazioEgoera;
import eus.goierri.autoak.model.AutoKatalogoa;
import eus.goierri.autoak.model.AutoKonfigurazioa;
import eus.goierri.autoak.model.Autoa;
import eus.goierri.autoak.model.IragazkiEgoera;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.control.ToggleButton;
import javafx.scene.image.ImageView;
import javafx.scene.layout.FlowPane;
import javafx.scene.layout.VBox;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.net.URL;
import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.FutureTask;
import java.util.concurrent.TimeUnit;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class E3AutoInterfazeakTest {
    @BeforeAll
    static void startJavaFx() throws InterruptedException {
        CountDownLatch started = new CountDownLatch(1);
        Platform.startup(started::countDown);
        if (!started.await(10, TimeUnit.SECONDS)) {
            throw new IllegalStateException("JavaFX plataforma ez da garaiz abiatu.");
        }
    }

    @AfterAll
    static void stopJavaFx() {
        Platform.exit();
    }

    @Test
    void bothFxmlScreensLoadCleanlyForSceneBuilderAndRuntime() throws Exception {
        for (String fxml : new String[] { "hasierako-iragazketa.fxml", "auto-pertsonalizazioa.fxml" }) {
            URL resource = getClass().getResource("/eus/goierri/autoak/fxml/" + fxml);
            assertNotNull(resource, fxml + " aurkitu behar da");
            FutureTask<Parent> loadTask = new FutureTask<>(() -> new FXMLLoader(resource).load());
            Platform.runLater(loadTask);
            assertNotNull(loadTask.get(10, TimeUnit.SECONDS), fxml + " kargatu behar da");
        }
    }

    @Test
    void filterControlsUpdateResultsAndAllowClearingOptionalValues() throws Exception {
        FutureTask<Void> task = new FutureTask<>(() -> {
            AplikazioEgoera egoera = new AplikazioEgoera(AutoKatalogoa.sortuKatalogoa());
            FXMLLoader loader = new FXMLLoader(getClass().getResource(
                    "/eus/goierri/autoak/fxml/hasierako-iragazketa.fxml"));
            loader.setControllerFactory(type -> new HasierakoIragazketaKontrolagailua(null, egoera));
            loader.load();

            ToggleButton use = (ToggleButton) loader.getNamespace().get("useFamiliaToggle");
            ComboBox<?> seats = (ComboBox<?>) loader.getNamespace().get("seatsCombo");
            ComboBox<?> doors = (ComboBox<?>) loader.getNamespace().get("doorsCombo");
            ComboBox<?> body = (ComboBox<?>) loader.getNamespace().get("bodyCombo");
            Slider maxPrice = (Slider) loader.getNamespace().get("maxPriceSlider");
            Label count = (Label) loader.getNamespace().get("resultCountLabel");
            FlowPane chips = (FlowPane) loader.getNamespace().get("activeFiltersPane");
            FlowPane brands = (FlowPane) loader.getNamespace().get("brandsPane");
            Button reset = (Button) loader.getNamespace().get("resetAllButton");
            IragazkiEgoera filters = egoera.getIragazkiak();

            use.fire();
            assertTrue(filters.getErabilerak().contains("Familia"));
            assertNull(filters.getGutxienekoPlazak());
            assertNull(filters.getAteak());
            assertEquals(egoera.getKatalogoa().stream().filter(filters::betetzenDu).count()
                    + " / " + egoera.getKatalogoa().size() + " auto", count.getText());
            assertTrue(chips.getChildren().stream().anyMatch(node ->
                    node instanceof Button button && button.getText().contains("Erabilera: Familia")));

            for (int i = 1; i <= 3; i++) {
                seats.getSelectionModel().select(i);
                assertEquals(i == 1 ? 4 : i == 2 ? 5 : 7, filters.getGutxienekoPlazak());
            }
            for (int i = 1; i <= 2; i++) {
                doors.getSelectionModel().select(i);
                assertEquals(i == 1 ? 4 : 5, filters.getAteak());
            }
            seats.getSelectionModel().select(0);
            doors.getSelectionModel().select(0);
            assertNull(filters.getGutxienekoPlazak());
            assertNull(filters.getAteak());

            body.getSelectionModel().select(1);
            assertEquals("SUV", filters.getKarrozeria());
            Button brand = (Button) brands.getChildren().stream()
                    .filter(node -> node instanceof Button button && button.getText().equals("Hyundai"))
                    .findFirst().orElseThrow();
            brand.fire();
            assertTrue(filters.getMarkaNahiak().contains("Hyundai"));
            brand.fire();
            assertTrue(filters.getMarkaBaztertuak().contains("Hyundai"));
            brand.fire();
            assertFalse(filters.getMarkaBaztertuak().contains("Hyundai"));

            maxPrice.setValue(45000);
            assertEquals(45000, filters.getAurrekontuMax());
            assertEquals(egoera.getKatalogoa().stream().filter(filters::betetzenDu).count()
                    + " / " + egoera.getKatalogoa().size() + " auto", count.getText());

            reset.fire();
            assertEquals(0, filters.aktiboKopurua());
            assertEquals(egoera.getKatalogoa().size() + " / " + egoera.getKatalogoa().size()
                    + " auto", count.getText());
            return null;
        });
        Platform.runLater(task);
        task.get(10, TimeUnit.SECONDS);
    }

    @Test
    void everyCatalogueCarHasItsOwnPhotoInBothScreens() throws Exception {
        FutureTask<Void> task = new FutureTask<>(() -> {
            AplikazioEgoera egoera = new AplikazioEgoera(AutoKatalogoa.sortuKatalogoa());
            List<Autoa> cars = egoera.getKatalogoa();
            assertEquals(20, cars.size());
            assertEquals(20, cars.stream().map(Autoa::irudiFitxategia).distinct().count());

            FXMLLoader catalogue = new FXMLLoader(getClass().getResource(
                    "/eus/goierri/autoak/fxml/hasierako-iragazketa.fxml"));
            catalogue.setControllerFactory(type -> new HasierakoIragazketaKontrolagailua(null, egoera));
            catalogue.load();
            FlowPane cards = (FlowPane) catalogue.getNamespace().get("carsFlowPane");
            assertEquals(cars.size(), cards.getChildren().size());

            for (int i = 0; i < cars.size(); i++) {
                Autoa car = cars.get(i);
                assertTrue(car.irudiFitxategia().endsWith(".jpg"));
                assertNotNull(getClass().getResource("/eus/goierri/autoak/img/" + car.irudiFitxategia()));
                VBox card = (VBox) cards.getChildren().get(i);
                ImageView cardPhoto = (ImageView) card.getChildren().get(1);
                assertNotNull(cardPhoto.getImage(), car.id());
                assertFalse(cardPhoto.getImage().isError(), car.id());
                assertTrue(cardPhoto.getImage().getWidth() > 0, car.id());

                egoera.setHautatutakoAutoa(car);
                FXMLLoader details = new FXMLLoader(getClass().getResource(
                        "/eus/goierri/autoak/fxml/auto-pertsonalizazioa.fxml"));
                details.setControllerFactory(type -> new AutoPertsonalizazioaKontrolagailua(null, egoera));
                details.load();
                ImageView detailPhoto = (ImageView) details.getNamespace().get("carImageView");
                assertNotNull(detailPhoto.getImage(), car.id());
                assertFalse(detailPhoto.getImage().isError(), car.id());
                assertEquals(cardPhoto.getImage().getWidth(), detailPhoto.getImage().getWidth(), car.id());
            }
            return null;
        });
        Platform.runLater(task);
        task.get(30, TimeUnit.SECONDS);
    }

    @Test
    void filtersSupportIncludeExcludeBrandsAndBudgetRange() {
        List<Autoa> katalogoa = AutoKatalogoa.sortuKatalogoa();
        IragazkiEgoera f = new IragazkiEgoera();
        f.getErabilerak().add("Familia");
        f.getMarkaBaztertuak().add("BMW");
        f.setAurrekontuMax(45000);

        List<Autoa> emaitza = katalogoa.stream().filter(f::betetzenDu).toList();
        assertFalse(emaitza.isEmpty());
        assertTrue(emaitza.stream().noneMatch(a -> a.marka().equals("BMW")));
        assertTrue(emaitza.stream().allMatch(a -> a.oinarrizkoPrezioa() <= 45000));
    }

    @Test
    void customizationUpdatesFinalPriceAndPersistsAcrossNavigation() {
        AplikazioEgoera egoera = new AplikazioEgoera(AutoKatalogoa.sortuKatalogoa());
        Autoa rav4 = egoera.getKatalogoa().stream()
                .filter(a -> a.id().equals("toyota-rav4"))
                .findFirst().orElseThrow();

        AutoKonfigurazioa cfg = egoera.lortuKonfigurazioa(rav4);
        cfg.setMotorra(rav4.motorrak().get(1)); // +3400
        cfg.setKolorea(rav4.koloreak().get(4)); // +950
        cfg.setAkabera(rav4.akaberak().get(1)); // +1100
        cfg.getGehigarriIdak().add("gps");      // +950

        assertEquals(38500 + 3400 + 950 + 1100 + 950, cfg.kalkulatuAzkenPrezioa());
        assertTrue(egoera.baduPertsonalizazioa(rav4));
    }
}
