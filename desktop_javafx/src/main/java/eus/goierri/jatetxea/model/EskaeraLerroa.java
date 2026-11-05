package eus.goierri.jatetxea.model;

/**
 * Eskaera baten barruko lerroa (plater bat, kantitatea, sukaldeko egoera eta oharrak).
 */
public class EskaeraLerroa {
    private int id;
    private int eskaeraId;
    private int plateraId;
    private String platerIzena;
    private int kantitatea;
    private double unitatePrezioa;
    private String egoera;
    private String oharrak;

    public EskaeraLerroa() {
    }

    public EskaeraLerroa(int id, int plateraId, String platerIzena, int kantitatea, double unitatePrezioa, String egoera, String oharrak) {
        this.id = id;
        this.plateraId = plateraId;
        this.platerIzena = platerIzena;
        this.kantitatea = kantitatea;
        this.unitatePrezioa = unitatePrezioa;
        this.egoera = egoera;
        this.oharrak = oharrak;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getEskaeraId() {
        return eskaeraId;
    }

    public void setEskaeraId(int eskaeraId) {
        this.eskaeraId = eskaeraId;
    }

    public int getPlateraId() {
        return plateraId;
    }

    public void setPlateraId(int plateraId) {
        this.plateraId = plateraId;
    }

    public String getPlaterIzena() {
        return platerIzena != null ? platerIzena : "Platera #" + plateraId;
    }

    public void setPlaterIzena(String platerIzena) {
        this.platerIzena = platerIzena;
    }

    public int getKantitatea() {
        return kantitatea;
    }

    public void setKantitatea(int kantitatea) {
        this.kantitatea = kantitatea;
    }

    public double getUnitatePrezioa() {
        return unitatePrezioa;
    }

    public void setUnitatePrezioa(double unitatePrezioa) {
        this.unitatePrezioa = unitatePrezioa;
    }

    public String getEgoera() {
        return egoera != null ? egoera : "Eskatua";
    }

    public void setEgoera(String egoera) {
        this.egoera = egoera;
    }

    public String getOharrak() {
        return oharrak != null ? oharrak : "-";
    }

    public void setOharrak(String oharrak) {
        this.oharrak = oharrak;
    }

    public double getGuztira() {
        return kantitatea * unitatePrezioa;
    }
}
