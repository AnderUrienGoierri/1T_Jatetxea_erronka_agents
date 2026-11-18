package eus.goierri.jatetxea.core.model;

/**
 * Jatetxeko langilearen eredu orokorra (MVC ereduaren Model geruza).
 */
public class Langilea {
    private int id;
    private String izena;
    private String abizena;
    private String erabiltzailea;
    private String rola;
    private String irudia;
    private boolean aktibo;

    public Langilea() {}

    public Langilea(int id, String izena, String abizena, String erabiltzailea, String rola, String irudia, boolean aktibo) {
        this.id = id;
        this.izena = izena;
        this.abizena = abizena;
        this.erabiltzailea = erabiltzailea;
        this.rola = rola;
        this.irudia = irudia;
        this.aktibo = aktibo;
    }

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getIzena() { return izena != null ? izena : ""; }
    public void setIzena(String izena) { this.izena = izena; }

    public String getAbizena() { return abizena != null ? abizena : ""; }
    public void setAbizena(String abizena) { this.abizena = abizena; }

    public String getIzenOsoa() {
        return (getIzena() + " " + getAbizena()).trim();
    }

    public String getErabiltzailea() { return erabiltzailea != null ? erabiltzailea : ""; }
    public void setErabiltzailea(String erabiltzailea) { this.erabiltzailea = erabiltzailea; }

    public String getRola() { return rola != null ? rola : ""; }
    public void setRola(String rola) { this.rola = rola; }

    public String getIrudia() { return irudia; }
    public void setIrudia(String irudia) { this.irudia = irudia; }

    public boolean isAktibo() { return aktibo; }
    public void setAktibo(boolean aktibo) { this.aktibo = aktibo; }

    public String getInitialak() {
        String n = getIzena().trim();
        String a = getAbizena().trim();
        String res = "";
        if (!n.isEmpty()) res += n.substring(0, 1).toUpperCase();
        if (!a.isEmpty()) res += a.substring(0, 1).toUpperCase();
        if (res.isEmpty() && !getErabiltzailea().isEmpty()) {
            res = getErabiltzailea().substring(0, Math.min(2, getErabiltzailea().length())).toUpperCase();
        }
        return res.isEmpty() ? "??" : res;
    }

    @Override
    public String toString() {
        return getIzenOsoa() + " (" + getRola() + ")";
    }
}
