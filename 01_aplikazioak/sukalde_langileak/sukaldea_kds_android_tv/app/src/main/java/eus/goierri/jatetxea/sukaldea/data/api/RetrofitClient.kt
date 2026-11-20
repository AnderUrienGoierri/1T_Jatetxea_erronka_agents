package eus.goierri.jatetxea.sukaldea.data.api

import android.content.Context
import android.content.SharedPreferences
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.net.URI
import java.util.concurrent.TimeUnit

object RetrofitClient {

    private const val PREFS_NAME = "sukaldea_kds_prefs"
    private const val KEY_BASE_URL = "api_base_url"

    // Lehenetsia telebista eta LAN sarerako: 172.16.243.102 (portua 5050)
    const val DEFAULT_HOST = "172.16.243.102"
    const val DEFAULT_PORT = 5050
    const val DEFAULT_URL = "http://$DEFAULT_HOST:$DEFAULT_PORT/api/v1/"

    private var sharedPrefs: SharedPreferences? = null

    var serverBaseUrl: String = DEFAULT_URL
        set(value) {
            val normalized = if (value.endsWith("/")) value else "$value/"
            field = normalized
            sharedPrefs?.edit()?.putString(KEY_BASE_URL, normalized)?.apply()
            apiService = sortuService(normalized)
        }

    fun hasieratu(context: Context) {
        sharedPrefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        val saved = sharedPrefs?.getString(KEY_BASE_URL, null)
        if (!saved.isNullOrBlank()) {
            serverBaseUrl = saved
        }
    }

    fun lortuHost(url: String = serverBaseUrl): String {
        return try {
            val uri = URI(url)
            uri.host ?: DEFAULT_HOST
        } catch (_: Exception) {
            DEFAULT_HOST
        }
    }

    fun lortuPort(url: String = serverBaseUrl): Int {
        return try {
            val uri = URI(url)
            if (uri.port != -1) uri.port else DEFAULT_PORT
        } catch (_: Exception) {
            DEFAULT_PORT
        }
    }

    fun eraikiUrl(host: String, port: Int): String {
        val garbia = host.trim()
            .removePrefix("http://")
            .removePrefix("https://")
            .trimEnd('/')
        val portua = if (port in 1..65535) port else DEFAULT_PORT
        return "http://$garbia:$portua/api/v1/"
    }

    private val loggingInterceptor = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val okHttpClient = OkHttpClient.Builder()
        .connectTimeout(5, TimeUnit.SECONDS)
        .readTimeout(8, TimeUnit.SECONDS)
        .addInterceptor { chain ->
            val request = chain.request().newBuilder()
                .header("X-App-Id", "android_tv")
                .header("X-App-Name", "Android TV Sukaldea KDS")
                .build()
            chain.proceed(request)
        }
        .addInterceptor(loggingInterceptor)
        .build()

    private fun sortuService(url: String): JatetxeaApiService {
        return Retrofit.Builder()
            .baseUrl(url)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(JatetxeaApiService::class.java)
    }

    var apiService: JatetxeaApiService = sortuService(serverBaseUrl)
        private set
}
