package eus.goierri.autoak.model;

import java.text.NumberFormat;
import java.util.LinkedHashSet;
import java.util.Locale;
import java.util.Set;

/**
 * Bezeroak auto jakin batean egindako pertsonalizazioa gordetzen du,
 * 1. pantailara itzuli eta berriz sartzean aukeraketak gal ez daitezen.
 */
public final class AutoKonfigurazioa {
    private static final Locale EU_LOCALE = Locale.of("es", "ES");

    private final Autoa autoa;
    private Autoa.MotorBertsioa motorra;
    private Autoa.KoloreAukera kolorea;
    private Autoa.AkaberaAukera akabera;
    private final Set<String> gehigarriIdak = new LinkedHashSet<>();

    public AutoKonfigurazioa(Autoa autoa) {
        this.autoa = autoa;
        berrezarri();
    }

    public void berrezarri() {
        this.motorra = autoa.getMotorLehenetsia();
        this.kolorea = autoa.getKoloreLehenetsia();
        this.akabera = autoa.getAkaberaLehenetsia();
        this.gehigarriIdak.clear();
    }

    public Autoa getAutoa() {
        return autoa;
    }

    public Autoa.MotorBertsioa getMotorra() {
        return motorra;
    }

    public void setMotorra(Autoa.MotorBertsioa motorra) {
        this.motorra = motorra;
    }

    public Autoa.KoloreAukera getKolorea() {
        return kolorea;
    }

    public void setKolorea(Autoa.KoloreAukera kolorea) {
        this.kolorea = kolorea;
    }

    public Autoa.AkaberaAukera getAkabera() {
        return akabera;
    }

    public void setAkabera(Autoa.AkaberaAukera akabera) {
        this.akabera = akabera;
    }

    public Set<String> getGehigarriIdak() {
        return gehigarriIdak;
    }

    public int getGehigarrienPrezioa() {
        return autoa.gehigarriak().stream()
                .filter(g -> gehigarriIdak.contains(g.id()))
                .mapToInt(Autoa.EkipamenduGehigarria::prezioa)
                .sum();
    }

    public int kalkulatuAzkenPrezioa() {
        return autoa.oinarrizkoPrezioa()
                + motorra.prezioGehigarria()
                + kolorea.prezioGehigarria()
                + akabera.prezioGehigarria()
                + getGehigarrienPrezioa();
    }

    public boolean isPertsonalizatua() {
        return !motorra.equals(autoa.getMotorLehenetsia())
                || !kolorea.equals(autoa.getKoloreLehenetsia())
                || !akabera.equals(autoa.getAkaberaLehenetsia())
                || !gehigarriIdak.isEmpty();
    }

    public static String formatEuro(int kopurua) {
        NumberFormat nf = NumberFormat.getIntegerInstance(EU_LOCALE);
        return nf.format(kopurua) + " €";
    }
}
