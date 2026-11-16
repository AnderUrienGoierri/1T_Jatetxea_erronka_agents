package eus.goierri.jatetxea.sukaldea.data.model

import com.google.gson.annotations.SerializedName

data class Eskaera(
    @SerializedName("id")
    val id: Int,
    @SerializedName("mahaia_id")
    val mahaiaId: Int? = null,
    @SerializedName("mahai_zenbakia")
    val mahaiZenbakia: Int? = null,
    @SerializedName("mahaia_zenbakia")
    val mahaiaZenbakia: Int? = null,
    @SerializedName("mahaiaTestua")
    val mahaiaTestua: String? = null,
    @SerializedName("langilea_id")
    val langileaId: Int? = null,
    @SerializedName("langilea_izena")
    val langileaIzena: String? = null,
    @SerializedName("langile_izena")
    val langileIzena: String? = null,
    @SerializedName("langileIzena")
    val langileIzenaAlt: String? = null,
    @SerializedName("egoera")
    val egoera: String = "Sukaldean",
    @SerializedName("sortze_data")
    val sortzeData: String? = null,
    @SerializedName("sortzeData")
    val sortzeDataAlt: String? = null,
    @SerializedName("sukaldekoPlaterKopurua")
    val sukaldekoPlaterKopurua: Int = 0,
    @SerializedName("lerroak")
    val lerroak: List<EskaeraLerroa> = emptyList()
) {
    val mahaiaErakutsi: String
        get() = mahaiaTestua ?: (if (mahaiZenbakia != null) "Mahaia $mahaiZenbakia" else if (mahaiaZenbakia != null) "Mahaia $mahaiaZenbakia" else "Mahaia -")

    val langileaErakutsi: String
        get() = langileIzena ?: langileaIzena ?: langileIzenaAlt ?: "Zerbitzaria"

    val orduaErakutsi: String
        get() {
            val d = sortzeData ?: sortzeDataAlt ?: ""
            return if (d.contains("T")) d.substringAfter("T").take(5)
            else if (d.contains(" ")) d.substringAfter(" ").take(5)
            else d
        }
}
