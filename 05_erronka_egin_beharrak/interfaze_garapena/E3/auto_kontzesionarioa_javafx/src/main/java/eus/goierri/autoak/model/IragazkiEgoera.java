package eus.goierri.autoak.model;

import java.util.LinkedHashSet;
import java.util.Set;

/**
 * 1. pantailako iragazki guztien egoera gordetzen du,
 * edozein ordenatan aldatu, banaka kendu edo hutsetik hasteko.
 */
public final class IragazkiEgoera {
    public static final int AURREKONTU_MIN_DEFAULT = 15000;
    public static final int AURREKONTU_MAX_DEFAULT = 70000;

    private final Set<String> erabilerak = new LinkedHashSet<>();
    private final Set<String> erregaiak = new LinkedHashSet<>();
    private int aurrekontuMin = AURREKONTU_MIN_DEFAULT;
    private int aurrekontuMax = AURREKONTU_MAX_DEFAULT;
    private Integer gutxienekoPlazak = null;
    private Integer ateak = null;
    private String karrozeria = "Guztiak";
    private final Set<String> markaNahiak = new LinkedHashSet<>();
    private final Set<String> markaBaztertuak = new LinkedHashSet<>();

    public Set<String> getErabilerak() {
        return erabilerak;
    }

    public Set<String> getErregaiak() {
        return erregaiak;
    }

    public int getAurrekontuMin() {
        return aurrekontuMin;
    }

    public void setAurrekontuMin(int aurrekontuMin) {
        this.aurrekontuMin = aurrekontuMin;
    }

    public int getAurrekontuMax() {
        return aurrekontuMax;
    }

    public void setAurrekontuMax(int aurrekontuMax) {
        this.aurrekontuMax = aurrekontuMax;
    }

    public Integer getGutxienekoPlazak() {
        return gutxienekoPlazak;
    }

    public void setGutxienekoPlazak(Integer gutxienekoPlazak) {
        this.gutxienekoPlazak = gutxienekoPlazak;
    }

    public Integer getAteak() {
        return ateak;
    }

    public void setAteak(Integer ateak) {
        this.ateak = ateak;
    }

    public String getKarrozeria() {
        return karrozeria;
    }

    public void setKarrozeria(String karrozeria) {
        this.karrozeria = (karrozeria == null || karrozeria.isBlank()) ? "Guztiak" : karrozeria;
    }

    public Set<String> getMarkaNahiak() {
        return markaNahiak;
    }

    public Set<String> getMarkaBaztertuak() {
        return markaBaztertuak;
    }

    public void garbituGuztia() {
        erabilerak.clear();
        erregaiak.clear();
        aurrekontuMin = AURREKONTU_MIN_DEFAULT;
        aurrekontuMax = AURREKONTU_MAX_DEFAULT;
        gutxienekoPlazak = null;
        ateak = null;
        karrozeria = "Guztiak";
        markaNahiak.clear();
        markaBaztertuak.clear();
    }

    public boolean betetzenDu(Autoa autoa) {
        if (!erabilerak.isEmpty() && erabilerak.stream().noneMatch(u -> autoa.erabilerak().contains(u))) {
            return false;
        }
        if (!erregaiak.isEmpty() && !erregaiak.contains(autoa.erregaia())) {
            return false;
        }
        if (autoa.oinarrizkoPrezioa() < aurrekontuMin || autoa.oinarrizkoPrezioa() > aurrekontuMax) {
            return false;
        }
        if (gutxienekoPlazak != null && autoa.plazak() < gutxienekoPlazak) {
            return false;
        }
        if (ateak != null && autoa.ateak() != ateak) {
            return false;
        }
        if (!"Guztiak".equalsIgnoreCase(karrozeria) && !karrozeria.equalsIgnoreCase(autoa.karrozeria())) {
            return false;
        }
        if (!markaNahiak.isEmpty() && !markaNahiak.contains(autoa.marka())) {
            return false;
        }
        if (markaBaztertuak.contains(autoa.marka())) {
            return false;
        }
        return true;
    }

    public int aktiboKopurua() {
        int count = erabilerak.size() + erregaiak.size() + markaNahiak.size() + markaBaztertuak.size();
        if (aurrekontuMin > AURREKONTU_MIN_DEFAULT || aurrekontuMax < AURREKONTU_MAX_DEFAULT) {
            count++;
        }
        if (gutxienekoPlazak != null) {
            count++;
        }
        if (ateak != null) {
            count++;
        }
        if (!"Guztiak".equalsIgnoreCase(karrozeria)) {
            count++;
        }
        return count;
    }
}
