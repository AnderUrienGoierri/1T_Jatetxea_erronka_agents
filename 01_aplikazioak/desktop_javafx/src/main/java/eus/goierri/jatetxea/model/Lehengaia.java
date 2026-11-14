package eus.goierri.jatetxea.model;

/**
 * Biltegiko lehengai / stock elementu baten eredua.
 */
public class Lehengaia {
    private int id;
    private String izena;
    private String unitatea;
    private double unekoStocka;
    private double gutxienekoStocka;
    private double kostuUnitarioa;
    private String hornitzailea;
    private boolean ezabatua;

    public Lehengaia() {
    }

    public Lehengaia(int id, String izena, String unitatea, double unekoStocka, double gutxienekoStocka, double kostuUnitarioa, String hornitzailea) {
        this.id = id;
        this.izena = izena;
        this.unitatea = unitatea;
        this.unekoStocka = unekoStocka;
        this.gutxienekoStocka = gutxienekoStocka;
        this.kostuUnitarioa = kostuUnitarioa;
        this.hornitzailea = hornitzailea;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getIzena() {
        return izena != null ? izena : "";
    }

    public void setIzena(String izena) {
        this.izena = izena;
    }

    public String getUnitatea() {
        return unitatea != null ? unitatea : "";
    }

    public void setUnitatea(String unitatea) {
        this.unitatea = unitatea;
    }

    public double getUnekoStocka() {
        return unekoStocka;
    }

    public void setUnekoStocka(double unekoStocka) {
        this.unekoStocka = unekoStocka;
    }

    public double getGutxienekoStocka() {
        return gutxienekoStocka;
    }

    public void setGutxienekoStocka(double gutxienekoStocka) {
        this.gutxienekoStocka = gutxienekoStocka;
    }

    public double getKostuUnitarioa() {
        return kostuUnitarioa;
    }

    public void setKostuUnitarioa(double kostuUnitarioa) {
        this.kostuUnitarioa = kostuUnitarioa;
    }

    public String getHornitzailea() {
        return hornitzailea != null ? hornitzailea : "-";
    }

    public void setHornitzailea(String hornitzailea) {
        this.hornitzailea = hornitzailea;
    }

    public boolean isEzabatua() {
        return ezabatua;
    }

    public void setEzabatua(boolean ezabatua) {
        this.ezabatua = ezabatua;
    }

    public boolean isAlerta() {
        return unekoStocka <= gutxienekoStocka;
    }

    public String getEgoera() {
        if (unekoStocka <= 0) {
            return "AGORTUTA";
        } else if (unekoStocka <= gutxienekoStocka) {
            return "STOCK BAXUA";
        } else {
            return "EGOKIA";
        }
    }

    public String getStockFormateatua() {
        return String.format("%.2f %s", unekoStocka, unitatea);
    }

    public String getGutxienekoFormateatua() {
        return String.format("%.2f %s", gutxienekoStocka, unitatea);
    }

    public String getKostuaFormateatua() {
        return String.format("%.2f €", kostuUnitarioa);
    }

    @Override
    public String toString() {
        return izena + " (" + getStockFormateatua() + ")";
    }
}
