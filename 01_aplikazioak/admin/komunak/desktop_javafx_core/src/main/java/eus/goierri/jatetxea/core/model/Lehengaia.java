package eus.goierri.jatetxea.core.model;

import com.google.gson.annotations.SerializedName;

/**
 * Biltegiko lehengaiaren eredu orokorra (Stocka eta hornikuntza).
 */
public class Lehengaia {
    private int id;
    private String izena;

    @SerializedName(value = "stock", alternate = {"unekoStocka", "uneko_stocka"})
    private double stock;

    @SerializedName(value = "stockMinimoa", alternate = {"gutxienekoStocka", "gutxieneko_stocka"})
    private double stockMinimoa;

    private String unitatea;

    @SerializedName(value = "kostua", alternate = {"kostuUnitarioa", "kostu_unitarioa"})
    private double kostua;

    private String hornitzailea;

    public Lehengaia() {}

    public Lehengaia(int id, String izena, double stock, double stockMinimoa, String unitatea, double kostua, String hornitzailea) {
        this.id = id;
        this.izena = izena;
        this.stock = stock;
        this.stockMinimoa = stockMinimoa;
        this.unitatea = unitatea;
        this.kostua = kostua;
        this.hornitzailea = hornitzailea;
    }

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getIzena() { return izena != null ? izena : ""; }
    public void setIzena(String izena) { this.izena = izena; }

    public double getStock() { return stock; }
    public void setStock(double stock) { this.stock = stock; }

    public double getStockMinimoa() { return stockMinimoa; }
    public void setStockMinimoa(double stockMinimoa) { this.stockMinimoa = stockMinimoa; }

    public double getUnekoStocka() { return stock; }
    public void setUnekoStocka(double unekoStocka) { this.stock = unekoStocka; }

    public double getGutxienekoStocka() { return stockMinimoa; }
    public void setGutxienekoStocka(double gutxienekoStocka) { this.stockMinimoa = gutxienekoStocka; }

    public double getKostuUnitarioa() { return kostua; }
    public void setKostuUnitarioa(double kostuUnitarioa) { this.kostua = kostuUnitarioa; }

    public String getUnitatea() { return unitatea != null ? unitatea : ""; }
    public void setUnitatea(String unitatea) { this.unitatea = unitatea; }

    public double getKostua() { return kostua; }
    public void setKostua(double kostua) { this.kostua = kostua; }

    public String getHornitzailea() { return hornitzailea != null ? hornitzailea : ""; }
    public void setHornitzailea(String hornitzailea) { this.hornitzailea = hornitzailea; }

    public String getStockFormateatua() {
        return String.format(java.util.Locale.US, "%.2f %s", stock, getUnitatea()).trim();
    }

    public String getGutxienekoFormateatua() {
        return String.format(java.util.Locale.US, "%.2f %s", stockMinimoa, getUnitatea()).trim();
    }

    public String getKostuaFormateatua() {
        return String.format(java.util.Locale.US, "%.2f €", kostua);
    }

    public String getEgoera() {
        if (stock <= 0) return "AGORTUTA";
        if (stock < stockMinimoa) return "ALERTA (Gutxienekotik behera)";
        return "EGOKIA";
    }

    public boolean isAlertaDauka() {
        return stock < stockMinimoa;
    }

    @Override
    public String toString() {
        return getIzena() + " (" + getStockFormateatua() + ")";
    }
}
