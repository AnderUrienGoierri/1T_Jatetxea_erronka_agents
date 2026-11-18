package eus.goierri.jatetxea.admin.model;

import java.util.Locale;
import java.util.Set;

public final class KudeatzaileErabiltzailea {
    public int id;
    public String erabiltzailea;
    public String izena;
    public String rola;
    public boolean aktibo;

    public boolean isManager() {
        if (rola == null) {
            return false;
        }
        return Set.of("admin", "adminit", "administrator", "kudeatzailea", "kudeatzaile", "jabea", "jabe", "owner")
                .contains(rola.trim().toLowerCase(Locale.ROOT));
    }

    public String displayName() {
        return izena == null || izena.isBlank() ? erabiltzailea : izena;
    }
}
