package eus.goierri.autoak.model;

import java.util.List;

/**
 * Kontzesionarioko auto baten domeinu-eredua eta bere pertsonalizazio-aukerak.
 */
public record Autoa(
        String id,
        String marka,
        String modeloa,
        String karrozeria,
        String erregaia,
        int oinarrizkoPrezioa,
        int plazak,
        int ateak,
        List<String> erabilerak,
        String neurriak,
        int maletategiaL,
        String irudiFitxategia,
        String deskribapena,
        List<MotorBertsioa> motorrak,
        List<KoloreAukera> koloreak,
        List<AkaberaAukera> akaberak,
        List<EkipamenduGehigarria> gehigarriak
) {
    public String getIzenOsoa() {
        return marka + " " + modeloa;
    }

    public MotorBertsioa getMotorLehenetsia() {
        return motorrak.get(0);
    }

    public KoloreAukera getKoloreLehenetsia() {
        return koloreak.get(0);
    }

    public AkaberaAukera getAkaberaLehenetsia() {
        return akaberak.get(0);
    }

    public record MotorBertsioa(
            String id,
            String izena,
            String erregaia,
            int potentziaCv,
            String kontsumoa,
            int prezioGehigarria
    ) {
        public String formatLaburra() {
            String plus = prezioGehigarria == 0 ? "Barne" : "+" + AutoKonfigurazioa.formatEuro(prezioGehigarria);
            return izena + " (" + potentziaCv + " CV · " + kontsumoa + ")  [" + plus + "]";
        }
    }

    public record KoloreAukera(
            String izena,
            String hexKodea,
            int prezioGehigarria
    ) {
    }

    public record AkaberaAukera(
            String izena,
            String deskribapena,
            String hexKodea,
            int prezioGehigarria
    ) {
    }

    public record EkipamenduGehigarria(
            String id,
            String izena,
            String deskribapena,
            int prezioa
    ) {
    }
}
