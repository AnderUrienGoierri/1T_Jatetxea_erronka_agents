package eus.goierri.jatetxea.komandera.data.api

import eus.goierri.jatetxea.komandera.data.model.Eskaera
import eus.goierri.jatetxea.komandera.data.model.Langilea
import eus.goierri.jatetxea.komandera.data.model.Mahaia
import eus.goierri.jatetxea.komandera.data.model.Platera
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import java.util.concurrent.TimeUnit

interface JatetxeaApiService {
    @GET("langileak")
    suspend fun getLangileak(): Response<List<Langilea>>

    @GET("mahaiak")
    suspend fun getMahaiak(): Response<List<Mahaia>>

    @GET("platerak")
    suspend fun getPlaterak(): Response<List<Platera>>

    @GET("eskaerak/mahaia/{mahaiId}")
    suspend fun getMahaiarenEskaera(@Path("mahaiId") mahaiId: Int): Response<Eskaera>

    @POST("eskaerak")
    suspend fun sortuEskaera(@Body eskaera: Eskaera): Response<Eskaera>
}

object ApiClient {
    // Lehenetsitako API URL-a (10.0.2.2 Android emuladoreak ostalarira konektatzeko, edo tokiko sareko IPa)
    var baseUrl: String = "http://10.0.2.2:5050/api/v1/"

    private val logging = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val okHttpClient = OkHttpClient.Builder()
        .addInterceptor(logging)
        .connectTimeout(5, TimeUnit.SECONDS)
        .readTimeout(10, TimeUnit.SECONDS)
        .build()

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
