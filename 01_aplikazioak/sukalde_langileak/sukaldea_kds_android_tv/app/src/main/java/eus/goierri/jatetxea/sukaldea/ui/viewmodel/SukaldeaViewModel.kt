package eus.goierri.jatetxea.sukaldea.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import eus.goierri.jatetxea.sukaldea.data.api.RetrofitClient
import eus.goierri.jatetxea.sukaldea.data.model.EgoeraUpdateRequest
import eus.goierri.jatetxea.sukaldea.data.model.Eskaera
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

data class SukaldeaUiState(
    val isLoading: Boolean = false,
    val eskaerak: List<Eskaera> = emptyList(),
    val isOnline: Boolean = false,
    val errorea: String? = null,
    val azkenFreskatzea: String = "--:--:--",
    val unekoFiltroa: String = "Guztiak", // "Guztiak", "Sukaldean", "Prestatua"
    val serverUrl: String = RetrofitClient.serverBaseUrl
)

class SukaldeaViewModel : ViewModel() {

    private val _uiState = MutableStateFlow(SukaldeaUiState())
    val uiState: StateFlow<SukaldeaUiState> = _uiState.asStateFlow()

    private var pollingJob: Job? = null
    private val timeFormatter = SimpleDateFormat("HH:mm:ss", Locale.getDefault())

    init {
        hasieratuPolling()
    }

    fun hasieratuPolling() {
        pollingJob?.cancel()
        pollingJob = viewModelScope.launch {
            while (isActive) {
                kargatuEskaerak()
                delay(4000) // 4 segundoz behin freskatu automatikoki
            }
        }
    }

    fun kargatuEskaerak() {
        viewModelScope.launch {
            try {
                val erantzuna = RetrofitClient.apiService.lortuEskaerak()
                val zerrenda = erantzuna.dataList ?: erantzuna.valueList ?: emptyList()

                // Sukaldean interesgarri diren eskaerak soilik: Kobratu gabekoak
                val aktiboak = mutableListOf<Eskaera>()
                for (e in zerrenda) {
                    if (!e.egoera.equals("Kobratua", ignoreCase = true) &&
                        !e.egoera.equals("Ezeztatua", ignoreCase = true)
                    ) {
                        try {
                            val xehe = RetrofitClient.apiService.lortuEskaeraXehetasunak(e.id)
                            aktiboak.add(xehe.data ?: e)
                        } catch (_: Exception) {
                            aktiboak.add(e)
                        }
                    }
                }

                val ordua = timeFormatter.format(Date())
                _uiState.update {
                    it.copy(
                        isLoading = false,
                        eskaerak = aktiboak,
                        isOnline = true,
                        errorea = null,
                        azkenFreskatzea = ordua
                    )
                }
            } catch (ex: Exception) {
                _uiState.update {
                    it.copy(
                        isLoading = false,
                        isOnline = false,
                        errorea = ex.localizedMessage ?: "Konexio errorea"
                    )
                }
            }
        }
    }

    fun markatuLerroaEginda(lerroId: Int, egoeraBerria: String = "Prestatua") {
        viewModelScope.launch {
            try {
                val res = RetrofitClient.apiService.eguneratuLerroEgoera(
                    lerroId = lerroId,
                    body = EgoeraUpdateRequest(egoeraBerria)
                )
                if (res.isSuccessful) {
                    kargatuEskaerak()
                }
            } catch (ex: Exception) {
                _uiState.update { it.copy(errorea = "Errorea platera eguneratzean: ${ex.message}") }
            }
        }
    }

    fun markatuEskaeraEginda(eskaeraId: Int, egoeraBerria: String = "Prestatua") {
        viewModelScope.launch {
            try {
                val res = RetrofitClient.apiService.eguneratuEskaeraEgoera(
                    eskaeraId = eskaeraId,
                    body = EgoeraUpdateRequest(egoeraBerria)
                )
                if (res.isSuccessful) {
                    kargatuEskaerak()
                }
            } catch (ex: Exception) {
                _uiState.update { it.copy(errorea = "Errorea eskaera eguneratzean: ${ex.message}") }
            }
        }
    }

    fun aldatuFiltroa(filtroa: String) {
        _uiState.update { it.copy(unekoFiltroa = filtroa) }
    }

    fun aldatuServerUrl(berria: String) {
        RetrofitClient.serverBaseUrl = berria
        _uiState.update { it.copy(serverUrl = berria) }
        kargatuEskaerak()
    }
}
