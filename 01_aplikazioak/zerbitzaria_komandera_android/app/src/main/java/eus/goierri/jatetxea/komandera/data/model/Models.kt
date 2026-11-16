package eus.goierri.jatetxea.komandera.data.model

import com.google.gson.annotations.SerializedName

data class Langilea(
    val id: Int,
    val izena: String,
    val abizena: String = "",
    val rola: String = "Zerbitzaria",
    val aktibo: Boolean = true
)

data class Mahaia(
    val id: Int,
    val zenbakia: Int,
    val izena: String,
    val kapazitatea: Int = 4,
    val egoera: String = "Libre", // Libre, Okupatuta, Eskaeran, Fakturatua
    val eremua: String = "Areto Nagusia",
    val unekoEskaeraZenb: Int? = null,
    val guztira: Double = 0.0
)

data class Platera(
    val id: Int,
    val izena: String,
    val deskribapena: String = "",
    val prezioa: Double,
    val kategoria: String = "Plater Nagusiak", // Hasierakoak, Plater Nagusiak, Postreak, Edariak
    val eskuragarri: Boolean = true
)

data class EskaeraLerroa(
    val id: Int = 0,
    val platerId: Int,
    val platerIzena: String,
    val kopurua: Int = 1,
    val prezioUnitarioa: Double,
    val oharrak: String = ""
) {
    val guztira: Double get() = kopurua * prezioUnitarioa
}

data class Eskaera(
    val id: Int = 0,
    val mahaiId: Int,
    val mahaiZenbakia: Int,
    val zerbitzariId: Int,
    val zerbitzariIzena: String,
    val egoera: String = "Bidalita", // Bidalita, Sukaldean, Prestatua, Zerbitzatua
    val lerroak: List<EskaeraLerroa> = emptyList(),
    val data: String = ""
) {
    val guztira: Double get() = lerroak.sumOf { it.guztira }
}
