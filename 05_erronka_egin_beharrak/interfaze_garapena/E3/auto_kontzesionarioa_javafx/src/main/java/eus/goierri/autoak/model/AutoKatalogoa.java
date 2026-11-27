package eus.goierri.autoak.model;

import eus.goierri.autoak.model.Autoa.AkaberaAukera;
import eus.goierri.autoak.model.Autoa.EkipamenduGehigarria;
import eus.goierri.autoak.model.Autoa.KoloreAukera;
import eus.goierri.autoak.model.Autoa.MotorBertsioa;

import java.util.List;

/**
 * Kontzesionarioko demo-katalogoa: auto errealak eta datu orientagarriak.
 */
public final class AutoKatalogoa {
    private AutoKatalogoa() {
    }

    public static List<Autoa> sortuKatalogoa() {
        List<KoloreAukera> koloreEstandarrak = List.of(
                new KoloreAukera("Zuri Polar", "#F8FAFC", 0),
                new KoloreAukera("Gris Titanio", "#64748B", 650),
                new KoloreAukera("Beltz Obsidiana", "#0F172A", 750),
                new KoloreAukera("Urdin Atlantikoa", "#1D4ED8", 850),
                new KoloreAukera("Gorri Chianti", "#9F1239", 950)
        );

        List<AkaberaAukera> akaberaEstandarrak = List.of(
                new AkaberaAukera("Oihal Iluna Essential", "Eserleku ergonomikoak oihal iraunkor ilunean", "#334155", 0),
                new AkaberaAukera("Larru Sintetikoa Gris", "Jostura kontrastatuak eta aginte-panel leuna", "#64748B", 1100),
                new AkaberaAukera("Larru Naturala Beltza", "Premium larru zulatua eta akabera kromatuak", "#1E293B", 2200),
                new AkaberaAukera("Alcantara Kirol", "Euskarri laterala eta karbono itxurako xehetasunak", "#475569", 2600)
        );

        List<EkipamenduGehigarria> gehigarriEstandarrak = List.of(
                new EkipamenduGehigarria("gps", "GPS Nabigatzaile Pro 12\"", "Mapa eguneratuak, trafikoa denbora errealean eta Apple CarPlay / Android Auto", 950),
                new EkipamenduGehigarria("roof", "Teilatu Panoramiko Irekia", "Kristal tintatu elektrikoa eguzki-babeslearekin", 1350),
                new EkipamenduGehigarria("cam360", "360° Aparkatzeko Kamera", "Aurreko, atzeko eta alboko ikuspegi panoramikoa + sentsoreak", 780),
                new EkipamenduGehigarria("heated", "Larruzko Eserleku Berotuak", "Aurreko eserleku eta bolante berotuak negu hotzerako", 1200),
                new EkipamenduGehigarria("audio", "Soinu Sistema Premium (10 bozgorailu)", "Subwoofer aktiboa eta zarata-ezabatze akustikoa", 890),
                new EkipamenduGehigarria("adas", "Gidatze Laguntzaile Paketea", "Abiadura-kontrol moldagarria, errei-mantentzea eta angelu hilaren alerta", 1150)
        );

        return List.of(
                new Autoa(
                        "toyota-yaris", "Toyota", "Yaris Hybrid", "Trinkoa", "Hibridoa",
                        22900, 5, 5, List.of("Hiria", "Bidaiak"),
                        "3.940 × 1.745 × 1.500 mm", 286, "toyota-yaris.jpg",
                        "Hiri-gidatzerako erreferente hibridoa: kontsumo oso baxua, aparkatzeko erraztasuna eta ECO etiketa.",
                        List.of(
                                new MotorBertsioa("y115", "115H Active e-CVT", "Hibridoa", 116, "3,9 L/100km", 0),
                                new MotorBertsioa("y130", "130H GR Sport e-CVT", "Hibridoa", 130, "4,2 L/100km", 2800)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "toyota-rav4", "Toyota", "RAV4 Hybrid", "SUV", "Hibridoa",
                        38500, 5, 5, List.of("Familia", "Bidaiak"),
                        "4.600 × 1.855 × 1.685 mm", 580, "toyota-rav4.jpg",
                        "SUV familiar zabal eta fidagarria, maletategi handiarekin eta bidaia luzeetarako erosotasun apartarekin.",
                        List.of(
                                new MotorBertsioa("r220", "220H 4x2 Advance", "Hibridoa", 218, "5,6 L/100km", 0),
                                new MotorBertsioa("r220awd", "220H AWD-i Style", "Hibridoa", 222, "5,8 L/100km", 3400),
                                new MotorBertsioa("r300phev", "300PH Plug-in Hybrid AWD", "Hibridoa", 306, "1,0 L/100km", 8900)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "vw-golf", "Volkswagen", "Golf VIII", "Trinkoa", "Gasolina",
                        27400, 5, 5, List.of("Hiria", "Bidaiak", "Lana"),
                        "4.284 × 1.789 × 1.491 mm", 381, "vw-golf.jpg",
                        "Trinko orekatu eta moldakorra: kalitate handiko barrualdea, teknologia digitala eta errepideko egonkortasuna.",
                        List.of(
                                new MotorBertsioa("g115", "1.5 TSI 115 CV Manual", "Gasolina", 115, "5,4 L/100km", 0),
                                new MotorBertsioa("g150", "1.5 eTSI 150 CV DSG", "Gasolina", 150, "5,6 L/100km", 3200),
                                new MotorBertsioa("g265", "2.0 TSI GTI 265 CV DSG", "Gasolina", 265, "7,1 L/100km", 11500)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "tesla-model3", "Tesla", "Model 3 Highland", "Berlina", "Elektrikoa",
                        39990, 5, 5, List.of("Bidaiak", "Lana", "Hiria"),
                        "4.720 × 1.850 × 1.441 mm", 594, "tesla-model3.jpg",
                        "Berlina %100 elektrikoa autonomia handiarekin, karga azkarrarekin eta gidatze minimalista eta isilarekin.",
                        List.of(
                                new MotorBertsioa("tm3-rwd", "Trakzio Atzekoa (513 km WLTP)", "Elektrikoa", 283, "13,2 kWh/100km", 0),
                                new MotorBertsioa("tm3-lr", "Autonomia Handia AWD (629 km)", "Elektrikoa", 498, "14,0 kWh/100km", 8500),
                                new MotorBertsioa("tm3-perf", "Performance AWD (0-100: 3,1s)", "Elektrikoa", 627, "16,5 kWh/100km", 15500)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "bmw-320d", "BMW", "Serie 3 Touring", "Familiarra", "Diesela",
                        48900, 5, 5, List.of("Familia", "Bidaiak", "Lana"),
                        "4.713 × 1.827 × 1.440 mm", 500, "bmw-320d.jpg",
                        "Familiarra kirol-sena eta kilometro luzeetarako diesel eraginkortasun gorena uztartzen dituena.",
                        List.of(
                                new MotorBertsioa("b318d", "318d 150 CV Steptronic", "Diesela", 150, "4,9 L/100km", 0),
                                new MotorBertsioa("b320d", "320d xDrive 190 CV", "Diesela", 190, "5,2 L/100km", 4600),
                                new MotorBertsioa("b330d", "330d xDrive 286 CV M-Sport", "Diesela", 286, "5,9 L/100km", 10900)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "kia-ev6", "Kia", "EV6", "SUV", "Elektrikoa",
                        44900, 5, 5, List.of("Familia", "Bidaiak"),
                        "4.680 × 1.880 × 1.550 mm", 490, "kia-ev6.jpg",
                        "800V arkitekturako crossover elektrikoa: 18 minututan %10etik %80ra kargatzen da eta barne-espazio zabala du.",
                        List.of(
                                new MotorBertsioa("ev6-170", "Air 58 kWh RWD (394 km)", "Elektrikoa", 170, "16,6 kWh/100km", 0),
                                new MotorBertsioa("ev6-229", "GT-Line 77 kWh RWD (528 km)", "Elektrikoa", 229, "16,5 kWh/100km", 5200),
                                new MotorBertsioa("ev6-325", "GT-Line 77 kWh AWD", "Elektrikoa", 325, "18,0 kWh/100km", 9400)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "peugeot-5008", "Peugeot", "5008 Family 7P", "SUV", "Diesela",
                        36800, 7, 5, List.of("Familia", "Bidaiak"),
                        "4.641 × 1.844 × 1.646 mm", 780, "peugeot-5008.jpg",
                        "7 plazako SUV modularra familia handientzat: 3 eserleku independente bigarren ilaran eta maletategi erraldoia.",
                        List.of(
                                new MotorBertsioa("p130d", "BlueHDi 130 CV EAT8", "Diesela", 130, "5,3 L/100km", 0),
                                new MotorBertsioa("p180d", "BlueHDi 180 CV GT EAT8", "Diesela", 180, "5,8 L/100km", 4200)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "seat-ibiza", "Seat", "Ibiza FR", "Trinkoa", "Gasolina",
                        19800, 5, 5, List.of("Hiria"),
                        "4.059 × 1.780 × 1.444 mm", 355, "seat-ibiza.jpg",
                        "Hiri-auto arin, ekonomiko eta dinamikoa, eguneroko joan-etorrietarako eta gazteentzat aproposa.",
                        List.of(
                                new MotorBertsioa("s95", "1.0 TSI 95 CV Style", "Gasolina", 95, "5,1 L/100km", 0),
                                new MotorBertsioa("s115", "1.0 TSI 115 CV FR DSG", "Gasolina", 115, "5,4 L/100km", 2600),
                                new MotorBertsioa("s150", "1.5 TSI 150 CV FR DSG", "Gasolina", 150, "5,8 L/100km", 4500)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "ford-transit", "Ford", "Transit Custom", "Furgoneta", "Diesela",
                        33900, 3, 5, List.of("Lana"),
                        "5.050 × 2.032 × 1.968 mm", 5800, "ford-transit.jpg",
                        "Lan profesionalerako furgoneta erresistentea: karga-bolumen handia, kabina ergonomikoa eta mantentze merkea.",
                        List.of(
                                new MotorBertsioa("f136", "2.0 EcoBlue 136 CV Trend", "Diesela", 136, "7,1 L/100km", 0),
                                new MotorBertsioa("f170", "2.0 EcoBlue 170 CV Auto Limited", "Diesela", 170, "7,5 L/100km", 4100)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "mercedes-c", "Mercedes-Benz", "Clase C Berlina", "Berlina", "Hibridoa",
                        53400, 5, 4, List.of("Bidaiak", "Lana"),
                        "4.751 × 1.820 × 1.437 mm", 455, "mercedes-c.jpg",
                        "Gama altuko berlina dotorea: MBUX pantaila bertikala, erosotasun akustiko bikaina eta mikro-hibridazioa.",
                        List.of(
                                new MotorBertsioa("mc200", "C 200 EQ Boost 204 CV", "Hibridoa", 204, "6,2 L/100km", 0),
                                new MotorBertsioa("mc300e", "C 300 e Plug-in Hybrid 313 CV", "Hibridoa", 313, "0,6 L/100km", 7800)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "toyota-corolla", "Toyota", "Corolla Hybrid", "Trinkoa", "Hibridoa",
                        28900, 5, 5, List.of("Hiria", "Familia", "Bidaiak"),
                        "4.370 × 1.790 × 1.435 mm", 361, "toyota-corolla.jpg",
                        "Trinko hibrido erosoa, eguneroko ibilbideetarako zein bidaietarako.",
                        List.of(
                                new MotorBertsioa("c140", "140H e-CVT", "Hibridoa", 140, "4,5 L/100km", 0),
                                new MotorBertsioa("c200", "200H e-CVT", "Hibridoa", 196, "4,7 L/100km", 3500)
                        ),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "hyundai-tucson", "Hyundai", "Tucson Plug-in Hybrid", "SUV", "Hibridoa",
                        42900, 5, 5, List.of("Familia", "Bidaiak"),
                        "4.500 × 1.865 × 1.650 mm", 558, "hyundai-tucson.jpg",
                        "SUV hibrido entxufagarria, familiarekin bidaiatzeko espazio zabalarekin.",
                        List.of(new MotorBertsioa("tucson-phev", "1.6 T-GDi Plug-in Hybrid", "Hibridoa", 265, "1,4 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "renault-clio", "Renault", "Clio", "Trinkoa", "Gasolina",
                        18900, 5, 5, List.of("Hiria"),
                        "4.050 × 1.798 × 1.440 mm", 391, "renault-clio.jpg",
                        "Hiri-auto trinkoa, aparkatzeko erraza eta eguneroko erabilerarako egokia.",
                        List.of(new MotorBertsioa("clio-tce", "TCe 90", "Gasolina", 90, "5,2 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "dacia-sandero", "Dacia", "Sandero", "Trinkoa", "Gasolina",
                        15900, 5, 5, List.of("Hiria", "Familia"),
                        "4.088 × 1.848 × 1.499 mm", 328, "dacia-sandero.jpg",
                        "Auto praktiko eta ekonomikoa, bost plazarekin eta maletategi erabilgarriarekin.",
                        List.of(new MotorBertsioa("sandero-tce", "TCe 90", "Gasolina", 90, "5,3 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "nissan-qashqai", "Nissan", "Qashqai", "SUV", "Hibridoa",
                        33900, 5, 5, List.of("Familia", "Bidaiak"),
                        "4.425 × 1.835 × 1.625 mm", 504, "nissan-qashqai.jpg",
                        "SUV familiarra, espazio eta erosotasun orekatuekin.",
                        List.of(new MotorBertsioa("qashqai-mhev", "1.3 Mild Hybrid", "Hibridoa", 140, "6,3 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "hyundai-ioniq5", "Hyundai", "IONIQ 5", "SUV", "Elektrikoa",
                        45900, 5, 5, List.of("Familia", "Bidaiak"),
                        "4.635 × 1.890 × 1.605 mm", 527, "hyundai-ioniq5.jpg",
                        "Crossover elektrikoa, barrualde zabalarekin eta karga azkarreko aukerarekin.",
                        List.of(new MotorBertsioa("ioniq5-rwd", "77 kWh RWD", "Elektrikoa", 229, "16,8 kWh/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "renault-kangoo", "Renault", "Kangoo", "Furgoneta", "Diesela",
                        27900, 5, 5, List.of("Familia", "Lana"),
                        "4.486 × 1.860 × 1.838 mm", 775, "renault-kangoo.jpg",
                        "Furgoneta praktikoa, ate lerragarriekin eta kargarako espazio zabalarekin.",
                        List.of(new MotorBertsioa("kangoo-dci", "Blue dCi 95", "Diesela", 95, "5,4 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "skoda-octavia", "Skoda", "Octavia Combi RS", "Familiarra", "Gasolina",
                        39900, 5, 5, List.of("Familia", "Bidaiak", "Lana"),
                        "4.702 × 1.829 × 1.467 mm", 640, "skoda-octavia.jpg",
                        "Familiar kirolaria, maletategi handiarekin eta bidaia luzeetarako erosotasunarekin.",
                        List.of(new MotorBertsioa("octavia-rs", "2.0 TSI RS", "Gasolina", 245, "7,0 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "volvo-xc60", "Volvo", "XC60", "SUV", "Hibridoa",
                        57900, 5, 5, List.of("Familia", "Bidaiak"),
                        "4.708 × 1.902 × 1.658 mm", 483, "volvo-xc60.jpg",
                        "SUV erosoa, familiarekin bidaiatzeko eta eguneroko erabilerarako.",
                        List.of(new MotorBertsioa("xc60-b5", "B5 Mild Hybrid", "Hibridoa", 250, "7,4 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                ),
                new Autoa(
                        "citroen-berlingo", "Citroen", "Berlingo", "Furgoneta", "Diesela",
                        26900, 5, 5, List.of("Familia", "Lana"),
                        "4.403 × 1.848 × 1.844 mm", 775, "citroen-berlingo.jpg",
                        "Familia eta lanerako furgoneta praktikoa, ate lerragarriekin.",
                        List.of(new MotorBertsioa("berlingo-d", "BlueHDi 100", "Diesela", 100, "5,4 L/100km", 0)),
                        koloreEstandarrak, akaberaEstandarrak, gehigarriEstandarrak
                )
        );
    }

    public static List<String> lortuMarkak(List<Autoa> katalogoa) {
        return katalogoa.stream().map(Autoa::marka).distinct().sorted().toList();
    }
}
