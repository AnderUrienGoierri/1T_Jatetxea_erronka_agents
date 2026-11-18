package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import eus.goierri.jatetxea.admin.api.EkintzaAsinkronoak;
import eus.goierri.jatetxea.admin.model.Langilea;
import eus.goierri.jatetxea.admin.model.Lehengaia;
import eus.goierri.jatetxea.admin.model.Platera;
import eus.goierri.jatetxea.admin.model.Mahaia;
import javafx.fxml.FXML;
import javafx.scene.control.Label;

public final class LaburpenKontrolagailua {
    private final ApiBezeroa apiClient;

    @FXML private Label staffCountLabel;
    @FXML private Label menuCountLabel;
    @FXML private Label lowStockCountLabel;
    @FXML private Label tableCountLabel;
    @FXML private Label dashboardMessage;

    public LaburpenKontrolagailua() {
        this(null);
    }

    public LaburpenKontrolagailua(ApiBezeroa apiClient) {
        this.apiClient = apiClient;
    }

    @FXML
    private void initialize() {
        if (apiClient != null) {
            refresh();
        }
    }

    @FXML
    private void refresh() {
        dashboardMessage.setText("Datuak APItik kargatzen...");
        EkintzaAsinkronoak.run(() -> {
                    var employees = apiClient.getList("/langileak", Langilea.class);
                    var menuItems = apiClient.getList("/platerak", Platera.class);
                    var ingredients = apiClient.getList("/lehengaiak", Lehengaia.class);
                    return new DashboardData(
                            (int) employees.stream().filter(employee -> employee.aktibo).count(),
                            (int) menuItems.stream().filter(item -> item.aktibo).count(),
                            ingredients.stream()
                                    .filter(item -> item.unekoStocka <= item.gutxienekoStocka).count(),
                            apiClient.getList("/mahaiak", Mahaia.class).size());
                },
                data -> {
                    staffCountLabel.setText(Integer.toString(data.staffCount()));
                    menuCountLabel.setText(Integer.toString(data.menuCount()));
                    lowStockCountLabel.setText(Long.toString(data.lowStockCount()));
                    tableCountLabel.setText(Integer.toString(data.tableCount()));
                    dashboardMessage.setText("Datuak azken aldiz APItik eguneratu dira.");
                },
                error -> dashboardMessage.setText("Ezin izan dira laburpen-datuak kargatu: " + error.getMessage()));
    }

    private record DashboardData(int staffCount, int menuCount, long lowStockCount, int tableCount) {
    }
}
