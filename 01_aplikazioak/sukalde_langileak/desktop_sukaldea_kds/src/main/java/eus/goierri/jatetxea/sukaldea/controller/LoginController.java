package eus.goierri.jatetxea.sukaldea.controller;

import eus.goierri.jatetxea.core.controller.BaseLoginController;
import eus.goierri.jatetxea.sukaldea.SukaldeaApp;

public class LoginController extends BaseLoginController {

    @Override
    protected boolean baimenduRola(String rola) {
        if (rola == null) return false;
        String r = rola.trim().toLowerCase();
        return r.contains("sukaldari") || r.contains("admin") || r.contains("it") || r.contains("kudeatzaile");
    }

    @Override
    protected String getMainFxml() {
        return "sukaldea-view.fxml";
    }

    @Override
    protected Class<?> getAppResourceClass() {
        return SukaldeaApp.class;
    }
}
