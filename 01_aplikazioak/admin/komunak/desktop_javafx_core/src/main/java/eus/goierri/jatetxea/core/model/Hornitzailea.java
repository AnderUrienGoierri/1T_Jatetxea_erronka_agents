package eus.goierri.jatetxea.core.model;

import com.google.gson.annotations.SerializedName;

/**
 * Produktu-mota bakoitzeko hornitzailearen eredua.
 */
public class Hornitzailea {
    private int id;
    private String izena;
    private String cif;

    @SerializedName(value = "produktuMota", alternate = {"produktu_mota"})
    private String produktuMota;

    @SerializedName(value = "bezEhunekoa", alternate = {"bez_ehunekoa"})
    private double bezEhunekoa = 10.0;

    private String telefonoa;
    private String emaila;
    private String helbidea;
    private boolean aktibo = true;

    @SerializedName(value = "lehengaiKopurua", alternate = {"lehengai_kopurua"})
    private int lehengaiKopurua;

    public Hornitzailea() {}

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getIzena() { return izena != null ? izena : ""; }
    public void setIzena(String izena) { this.izena = izena; }

    public String getCif() { return cif != null ? cif : ""; }
    public void setCif(String cif) { this.cif = cif; }

    public String getProduktuMota() { return produktuMota != null ? produktuMota : ""; }
    public void setProduktuMota(String produktuMota) { this.produktuMota = produktuMota; }

    public double getBezEhunekoa() { return bezEhunekoa > 0 ? bezEhunekoa : 10.0; }
    public void setBezEhunekoa(double bezEhunekoa) { this.bezEhunekoa = bezEhunekoa; }

    public String getTelefonoa() { return telefonoa != null ? telefonoa : ""; }
    public void setTelefonoa(String telefonoa) { this.telefonoa = telefonoa; }

    public String getEmaila() { return emaila != null ? emaila : ""; }
    public void setEmaila(String emaila) { this.emaila = emaila; }

    public String getHelbidea() { return helbidea != null ? helbidea : ""; }
    public void setHelbidea(String helbidea) { this.helbidea = helbidea; }

    public boolean isAktibo() { return aktibo; }
    public void setAktibo(boolean aktibo) { this.aktibo = aktibo; }

    public int getLehengaiKopurua() { return lehengaiKopurua; }
    public void setLehengaiKopurua(int lehengaiKopurua) { this.lehengaiKopurua = lehengaiKopurua; }

    @Override
    public String toString() {
        if (!getProduktuMota().isBlank()) {
            return getIzena() + " — " + getProduktuMota();
        }
        return getIzena();
    }
}
