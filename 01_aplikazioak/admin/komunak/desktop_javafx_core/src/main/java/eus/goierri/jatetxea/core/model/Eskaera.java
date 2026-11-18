package eus.goierri.jatetxea.core.model;

import java.util.ArrayList;
import java.util.List;

/**
 * Eskaera osoaren eredua.
 */
public class Eskaera {
    private int id;
    private int mahaiaId;
    private int langileId;
    private String egoera;
    private String sortzeData;
    private String oharrak;
    private List<EskaeraLerroa> lerroak = new ArrayList<>();

    // Informazio osagarria (zerbitzariaren izena, mahai zenbakia...)
    private String langileIzena;
    private String mahaiaZenbakia;

    public Eskaera() {}

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public int getMahaiaId() { return mahaiaId; }
    public void setMahaiaId(int mahaiaId) { this.mahaiaId = mahaiaId; }

    public int getLangileId() { return langileId; }
    public void setLangileId(int langileId) { this.langileId = langileId; }

    public String getEgoera() { return egoera != null ? egoera : "Sukaldean"; }
    public void setEgoera(String egoera) { this.egoera = egoera; }

    public String getSortzeData() { return sortzeData != null ? sortzeData : ""; }
    public void setSortzeData(String sortzeData) { this.sortzeData = sortzeData; }

    public String getOharrak() { return oharrak != null ? oharrak : ""; }
    public void setOharrak(String oharrak) { this.oharrak = oharrak; }

    public List<EskaeraLerroa> getLerroak() { return lerroak != null ? lerroak : new ArrayList<>(); }
    public void setLerroak(List<EskaeraLerroa> lerroak) { this.lerroak = lerroak; }

    public String getLangileIzena() {
        return langileIzena != null ? langileIzena : ("Zerbitzaria #" + langileId);
    }
    public void setLangileIzena(String langileIzena) { this.langileIzena = langileIzena; }

    public String getMahaiaZenbakia() {
        return mahaiaZenbakia != null ? mahaiaZenbakia : String.valueOf(mahaiaId);
    }
    public void setMahaiaZenbakia(String mahaiaZenbakia) { this.mahaiaZenbakia = mahaiaZenbakia; }

    public String getMahaiaTestua() {
        return "Mahaia " + getMahaiaZenbakia();
    }

    public int getSukaldekoPlaterKopurua() {
        int batura = 0;
        for (EskaeraLerroa l : getLerroak()) {
            if (!l.isEdaria()) {
                batura += l.getKantitatea();
            }
        }
        return batura;
    }

    public int getBarrakoEdariKopurua() {
        int batura = 0;
        for (EskaeraLerroa l : getLerroak()) {
            if (l.isEdaria()) {
                batura += l.getKantitatea();
            }
        }
        return batura;
    }

    public String getBarrakoEgoera() {
        boolean edariakDitu = false;
        boolean guztiakEginda = true;
        for (EskaeraLerroa l : getLerroak()) {
            if (l.isEdaria()) {
                edariakDitu = true;
                String e = l.getEgoera().toLowerCase();
                if (!e.contains("prestat") && !e.contains("egin") && !e.contains("zerbitzat")) {
                    guztiakEginda = false;
                }
            }
        }
        if (!edariakDitu) return "Edaririk ez";
        return guztiakEginda ? "Prest / Zerbitzatua" : "Prestatzen";
    }
}
