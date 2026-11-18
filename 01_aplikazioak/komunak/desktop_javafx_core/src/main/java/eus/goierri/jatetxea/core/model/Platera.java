package eus.goierri.jatetxea.core.model;

/**
 * Karta plater edo edariaren eredu orokorra.
 */
public class Platera {
    private int id;
    private String izena;
    private String deskribapena;
    private double prezioa;
    private String kategoria;
    private boolean eskuragarri;

    public Platera() {}

    public Platera(int id, String izena, String deskribapena, double prezioa, String kategoria, boolean eskuragarri) {
        this.id = id;
        this.izena = izena;
        this.deskribapena = deskribapena;
        this.prezioa = prezioa;
        this.kategoria = kategoria;
        this.eskuragarri = eskuragarri;
    }

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getIzena() { return izena != null ? izena : ""; }
    public void setIzena(String izena) { this.izena = izena; }

    public String getDeskribapena() { return deskribapena != null ? deskribapena : ""; }
    public void setDeskribapena(String deskribapena) { this.deskribapena = deskribapena; }

    public double getPrezioa() { return prezioa; }
    public void setPrezioa(double prezioa) { this.prezioa = prezioa; }

    public String getKategoria() { return kategoria != null ? kategoria : ""; }
    public void setKategoria(String kategoria) { this.kategoria = kategoria; }

    public boolean isEskuragarri() { return eskuragarri; }
    public void setEskuragarri(boolean eskuragarri) { this.eskuragarri = eskuragarri; }

    public boolean isEdaria() {
        String kat = getKategoria().toLowerCase();
        return kat.contains("edari") || kat.contains("ardo") || kat.contains("kafe") || kat.contains("garagardo") || kat.contains("ura");
    }

    @Override
    public String toString() {
        return getIzena() + " (" + String.format("%.2f €", prezioa) + ")";
    }
}
