package eus.goierri.jatetxea.sukaldea.data.api

import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

object RetrofitClient {

    // Lehenetsia: 10.0.2.2 emuladoretik PC ostalariaren (port 5000) atzitzeko
    var serverBaseUrl: String = "http://10.0.2.2:5000/api/v1/"
        set(value) {
            val normalized = if (value.endsWith("/")) value else "$value/"
            field = normalized
            apiService = sortuService(normalized)
        }

    private val loggingInterceptor = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val okHttpClient = OkHttpClient.Builder()
        .connectTimeout(5, TimeUnit.SECONDS)
        .readTimeout(8, TimeUnit.SECONDS)
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
