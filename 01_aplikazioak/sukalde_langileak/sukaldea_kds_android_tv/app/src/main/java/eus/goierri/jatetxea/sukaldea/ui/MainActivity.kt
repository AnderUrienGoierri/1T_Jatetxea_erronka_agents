package eus.goierri.jatetxea.sukaldea.ui

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.viewModels
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.sukaldea.ui.components.EskaeraTxartela
import eus.goierri.jatetxea.sukaldea.ui.components.GoiburuaBarra
import eus.goierri.jatetxea.sukaldea.ui.theme.DarkBackground
import eus.goierri.jatetxea.sukaldea.ui.theme.RoseDark
import eus.goierri.jatetxea.sukaldea.ui.theme.RosePrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.SukaldeaKdsTheme
import eus.goierri.jatetxea.sukaldea.ui.theme.TextMuted
import eus.goierri.jatetxea.sukaldea.ui.theme.TextPrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.TextSecondary
import eus.goierri.jatetxea.sukaldea.ui.viewmodel.SukaldeaViewModel

class MainActivity : ComponentActivity() {

    private val viewModel: SukaldeaViewModel by viewModels()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            SukaldeaKdsTheme {
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = DarkBackground
                ) {
                    SukaldeaKdsScreen(viewModel)
                }
            }
        }
    }
}

@Composable
fun SukaldeaKdsScreen(viewModel: SukaldeaViewModel) {
    val state by viewModel.uiState.collectAsState()

    // Filtroaren araberako zerrenda
    val filtratuak = when (state.unekoFiltroa.lowercase()) {
        "sukaldean" -> state.eskaerak.filter { !it.egoera.equals("Prestatua", ignoreCase = true) }
        "prestatua" -> state.eskaerak.filter { it.egoera.equals("Prestatua", ignoreCase = true) }
        else -> state.eskaerak
    }

    Column(modifier = Modifier.fillMaxSize()) {
        // GOIBURUKO BARRA
        GoiburuaBarra(
            isOnline = state.isOnline,
            azkenOrdua = state.azkenFreskatzea,
            eskaeraKopurua = filtratuak.size,
            unekoFiltroa = state.unekoFiltroa,
            serverUrl = state.serverUrl,
            onFiltroAldatu = { viewModel.aldatuFiltroa(it) },
            onFreskatu = { viewModel.kargatuEskaerak() },
            onServerUrlAldatu = { viewModel.aldatuServerUrl(it) }
        )

        // ERRORE BARRUA (Konexio arazorik balego)
        if (state.errorea != null && !state.isOnline) {
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .background(RoseDark)
                    .padding(horizontal = 24.dp, vertical = 6.dp)
            ) {
                Text(
                    text = "Konexio errorea: ${state.errorea} - Egiaztatu API zerbitzaria martxan dagoen portu 5000n.",
                    color = Color.White,
                    fontSize = 12.sp,
                    fontWeight = FontWeight.SemiBold
                )
            }
        }

        // ESKAEREN ZERRENDA NAGUSIA (HORIZONTAL SCROLL TV REMOTE-RAKO)
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(top = 16.dp, bottom = 16.dp),
            contentAlignment = Alignment.Center
        ) {
            when {
                state.isLoading && state.eskaerak.isEmpty() -> {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        CircularProgressIndicator(color = RosePrimary, strokeWidth = 3.dp)
                        Spacer(modifier = Modifier.height(14.dp))
                        Text(
                            text = "Eskaerak kargatzen...",
                            color = TextSecondary,
                            fontSize = 14.sp
                        )
                    }
                }

                filtratuak.isEmpty() -> {
                    Column(
                        horizontalAlignment = Alignment.CenterHorizontally,
                        verticalArrangement = Arrangement.Center
                    ) {
                        Box(
                            modifier = Modifier
                                .size(64.dp)
                                .background(Color(0xFF1E293B), shape = androidx.compose.foundation.shape.CircleShape),
                            contentAlignment = Alignment.Center
                        ) {
                            Text(
                                text = "KDS",
                                color = TextMuted,
                                fontWeight = FontWeight.Bold,
                                fontSize = 16.sp
                            )
                        }
                        Spacer(modifier = Modifier.height(14.dp))
                        Text(
                            text = "Ez dago eskaera aktiborik une honetan.",
                            color = TextPrimary,
                            fontSize = 16.sp,
                            fontWeight = FontWeight.Bold
                        )
                        Spacer(modifier = Modifier.height(4.dp))
                        Text(
                            text = "Zerbitzariek eskaera berriak sartu ahala hemen agertuko dira automatikoki.",
                            color = TextMuted,
                            fontSize = 13.sp,
                            textAlign = TextAlign.Center
                        )
                    }
                }

                else -> {
                    LazyRow(
                        modifier = Modifier.fillMaxSize(),
                        contentPadding = PaddingValues(horizontal = 24.dp),
                        horizontalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        items(filtratuak, key = { it.id }) { eskaera ->
                            EskaeraTxartela(
                                eskaera = eskaera,
                                onMarkatuLerroa = { lerroId ->
                                    viewModel.markatuLerroaEginda(lerroId)
                                },
                                onMarkatuEskaera = { eskaeraId ->
                                    viewModel.markatuEskaeraEginda(eskaeraId)
                                }
                            )
                        }
                    }
                }
            }
        }
    }
}
