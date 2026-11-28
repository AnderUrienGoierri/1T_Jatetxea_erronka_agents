package eus.goierri.jatetxea.admin.model;

public final class Hornitzailea {
    public int id;
    public String izena;
    public String cif;
    public String produktuMota;
    public double bezEhunekoa = 10.0;
    public String telefonoa;
    public String emaila;
    public String helbidea;
    public boolean aktibo = true;
    public int lehengaiKopurua;

    @Override
    public String toString() {
        if (produktuMota != null && !produktuMota.isBlank()) {
            return izena + " — " + produktuMota;
        }
        return izena != null ? izena : "";
    }
}
