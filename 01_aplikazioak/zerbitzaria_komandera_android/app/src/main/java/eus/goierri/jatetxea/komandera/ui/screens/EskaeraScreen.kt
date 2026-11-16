package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Check
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material.icons.filled.Remove
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
import eus.goierri.jatetxea.komandera.data.model.GehituPlateraRequest
import eus.goierri.jatetxea.komandera.data.model.PlateraDto
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch
import java.util.Locale

data class SaskikoElementua(
    val platera: PlateraDto,
    var kopurua: Int = 1,
    var oharrak: String = ""
) {
    val azpitotala: Double get() = platera.prezioa * kopurua
}

@Suppress("UNUSED_PARAMETER")
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun EskaeraScreen(
    eskaeraId: Int,
    mahaiId: Int,
    onAtzera: () -> Unit,
    modifier: Modifier = Modifier
) {
    val scope = rememberCoroutineScope()
    var platerak by remember { mutableStateOf<List<PlateraDto>>(emptyList()) }
    var kargatzen by remember { mutableStateOf(true) }
    var erroreMezua by remember { mutableStateOf<String?>(null) }
    var gehitzen by remember { mutableStateOf(false) }

    var hautatutakoKategoria by remember { mutableStateOf("Guztiak") }
    var saskia by remember { mutableStateOf<List<SaskikoElementua>>(emptyList()) }

    fun kargatuKatalogoa() {
        scope.launch {
            kargatzen = true
            try {
                // 1. Platerak lortu
                val platerRes = ApiClient.service.getPlaterak()
                // 2. Stock erabilgarritasuna lortu
                val stockRes = ApiClient.service.getPlaterenStockAhalmena()

                val stockMap = mutableMapOf<Int, Int>()
                if (stockRes.isSuccessful && stockRes.body()?.dataList != null) {
                    stockRes.body()!!.dataList!!.forEach {
                        stockMap[it.plateraId] = it.eginDaitezkeenPlaterak
                    }
                }

                if (platerRes.isSuccessful && platerRes.body()?.dataList != null) {
                    val lista = platerRes.body()!!.dataList!!.filter { it.aktibo }
                    lista.forEach { p ->
                        p.stockAhalmena = stockMap[p.id] ?: 999
                    }
                    platerak = lista
                    erroreMezua = null
                } else {
                    erroreMezua = "Ezin izan da plater katalogoa kargatu."
                }
            } catch (e: Exception) {
                erroreMezua = "Konexio errorea: ${e.localizedMessage ?: "Zerbitzaria ez dago eskuragarri"}"
            } finally {
                kargatzen = false
            }
        }
    }

    LaunchedEffect(Unit) {
        kargatuKatalogoa()
    }

    val kategoriak = remember(platerak) {
        val katSet = platerak.map { it.kategoria }.distinct().filter { it.isNotBlank() }
        listOf("Guztiak") + katSet
    }

    val filtratutakoPlaterak = if (hautatutakoKategoria == "Guztiak") {
        platerak
    } else {
        platerak.filter { it.kategoria.equals(hautatutakoKategoria, ignoreCase = true) }
    }

    val guztira = saskia.sumOf { it.azpitotala }

    fun gehituSaskira(platera: PlateraDto) {
        val existitzen = saskia.find { it.platera.id == platera.id }
        if (existitzen != null) {
            if (existitzen.kopurua < platera.stockAhalmena) {
                saskia = saskia.map {
                    if (it.platera.id == platera.id) it.copy(kopurua = it.kopurua + 1) else it
                }
            }
        } else {
            saskia = saskia + SaskikoElementua(platera = platera, kopurua = 1)
        }
    }

    fun kenduSaskitik(plateraId: Int) {
        val existitzen = saskia.find { it.platera.id == plateraId }
        if (existitzen != null) {
            if (existitzen.kopurua > 1) {
                saskia = saskia.map {
                    if (it.platera.id == plateraId) it.copy(kopurua = it.kopurua - 1) else it
                }
            } else {
                saskia = saskia.filter { it.platera.id != plateraId }
            }
        }
    }

    fun gordePlaterakEskaeran() {
        if (saskia.isEmpty()) return
        scope.launch {
            gehitzen = true
            try {
                for (item in saskia) {
                    val req = GehituPlateraRequest(
                        plateraId = item.platera.id,
                        kantitatea = item.kopurua,
                        oharrak = item.oharrak.ifBlank { null }
                    )
                    ApiClient.service.gehituPlatera(eskaeraId, req)
                }
                onAtzera()
            } catch (_: Exception) {
            } finally {
                gehitzen = false
            }
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "Gehitu Platerak - Eskaera #$eskaeraId",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = TextPrimary
                        )
                        Text(
                            text = "Saskian: ${saskia.sumOf { it.kopurua }} plater (${String.format(Locale.ROOT, "%.2f €", guztira)})",
                            fontSize = 12.sp,
                            color = AmberPrimary
                        )
                    }
                },
                navigationIcon = {
                    IconButton(onClick = onAtzera) {
                        Icon(
                            imageVector = Icons.AutoMirrored.Filled.ArrowBack,
                            contentDescription = "Atzera",
                            tint = TextPrimary
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = DarkSurface)
            )
        },
        containerColor = DarkBackground,
        bottomBar = {
            if (saskia.isNotEmpty()) {
                Surface(
                    color = DarkSurface,
                    tonalElevation = 8.dp,
                    shadowElevation = 8.dp
                ) {
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(16.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            Text(text = "SASKIA GUZTIRA", fontSize = 11.sp, color = TextSecondary)
                            Text(
                                text = String.format(Locale.ROOT, "%.2f €", guztira),
                                fontSize = 22.sp,
                                fontWeight = FontWeight.Bold,
                                color = AmberPrimary
                            )
                        }

                        Button(
                            onClick = { gordePlaterakEskaeran() },
                            colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary),
                            shape = RoundedCornerShape(12.dp),
                            enabled = !gehitzen,
                            modifier = Modifier.height(48.dp)
                        ) {
                            if (gehitzen) {
                                CircularProgressIndicator(color = DarkBackground, modifier = Modifier.size(20.dp))
                            } else {
                                Icon(imageVector = Icons.Default.Check, contentDescription = null, tint = DarkBackground)
                                Spacer(modifier = Modifier.width(6.dp))
                                Text("Gehitu Eskaerara", color = DarkBackground, fontWeight = FontWeight.Bold)
                            }
                        }
                    }
                }
            }
        }
    ) { innerPadding ->
        Column(
            modifier = modifier
                .fillMaxSize()
                .padding(innerPadding)
                .padding(16.dp)
        ) {
            // Kategoriak karusela
            LazyRow(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(kategoriak) { kategoria ->
                    val hautatua = hautatutakoKategoria == kategoria
                    FilterChip(
                        selected = hautatua,
                        onClick = { hautatutakoKategoria = kategoria },
                        label = { Text(kategoria, fontSize = 12.sp) },
                        colors = FilterChipDefaults.filterChipColors(
                            containerColor = DarkSurface,
                            labelColor = TextSecondary,
                            selectedContainerColor = AmberPrimary,
                            selectedLabelColor = DarkBackground
                        ),
                        border = FilterChipDefaults.filterChipBorder(
                            borderColor = if (hautatua) AmberPrimary else TextMuted.copy(alpha = 0.3f),
                            selectedBorderColor = AmberPrimary,
                            enabled = true,
                            selected = hautatua
                        )
                    )
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            if (kargatzen) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator(color = AmberPrimary)
                }
            } else if (erroreMezua != null) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Text(text = erroreMezua!!, color = StatusBusy, textAlign = TextAlign.Center)
                        Spacer(modifier = Modifier.height(12.dp))
                        Button(
                            onClick = { kargatuKatalogoa() },
                            colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary)
                        ) {
                            Text("Saiatu Berriro", color = DarkBackground)
                        }
                    }
                }
            } else {
                LazyColumn(
                    modifier = Modifier.fillMaxSize(),
                    verticalArrangement = Arrangement.spacedBy(10.dp)
                ) {
                    items(filtratutakoPlaterak) { platera ->
                        val saskikoItem = saskia.find { it.platera.id == platera.id }
                        val kopurua = saskikoItem?.kopurua ?: 0
                        val stocka = platera.stockAhalmena
                        val agortuta = stocka <= 0

                        Card(
                            modifier = Modifier.fillMaxWidth(),
                            colors = CardDefaults.cardColors(containerColor = DarkSurface),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Column(modifier = Modifier.padding(14.dp)) {
                                Row(
                                    modifier = Modifier.fillMaxWidth(),
                                    horizontalArrangement = Arrangement.SpaceBetween,
                                    verticalAlignment = Alignment.Top
                                ) {
                                    Column(modifier = Modifier.weight(1f)) {
                                        Text(
                                            text = platera.izena,
                                            fontSize = 15.sp,
                                            fontWeight = FontWeight.SemiBold,
                                            color = if (agortuta) TextMuted else TextPrimary
                                        )
                                        if (!platera.deskribapena.isNullOrBlank()) {
                                            Text(
                                                text = platera.deskribapena,
                                                fontSize = 12.sp,
                                                color = TextSecondary,
                                                maxLines = 2
                                            )
                                        }
                                        if (!platera.alergenoak.isNullOrBlank()) {
                                            Text(
                                                text = "Alergenoak: ${platera.alergenoak}",
                                                fontSize = 11.sp,
                                                color = AmberPrimary.copy(alpha = 0.8f)
                                            )
                                        }
                                    }

                                    Spacer(modifier = Modifier.width(8.dp))

                                    Column(horizontalAlignment = Alignment.End) {
                                        Text(
                                            text = String.format(Locale.ROOT, "%.2f €", platera.prezioa),
                                            fontSize = 16.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = AmberPrimary
                                        )

                                        Text(
                                            text = if (agortuta) "Agortuta" else "Stock: $stocka",
                                            fontSize = 11.sp,
                                            color = if (agortuta) StatusBusy else StatusFree,
                                            fontWeight = FontWeight.Medium
                                        )
                                    }
                                }

                                Spacer(modifier = Modifier.height(10.dp))

                                // Gehitu / Kendu botoiak
                                Row(
                                    modifier = Modifier.fillMaxWidth(),
                                    horizontalArrangement = Arrangement.End,
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    if (kopurua > 0) {
                                        IconButton(
                                            onClick = { kenduSaskitik(platera.id) },
                                            modifier = Modifier
                                                .size(32.dp)
                                                .clip(CircleShape)
                                                .background(DarkSurfaceVariant)
                                        ) {
                                            Icon(
                                                imageVector = Icons.Default.Remove,
                                                contentDescription = "Kendu",
                                                tint = TextPrimary,
                                                modifier = Modifier.size(16.dp)
                                            )
                                        }

                                        Text(
                                            text = "$kopurua",
                                            fontSize = 15.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = AmberPrimary,
                                            modifier = Modifier.padding(horizontal = 14.dp)
                                        )
                                    }

                                    Button(
                                        onClick = { gehituSaskira(platera) },
                                        enabled = !agortuta && kopurua < stocka,
                                        colors = ButtonDefaults.buttonColors(
                                            containerColor = if (kopurua > 0) AmberPrimary else DarkSurfaceVariant
                                        ),
                                        shape = RoundedCornerShape(8.dp),
                                        contentPadding = PaddingValues(horizontal = 12.dp, vertical = 6.dp),
                                        modifier = Modifier.height(36.dp)
                                    ) {
                                        Icon(
                                            imageVector = Icons.Default.Add,
                                            contentDescription = "Gehitu",
                                            tint = if (kopurua > 0) DarkBackground else TextPrimary,
                                            modifier = Modifier.size(16.dp)
                                        )
                                        Spacer(modifier = Modifier.width(4.dp))
                                        Text(
                                            text = if (kopurua > 0) "Gehiago" else "Hautatu",
                                            fontSize = 12.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = if (kopurua > 0) DarkBackground else TextPrimary
                                        )
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
