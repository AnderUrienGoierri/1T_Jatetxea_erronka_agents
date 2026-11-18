package eus.goierri.jatetxea.core.api;

public class ApiErantzuna<T> {
    private boolean arrakasta;
    private String mezua;
    private T datuak;

    public ApiErantzuna() {}

    public ApiErantzuna(boolean arrakasta, String mezua, T datuak) {
        this.arrakasta = arrakasta;
        this.mezua = mezua;
        this.datuak = datuak;
    }

    public boolean isArrakasta() { return arrakasta; }
    public void setArrakasta(boolean arrakasta) { this.arrakasta = arrakasta; }

    public String getMezua() { return mezua; }
    public void setMezua(String mezua) { this.mezua = mezua; }

    public T getDatuak() { return datuak; }
    public void setDatuak(T datuak) { this.datuak = datuak; }
}
