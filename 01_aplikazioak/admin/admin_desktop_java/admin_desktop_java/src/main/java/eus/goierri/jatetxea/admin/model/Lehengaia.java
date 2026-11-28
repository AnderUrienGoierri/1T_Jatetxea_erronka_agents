package eus.goierri.jatetxea.admin.model;

import java.util.Locale;

public final class Lehengaia {
    public int id;
    public String izena;
    public String unitatea;
    public double unekoStocka;
    public double gutxienekoStocka;
    public double kostuUnitarioa;
    public Integer hornitzaileaId;
    public String hornitzailea;
    public double bezEhunekoa = 10.0;
    public boolean ezabatua;

    @Override
    public String toString() {
        return izena + " (" + String.format(Locale.ROOT, "%.2f", unekoStocka) + " "
                + (unitatea == null ? "" : unitatea) + ")";
    }
}
