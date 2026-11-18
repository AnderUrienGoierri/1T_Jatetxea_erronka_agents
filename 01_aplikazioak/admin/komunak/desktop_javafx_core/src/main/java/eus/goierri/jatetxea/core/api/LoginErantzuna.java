package eus.goierri.jatetxea.core.api;

import eus.goierri.jatetxea.core.model.Langilea;

public class LoginErantzuna {
    private boolean arrakasta;
    private String mezua;
    private Langilea langilea;

    public LoginErantzuna() {}

    public LoginErantzuna(boolean arrakasta, String mezua, Langilea langilea) {
        this.arrakasta = arrakasta;
        this.mezua = mezua;
        this.langilea = langilea;
    }

    public boolean isArrakasta() { return arrakasta; }
    public void setArrakasta(boolean arrakasta) { this.arrakasta = arrakasta; }

    public String getMezua() { return mezua; }
    public void setMezua(String mezua) { this.mezua = mezua; }

    public Langilea getLangilea() { return langilea; }
    public void setLangilea(Langilea langilea) { this.langilea = langilea; }
}
