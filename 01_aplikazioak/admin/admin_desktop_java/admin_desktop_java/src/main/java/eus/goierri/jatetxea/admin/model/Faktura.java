package eus.goierri.jatetxea.admin.model;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public final class Faktura {
    public int id;
    public Integer eskaeraId;
    public String fakturaZenbakia;
    public String mota; // "Salmenta", "Erosketa"
    public String azpimota; // "Sinplifikatua", "Osoa"
    public String kontzeptua;
    public String hornitzailea;
    public String hornitzaileCif;
    public String bezeroNif;
    public String bezeroIzena;
    public String bezeroHelbidea;
    public double oinarriZergagarria;
    public double bezEhunekoa;
    public double bezZenbatekoa;
    public double guztira;
    public String ordainketaMetodoa;
    public String egoera; // "Ordaindua", "Zain", "Ezeztatua"
    public String data;
    public String oharrak;
    public boolean ezabatua;

    public int getId() { return id; }
    public String getFakturaZenbakia() { return fakturaZenbakia != null ? fakturaZenbakia : ""; }
    public String getMota() { return mota != null ? mota : "Salmenta"; }
    public String getAzpimota() { return azpimota != null ? azpimota : "Sinplifikatua"; }
    public String getKontzeptua() { return kontzeptua != null ? kontzeptua : ""; }
    public String getHornitzailea() { return hornitzailea != null ? hornitzailea : ""; }
    public String getBezeroIzena() { return bezeroIzena != null ? bezeroIzena : ""; }
    public double getOinarriZergagarria() { return oinarriZergagarria; }
    public double getBezEhunekoa() { return bezEhunekoa; }
    public double getBezZenbatekoa() { return bezZenbatekoa; }
    public double getGuztira() { return guztira; }
    public String getOrdainketaMetodoa() { return ordainketaMetodoa != null ? ordainketaMetodoa : "Txartela"; }
    public String getEgoera() { return egoera != null ? egoera : "Ordaindua"; }
    public String getData() { return data != null ? data : ""; }

    public String getEntitatea() {
        if ("Erosketa".equalsIgnoreCase(mota)) {
            return hornitzailea != null && !hornitzailea.isBlank() ? hornitzailea : "Hornitzailea";
        } else {
            return bezeroIzena != null && !bezeroIzena.isBlank() ? bezeroIzena : "Bezero Arrunta";
        }
    }

    public String getFormatGuztira() {
        return String.format("%.2f €", guztira);
    }

    public String getFormatOinarria() {
        return String.format("%.2f €", oinarriZergagarria);
    }

    public String getFormatBez() {
        return String.format("%.2f € (%%%.0f)", bezZenbatekoa, bezEhunekoa);
    }

    public String getDataLaburra() {
        if (data == null || data.isBlank()) return "-";
        try {
            if (data.length() >= 16) {
                return data.substring(0, 16).replace("T", " ");
            }
            return data.replace("T", " ");
        } catch (Exception e) {
            return data;
        }
    }

    @Override
    public String toString() {
        return getFakturaZenbakia() + " - " + getKontzeptua() + " (" + getFormatGuztira() + ")";
    }
}
