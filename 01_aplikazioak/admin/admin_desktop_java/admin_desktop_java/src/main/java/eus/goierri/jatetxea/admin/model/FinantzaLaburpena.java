package eus.goierri.jatetxea.admin.model;

public final class FinantzaLaburpena {
    public double salmentakGuztira;
    public double gastuakGuztira;
    public double mozkinGarbia;
    public int salmentaKopurua;
    public int gastuKopurua;
    public double jasotakoBez;
    public double ordaindutakoBez;
    public double bezLikidazioa;

    public String getFormatSalmentak() {
        return String.format("%.2f €", salmentakGuztira);
    }

    public String getFormatGastuak() {
        return String.format("%.2f €", gastuakGuztira);
    }

    public String getFormatMozkina() {
        return String.format("%s%.2f €", mozkinGarbia >= 0 ? "+" : "", mozkinGarbia);
    }

    public String getFormatBezLikidazioa() {
        return String.format("%s%.2f €", bezLikidazioa >= 0 ? "+" : "", bezLikidazioa);
    }

    public String getFormatJasotakoBez() {
        return String.format("%.2f €", jasotakoBez);
    }

    public String getFormatOrdaindutakoBez() {
        return String.format("%.2f €", ordaindutakoBez);
    }
}
