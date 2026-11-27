package eus.goierri.jatetxea.admin.controller;

import eus.goierri.jatetxea.admin.KudeatzaileSaioa;
import eus.goierri.jatetxea.admin.api.ApiKontratua;
import javafx.scene.control.Label;

/**
 * Kudeaketa-pantaila guztien klase abstraktu gurasoa (Herentzia + Template Method patroia).
 * API bezeroaren eta saioaren injekzioa eta egoera-mezuen formatua zentralizatzen ditu.
 */
public abstract class BaseAdminKontrolagailua implements PantailaFreskagarria {
    protected final ApiKontratua apiClient;
    protected final KudeatzaileSaioa session;

    protected BaseAdminKontrolagailua(ApiKontratua apiClient, KudeatzaileSaioa session) {
        this.apiClient = apiClient;
        this.session = session;
    }

    protected boolean apiPrestDago() {
        return apiClient != null;
    }

    protected boolean saioaAktiboDago() {
        return session != null && session.getUser() != null;
    }

    protected void ezarriMezua(Label label, String text, String styleClass) {
        if (label == null) {
            return;
        }
        label.setText(text);
        if (styleClass != null && !styleClass.isBlank()) {
            label.getStyleClass().setAll("inline-message", styleClass);
        } else {
            label.getStyleClass().setAll("inline-message");
        }
    }
}
