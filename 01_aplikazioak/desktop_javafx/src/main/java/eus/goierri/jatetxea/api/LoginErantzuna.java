package eus.goierri.jatetxea.api;

import com.google.gson.annotations.SerializedName;
import eus.goierri.jatetxea.model.Langilea;

/**
 * Login eskaeraren erantzuna REST API bidez.
 */
public class LoginErantzuna {
    @SerializedName("arrakasta")
    private boolean arrakasta;

    @SerializedName("mezua")
    private String mezua;

    @SerializedName("langilea")
    private Langilea langilea;

    public LoginErantzuna() {
    }

    public LoginErantzuna(boolean arrakasta, String mezua, Langilea langilea) {
        this.arrakasta = arrakasta;
        this.mezua = mezua;
        this.langilea = langilea;
    }

    public boolean isArrakasta() {
        return arrakasta;
    }

    public void setArrakasta(boolean arrakasta) {
        this.arrakasta = arrakasta;
    }

    public String getMezua() {
        return mezua != null ? mezua : "";
    }

    public void setMezua(String mezua) {
        this.mezua = mezua;
    }

    public Langilea getLangilea() {
        return langilea;
    }

    public void setLangilea(Langilea langilea) {
        this.langilea = langilea;
    }
}
