package eus.goierri.jatetxea.sukaldea.ui

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.viewModels
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.sukaldea.data.api.RetrofitClient
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
        RetrofitClient.hasieratu(applicationContext)
        viewModel.sinkronizatuUrl()

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
    var showConfigDialog by remember { mutableStateOf(false) }

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
            showConfigExternal = showConfigDialog,
            onDismissConfig = { showConfigDialog = false },
            onFiltroAldatu = { viewModel.aldatuFiltroa(it) },
            onFreskatu = { viewModel.kargatuEskaerak() },
            onServerUrlAldatu = { viewModel.aldatuServerUrl(it) }
        )

        // ERRORE BARRUA (Konexio arazorik balego)
        if (state.errorea != null && !state.isOnline) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .background(RoseDark)
                    .clickable { showConfigDialog = true }
                    .padding(horizontal = 24.dp, vertical = 8.dp),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = "Konexio errorea: ${state.errorea}\nEgiaztatu zerbitzariaren IP-a eta ataka (Docker: 5050 | Zuzenekoa: 5000).",
                    color = Color.White,
                    fontSize = 12.sp,
                    fontWeight = FontWeight.SemiBold,
                    modifier = Modifier.weight(1f)
                )
                Spacer(modifier = Modifier.width(12.dp))
                Button(
                    onClick = { showConfigDialog = true },
                    colors = ButtonDefaults.buttonColors(containerColor = RosePrimary),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Text(
                        text = "⚙️ Aldatu Ataka / IP",
                        color = Color.White,
                        fontWeight = FontWeight.Bold,
                        fontSize = 12.sp
                    )
                }
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
