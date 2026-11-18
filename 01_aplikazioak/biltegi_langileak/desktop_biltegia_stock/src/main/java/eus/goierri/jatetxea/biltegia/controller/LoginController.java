package eus.goierri.jatetxea.biltegia.controller;

import eus.goierri.jatetxea.biltegia.BiltegiaApp;
import eus.goierri.jatetxea.core.controller.BaseLoginController;

public class LoginController extends BaseLoginController {

    @Override
    protected boolean baimenduRola(String rola) {
        if (rola == null) return false;
        String r = rola.trim().toLowerCase();
        return r.contains("biltegi") || r.contains("admin") || r.contains("it") || r.contains("kudeatzaile");
    }

    @Override
    protected String getMainFxml() {
        return "biltegia-view.fxml";
    }

    @Override
    protected Class<?> getAppResourceClass() {
        return BiltegiaApp.class;
    }
}
