package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.tween
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ExitToApp
import androidx.compose.material.icons.filled.CalendarMonth
import androidx.compose.material.icons.filled.Check
import androidx.compose.material.icons.filled.CleaningServices
import androidx.compose.material.icons.filled.EventAvailable
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material.icons.filled.Restaurant
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.model.MahaiaDto
import eus.goierri.jatetxea.komandera.ui.components.ErreserbakDialog
import eus.goierri.jatetxea.komandera.ui.components.EzarpenakDialog
import eus.goierri.jatetxea.komandera.ui.state.AppPreferences
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

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
    var eremuFiltroa by rememberSaveable { mutableStateOf("Guztiak") }
    var ezarpenakIrekita by remember { mutableStateOf(false) }
    var erreserbakIrekita by remember { mutableStateOf(false) }

    fun kargatuMahaiak(erakutsiKargatzen: Boolean = true) {
        scope.launch {
            if (erakutsiKargatzen) kargatzen = true
            try {
                val res = ApiClient.service.getMahaiak()
                if (res.isSuccessful && res.body()?.dataList != null) {
                    mahaiak = res.body()!!.dataList!!
                    erroreMezua = null
                } else if (mahaiak.isEmpty()) {
                    erroreMezua = AppPreferences.tr(
                        "Ezin izan dira mahaiak kargatu API zerbitzaritik.",
                        "No se pudieron cargar las mesas del servidor.",
                        "Could not load tables from server."
                    )
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

    LaunchedEffect(Unit) {
        kargatuMahaiak()
        while (true) {
            delay(5000)
            kargatuMahaiak(erakutsiKargatzen = false)
        }
    }

    MahaiakScreenContent(
        zerbitzaria = zerbitzaria,
        mahaiak = mahaiak,
        kargatzen = kargatzen,
        erroreMezua = erroreMezua,
        eremuFiltroa = eremuFiltroa,
        onEremuFiltroaAldatu = { eremuFiltroa = it },
        onMahaiaAukeratua = onMahaiaAukeratua,
        onSaioaItxi = onSaioaItxi,
        onFreskatu = { kargatuMahaiak(erakutsiKargatzen = true) },
        onIrekiErreserbak = { erreserbakIrekita = true },
        onIrekiEzarpenak = { ezarpenakIrekita = true },
        modifier = modifier
    )

    if (ezarpenakIrekita) {
        EzarpenakDialog(
            zerbitzaria = zerbitzaria,
            onDismiss = { ezarpenakIrekita = false },
            onApiConfigAldatua = { kargatuMahaiak(erakutsiKargatzen = true) }
        )
    }

    if (erreserbakIrekita) {
        ErreserbakDialog(
            mahaiak = mahaiak,
            onDismiss = { erreserbakIrekita = false },
            onErreserbaSortua = { kargatuMahaiak(erakutsiKargatzen = false) }
        )
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MahaiakScreenContent(
    zerbitzaria: LangileaDto,
    mahaiak: List<MahaiaDto>,
    kargatzen: Boolean,
    erroreMezua: String?,
    eremuFiltroa: String,
    onEremuFiltroaAldatu: (String) -> Unit,
    onMahaiaAukeratua: (MahaiaDto) -> Unit,
    onSaioaItxi: () -> Unit,
    onFreskatu: () -> Unit,
    onIrekiErreserbak: () -> Unit,
    onIrekiEzarpenak: () -> Unit,
    modifier: Modifier = Modifier
) {
    val isDark = AppPreferences.isDarkTheme
    val eremuak = listOf("Guztiak", "Jantoki Nagusia", "Barra", "Terraza", "VIP")
    val filtratuak = remember(mahaiak, eremuFiltroa) {
        if (eremuFiltroa == "Guztiak") mahaiak
        else mahaiak.filter { it.kokapena.equals(eremuFiltroa, ignoreCase = true) }
    }

    Scaffold(
        topBar = {
            Surface(
                color = MaterialTheme.colorScheme.surface,
                shadowElevation = 0.dp
            ) {
                Column {
                    TopAppBar(
                        title = {
                            Column {
                                Text(
                                    text = AppPreferences.tr("Aretoko Mahaiak", "Mesas de Sala", "Dining Tables"),
                                    style = MaterialTheme.typography.titleLarge,
                                    color = MaterialTheme.colorScheme.onSurface
                                )
                                Text(
                                    text = "${zerbitzaria.izena} · ${zerbitzaria.rola}",
                                    fontSize = 12.sp,
                                    fontWeight = FontWeight.SemiBold,
                                    color = MaterialTheme.colorScheme.secondary
                                )
                            }
                        },
                        navigationIcon = {
                            Box(modifier = Modifier.padding(start = 12.dp, end = 4.dp)) {
                                Surface(
                                    onClick = onSaioaItxi,
                                    shape = RoundedCornerShape(12.dp),
                                    color = if (isDark) Color(0xFF2D1214) else StatusOccupiedLight
                                ) {
                                    Icon(
                                        imageVector = Icons.AutoMirrored.Filled.ExitToApp,
                                        contentDescription = AppPreferences.tr("Saioa Itxi", "Cerrar Sesión", "Sign Out"),
                                        tint = MaterialTheme.colorScheme.error,
                                        modifier = Modifier.padding(9.dp).size(18.dp)
                                    )
                                }
                            }
                        },
                        actions = {
                            Row(
                                horizontalArrangement = Arrangement.spacedBy(6.dp),
                                modifier = Modifier.padding(end = 12.dp)
                            ) {
                                Surface(
                                    onClick = onIrekiErreserbak,
                                    shape = RoundedCornerShape(11.dp),
                                    color = MaterialTheme.colorScheme.background,
                                    border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                                ) {
                                    Icon(
                                        imageVector = Icons.Default.CalendarMonth,
                                        contentDescription = AppPreferences.tr("Erreserbak", "Reservas", "Reservations"),
                                        tint = MaterialTheme.colorScheme.secondary,
                                        modifier = Modifier.padding(8.dp).size(18.dp)
                                    )
                                }
                                Surface(
                                    onClick = onIrekiEzarpenak,
                                    shape = RoundedCornerShape(11.dp),
                                    color = MaterialTheme.colorScheme.background,
                                    border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                                ) {
                                    Icon(
                                        imageVector = Icons.Default.Settings,
                                        contentDescription = AppPreferences.tr("Ezarpenak", "Ajustes", "Settings"),
                                        tint = MaterialTheme.colorScheme.onSurfaceVariant,
                                        modifier = Modifier.padding(8.dp).size(18.dp)
                                    )
                                }
                                Surface(
                                    onClick = onFreskatu,
                                    shape = RoundedCornerShape(11.dp),
                                    color = MaterialTheme.colorScheme.background,
                                    border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                                ) {
                                    Icon(
                                        imageVector = Icons.Default.Refresh,
                                        contentDescription = AppPreferences.tr("Freskatu", "Actualizar", "Refresh"),
                                        tint = MaterialTheme.colorScheme.primary,
                                        modifier = Modifier.padding(8.dp).size(18.dp)
                                    )
                                }
                            }
                        },
                        colors = TopAppBarDefaults.topAppBarColors(containerColor = MaterialTheme.colorScheme.surface)
                    )
                    HorizontalDivider(color = MaterialTheme.colorScheme.outline.copy(alpha = 0.7f))
                }
            }
        },
        containerColor = MaterialTheme.colorScheme.background
    ) { innerPadding ->
        Column(
            modifier = modifier
                .fillMaxSize()
                .padding(innerPadding)
                .padding(horizontal = 16.dp, vertical = 14.dp)
        ) {
            // Eremuen pill filtroak
            val scrollState = rememberScrollState()
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .horizontalScroll(scrollState),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                eremuak.forEach { eremua ->
                    val hautatua = eremuFiltroa == eremua
                    val eremuEtiketa = if (eremua == "Guztiak") {
                        AppPreferences.tr("Guztiak", "Todas", "All")
                    } else eremua

                    Surface(
                        modifier = Modifier
                            .clip(RoundedCornerShape(999.dp))
                            .clickable { onEremuFiltroaAldatu(eremua) },
                        shape = RoundedCornerShape(999.dp),
                        color = if (hautatua) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.surface,
                        border = BorderStroke(
                            1.dp,
                            if (hautatua) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.outline
                        )
                    ) {
                        Text(
                            text = eremuEtiketa,
                            fontSize = 12.sp,
                            fontWeight = if (hautatua) FontWeight.Bold else FontWeight.Medium,
                            color = if (hautatua) MaterialTheme.colorScheme.onPrimary else MaterialTheme.colorScheme.onSurfaceVariant,
                            maxLines = 1,
                            softWrap = false,
                            modifier = Modifier.padding(horizontal = 14.dp, vertical = 8.dp)
                        )
                    }
                }
            }

            Spacer(modifier = Modifier.height(12.dp))

            // 4-zutabeko KPI Barra Modernoa (JetBrains Mono zenbakiekin)
            val libreKop = mahaiak.count { it.egoera.equals("Libre", ignoreCase = true) }
            val okupatuKop = mahaiak.count { it.egoera.equals("Okupatuta", ignoreCase = true) }
            val erreserbaKop = mahaiak.count { it.egoera.equals("Erreserbatuta", ignoreCase = true) }
            val garbitzenKop = mahaiak.count { it.egoera.equals("Garbitzen", ignoreCase = true) }

            Surface(
                modifier = Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(14.dp),
                color = MaterialTheme.colorScheme.surface,
                border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 8.dp, vertical = 10.dp),
                    horizontalArrangement = Arrangement.SpaceEvenly
                ) {
                    KpiStatItem(
                        kopurua = libreKop,
                        etiketa = AppPreferences.tr("Libre", "Libres", "Free"),
                        kolorea = if (isDark) StatusFreeDark else StatusFree,
                        modifier = Modifier.weight(1f)
                    )
                    KpiStatItem(
                        kopurua = okupatuKop,
                        etiketa = AppPreferences.tr("Okupatuta", "Ocupadas", "Busy"),
                        kolorea = if (isDark) StatusOccupiedDark else StatusOccupied,
                        modifier = Modifier.weight(1f)
                    )
                    KpiStatItem(
                        kopurua = erreserbaKop,
                        etiketa = AppPreferences.tr("Erreserba", "Reserva", "Booked"),
                        kolorea = if (isDark) GoldBorder else AntiqueGold,
                        modifier = Modifier.weight(1f)
                    )
                    KpiStatItem(
                        kopurua = garbitzenKop,
                        etiketa = AppPreferences.tr("Garbitzen", "Limpieza", "Clean"),
                        kolorea = if (isDark) StatusCleaningDark else StatusCleaning,
                        modifier = Modifier.weight(1f)
                    )
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            when {
                kargatzen && mahaiak.isEmpty() -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        CircularProgressIndicator(color = MaterialTheme.colorScheme.primary)
                    }
                }
                erroreMezua != null && mahaiak.isEmpty() -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                            Text(text = erroreMezua, color = MaterialTheme.colorScheme.error, textAlign = TextAlign.Center)
                            Spacer(modifier = Modifier.height(12.dp))
                            Button(
                                onClick = onFreskatu,
                                shape = RoundedCornerShape(12.dp),
                                colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
                            ) {
                                Text(
                                    AppPreferences.tr("Saiatu Berriro", "Reintentar", "Retry"),
                                    color = MaterialTheme.colorScheme.onPrimary
                                )
                            }
                        }
                    }
                }
                else -> {
                    LazyVerticalGrid(
                        columns = GridCells.Adaptive(minSize = 165.dp),
                        horizontalArrangement = Arrangement.spacedBy(12.dp),
                        verticalArrangement = Arrangement.spacedBy(12.dp),
                        modifier = Modifier.fillMaxSize()
                    ) {
                        items(filtratuak, key = { it.id }) { mahaia ->
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
}

@Composable
private fun KpiStatItem(
    kopurua: Int,
    etiketa: String,
    kolorea: Color,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = "$kopurua",
            fontFamily = JetBrainsMono,
            fontWeight = FontWeight.Bold,
            fontSize = 16.sp,
            color = kolorea
        )
        Text(
            text = etiketa,
            fontSize = 10.sp,
            fontWeight = FontWeight.Medium,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
            maxLines = 1
        )
    }
}

@Composable
fun MahaiaTxartela(
    mahaia: MahaiaDto,
    onClick: () -> Unit,
    modifier: Modifier = Modifier
) {
    val isDark = AppPreferences.isDarkTheme
    val egoeraLower = mahaia.egoera.lowercase()

    val egoeraKolorea by animateColorAsState(
        targetValue = when (egoeraLower) {
            "libre" -> if (isDark) StatusFreeDark else StatusFree
            "okupatuta" -> if (isDark) StatusOccupiedDark else StatusOccupied
            "erreserbatuta" -> if (isDark) GoldBorder else AntiqueGold
            "garbitzen" -> if (isDark) StatusCleaningDark else StatusCleaning
            else -> StatusPending
        },
        animationSpec = tween(300),
        label = "tableStatusColor"
    )

    val pillBg = when (egoeraLower) {
        "libre" -> if (isDark) Color(0xFF102819) else StatusFreeLight
        "okupatuta" -> if (isDark) Color(0xFF2D1214) else StatusOccupiedLight
        "erreserbatuta" -> if (isDark) Color(0xFF2B170B) else GoldTint
        "garbitzen" -> if (isDark) Color(0xFF0C2233) else StatusCleaningLight
        else -> MaterialTheme.colorScheme.surfaceVariant
    }

    val egoeraIkonoa = when (egoeraLower) {
        "libre" -> Icons.Default.Check
        "okupatuta" -> Icons.Default.Restaurant
        "erreserbatuta" -> Icons.Default.EventAvailable
        "garbitzen" -> Icons.Default.CleaningServices
        else -> Icons.Default.Restaurant
    }

    Surface(
        onClick = onClick,
        modifier = modifier.fillMaxWidth(),
        color = MaterialTheme.colorScheme.surface,
        shape = RoundedCornerShape(16.dp),
        border = BorderStroke(1.dp, egoeraKolorea.copy(alpha = if (isDark) 0.32f else 0.28f))
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .height(IntrinsicSize.Min)
        ) {
            // Ezkerreko 4dp egoera-marra bertikala (Figma cardMahaia)
            Box(
                modifier = Modifier
                    .width(4.dp)
                    .fillMaxHeight()
                    .background(egoeraKolorea)
            )

            Column(
                modifier = Modifier
                    .weight(1f)
                    .padding(horizontal = 12.dp, vertical = 12.dp),
                verticalArrangement = Arrangement.spacedBy(10.dp)
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = "M${mahaia.zenbakia}",
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold,
                        color = MaterialTheme.colorScheme.onSurface,
                        maxLines = 1,
                        overflow = TextOverflow.Ellipsis,
                        modifier = Modifier.weight(1f, fill = false)
                    )

                    Spacer(modifier = Modifier.width(6.dp))

                    // WCAG 2.2 POUR egoera-txapa (Ikonoa + Testua)
                    Surface(
                        color = pillBg,
                        shape = RoundedCornerShape(999.dp)
                    ) {
                        Row(
                            modifier = Modifier.padding(horizontal = 7.dp, vertical = 3.dp),
                            verticalAlignment = Alignment.CenterVertically,
                            horizontalArrangement = Arrangement.spacedBy(3.dp)
                        ) {
                            Icon(
                                imageVector = egoeraIkonoa,
                                contentDescription = null,
                                tint = egoeraKolorea,
                                modifier = Modifier.size(11.dp)
                            )
                            Text(
                                text = mahaia.egoera,
                                fontSize = 10.sp,
                                fontWeight = FontWeight.Bold,
                                color = egoeraKolorea,
                                maxLines = 1,
                                softWrap = false
                            )
                        }
                    }
                }

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = mahaia.kokapena,
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                        maxLines = 1,
                        overflow = TextOverflow.Ellipsis,
                        modifier = Modifier.weight(1f, fill = false)
                    )

                    Spacer(modifier = Modifier.width(6.dp))

                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        horizontalArrangement = Arrangement.spacedBy(3.dp)
                    ) {
                        Icon(
                            imageVector = Icons.Default.Person,
                            contentDescription = null,
                            tint = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.7f),
                            modifier = Modifier.size(13.dp)
                        )
                        Text(
                            text = "${mahaia.edukiera} pax",
                            fontFamily = JetBrainsMono,
                            fontSize = 11.sp,
                            fontWeight = FontWeight.Medium,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }
                }
            }
        }
    }
}

@Preview(showBackground = true, name = "MahaiakScreen - Argia")
@Composable
fun MahaiakScreenLightPreview() {
    KomanderaTheme(darkTheme = false) {
        MahaiakScreenContent(
            zerbitzaria = LangileaDto(1, "maite_zerb", "Maite Etxebarria", "Zerbitzaria"),
            mahaiak = listOf(
                MahaiaDto(1, 1, 2, "Jantoki Nagusia", "Okupatuta"),
                MahaiaDto(2, 2, 4, "Jantoki Nagusia", "Libre"),
                MahaiaDto(3, 5, 8, "VIP", "Erreserbatuta"),
                MahaiaDto(4, 6, 4, "Terraza", "Garbitzen")
            ),
            kargatzen = false,
            erroreMezua = null,
            eremuFiltroa = "Guztiak",
            onEremuFiltroaAldatu = {},
            onMahaiaAukeratua = {},
            onSaioaItxi = {},
            onFreskatu = {},
            onIrekiErreserbak = {},
            onIrekiEzarpenak = {}
        )
    }
}

@Preview(showBackground = true, name = "MahaiakScreen - Iluna")
@Composable
fun MahaiakScreenDarkPreview() {
    KomanderaTheme(darkTheme = true) {
        MahaiakScreenContent(
            zerbitzaria = LangileaDto(1, "maite_zerb", "Maite Etxebarria", "Zerbitzaria"),
            mahaiak = listOf(
                MahaiaDto(1, 1, 2, "Jantoki Nagusia", "Okupatuta"),
                MahaiaDto(2, 2, 4, "Jantoki Nagusia", "Libre")
            ),
            kargatzen = false,
            erroreMezua = null,
            eremuFiltroa = "Guztiak",
            onEremuFiltroaAldatu = {},
            onMahaiaAukeratua = {},
            onSaioaItxi = {},
            onFreskatu = {},
            onIrekiErreserbak = {},
            onIrekiEzarpenak = {}
        )
    }
}
