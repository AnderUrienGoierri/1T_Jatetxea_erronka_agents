package eus.goierri.jatetxea.admin.controller;

/**
 * Gerente aplikazioko pantaila freskagarri guztiek betetzen duten interfazea (OOP polimorfismoa).
 */
public interface PantailaFreskagarria {
    void refresh();

    String getPantailaIzena();

    default void garbitu() {
        // Tenporizadore edo baliabide aktiboak dituzten pantailek gainidatzi dezakete
    }
}
