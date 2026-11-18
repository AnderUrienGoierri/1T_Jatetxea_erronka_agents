package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ExitToApp
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.CalendarMonth
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material.icons.filled.Settings
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.rememberScrollState
import eus.goierri.jatetxea.komandera.ui.components.ErreserbakDialog
import eus.goierri.jatetxea.komandera.ui.components.EzarpenakDialog
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.model.MahaiaDto
import eus.goierri.jatetxea.komandera.ui.theme.AmberAccent
import eus.goierri.jatetxea.komandera.ui.theme.AmberPrimary
import eus.goierri.jatetxea.komandera.ui.theme.ChiantiPrimary
import eus.goierri.jatetxea.komandera.ui.theme.DarkBackground
import eus.goierri.jatetxea.komandera.ui.theme.DarkSurface
import eus.goierri.jatetxea.komandera.ui.theme.StatusBusy
import eus.goierri.jatetxea.komandera.ui.theme.StatusCleaning
import eus.goierri.jatetxea.komandera.ui.theme.StatusFree
import eus.goierri.jatetxea.komandera.ui.theme.StatusPending
import eus.goierri.jatetxea.komandera.ui.theme.TextMuted
import eus.goierri.jatetxea.komandera.ui.theme.TextPrimary
import eus.goierri.jatetxea.komandera.ui.theme.TextSecondary
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MahaiakScreen(
    zerbitzaria: LangileaDto,
    onMahaiaAukeratua: (MahaiaDto) -> Unit,
    onSaioaItxi: () -> Unit,
    modifier: Modifier = Modifier
) {
    val scope = rememberCoroutineScope()
    var mahaiak by remember { mutableStateOf<List<MahaiaDto>>(emptyList()) }
    var kargatzen by remember { mutableStateOf(true) }
    var erroreMezua by remember { mutableStateOf<String?>(null) }
    var eremuFiltroa by remember { mutableStateOf("Guztiak") }
    var ezarpenakIrekita by remember { mutableStateOf(false) }
    var erreserbakIrekita by remember { mutableStateOf(false) }

    val eremuak = listOf("Guztiak", "Jantoki Nagusia", "Barra", "Terraza", "VIP")

    fun kargatuMahaiak(erakutsiKargatzen: Boolean = true) {
        scope.launch {
            if (erakutsiKargatzen) kargatzen = true
            try {
                val res = ApiClient.service.getMahaiak()
                if (res.isSuccessful && res.body()?.dataList != null) {
                    val zerrenda = res.body()!!.dataList!!
                    mahaiak = zerrenda
                    erroreMezua = null
                } else {
                    if (mahaiak.isEmpty()) {
                        erroreMezua = "Ezin izan dira mahaiak kargatu API zerbitzaritik."
                    }
                }
            } catch (e: Exception) {
                if (mahaiak.isEmpty()) {
                    erroreMezua = "Konexio errorea: ${e.localizedMessage ?: "Zerbitzaria ez dago eskuragarri"}"
                }
            } finally {
                kargatzen = false
            }
        }
    }

    // Hasierako karga eta aldizkako auto-freskatzea (5 segundoro)
    LaunchedEffect(Unit) {
        kargatuMahaiak()
        while (true) {
            delay(5000)
            kargatuMahaiak(erakutsiKargatzen = false)
        }
    }

    val filtratuak = if (eremuFiltroa == "Guztiak") {
        mahaiak
    } else {
        mahaiak.filter { it.kokapena.equals(eremuFiltroa, ignoreCase = true) }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "Aretoko Mahaiak",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.onSurface
                        )
                        Text(
                            text = "${zerbitzaria.izena} (${zerbitzaria.rola})",
                            fontSize = 12.sp,
                            color = AmberAccent
                        )
                    }
                },
                navigationIcon = {
                    IconButton(onClick = onSaioaItxi) {
                        Icon(
                            imageVector = Icons.AutoMirrored.Filled.ExitToApp,
                            contentDescription = "Saioa Itxi",
                            tint = StatusBusy
                        )
                    }
                },
                actions = {
                    IconButton(onClick = { erreserbakIrekita = true }) {
                        Icon(
                            imageVector = Icons.Default.CalendarMonth,
                            contentDescription = "Erreserbak",
                            tint = AmberAccent
                        )
                    }
                    IconButton(onClick = { ezarpenakIrekita = true }) {
                        Icon(
                            imageVector = Icons.Default.Settings,
                            contentDescription = "Ezarpenak",
                            tint = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }
                    IconButton(onClick = { kargatuMahaiak(erakutsiKargatzen = true) }) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = "Freskatu",
                            tint = AmberPrimary
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = MaterialTheme.colorScheme.surface)
            )
        },
        containerColor = MaterialTheme.colorScheme.background
    ) { innerPadding ->
        if (ezarpenakIrekita) {
            EzarpenakDialog(
                zerbitzaria = zerbitzaria,
                onDismiss = { ezarpenakIrekita = false }
            )
        }

        if (erreserbakIrekita) {
            ErreserbakDialog(
                mahaiak = mahaiak,
                onDismiss = { erreserbakIrekita = false },
                onErreserbaSortua = {
                    kargatuMahaiak(erakutsiKargatzen = false)
                }
            )
        }

        Column(
            modifier = modifier
                .fillMaxSize()
                .padding(innerPadding)
                .padding(16.dp)
        ) {
            // Eremuen botoi-filtroa (Horizontalki mugikorra - testurik inoiz ez mozteko)
            val scrollState = rememberScrollState()
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .horizontalScroll(scrollState),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                eremuak.forEach { eremua ->
                    val hautatua = eremuFiltroa == eremua
                    FilterChip(
                        selected = hautatua,
                        onClick = { eremuFiltroa = eremua },
                        label = {
                            Text(
                                text = eremua,
                                fontSize = 12.sp,
                                fontWeight = if (hautatua) FontWeight.Bold else FontWeight.Normal,
                                maxLines = 1,
                                softWrap = false
                            )
                        },
                        colors = FilterChipDefaults.filterChipColors(
                            containerColor = MaterialTheme.colorScheme.surface,
                            labelColor = MaterialTheme.colorScheme.onSurfaceVariant,
                            selectedContainerColor = ChiantiPrimary,
                            selectedLabelColor = Color.White
                        ),
                        border = FilterChipDefaults.filterChipBorder(
                            borderColor = if (hautatua) ChiantiPrimary else MaterialTheme.colorScheme.outline.copy(alpha = 0.4f),
                            selectedBorderColor = ChiantiPrimary,
                            enabled = true,
                            selected = hautatua
                        )
                    )
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            // Estatistikak barra laburra
            val libreKop = mahaiak.count { it.egoera.equals("Libre", ignoreCase = true) }
            val okupatuKop = mahaiak.count { it.egoera.equals("Okupatuta", ignoreCase = true) }
            val erreserbaKop = mahaiak.count { it.egoera.equals("Erreserbatuta", ignoreCase = true) }
            val garbitzenKop = mahaiak.count { it.egoera.equals("Garbitzen", ignoreCase = true) }

            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .clip(RoundedCornerShape(10.dp))
                    .background(MaterialTheme.colorScheme.surface)
                    .padding(horizontal = 8.dp, vertical = 8.dp),
                horizontalArrangement = Arrangement.SpaceAround
            ) {
                Text(
                    text = "Guztira: ${mahaiak.size}",
                    fontSize = 11.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Text(
                    text = "Libre: $libreKop",
                    fontSize = 11.sp,
                    color = StatusFree,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    text = "Okupatuta: $okupatuKop",
                    fontSize = 11.sp,
                    color = StatusBusy,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    text = "Erreserbak: $erreserbaKop",
                    fontSize = 11.sp,
                    color = StatusPending,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    text = "Garbitzen: $garbitzenKop",
                    fontSize = 11.sp,
                    color = StatusCleaning,
                    fontWeight = FontWeight.Bold
                )
            }

            Spacer(modifier = Modifier.height(14.dp))

            if (kargatzen && mahaiak.isEmpty()) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator(color = AmberPrimary)
                }
            } else if (erroreMezua != null && mahaiak.isEmpty()) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Text(text = erroreMezua!!, color = StatusBusy, textAlign = TextAlign.Center)
                        Spacer(modifier = Modifier.height(12.dp))
                        Button(
                            onClick = { kargatuMahaiak(erakutsiKargatzen = true) },
                            colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary)
                        ) {
                            Text("Saiatu Berriro", color = DarkBackground)
                        }
                    }
                }
            } else {
                LazyVerticalGrid(
                    columns = GridCells.Fixed(2),
                    horizontalArrangement = Arrangement.spacedBy(12.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp),
                    modifier = Modifier.fillMaxSize()
                ) {
                    items(filtratuak) { mahaia ->
                        MahaiaTxartela(
                            mahaia = mahaia,
                            onClick = { onMahaiaAukeratua(mahaia) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun MahaiaTxartela(
    mahaia: MahaiaDto,
    onClick: () -> Unit
) {
    val egoeraKolorea = when (mahaia.egoera.lowercase()) {
        "libre" -> StatusFree
        "okupatuta" -> StatusBusy
        "erreserbatuta" -> StatusPending
        "garbitzen" -> StatusCleaning
        else -> StatusPending
    }

    Card(
        onClick = onClick,
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        shape = RoundedCornerShape(16.dp),
        border = androidx.compose.foundation.BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.25f)),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(modifier = Modifier.padding(12.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = "Mahaia ${mahaia.zenbakia}",
                    fontSize = 16.sp,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.onSurface,
                    modifier = Modifier.weight(1f, fill = false)
                )

                Spacer(modifier = Modifier.width(4.dp))

                // Egoera puntua eta txapa (lerro bakarrean)
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.End
                ) {
                    Box(
                        modifier = Modifier
                            .size(8.dp)
                            .clip(CircleShape)
                            .background(egoeraKolorea)
                    )
                    Spacer(modifier = Modifier.width(4.dp))
                    Text(
                        text = mahaia.egoera,
                        fontSize = 11.sp,
                        fontWeight = FontWeight.SemiBold,
                        color = egoeraKolorea,
                        maxLines = 1,
                        softWrap = false
                    )
                }
            }

            Spacer(modifier = Modifier.height(6.dp))

            Text(
                text = "${mahaia.kokapena} · ${mahaia.edukiera} pax",
                fontSize = 12.sp,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )

            Spacer(modifier = Modifier.height(10.dp))

            // Beheko informazioa (okupatuta badago zenbatekoa)
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.Bottom
            ) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Icon(
                        imageVector = Icons.Default.Person,
                        contentDescription = null,
                        tint = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.6f),
                        modifier = Modifier.size(14.dp)
                    )
                    Spacer(modifier = Modifier.width(4.dp))
                    Text(
                        text = "Geh. ${mahaia.edukiera}",
                        fontSize = 11.sp,
                        color = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.8f)
                    )
                }

                Text(
                    text = "Kudeatu ->",
                    fontSize = 12.sp,
                    fontWeight = FontWeight.SemiBold,
                    color = AmberAccent
                )
            }
        }
    }
}
