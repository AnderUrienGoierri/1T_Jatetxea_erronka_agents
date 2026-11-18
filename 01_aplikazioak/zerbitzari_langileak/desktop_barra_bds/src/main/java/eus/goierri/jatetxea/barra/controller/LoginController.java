package eus.goierri.jatetxea.barra.controller;

import eus.goierri.jatetxea.barra.BarraApp;
import eus.goierri.jatetxea.core.controller.BaseLoginController;

public class LoginController extends BaseLoginController {

    @Override
    protected boolean baimenduRola(String rola) {
        if (rola == null) return false;
        String r = rola.trim().toLowerCase();
        return r.contains("barra") || r.contains("zerbitzari") || r.contains("admin") || r.contains("it") || r.contains("kudeatzaile");
    }

    @Override
    protected String getMainFxml() {
        return "barra-view.fxml";
    }

    @Override
    protected Class<?> getAppResourceClass() {
        return BarraApp.class;
    }
}
