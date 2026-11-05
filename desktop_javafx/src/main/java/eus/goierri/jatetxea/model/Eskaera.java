package eus.goierri.jatetxea.model;

import java.util.ArrayList;
import java.util.List;

/**
 * Sukaldean eta kudeaketan erabiltzen den eskaera eredua.
 */
public class Eskaera {
    private int id;
    private int mahaiaId;
    private int mahaiaZenbakia;
    private int langileaId;
    private String langileIzena;
    private String egoera;
    private double guztira;
    private String sortzeData;
    private List<EskaeraLerroa> lerroak = new ArrayList<>();

    public Eskaera() {
    }

    public Eskaera(int id, int mahaiaZenbakia, String langileIzena, String egoera, double guztira, String sortzeData) {
        this.id = id;
        this.mahaiaZenbakia = mahaiaZenbakia;
        this.langileIzena = langileIzena;
        this.egoera = egoera;
        this.guztira = guztira;
        this.sortzeData = sortzeData;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getMahaiaId() {
        return mahaiaId;
    }

    public void setMahaiaId(int mahaiaId) {
        this.mahaiaId = mahaiaId;
    }

    public int getMahaiaZenbakia() {
        return mahaiaZenbakia;
    }

    public void setMahaiaZenbakia(int mahaiaZenbakia) {
        this.mahaiaZenbakia = mahaiaZenbakia;
    }

    public int getLangileaId() {
        return langileaId;
    }

    public void setLangileaId(int langileaId) {
        this.langileaId = langileaId;
    }

    public String getLangileIzena() {
        return langileIzena != null ? langileIzena : "";
    }

    public void setLangileIzena(String langileIzena) {
        this.langileIzena = langileIzena;
    }

    public String getEgoera() {
        return egoera != null ? egoera : "";
    }

    public void setEgoera(String egoera) {
        this.egoera = egoera;
    }

    public double getGuztira() {
        return guztira;
    }

    public void setGuztira(double guztira) {
        this.guztira = guztira;
    }

    public String getSortzeData() {
        return sortzeData != null ? sortzeData : "";
    }

    public void setSortzeData(String sortzeData) {
        this.sortzeData = sortzeData;
    }

    public List<EskaeraLerroa> getLerroak() {
        return lerroak != null ? lerroak : new ArrayList<>();
    }

    public void setLerroak(List<EskaeraLerroa> lerroak) {
        this.lerroak = lerroak;
    }

    public int getPlaterKopurua() {
        if (lerroak == null) return 0;
        int bat = 0;
        for (EskaeraLerroa l : lerroak) {
            bat += l.getKantitatea();
        }
        return bat;
    }

    public List<EskaeraLerroa> getSukaldekoLerroak() {
        List<EskaeraLerroa> emaitza = new ArrayList<>();
        if (lerroak != null) {
            for (EskaeraLerroa l : lerroak) {
                if (!l.isEdaria()) {
                    emaitza.add(l);
                }
            }
        }
        return emaitza;
    }

    public int getSukaldekoPlaterKopurua() {
        int bat = 0;
        for (EskaeraLerroa l : getSukaldekoLerroak()) {
            bat += l.getKantitatea();
        }
        return bat;
    }

    public List<EskaeraLerroa> getBarrakoLerroak() {
        List<EskaeraLerroa> emaitza = new ArrayList<>();
        if (lerroak != null) {
            for (EskaeraLerroa l : lerroak) {
                if (l.isEdaria()) {
                    emaitza.add(l);
                }
            }
        }
        return emaitza;
    }

    public int getBarrakoEdariKopurua() {
        int bat = 0;
        for (EskaeraLerroa l : getBarrakoLerroak()) {
            bat += l.getKantitatea();
        }
        return bat;
    }

    public String getMahaiaTestua() {
        return "Mahaia " + mahaiaZenbakia;
    }

    @Override
    public String toString() {
        return "Eskaera #" + id + " - Mahaia " + mahaiaZenbakia + " (" + egoera + ")";
    }
}
