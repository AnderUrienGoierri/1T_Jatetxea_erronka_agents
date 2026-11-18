package eus.goierri.jatetxea.core.model;

/**
 * Eskaera baten barruko lerro bakoitzaren eredua.
 */
public class EskaeraLerroa {
    private int id;
    private int eskaeraId;
    private int platerId;
    private String platerIzena;
    private int kantitatea;
    private double prezioa;
    private String egoera;
    private String oharrak;
    private String kategoria;

    public EskaeraLerroa() {}

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public int getEskaeraId() { return eskaeraId; }
    public void setEskaeraId(int eskaeraId) { this.eskaeraId = eskaeraId; }

    public int getPlaterId() { return platerId; }
    public void setPlaterId(int platerId) { this.platerId = platerId; }

    public String getPlaterIzena() { return platerIzena != null ? platerIzena : ("Platera #" + platerId); }
    public void setPlaterIzena(String platerIzena) { this.platerIzena = platerIzena; }

    public int getKantitatea() { return kantitatea; }
    public void setKantitatea(int kantitatea) { this.kantitatea = kantitatea; }

    public double getPrezioa() { return prezioa; }
    public void setPrezioa(double prezioa) { this.prezioa = prezioa; }

    public String getEgoera() { return egoera != null ? egoera : "Eskatuta"; }
    public void setEgoera(String egoera) { this.egoera = egoera; }

    public String getOharrak() { return oharrak != null ? oharrak : ""; }
    public void setOharrak(String oharrak) { this.oharrak = oharrak; }

    public String getKategoria() { return kategoria != null ? kategoria : ""; }
    public void setKategoria(String kategoria) { this.kategoria = kategoria; }

    public boolean isEdaria() {
        String kat = getKategoria().toLowerCase();
        String izena = getPlaterIzena().toLowerCase();
        return kat.contains("edari") || kat.contains("ardo") || kat.contains("kafe") ||
               kat.contains("garagardo") || kat.contains("ura") ||
               izena.contains("ardo") || izena.contains("kafe") || izena.contains("garagardo") ||
               izena.contains("ura") || izena.contains("sagardo") || izena.contains("refresco") ||
               izena.contains("coca");
    }

    public String getBarrakoEgoera() {
        return getEgoera();
    }
}
