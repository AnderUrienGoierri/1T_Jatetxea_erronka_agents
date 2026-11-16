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

    @POST("ordainketak")
    suspend fun prozesatuOrdainketa(@Body req: OrdainketaEskaeraDto): Response<ApiResponse<OrdainketaErantzunaDto>>
}

object ApiClient {
    // 10.0.2.2 Android emuladoreak ordenagailu ostalarira sartzeko (portu 5050)
    var baseUrl: String = "http://10.0.2.2:5050/api/v1/"

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

    val service: JatetxeaApiService by lazy {
        createService()
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
