package eus.goierri.jatetxea.komandera.ui.components

import androidx.compose.animation.AnimatedContent
import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.togetherWith
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.KeyboardArrowLeft
import androidx.compose.material.icons.automirrored.filled.KeyboardArrowRight
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import androidx.compose.ui.window.DialogProperties
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.ErreserbaDto
import eus.goierri.jatetxea.komandera.data.model.ErreserbaSortuDto
import eus.goierri.jatetxea.komandera.data.model.MahaiaDto
import eus.goierri.jatetxea.komandera.ui.state.AppPreferences
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch
import java.time.DayOfWeek
import java.time.LocalDate
import java.time.YearMonth
import java.time.format.DateTimeFormatter

@Composable
fun ErreserbakDialog(
    mahaiak: List<MahaiaDto>,
    onDismiss: () -> Unit,
    onErreserbaSortua: () -> Unit
) {
    val scope = rememberCoroutineScope()
    var fitxaAktiboa by remember { mutableStateOf("ZERRENDA") } // "ZERRENDA" edo "BERRIA"

    val gaur = remember { LocalDate.now() }
    var erakutsitakoHilabetea by remember { mutableStateOf(YearMonth.from(gaur)) }
    var hautatutakoEguna by remember { mutableStateOf(gaur) }

    var erreserbaGuztiak by remember { mutableStateOf<List<ErreserbaDto>>(emptyList()) }
    var kargatzen by remember { mutableStateOf(false) }
    var mezua by remember { mutableStateOf<String?>(null) }
    var errorea by remember { mutableStateOf<String?>(null) }

    // Formulario Berria
    var bezeroIzena by remember { mutableStateOf("") }
    var bezeroEmaila by remember { mutableStateOf("") }
    var bezeroTelefonoa by remember { mutableStateOf("") }
    var txanda by remember { mutableStateOf("Bazkaria_1") }
    var paxKop by remember { mutableStateOf("2") }
    var hautatutakoMahaia by remember { mutableStateOf<MahaiaDto?>(null) }
    var oharrak by remember { mutableStateOf("") }
    var gordetzen by remember { mutableStateOf(false) }

    val dateFormatter = remember { DateTimeFormatter.ISO_LOCAL_DATE }
    val hautatutakoDataStr = hautatutakoEguna.format(dateFormatter)

    val erreserbakDataka = remember(erreserbaGuztiak) {
        erreserbaGuztiak
            .filter { !it.egoera.equals("Ezeztatua", ignoreCase = true) }
            .groupBy { it.data.take(10) }
    }
    val egunekoErreserbak = erreserbakDataka[hautatutakoDataStr].orEmpty()

    fun kargatuErreserbak() {
        scope.launch {
            kargatzen = true
            errorea = null
            try {
                // Erreserba guztiak ekarri hilabeteko egutegian egun markatu guztiak erakusteko
                val res = ApiClient.service.getErreserbak(data = null)
                if (res.isSuccessful && res.body()?.dataList != null) {
                    erreserbaGuztiak = res.body()!!.dataList!!
                } else {
                    erreserbaGuztiak = emptyList()
                }
            } catch (e: Exception) {
                errorea = "Errorea: ${e.localizedMessage}"
            } finally {
                kargatzen = false
            }
        }
    }

    LaunchedEffect(Unit) {
        kargatuErreserbak()
    }

    Dialog(
        onDismissRequest = onDismiss,
        properties = DialogProperties(usePlatformDefaultWidth = false)
    ) {
        Card(
            modifier = Modifier
                .fillMaxWidth(0.95f)
                .fillMaxHeight(0.90f),
            shape = RoundedCornerShape(20.dp),
            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
            border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.4f)),
            elevation = CardDefaults.cardElevation(defaultElevation = 8.dp)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(16.dp)
            ) {
                // Goiburua
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        Box(
                            modifier = Modifier
                                .size(36.dp)
                                .clip(CircleShape)
                                .background(AmberAccent.copy(alpha = 0.2f)),
                            contentAlignment = Alignment.Center
                        ) {
                            Icon(
                                imageVector = Icons.Default.CalendarMonth,
                                contentDescription = null,
                                tint = AmberAccent,
                                modifier = Modifier.size(20.dp)
                            )
                        }
                        Text(
                            text = AppPreferences.tr("Aretoko Erreserbak", "Reservas de Sala", "Reservations"),
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.onSurface
                        )
                    }

                    Row(verticalAlignment = Alignment.CenterVertically) {
                        IconButton(onClick = { kargatuErreserbak() }, modifier = Modifier.size(32.dp)) {
                            Icon(Icons.Default.Refresh, contentDescription = "Freskatu", tint = AmberAccent)
                        }
                        IconButton(onClick = onDismiss, modifier = Modifier.size(32.dp)) {
                            Icon(
                                imageVector = Icons.Default.Close,
                                contentDescription = "Itxi",
                                tint = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(10.dp))

                // Nabigazio Fitxak (Egutegia vs Erreserba Berria)
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .clip(RoundedCornerShape(10.dp))
                        .background(MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.6f))
                        .padding(4.dp)
                ) {
                    TabItem(
                        titulua = "${AppPreferences.tr("Egutegia", "Calendario", "Calendar")} (${egunekoErreserbak.size})",
                        aktibo = fitxaAktiboa == "ZERRENDA",
                        modifier = Modifier.weight(1f),
                        onClick = { fitxaAktiboa = "ZERRENDA" }
                    )
                    TabItem(
                        titulua = AppPreferences.tr("+ Sortu Erreserba", "+ Nueva Reserva", "+ New Booking"),
                        aktibo = fitxaAktiboa == "BERRIA",
                        modifier = Modifier.weight(1f),
                        onClick = {
                            errorea = null
                            mezua = null
                            fitxaAktiboa = "BERRIA"
                        }
                    )
                }

                Spacer(modifier = Modifier.height(10.dp))

                AnimatedContent(
                    targetState = fitxaAktiboa,
                    modifier = Modifier.weight(1f),
                    transitionSpec = { fadeIn(tween(200)) togetherWith fadeOut(tween(150)) },
                    label = "erreserbaTab"
                ) { tab ->
                    if (tab == "ZERRENDA") {
                        Column(modifier = Modifier.fillMaxSize()) {
                            // HILABETEKO EGUTEGI INTERAKTIBOA
                            HilabetekoEgutegia(
                                hilabetea = erakutsitakoHilabetea,
                                hautatutakoEguna = hautatutakoEguna,
                                gaur = gaur,
                                erreserbaKopuruak = erreserbakDataka.mapValues { it.value.size },
                                onHilabeteaAldatu = { erakutsitakoHilabetea = it },
                                onEgunaAukeratua = { eguna ->
                                    hautatutakoEguna = eguna
                                }
                            )

                            Spacer(modifier = Modifier.height(10.dp))

                            // Hautatutako egunaren goiburua
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Text(
                                    text = "$hautatutakoDataStr · ${egunekoErreserbak.size} ${
                                        AppPreferences.tr("erreserba", "reservas", "bookings")
                                    }",
                                    fontSize = 13.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = ChiantiPrimary
                                )
                                if (hautatutakoEguna.dayOfWeek == DayOfWeek.MONDAY) {
                                    Text(
                                        text = AppPreferences.tr("Astelehena (Itxita)", "Lunes (Cerrado)", "Monday (Closed)"),
                                        fontSize = 11.sp,
                                        color = StatusBusy,
                                        fontWeight = FontWeight.SemiBold
                                    )
                                }
                            }

                            Spacer(modifier = Modifier.height(6.dp))

                            if (kargatzen && erreserbaGuztiak.isEmpty()) {
                                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                                    CircularProgressIndicator(color = ChiantiPrimary)
                                }
                            } else if (egunekoErreserbak.isEmpty()) {
                                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                                        Icon(
                                            Icons.Default.EventAvailable,
                                            contentDescription = null,
                                            tint = MaterialTheme.colorScheme.onSurfaceVariant,
                                            modifier = Modifier.size(36.dp)
                                        )
                                        Spacer(modifier = Modifier.height(6.dp))
                                        Text(
                                            text = AppPreferences.tr(
                                                "Ez dago erreserbarik egun honetan",
                                                "No hay reservas para este día",
                                                "No reservations on this day"
                                            ),
                                            color = MaterialTheme.colorScheme.onSurfaceVariant,
                                            fontSize = 13.sp
                                        )
                                        if (hautatutakoEguna.dayOfWeek != DayOfWeek.MONDAY) {
                                            Spacer(modifier = Modifier.height(8.dp))
                                            Button(
                                                onClick = { fitxaAktiboa = "BERRIA" },
                                                colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                                                shape = RoundedCornerShape(10.dp),
                                                contentPadding = PaddingValues(horizontal = 16.dp, vertical = 8.dp)
                                            ) {
                                                Text(
                                                    AppPreferences.tr("Gehitu Erreserba Egun Honetan", "Añadir Reserva Este Día", "Add Booking For Day"),
                                                    color = Color.White,
                                                    fontSize = 12.sp,
                                                    fontWeight = FontWeight.Bold
                                                )
                                            }
                                        }
                                    }
                                }
                            } else {
                                LazyColumn(
                                    verticalArrangement = Arrangement.spacedBy(8.dp),
                                    modifier = Modifier.fillMaxSize()
                                ) {
                                    items(egunekoErreserbak, key = { it.id }) { erreserba ->
                                        ErreserbaKarta(erreserba)
                                    }
                                }
                            }
                        }
                    } else {
                        // BERRIA FORMULARIOA
                        Column(modifier = Modifier.fillMaxSize()) {
                            LazyColumn(
                                verticalArrangement = Arrangement.spacedBy(10.dp),
                                modifier = Modifier.weight(1f)
                            ) {
                                item {
                                    // Aukeratutako data bannerra (egutegitik hartua)
                                    Surface(
                                        modifier = Modifier
                                            .fillMaxWidth()
                                            .clickable { fitxaAktiboa = "ZERRENDA" },
                                        color = ChiantiPrimary.copy(alpha = 0.1f),
                                        shape = RoundedCornerShape(10.dp),
                                        border = BorderStroke(1.dp, ChiantiPrimary.copy(alpha = 0.35f))
                                    ) {
                                        Row(
                                            modifier = Modifier.padding(horizontal = 12.dp, vertical = 9.dp),
                                            horizontalArrangement = Arrangement.SpaceBetween,
                                            verticalAlignment = Alignment.CenterVertically
                                        ) {
                                            Row(
                                                verticalAlignment = Alignment.CenterVertically,
                                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                                            ) {
                                                Icon(
                                                    Icons.Default.Event,
                                                    contentDescription = null,
                                                    tint = ChiantiPrimary,
                                                    modifier = Modifier.size(18.dp)
                                                )
                                                Text(
                                                    text = "${AppPreferences.tr("Data", "Fecha", "Date")}: $hautatutakoDataStr",
                                                    fontSize = 13.sp,
                                                    fontWeight = FontWeight.Bold,
                                                    color = MaterialTheme.colorScheme.onSurface
                                                )
                                            }
                                            Text(
                                                text = AppPreferences.tr("Aldatu egutegian", "Cambiar", "Change"),
                                                fontSize = 11.sp,
                                                fontWeight = FontWeight.SemiBold,
                                                color = ChiantiPrimary
                                            )
                                        }
                                    }
                                }
                                item {
                                    OutlinedTextField(
                                        value = bezeroIzena,
                                        onValueChange = { bezeroIzena = it },
                                        label = { Text(AppPreferences.tr("Bezeroaren Izena *", "Nombre del Cliente *", "Customer Name *")) },
                                        modifier = Modifier.fillMaxWidth(),
                                        singleLine = true
                                    )
                                }
                                item {
                                    OutlinedTextField(
                                        value = bezeroEmaila,
                                        onValueChange = { bezeroEmaila = it },
                                        label = { Text(AppPreferences.tr("Emaila (erregistro automatikorako) *", "Email *", "Email *")) },
                                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                                        modifier = Modifier.fillMaxWidth(),
                                        singleLine = true
                                    )
                                }
                                item {
                                    OutlinedTextField(
                                        value = bezeroTelefonoa,
                                        onValueChange = { bezeroTelefonoa = it },
                                        label = { Text(AppPreferences.tr("Telefonoa (aukerakoa)", "Teléfono (opcional)", "Phone (optional)")) },
                                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                                        modifier = Modifier.fillMaxWidth(),
                                        singleLine = true
                                    )
                                }
                                item {
                                    Text("Txanda:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = AmberAccent)
                                    Spacer(modifier = Modifier.height(6.dp))
                                    Row(
                                        modifier = Modifier.fillMaxWidth(),
                                        horizontalArrangement = Arrangement.spacedBy(6.dp)
                                    ) {
                                        listOf(
                                            "Bazkaria_1" to "13:30 (B1)",
                                            "Bazkaria_2" to "15:00 (B2)",
                                            "Afaria_1" to "20:30 (A1)",
                                            "Afaria_2" to "22:00 (A2)"
                                        ).forEach { (id, label) ->
                                            val sel = txanda == id
                                            Surface(
                                                modifier = Modifier
                                                    .weight(1f)
                                                    .clickable { txanda = id },
                                                shape = RoundedCornerShape(8.dp),
                                                color = if (sel) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant,
                                                border = BorderStroke(1.dp, if (sel) GoldBorder else Color.Transparent)
                                            ) {
                                                Text(
                                                    text = label,
                                                    fontSize = 11.sp,
                                                    fontWeight = if (sel) FontWeight.Bold else FontWeight.Normal,
                                                    color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface,
                                                    maxLines = 1,
                                                    softWrap = false,
                                                    modifier = Modifier.padding(vertical = 8.dp, horizontal = 2.dp),
                                                    textAlign = TextAlign.Center
                                                )
                                            }
                                        }
                                    }
                                }
                                item {
                                    Text("Pertsona kopurua:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = AmberAccent)
                                    Spacer(modifier = Modifier.height(6.dp))
                                    Row(
                                        modifier = Modifier.fillMaxWidth(),
                                        horizontalArrangement = Arrangement.spacedBy(6.dp)
                                    ) {
                                        listOf("1", "2", "4", "6", "8", "10").forEach { p ->
                                            val sel = paxKop == p
                                            Surface(
                                                modifier = Modifier
                                                    .weight(1f)
                                                    .clickable { paxKop = p },
                                                shape = RoundedCornerShape(8.dp),
                                                color = if (sel) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant,
                                                border = BorderStroke(1.dp, if (sel) GoldBorder else Color.Transparent)
                                            ) {
                                                Text(
                                                    text = "$p pax",
                                                    fontSize = 11.sp,
                                                    fontWeight = if (sel) FontWeight.Bold else FontWeight.Normal,
                                                    color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface,
                                                    maxLines = 1,
                                                    softWrap = false,
                                                    modifier = Modifier.padding(vertical = 8.dp, horizontal = 2.dp),
                                                    textAlign = TextAlign.Center
                                                )
                                            }
                                        }
                                    }
                                }
                                item {
                                    // Mahaia esleitu — Lerro bakarrean horizontalki mugikorra ("Auto-Esleitu" inoiz ez mozteko!)
                                    Text(
                                        text = "Mahaia (Aukeratu edo Utzi Auto-Esleitu):",
                                        fontSize = 12.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = AmberAccent
                                    )
                                    Spacer(modifier = Modifier.height(6.dp))
                                    LazyRow(
                                        modifier = Modifier.fillMaxWidth(),
                                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                                    ) {
                                        item {
                                            Surface(
                                                modifier = Modifier.clickable { hautatutakoMahaia = null },
                                                shape = RoundedCornerShape(8.dp),
                                                color = if (hautatutakoMahaia == null) AmberAccent else MaterialTheme.colorScheme.surfaceVariant,
                                                border = BorderStroke(
                                                    1.dp,
                                                    if (hautatutakoMahaia == null) GoldBorder else Color.Transparent
                                                )
                                            ) {
                                                Text(
                                                    text = "Auto-Esleitu",
                                                    fontSize = 12.sp,
                                                    fontWeight = FontWeight.Bold,
                                                    color = if (hautatutakoMahaia == null) Color.Black else MaterialTheme.colorScheme.onSurface,
                                                    maxLines = 1,
                                                    softWrap = false,
                                                    modifier = Modifier.padding(horizontal = 14.dp, vertical = 9.dp)
                                                )
                                            }
                                        }
                                        items(mahaiak, key = { it.id }) { m ->
                                            val sel = hautatutakoMahaia?.id == m.id
                                            Surface(
                                                modifier = Modifier.clickable { hautatutakoMahaia = m },
                                                shape = RoundedCornerShape(8.dp),
                                                color = if (sel) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant,
                                                border = BorderStroke(1.dp, if (sel) GoldBorder else Color.Transparent)
                                            ) {
                                                Text(
                                                    text = "M.${m.zenbakia} (${m.edukiera}p)",
                                                    fontSize = 12.sp,
                                                    fontWeight = if (sel) FontWeight.Bold else FontWeight.Normal,
                                                    color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface,
                                                    maxLines = 1,
                                                    softWrap = false,
                                                    modifier = Modifier.padding(horizontal = 12.dp, vertical = 9.dp)
                                                )
                                            }
                                        }
                                    }
                                }
                                item {
                                    OutlinedTextField(
                                        value = oharrak,
                                        onValueChange = { oharrak = it },
                                        label = { Text("Oharrak (Alergiak, VIP...)") },
                                        modifier = Modifier.fillMaxWidth()
                                    )
                                }
                            }

                            if (errorea != null) {
                                Spacer(modifier = Modifier.height(6.dp))
                                Text(errorea!!, color = StatusBusy, fontSize = 12.sp)
                            }
                            if (mezua != null) {
                                Spacer(modifier = Modifier.height(6.dp))
                                Text(mezua!!, color = StatusFree, fontSize = 12.sp)
                            }

                            Spacer(modifier = Modifier.height(8.dp))

                            // Gorde Botoia
                            Button(
                                onClick = {
                                    if (bezeroIzena.isBlank() || bezeroEmaila.isBlank()) {
                                        errorea = "Izena eta Emaila derrigorrezkoak dira."
                                        return@Button
                                    }
                                    if (hautatutakoEguna.dayOfWeek == DayOfWeek.MONDAY) {
                                        errorea = "Astelehenetan jatetxea itxita dago. Aukeratu beste egun bat egutegian."
                                        return@Button
                                    }
                                    scope.launch {
                                        gordetzen = true
                                        errorea = null
                                        mezua = null
                                        try {
                                            val req = ErreserbaSortuDto(
                                                mahaiaId = hautatutakoMahaia?.id,
                                                mahaiZenbakia = hautatutakoMahaia?.zenbakia,
                                                data = hautatutakoDataStr,
                                                txanda = txanda,
                                                pertsonaKopurua = paxKop.toIntOrNull() ?: 2,
                                                oharrak = oharrak.ifBlank { null },
                                                bezeroIzena = bezeroIzena.trim(),
                                                bezeroAbizena = "-",
                                                bezeroEmaila = bezeroEmaila.trim().lowercase(),
                                                bezeroTelefonoa = bezeroTelefonoa.ifBlank { "-" }
                                            )
                                            val res = ApiClient.service.sortuErreserba(req)
                                            if (res.isSuccessful && res.body()?.data != null) {
                                                mezua = "Erreserba ongi sortu da!"
                                                bezeroIzena = ""
                                                bezeroEmaila = ""
                                                bezeroTelefonoa = ""
                                                oharrak = ""
                                                onErreserbaSortua()
                                                fitxaAktiboa = "ZERRENDA"
                                                kargatuErreserbak()
                                            } else {
                                                errorea = res.body()?.message ?: "Errorea erreserba sortzean (${res.code()})"
                                            }
                                        } catch (e: Exception) {
                                            errorea = "Errorea: ${e.localizedMessage}"
                                        } finally {
                                            gordetzen = false
                                        }
                                    }
                                },
                                modifier = Modifier.fillMaxWidth(),
                                colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                                shape = RoundedCornerShape(10.dp),
                                enabled = !gordetzen
                            ) {
                                if (gordetzen) {
                                    CircularProgressIndicator(color = Color.White, modifier = Modifier.size(20.dp))
                                } else {
                                    Text("Baieztatu eta Gorde Erreserba", color = Color.White, fontWeight = FontWeight.Bold)
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun HilabetekoEgutegia(
    hilabetea: YearMonth,
    hautatutakoEguna: LocalDate,
    gaur: LocalDate,
    erreserbaKopuruak: Map<String, Int>,
    onHilabeteaAldatu: (YearMonth) -> Unit,
    onEgunaAukeratua: (LocalDate) -> Unit,
    modifier: Modifier = Modifier
) {
    val hilabeteIzenak = listOf(
        "Urtarrila", "Otsaila", "Martxoa", "Apirila", "Maiatza", "Ekaina",
        "Uztaila", "Abuztua", "Iraila", "Urria", "Azaroa", "Abendua"
    )
    val astegunak = listOf("Al", "As", "Az", "Og", "Or", "Lr", "Ig")

    val lehenEguna = hilabetea.atDay(1)
    val hutsuneak = lehenEguna.dayOfWeek.value - 1 // Astelehena = 0
    val egunKopurua = hilabetea.lengthOfMonth()
    val gelaxkak = List(hutsuneak) { null } + (1..egunKopurua).map { hilabetea.atDay(it) }
    val asteak = gelaxkak.chunked(7)

    Surface(
        modifier = modifier.fillMaxWidth(),
        shape = RoundedCornerShape(14.dp),
        color = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.45f),
        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.25f))
    ) {
        Column(modifier = Modifier.padding(10.dp)) {
            // Hilabetea eta nabigazio geziak
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                IconButton(
                    onClick = { onHilabeteaAldatu(hilabetea.minusMonths(1)) },
                    modifier = Modifier.size(32.dp)
                ) {
                    Icon(
                        imageVector = Icons.AutoMirrored.Filled.KeyboardArrowLeft,
                        contentDescription = "Aurreko hilabetea",
                        tint = MaterialTheme.colorScheme.onSurface
                    )
                }

                Text(
                    text = "${hilabeteIzenak[hilabetea.monthValue - 1]} ${hilabetea.year}",
                    fontSize = 15.sp,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.onSurface
                )

                IconButton(
                    onClick = { onHilabeteaAldatu(hilabetea.plusMonths(1)) },
                    modifier = Modifier.size(32.dp)
                ) {
                    Icon(
                        imageVector = Icons.AutoMirrored.Filled.KeyboardArrowRight,
                        contentDescription = "Hurrengo hilabetea",
                        tint = MaterialTheme.colorScheme.onSurface
                    )
                }
            }

            Spacer(modifier = Modifier.height(6.dp))

            // Astegunen goiburuak
            Row(modifier = Modifier.fillMaxWidth()) {
                astegunak.forEachIndexed { idx, eguna ->
                    Text(
                        text = eguna,
                        modifier = Modifier.weight(1f),
                        textAlign = TextAlign.Center,
                        fontSize = 11.sp,
                        fontWeight = FontWeight.Bold,
                        color = if (idx == 0) StatusBusy.copy(alpha = 0.7f) else MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }

            Spacer(modifier = Modifier.height(4.dp))

            // Egunen sarea (astez aste)
            asteak.forEach { astea ->
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(vertical = 2.dp)
                ) {
                    for (i in 0 until 7) {
                        val eguna = astea.getOrNull(i)
                        if (eguna == null) {
                            Spacer(modifier = Modifier.weight(1f))
                        } else {
                            val dataStr = eguna.format(DateTimeFormatter.ISO_LOCAL_DATE)
                            val erreserbaKop = erreserbaKopuruak[dataStr] ?: 0
                            val hautatua = eguna == hautatutakoEguna
                            val daGaur = eguna == gaur
                            val daAstelehena = eguna.dayOfWeek == DayOfWeek.MONDAY

                            val bgKolorea by animateColorAsState(
                                targetValue = when {
                                    hautatua -> ChiantiPrimary
                                    erreserbaKop > 0 -> AmberAccent.copy(alpha = 0.2f)
                                    else -> Color.Transparent
                                },
                                animationSpec = tween(200),
                                label = "calDayBg$dataStr"
                            )

                            Box(
                                modifier = Modifier
                                    .weight(1f)
                                    .height(38.dp)
                                    .padding(horizontal = 2.dp)
                                    .clip(RoundedCornerShape(10.dp))
                                    .background(bgKolorea)
                                    .then(
                                        if (daGaur && !hautatua) {
                                            Modifier.border(1.5.dp, ChiantiPrimary, RoundedCornerShape(10.dp))
                                        } else if (erreserbaKop > 0 && !hautatua) {
                                            Modifier.border(1.dp, AmberAccent.copy(alpha = 0.7f), RoundedCornerShape(10.dp))
                                        } else Modifier
                                    )
                                    .clickable { onEgunaAukeratua(eguna) },
                                contentAlignment = Alignment.Center
                            ) {
                                Column(horizontalAlignment = Alignment.CenterHorizontally) {
                                    Text(
                                        text = "${eguna.dayOfMonth}",
                                        fontSize = 12.sp,
                                        fontWeight = if (hautatua || daGaur || erreserbaKop > 0) FontWeight.Bold else FontWeight.Normal,
                                        color = when {
                                            hautatua -> Color.White
                                            daAstelehena -> MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.45f)
                                            else -> MaterialTheme.colorScheme.onSurface
                                        }
                                    )
                                    if (erreserbaKop > 0) {
                                        Box(
                                            modifier = Modifier
                                                .size(5.dp)
                                                .clip(CircleShape)
                                                .background(if (hautatua) GoldBorder else StatusFree)
                                        )
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Spacer(modifier = Modifier.height(4.dp))

            // Legenda laburra
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(12.dp, Alignment.CenterHorizontally),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(4.dp)) {
                    Box(modifier = Modifier.size(7.dp).clip(CircleShape).background(StatusFree))
                    Text("Erreserbak", fontSize = 10.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                }
                Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(4.dp)) {
                    Box(
                        modifier = Modifier
                            .size(9.dp)
                            .clip(RoundedCornerShape(3.dp))
                            .border(1.5.dp, ChiantiPrimary, RoundedCornerShape(3.dp))
                    )
                    Text("Gaur", fontSize = 10.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                }
                Text("Al = Itxita", fontSize = 10.sp, color = StatusBusy.copy(alpha = 0.8f))
            }
        }
    }
}

@Composable
fun TabItem(
    titulua: String,
    aktibo: Boolean,
    modifier: Modifier = Modifier,
    onClick: () -> Unit
) {
    Surface(
        modifier = modifier
            .clip(RoundedCornerShape(8.dp))
            .clickable(onClick = onClick),
        color = if (aktibo) ChiantiPrimary else Color.Transparent
    ) {
        Text(
            text = titulua,
            fontSize = 12.sp,
            fontWeight = if (aktibo) FontWeight.Bold else FontWeight.Medium,
            color = if (aktibo) Color.White else MaterialTheme.colorScheme.onSurfaceVariant,
            maxLines = 1,
            softWrap = false,
            modifier = Modifier.padding(vertical = 8.dp),
            textAlign = TextAlign.Center
        )
    }
}

@Composable
fun ErreserbaKarta(erreserba: ErreserbaDto) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.55f)),
        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.25f))
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(6.dp)) {
                    Text(
                        text = erreserba.bezeroIzena,
                        fontSize = 14.sp,
                        fontWeight = FontWeight.Bold,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                    Surface(
                        color = AmberAccent.copy(alpha = 0.2f),
                        shape = RoundedCornerShape(4.dp)
                    ) {
                        Text(
                            text = "${erreserba.pertsonaKopurua} pax",
                            fontSize = 10.sp,
                            fontWeight = FontWeight.Bold,
                            color = AmberAccent,
                            modifier = Modifier.padding(horizontal = 5.dp, vertical = 2.dp)
                        )
                    }
                }
                Spacer(modifier = Modifier.height(3.dp))
                Text(
                    text = "Mahaia ${erreserba.mahaiZenbakia} (${erreserba.kokapena}) · ${erreserba.txanda}",
                    fontSize = 12.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                if (erreserba.bezeroTelefonoa.isNotBlank() && erreserba.bezeroTelefonoa != "-") {
                    Text(
                        text = "Tel: ${erreserba.bezeroTelefonoa} · ${erreserba.bezeroEmaila}",
                        fontSize = 11.sp,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
                if (!erreserba.oharrak.isNullOrBlank()) {
                    Text(
                        text = "Oharra: ${erreserba.oharrak}",
                        fontSize = 11.sp,
                        color = AmberAccent
                    )
                }
            }

            Spacer(modifier = Modifier.width(8.dp))

            Surface(
                color = StatusFree.copy(alpha = 0.15f),
                shape = RoundedCornerShape(8.dp),
                border = BorderStroke(1.dp, StatusFree)
            ) {
                Text(
                    text = erreserba.egoera,
                    color = StatusFree,
                    fontSize = 10.sp,
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.padding(horizontal = 6.dp, vertical = 3.dp)
                )
            }
        }
    }
}

@Preview(showBackground = true, name = "HilabetekoEgutegia - Argia")
@Composable
fun HilabetekoEgutegiaPreview() {
    val gaur = LocalDate.of(2026, 11, 24)
    KomanderaTheme(darkTheme = false) {
        Surface(color = MaterialTheme.colorScheme.background, modifier = Modifier.padding(12.dp)) {
            HilabetekoEgutegia(
                hilabetea = YearMonth.of(2026, 11),
                hautatutakoEguna = gaur,
                gaur = gaur,
                erreserbaKopuruak = mapOf("2026-11-24" to 2, "2026-11-27" to 1, "2026-11-28" to 3),
                onHilabeteaAldatu = {},
                onEgunaAukeratua = {}
            )
        }
    }
}

@Preview(showBackground = true, name = "ErreserbaKarta")
@Composable
fun ErreserbaKartaPreview() {
    KomanderaTheme(darkTheme = false) {
        Surface(color = MaterialTheme.colorScheme.background, modifier = Modifier.padding(12.dp)) {
            ErreserbaKarta(
                erreserba = ErreserbaDto(
                    id = 1,
                    bezeroIzena = "Andoni",
                    bezeroEmaila = "andoni@gmail.com",
                    bezeroTelefonoa = "666666666",
                    mahaiZenbakia = 2,
                    kokapena = "Jantoki Nagusia",
                    txanda = "Bazkaria_1",
                    pertsonaKopurua = 2,
                    egoera = "Baieztatuta",
                    oharrak = "Glutenik gabeko menua"
                )
            )
        }
    }
}
