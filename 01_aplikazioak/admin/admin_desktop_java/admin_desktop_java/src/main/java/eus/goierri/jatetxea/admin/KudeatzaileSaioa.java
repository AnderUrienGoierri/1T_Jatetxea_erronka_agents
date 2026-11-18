package eus.goierri.jatetxea.admin;

import eus.goierri.jatetxea.admin.model.KudeatzaileErabiltzailea;
import javafx.beans.property.BooleanProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

public final class KudeatzaileSaioa {
    private final KudeatzaileErabiltzailea user;
    private final BooleanProperty configurationWritable = new SimpleBooleanProperty(false);
    private final StringProperty serviceStatusText = new SimpleStringProperty("Zerbitzu-egoera egiaztatzen...");

    public KudeatzaileSaioa(KudeatzaileErabiltzailea user) {
        this.user = user;
    }

    public KudeatzaileErabiltzailea getUser() {
        return user;
    }

    public BooleanProperty configurationWritableProperty() {
        return configurationWritable;
    }

    public boolean isConfigurationWritable() {
        return configurationWritable.get();
    }

    public void setServiceState(boolean writable, String message) {
        configurationWritable.set(writable);
        serviceStatusText.set(message);
    }

    public StringProperty serviceStatusTextProperty() {
        return serviceStatusText;
    }
}
