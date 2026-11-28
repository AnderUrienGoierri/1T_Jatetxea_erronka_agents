package eus.goierri.jatetxea.core.model;

/**
 * Albaran sarrera + erosketa-faktura automatikoaren erantzuna.
 */
public class AlbaranaSarreraErantzuna {
    private boolean arrakasta;
    private String mezua;
    private int lehengaiaId;
    private String izena;
    private double aurrekoStocka;
    private double gehitutakoKantitatea;
    private double unekoStockBerria;
    private String unitatea;
    private String hornitzailea;
    private String albaranZenbakia;
    private String ordainketaMetodoa;
    private double oinarriZergagarria;
    private double bezEhunekoa;
    private double bezZenbatekoa;
    private double guztira;
    private Integer fakturaId;
    private String fakturaZenbakia;

    public boolean isArrakasta() { return arrakasta; }
    public void setArrakasta(boolean arrakasta) { this.arrakasta = arrakasta; }

    public String getMezua() { return mezua != null ? mezua : ""; }
    public void setMezua(String mezua) { this.mezua = mezua; }

    public int getLehengaiaId() { return lehengaiaId; }
    public void setLehengaiaId(int lehengaiaId) { this.lehengaiaId = lehengaiaId; }

    public String getIzena() { return izena != null ? izena : ""; }
    public void setIzena(String izena) { this.izena = izena; }

    public double getAurrekoStocka() { return aurrekoStocka; }
    public void setAurrekoStocka(double aurrekoStocka) { this.aurrekoStocka = aurrekoStocka; }

    public double getGehitutakoKantitatea() { return gehitutakoKantitatea; }
    public void setGehitutakoKantitatea(double gehitutakoKantitatea) { this.gehitutakoKantitatea = gehitutakoKantitatea; }

    public double getUnekoStockBerria() { return unekoStockBerria; }
    public void setUnekoStockBerria(double unekoStockBerria) { this.unekoStockBerria = unekoStockBerria; }

    public String getUnitatea() { return unitatea != null ? unitatea : ""; }
    public void setUnitatea(String unitatea) { this.unitatea = unitatea; }

    public String getHornitzailea() { return hornitzailea != null ? hornitzailea : ""; }
    public void setHornitzailea(String hornitzailea) { this.hornitzailea = hornitzailea; }

    public String getAlbaranZenbakia() { return albaranZenbakia != null ? albaranZenbakia : ""; }
    public void setAlbaranZenbakia(String albaranZenbakia) { this.albaranZenbakia = albaranZenbakia; }

    public String getOrdainketaMetodoa() { return ordainketaMetodoa != null ? ordainketaMetodoa : ""; }
    public void setOrdainketaMetodoa(String ordainketaMetodoa) { this.ordainketaMetodoa = ordainketaMetodoa; }

    public double getOinarriZergagarria() { return oinarriZergagarria; }
    public void setOinarriZergagarria(double oinarriZergagarria) { this.oinarriZergagarria = oinarriZergagarria; }

    public double getBezEhunekoa() { return bezEhunekoa; }
    public void setBezEhunekoa(double bezEhunekoa) { this.bezEhunekoa = bezEhunekoa; }

    public double getBezZenbatekoa() { return bezZenbatekoa; }
    public void setBezZenbatekoa(double bezZenbatekoa) { this.bezZenbatekoa = bezZenbatekoa; }

    public double getGuztira() { return guztira; }
    public void setGuztira(double guztira) { this.guztira = guztira; }

    public Integer getFakturaId() { return fakturaId; }
    public void setFakturaId(Integer fakturaId) { this.fakturaId = fakturaId; }

    public String getFakturaZenbakia() { return fakturaZenbakia != null ? fakturaZenbakia : ""; }
    public void setFakturaZenbakia(String fakturaZenbakia) { this.fakturaZenbakia = fakturaZenbakia; }
}
