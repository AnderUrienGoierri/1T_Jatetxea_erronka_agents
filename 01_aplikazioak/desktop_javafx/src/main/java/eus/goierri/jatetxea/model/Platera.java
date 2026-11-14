package eus.goierri.jatetxea.model;

/**
 * Jatetxeko plateraren eredua.
 */
public class Platera {
    private int id;
    private String izena;
    private String deskribapena;
    private String kategoria;
    private double prezioa;
    private String alergenoak;
    private int prestatzeDenboraMin;
    private boolean aktibo;

    public Platera() {
    }

    public Platera(int id, String izena, String kategoria, double prezioa) {
        this.id = id;
        this.izena = izena;
        this.kategoria = kategoria;
        this.prezioa = prezioa;
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

    public String getDeskribapena() {
        return deskribapena != null ? deskribapena : "";
    }

    public void setDeskribapena(String deskribapena) {
        this.deskribapena = deskribapena;
    }

    public String getKategoria() {
        return kategoria != null ? kategoria : "";
    }

    public void setKategoria(String kategoria) {
        this.kategoria = kategoria;
    }

    public double getPrezioa() {
        return prezioa;
    }

    public void setPrezioa(double prezioa) {
        this.prezioa = prezioa;
    }

    public String getAlergenoak() {
        return alergenoak != null ? alergenoak : "";
    }

    public void setAlergenoak(String alergenoak) {
        this.alergenoak = alergenoak;
    }

    public int getPrestatzeDenboraMin() {
        return prestatzeDenboraMin;
    }

    public void setPrestatzeDenboraMin(int prestatzeDenboraMin) {
        this.prestatzeDenboraMin = prestatzeDenboraMin;
    }

    public boolean isAktibo() {
        return aktibo;
    }

    public void setAktibo(boolean aktibo) {
        this.aktibo = aktibo;
    }

    @Override
    public String toString() {
        return izena + " (" + String.format("%.2f €", prezioa) + ")";
    }
}
