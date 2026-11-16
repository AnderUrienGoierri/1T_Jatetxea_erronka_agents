package eus.goierri.jatetxea.sukaldea.data.model

import com.google.gson.annotations.SerializedName

data class EskaeraLerroa(
    @SerializedName("id")
    val id: Int,
    @SerializedName("eskaera_id")
    val eskaeraId: Int = 0,
    @SerializedName("platera_id")
    val plateraId: Int = 0,
    @SerializedName("plateraId")
    val plateraIdAlt: Int = 0,
    @SerializedName("plater_izena")
    val platerIzenaRaw: String? = null,
    @SerializedName("platerIzena")
    val platerIzenaAlt: String? = null,
    @SerializedName("kategoria")
    val kategoria: String? = null,
    @SerializedName("kantitatea")
    val kantitatea: Int = 1,
    @SerializedName("oharrak")
    val oharrak: String? = null,
    @SerializedName("egoera")
    val egoera: String = "Sukaldean"
) {
    val platerIzena: String
        get() = platerIzenaAlt ?: platerIzenaRaw ?: "Platera"

    val isEginda: Boolean
        get() = egoera.equals("Prestatua", ignoreCase = true) || egoera.equals("Eginda", ignoreCase = true)

    val isZerbitzatua: Boolean
        get() = egoera.equals("Zerbitzatua", ignoreCase = true)
}
