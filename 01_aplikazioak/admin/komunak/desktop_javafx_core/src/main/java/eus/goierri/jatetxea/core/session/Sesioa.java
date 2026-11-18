package eus.goierri.jatetxea.core.session;

import eus.goierri.jatetxea.core.model.Langilea;

public class Sesioa {
    private static Langilea unekoLangilea;

    public static Langilea getUnekoLangilea() {
        return unekoLangilea;
    }

    public static void setUnekoLangilea(Langilea langilea) {
        unekoLangilea = langilea;
    }

    public static boolean isSaioaHasita() {
        return unekoLangilea != null;
    }

    public static void itxiSaioa() {
        unekoLangilea = null;
    }

    public static boolean isSukaldaria() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().toLowerCase();
        return r.contains("sukaldari") || isAdminEdoKudeatzailea();
    }

    public static boolean isBarrakoa() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().toLowerCase();
        return r.contains("barra") || isAdminEdoKudeatzailea();
    }

    public static boolean isBiltegikoa() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().toLowerCase();
        return r.contains("biltegi") || isAdminEdoKudeatzailea();
    }

    public static boolean isAdminEdoKudeatzailea() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().toLowerCase();
        return r.contains("admin") || r.contains("it") || r.contains("kudeatzaile");
    }
}
