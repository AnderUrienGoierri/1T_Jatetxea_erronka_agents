package eus.goierri.jatetxea.model;

import com.google.gson.annotations.SerializedName;

/**
 * Jatetxeko langilearen eredua (JavaFX aplikazioan saio-hasiera eta baimenak kudeatzeko).
 */
public class Langilea {
    @SerializedName("id")
    private int id;

    @SerializedName("erabiltzailea")
    private String erabiltzailea;

    @SerializedName("izena")
    private String izena;

    @SerializedName("abizena")
    private String abizena;

    @SerializedName("rola")
    private String rola;

    @SerializedName("aktibo")
    private boolean aktibo;

    @SerializedName("ezabatua")
    private boolean ezabatua;

    public Langilea() {
    }

    public Langilea(int id, String erabiltzailea, String izena, String abizena, String rola, boolean aktibo) {
        this.id = id;
        this.erabiltzailea = erabiltzailea;
        this.izena = izena;
        this.abizena = abizena;
        this.rola = rola;
        this.aktibo = aktibo;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getErabiltzailea() {
        return erabiltzailea != null ? erabiltzailea : "";
    }

    public void setErabiltzailea(String erabiltzailea) {
        this.erabiltzailea = erabiltzailea;
    }

    public String getIzena() {
        return izena != null ? izena : "";
    }

    public void setIzena(String izena) {
        this.izena = izena;
    }

    public String getAbizena() {
        return abizena != null ? abizena : "";
    }

    public void setAbizena(String abizena) {
        this.abizena = abizena;
    }

    public String getRola() {
        return rola != null ? rola : "";
    }

    public void setRola(String rola) {
        this.rola = rola;
    }

    public boolean isAktibo() {
        return aktibo;
    }

    public void setAktibo(boolean aktibo) {
        this.aktibo = aktibo;
    }

    public boolean isEzabatua() {
        return ezabatua;
    }

    public void setEzabatua(boolean ezabatua) {
        this.ezabatua = ezabatua;
    }

    public String getIzenOsoa() {
        if (abizena != null && !abizena.isBlank()) {
            return izena + " " + abizena;
        }
        return izena != null ? izena : "";
    }

    public String getInitialak() {
        String n = getIzena().trim();
        String a = getAbizena().trim();
        if (!n.isEmpty() && !a.isEmpty()) {
            return ("" + n.charAt(0) + a.charAt(0)).toUpperCase();
        } else if (n.length() >= 2) {
            return n.substring(0, 2).toUpperCase();
        } else if (!n.isEmpty()) {
            return ("" + n.charAt(0)).toUpperCase();
        }
        return "T1";
    }

    @Override
    public String toString() {
        return getIzenOsoa() + " (" + getRola() + ")";
    }
}
