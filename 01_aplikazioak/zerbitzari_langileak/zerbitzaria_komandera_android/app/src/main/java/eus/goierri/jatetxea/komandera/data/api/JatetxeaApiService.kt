package eus.goierri.jatetxea.komandera.data.api

import eus.goierri.jatetxea.komandera.data.model.*
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.*
import java.util.concurrent.TimeUnit

interface JatetxeaApiService {
    @GET("langileak")
    suspend fun getLangileak(): Response<ApiResponse<LangileaDto>>

    @POST("langileak/login")
    suspend fun login(@Body req: LoginRequestDto): Response<ApiResponse<LoginEmaitzaDto>>

    @GET("mahaiak")
    suspend fun getMahaiak(): Response<ApiResponse<MahaiaDto>>

    @PUT("mahaiak/{id}/egoera")
    suspend fun eguneratuMahaiarenEgoera(
        @Path("id") id: Int,
        @Body req: EgoeraEguneratuRequest
    ): Response<ApiResponse<Any>>

    @GET("platerak")
    suspend fun getPlaterak(@Query("kategoria") kategoria: String? = null): Response<ApiResponse<PlateraDto>>

    @GET("platerak/erabilgarritasuna")
    suspend fun getPlaterenStockAhalmena(): Response<ApiResponse<ErabilgarritasunaDto>>

    @GET("eskaerak")
    suspend fun getEskaerakMahaia(@Query("mahaiaId") mahaiaId: Int): Response<ApiResponse<EskaeraDto>>

    @GET("eskaerak/{id}")
    suspend fun getEskaeraXehetasunak(@Path("id") id: Int): Response<ApiResponse<EskaeraDto>>

    @POST("eskaerak")
    suspend fun sortuEskaera(@Body req: SortuEskaeraRequest): Response<ApiResponse<EskaeraDto>>

    @POST("eskaerak/{id}/lerroak")
    suspend fun gehituPlatera(
        @Path("id") eskaeraId: Int,
        @Body req: GehituPlateraRequest
    ): Response<ApiResponse<Any>>

    @POST("eskaerak/{id}/sukaldera")
    suspend fun bidaliSukaldera(@Path("id") eskaeraId: Int): Response<ApiResponse<Any>>

    @PUT("eskaerak/lerroak/{lerroId}/egoera")
    suspend fun eguneratuLerroEgoera(
        @Path("lerroId") lerroId: Int,
        @Body req: EgoeraEguneratuRequest
    ): Response<ApiResponse<Any>>

    @PUT("eskaerak/{id}/egoera")
    suspend fun eguneratuEskaerarenEgoera(
        @Path("id") eskaeraId: Int,
        @Body req: EgoeraEguneratuRequest
    ): Response<ApiResponse<Any>>

    @DELETE("eskaerak/lerroak/{lerroId}")
    suspend fun ezabatuLerroa(@Path("lerroId") lerroId: Int): Response<ApiResponse<Any>>

    @DELETE("eskaerak/{id}")
    suspend fun ezabatuEskaera(@Path("id") eskaeraId: Int): Response<ApiResponse<Any>>

    @POST("ordainketak")
    suspend fun prozesatuOrdainketa(@Body req: OrdainketaEskaeraDto): Response<ApiResponse<OrdainketaErantzunaDto>>

    @GET("erreserbak")
    suspend fun getErreserbak(
        @Query("data") data: String? = null,
        @Query("txanda") txanda: String? = null
    ): Response<ApiResponse<ErreserbaDto>>

    @POST("erreserbak")
    suspend fun sortuErreserba(@Body req: ErreserbaSortuDto): Response<ApiResponse<ErreserbaDto>>

    @GET("health")
    suspend fun getHealth(): Response<Map<String, Any>>
}

object ApiClient {
    private const val PREFS_NAME = "komandera_prefs"
    private const val KEY_BASE_URL = "api_base_url"

    // Lehenetsia: mDNS (ANDER-SGB5PRO.local:5050) — IP aldakorrik gabe
    var baseUrl: String = "http://ANDER-SGB5PRO.local:5050/api/v1/"

    fun init(context: android.content.Context) {
        val prefs = context.getSharedPreferences(PREFS_NAME, android.content.Context.MODE_PRIVATE)
        val gordetakoUrl = prefs.getString(KEY_BASE_URL, null)
        val networkProfiles = eus.goierri.jatetxea.komandera.data.network.NetworkProfiles
        val recommendedProfile = networkProfiles.lortuGomendatutakoProfila(context)
        val savedUrl = gordetakoUrl?.trimEnd('/')
        val zaharkitutakoUrlak = setOf(
            "http://192.168.5.166:5050/api/v1",
            "http://10.100.30.101:5050/api/v1",
            "http://172.16.243.221:5050/api/v1",
            "http://192.168.1.134:5050/api/v1"
        )
        val daZaharkitua = savedUrl in zaharkitutakoUrlak ||
            (savedUrl == "http://10.0.2.2:5050/api/v1" && recommendedProfile.id != "emuladorea")

        if (gordetakoUrl.isNullOrBlank() || daZaharkitua) {
            baseUrl = recommendedProfile.baseUrl
            prefs.edit().putString(KEY_BASE_URL, baseUrl).apply()
        } else {
            baseUrl = garbituUrl(gordetakoUrl)
        }
    }

    fun saveBaseUrl(context: android.content.Context, url: String) {
        baseUrl = garbituUrl(url)
        cachedUrl = null
        cachedService = null
        val prefs = context.getSharedPreferences(PREFS_NAME, android.content.Context.MODE_PRIVATE)
        prefs.edit().putString(KEY_BASE_URL, baseUrl).apply()
    }

    private fun garbituUrl(url: String): String {
        var garbia = url.trim()
        if (!garbia.endsWith("/")) {
            garbia = "$garbia/"
        }
        return garbia
    }

    private val logging = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val okHttpClient = OkHttpClient.Builder()
        .addInterceptor { chain ->
            val request = chain.request().newBuilder()
                .header("X-App-Id", "android_komandera")
                .header("X-App-Name", "Android Zerbitzaria (Komandera)")
                .header("User-Agent", "1T-Jatetxea-Android-Komandera/1.0 (Android; Jetpack Compose)")
                .build()
            chain.proceed(request)
        }
        .addInterceptor(logging)
        .connectTimeout(5, TimeUnit.SECONDS)
        .readTimeout(10, TimeUnit.SECONDS)
        .build()

    @Volatile
    private var cachedUrl: String? = null

    @Volatile
    private var cachedService: JatetxeaApiService? = null

    val service: JatetxeaApiService
        get() {
            val currentUrl = baseUrl
            val existing = cachedService
            if (existing != null && cachedUrl == currentUrl) {
                return existing
            }
            return synchronized(this) {
                if (cachedService != null && cachedUrl == currentUrl) {
                    cachedService!!
                } else {
                    createService(currentUrl).also {
                        cachedUrl = currentUrl
                        cachedService = it
                    }
                }
            }
        }

    fun createService(url: String = baseUrl): JatetxeaApiService {
        val cleanUrl = if (url.endsWith("/")) url else "$url/"
        return Retrofit.Builder()
            .baseUrl(cleanUrl)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(JatetxeaApiService::class.java)
    }
}
