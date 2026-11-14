package eus.goierri.jatetxea.sukaldea.data.api

import eus.goierri.jatetxea.sukaldea.data.model.ApiErantzuna
import eus.goierri.jatetxea.sukaldea.data.model.EgoeraUpdateRequest
import eus.goierri.jatetxea.sukaldea.data.model.Eskaera
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.PUT
import retrofit2.http.Path

interface JatetxeaApiService {

    @GET("eskaerak")
    suspend fun lortuEskaerak(): ApiErantzuna<Eskaera>

    @GET("eskaerak/{id}")
    suspend fun lortuEskaeraXehetasunak(@Path("id") id: Int): ApiErantzuna<Eskaera>

    @PUT("eskaerak/lerroak/{id}/egoera")
    suspend fun eguneratuLerroEgoera(
        @Path("id") lerroId: Int,
        @Body body: EgoeraUpdateRequest
    ): Response<Unit>

    @PUT("eskaerak/{id}/egoera")
    suspend fun eguneratuEskaeraEgoera(
        @Path("id") eskaeraId: Int,
        @Body body: EgoeraUpdateRequest
    ): Response<Unit>
}
