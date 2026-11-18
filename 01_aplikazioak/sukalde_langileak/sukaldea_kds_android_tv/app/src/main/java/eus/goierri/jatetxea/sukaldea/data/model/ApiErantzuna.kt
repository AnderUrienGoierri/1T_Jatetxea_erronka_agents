package eus.goierri.jatetxea.sukaldea.data.model

import com.google.gson.annotations.SerializedName

data class ApiErantzuna<T>(
    @SerializedName("success")
    val success: Boolean = true,
    @SerializedName("message")
    val message: String? = null,
    @SerializedName("data")
    val data: T? = null,
    @SerializedName("dataList")
    val dataList: List<T>? = null,
    @SerializedName("value")
    val valueList: List<T>? = null
)

data class EgoeraUpdateRequest(
    @SerializedName("egoera")
    val egoera: String
)
