package eus.goierri.jatetxea.sukaldea.data.model

import com.google.gson.annotations.SerializedName

data class Eskaera(
    @SerializedName("id")
    val id: Int,
    @SerializedName("mahaia_zenbakia")
    val mahaiaZenbakia: Int? = null,
    @SerializedName("mahaiaTestua")
    val mahaiaTestua: String? = null,
    @SerializedName("langilea_izena")
    val langileaIzena: String? = null,
    @SerializedName("langileIzena")
    val langileIzenaAlt: String? = null,
    @SerializedName("egoera")
    val egoera: String = "Sukaldean",
    @SerializedName("sortze_data")
    val sortzeData: String? = null,
    @SerializedName("sukaldekoPlaterKopurua")
    val sukaldekoPlaterKopurua: Int = 0,
    @SerializedName("lerroak")
    val lerroak: List<EskaeraLerroa> = emptyList()
) {
    val mahaiaErakutsi: String
        get() = mahaiaTestua ?: (if (mahaiaZenbakia != null) "Mahaia $mahaiaZenbakia" else "Mahaia -")

    val langileaErakutsi: String
        get() = langileaIzena ?: langileIzenaAlt ?: "Zerbitzaria"

    val orduaErakutsi: String
        get() = sortzeData?.substringAfter("T")?.take(5) ?: (sortzeData ?: "")
}
