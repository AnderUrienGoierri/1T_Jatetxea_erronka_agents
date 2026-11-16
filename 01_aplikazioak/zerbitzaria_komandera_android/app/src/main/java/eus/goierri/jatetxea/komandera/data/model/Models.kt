package eus.goierri.jatetxea.komandera.data.model

import com.google.gson.annotations.SerializedName

data class ApiResponse<T>(
    @SerializedName("status") val status: String? = null,
    @SerializedName("code") val code: Int? = null,
    @SerializedName("message") val message: String? = null,
    @SerializedName("data") val data: T? = null,
    @SerializedName("dataList") val dataList: List<T>? = null
)

data class LangileaDto(
    @SerializedName("id") val id: Int,
    @SerializedName("erabiltzailea") val erabiltzailea: String = "",
    @SerializedName("izena") val izena: String = "",
    @SerializedName("rola") val rola: String = "Zerbitzaria",
    @SerializedName("aktibo") val aktibo: Boolean = true
)

data class LoginRequestDto(
    @SerializedName("erabiltzailea") val erabiltzailea: String,
    @SerializedName("pasahitza") val pasahitza: String
)

data class LoginEmaitzaDto(
    @SerializedName("arrakasta") val arrakasta: Boolean = false,
    @SerializedName("mezua") val mezua: String = "",
    @SerializedName("langilea") val langilea: LangileaDto? = null
)

data class MahaiaDto(
    @SerializedName("id") val id: Int,
    @SerializedName("zenbakia") val zenbakia: Int,
    @SerializedName("edukiera") val edukiera: Int = 4,
    @SerializedName("kokapena") val kokapena: String = "Jantoki Nagusia", // Jantoki Nagusia, Barra, Terraza, VIP
    @SerializedName("egoera") val egoera: String = "Libre", // Libre, Okupatuta, Erreserbatuta, Garbitzen
    var mahaikideak: Int? = null,
    var guztira: Double = 0.0,
    var aktiboEskaeraId: Int? = null
)

data class PlateraDto(
    @SerializedName("id") val id: Int,
    @SerializedName("izena") val izena: String = "",
    @SerializedName("deskribapena") val deskribapena: String? = null,
    @SerializedName("kategoria") val kategoria: String = "Bigarrena",
    @SerializedName("prezioa") val prezioa: Double = 0.0,
    @SerializedName("alergenoak") val alergenoak: String? = null,
    @SerializedName("prestatzeDenboraMin") val prestatzeDenboraMin: Int = 15,
    @SerializedName("aktibo") val aktibo: Boolean = true,
    var stockAhalmena: Int = 999
)

data class ErabilgarritasunaDto(
    @SerializedName("plateraId") val plateraId: Int,
    @SerializedName("plateraIzena") val plateraIzena: String = "",
    @SerializedName("eginDaitezkeenPlaterak") val eginDaitezkeenPlaterak: Int = 0
)

data class EskaeraDto(
    @SerializedName("id") val id: Int = 0,
    @SerializedName("mahaiaId") val mahaiaId: Int = 0,
    @SerializedName("mahaiZenbakia") val mahaiZenbakia: Int = 0,
    @SerializedName("langileaId") val langileaId: Int = 0,
    @SerializedName("langileIzena") val langileIzena: String = "",
    @SerializedName("pertsonaKopurua") val pertsonaKopurua: Int = 2,
    @SerializedName("egoera") val egoera: String = "Irekia", // Irekia, Sukaldean, Zerbitzatua, Kobratua
    @SerializedName("guztira") val guztira: Double = 0.0,
    @SerializedName("sortzeData") val sortzeData: String = "",
    @SerializedName("lerroak") val lerroak: List<EskaeraLerroaDto> = emptyList()
)

data class EskaeraLerroaDto(
    @SerializedName("id") val id: Int = 0,
    @SerializedName("eskaeraId") val eskaeraId: Int = 0,
    @SerializedName("plateraId") val plateraId: Int = 0,
    @SerializedName("platerIzena") val platerIzena: String = "",
    @SerializedName("kategoria") val kategoria: String = "",
    @SerializedName("kantitatea") val kantitatea: Int = 1,
    @SerializedName("unitatePrezioa") val unitatePrezioa: Double = 0.0,
    @SerializedName("egoera") val egoera: String = "Irekia", // Irekia, Sukaldean, Prestatua, Zerbitzatua
    @SerializedName("oharrak") val oharrak: String? = null
) {
    val azpitotala: Double get() = unitatePrezioa * kantitatea
}

data class SortuEskaeraRequest(
    @SerializedName("mahaiaId") val mahaiaId: Int,
    @SerializedName("langileaId") val langileaId: Int,
    @SerializedName("pertsonaKopurua") val pertsonaKopurua: Int
)

data class GehituPlateraRequest(
    @SerializedName("plateraId") val plateraId: Int,
    @SerializedName("kantitatea") val kantitatea: Int,
    @SerializedName("oharrak") val oharrak: String? = null
)

data class EgoeraEguneratuRequest(
    @SerializedName("egoera") val egoera: String
)

data class OrdainketaEskaeraDto(
    @SerializedName("eskaeraId") val eskaeraId: Int,
    @SerializedName("mota") val mota: String, // Eskudirua, Txartela, Bizum
    @SerializedName("zenbatekoa") val zenbatekoa: Double,
    @SerializedName("jasotakoa") val jasotakoa: Double? = null,
    @SerializedName("txartelZenbakia") val txartelZenbakia: String? = null,
    @SerializedName("titularra") val titularra: String? = null,
    @SerializedName("iraungitzea") val iraungitzea: String? = null,
    @SerializedName("cvv") val cvv: String? = null,
    @SerializedName("bizumTelefonoa") val bizumTelefonoa: String? = null,
    @SerializedName("langileaId") val langileaId: Int? = null
)

data class OrdainketaErantzunaDto(
    @SerializedName("id") val id: Int = 0,
    @SerializedName("eskaeraId") val eskaeraId: Int = 0,
    @SerializedName("tiketZenbakia") val tiketZenbakia: String = "",
    @SerializedName("mota") val mota: String = "",
    @SerializedName("zenbatekoa") val zenbatekoa: Double = 0.0,
    @SerializedName("jasotakoa") val jasotakoa: Double? = null,
    @SerializedName("itzulitakoa") val itzulitakoa: Double? = null,
    @SerializedName("txartelMaskaratua") val txartelMaskaratua: String? = null,
    @SerializedName("operazioKodea") val operazioKodea: String = ""
)
