package eus.goierri.jatetxea.model;

import com.google.gson.annotations.SerializedName;
import java.util.ArrayList;
import java.util.List;

/**
 * Sukaldean eta kudeaketan erabiltzen den eskaera eredua.
 */
public class Eskaera {
    private int id;

    @SerializedName(value = "mahaiaId", alternate = {"mahaia_id", "mahaiId"})
    private int mahaiaId;

    @SerializedName(value = "mahaiZenbakia", alternate = {"mahaiaZenbakia", "mahaia_zenbakia", "zenbakia"})
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
        if (mahaiaZenbakia > 0) return mahaiaZenbakia;
        if (mahaiaId > 0) return mahaiaId;
        return 1;
    }

    public void setMahaiaZenbakia(int mahaiaZenbakia) {
        this.mahaiaZenbakia = mahaiaZenbakia;
    }

    public int getMahaiZenbakia() {
        return getMahaiaZenbakia();
    }

    public void setMahaiZenbakia(int mahaiZenbakia) {
        this.mahaiaZenbakia = mahaiZenbakia;
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
                    String e = l.getEgoera() != null ? l.getEgoera().trim().toLowerCase() : "";
                    // Zerbitzariak 'Sukaldera bidali' sakatu arte ez dira sukaldean agertuko!
                    // 'Eskatua' eta 'zai' daudenak oraindik sukaldera bidali gabe daude
                    if (!e.contains("eskat") && !e.contains("zai")) {
                        emaitza.add(l);
                    }
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

    public int getBarrakoAktiboenKopurua() {
        int bat = 0;
        for (EskaeraLerroa l : getBarrakoLerroak()) {
            if (!"Zerbitzatua".equalsIgnoreCase(l.getBarrakoEgoera())) {
                bat += l.getKantitatea();
            }
        }
        return bat;
    }

    public String getBarrakoEgoera() {
        List<EskaeraLerroa> edariak = getBarrakoLerroak();
        if (edariak.isEmpty()) return "-";

        boolean guztiakZerbitzatuta = true;
        boolean batenbatEginda = false;
        boolean batenbatPrestatzen = false;

        for (EskaeraLerroa l : edariak) {
            String eg = l.getBarrakoEgoera();
            if (!"Zerbitzatua".equalsIgnoreCase(eg)) {
                guztiakZerbitzatuta = false;
                if ("Prestatua".equalsIgnoreCase(eg) || "Eginda".equalsIgnoreCase(eg)) {
                    batenbatEginda = true;
                } else if ("Prestatzen".equalsIgnoreCase(eg) || "Martxan".equalsIgnoreCase(eg)) {
                    batenbatPrestatzen = true;
                }
            }
        }

        if (guztiakZerbitzatuta) return "Zerbitzatua";
        if (batenbatEginda) return "Prestatua";
        if (batenbatPrestatzen) return "Prestatzen";
        return "Zain";
    }

    public String getMahaiaTestua() {
        return "Mahaia " + getMahaiaZenbakia();
    }

    @Override
    public String toString() {
        return "Eskaera #" + id + " - Mahaia " + getMahaiaZenbakia() + " (" + egoera + ")";
    }
}
