package eus.goierri.jatetxea.admin.model;

public final class TxatMezua {
    public int id;
    public int langileaId;
    public String langileIzena;
    public String erabiltzailea;
    public String rola;
    public String gela;
    public String testua;
    public String sortzeData;

    public String getOrduaLaburra() {
        if (sortzeData == null || sortzeData.length() < 16) {
            return sortzeData == null ? "" : sortzeData;
        }
        return sortzeData.substring(11, 16);
    }

    @Override
    public String toString() {
        String ordua = getOrduaLaburra();
        String igorlea = langileIzena == null || langileIzena.isBlank() ? erabiltzailea : langileIzena;
        String rolaTestua = rola == null || rola.isBlank() ? "" : " [" + rola + "]";
        return "[" + ordua + "] " + igorlea + rolaTestua + ": " + (testua == null ? "" : testua);
    }
}
