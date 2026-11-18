package eus.goierri.jatetxea.admin.model;

public final class Lehengaia {
    public int id;
    public String izena;
    public String unitatea;
    public double unekoStocka;
    public double gutxienekoStocka;
    public double kostuUnitarioa;
    public String hornitzailea;
    public boolean ezabatua;

    @Override
    public String toString() {
        return izena + " (" + id + ")";
    }
}
