package eus.goierri.jatetxea;

import eus.goierri.jatetxea.model.Langilea;

/**
 * Saio aktiboaren kudeatzailea (unean saioa hasi duen langilea gordetzeko).
 */
public class Sesioa {
    private static Langilea unekoLangilea;

    public static synchronized Langilea getUnekoLangilea() {
        return unekoLangilea;
    }

    public static synchronized void setUnekoLangilea(Langilea langilea) {
        unekoLangilea = langilea;
    }

    public static synchronized boolean isSaioaHasita() {
        return unekoLangilea != null;
    }

    public static synchronized void itxiSaioa() {
        unekoLangilea = null;
    }

    public static synchronized boolean isSukaldaria() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().trim().toLowerCase();
        return r.contains("sukaldari");
    }

    public static synchronized boolean isBarrakoa() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().trim().toLowerCase();
        return r.contains("barra");
    }

    public static synchronized boolean isBiltegikoa() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().trim().toLowerCase();
        return r.contains("biltegi");
    }

    public static synchronized boolean isAdminEdoKudeatzailea() {
        if (unekoLangilea == null) return false;
        String r = unekoLangilea.getRola().trim().toLowerCase();
        return r.contains("admin") || r.contains("it") || r.contains("kudeatzaile");
    }
}
