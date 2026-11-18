package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.JatetxeaKudeaketaAplikazioa;
import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiBezeroa;
import javafx.stage.Stage;

import java.lang.reflect.InvocationTargetException;

public final class KontrolagailuFabrika implements javafx.util.Callback<Class<?>, Object> {
    private final ApiBezeroa apiClient;
    private final Stage stage;
    private final KudeatzaileSaioa session;

    public KontrolagailuFabrika(ApiBezeroa apiClient, Stage stage, KudeatzaileSaioa session) {
        this.apiClient = apiClient;
        this.stage = stage;
        this.session = session;
    }

    @Override
    public Object call(Class<?> controllerType) {
        if (controllerType == SaioHasieraKontrolagailua.class) {
            return new SaioHasieraKontrolagailua(apiClient, (JatetxeaKudeaketaAplikazioa) stage.getUserData());
        }
        if (controllerType == DiseinuNagusiaKontrolagailua.class) {
            return new DiseinuNagusiaKontrolagailua(apiClient, session,
                    (JatetxeaKudeaketaAplikazioa) stage.getUserData(), stage);
        }
        if (controllerType == LaburpenKontrolagailua.class) {
            return new LaburpenKontrolagailua(apiClient);
        }
        if (controllerType == LangileakKontrolagailua.class) {
            return new LangileakKontrolagailua(apiClient, session);
        }
        if (controllerType == MenuaKontrolagailua.class) {
            return new MenuaKontrolagailua(apiClient, session);
        }
        if (controllerType == OsagaiakKontrolagailua.class) {
            return new OsagaiakKontrolagailua(apiClient);
        }
        if (controllerType == MahaiakKontrolagailua.class) {
            return new MahaiakKontrolagailua(apiClient, session);
        }
        try {
            return controllerType.getDeclaredConstructor().newInstance();
        } catch (InstantiationException | IllegalAccessException | InvocationTargetException | NoSuchMethodException ex) {
            throw new IllegalStateException("Ezin izan da FXML kontroladorea sortu: " + controllerType.getName(), ex);
        }
    }
}
