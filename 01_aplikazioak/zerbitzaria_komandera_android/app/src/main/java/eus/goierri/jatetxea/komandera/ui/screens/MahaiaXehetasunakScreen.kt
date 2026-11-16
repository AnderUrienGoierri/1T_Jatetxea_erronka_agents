package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.automirrored.filled.Send
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.*
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import java.util.Locale

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MahaiaXehetasunakScreen(
    mahaiId: Int,
    zerbitzaria: LangileaDto,
    onGehituPlaterak: (eskaeraId: Int, mahaiId: Int) -> Unit,
    onAtzera: () -> Unit,
    modifier: Modifier = Modifier
) {
    val scope = rememberCoroutineScope()
    var mahaia by remember { mutableStateOf<MahaiaDto?>(null) }
    var eskaeraAktiboa by remember { mutableStateOf<EskaeraDto?>(null) }
    var kargatzen by remember { mutableStateOf(true) }
    var erroreMezua by remember { mutableStateOf<String?>(null) }
    var ekintzanAri by remember { mutableStateOf(false) }

    // Dialog egoerak
    var sortuEskaeraDialogIrekita by remember { mutableStateOf(false) }
    var pertsonaKopuruaInput by remember { mutableStateOf("2") }

    var ordainketaDialogIrekita by remember { mutableStateOf(false) }
    var tiketModalIrekita by remember { mutableStateOf(false) }
    var azkenTiketa by remember { mutableStateOf<OrdainketaErantzunaDto?>(null) }

    fun kargatuDatuak(erakutsiKargatzen: Boolean = true) {
        scope.launch {
            if (erakutsiKargatzen) kargatzen = true
            try {
                // 1. Mahaiak kargatu
                val mahaiRes = ApiClient.service.getMahaiak()
                if (mahaiRes.isSuccessful && mahaiRes.body()?.dataList != null) {
                    val aurkitua = mahaiRes.body()!!.dataList!!.find { it.id == mahaiId }
                    if (aurkitua != null) {
                        mahaia = aurkitua
                    }
                }

                // 2. Mahaiaren eskaera aktiboa lortu
                val eskaeraRes = ApiClient.service.getEskaerakMahaia(mahaiId)
                if (eskaeraRes.isSuccessful && eskaeraRes.body()?.dataList != null) {
                    val eskaerak = eskaeraRes.body()!!.dataList!!
                    val aktiboa = eskaerak.firstOrNull { it.egoera != "Kobratua" && it.egoera != "Ezeztatua" }
                    if (aktiboa != null) {
                        // Xehetasun osoak lerroekin lortu
                        val xehRes = ApiClient.service.getEskaeraXehetasunak(aktiboa.id)
                        if (xehRes.isSuccessful && xehRes.body()?.data != null) {
                            eskaeraAktiboa = xehRes.body()!!.data
                        } else {
                            eskaeraAktiboa = aktiboa
                        }
                    } else {
                        eskaeraAktiboa = null
                    }
                }
                erroreMezua = null
            } catch (e: Exception) {
                if (mahaia == null) {
                    erroreMezua = "Konexio errorea: ${e.localizedMessage ?: "Ezin izan da kargatu"}"
                }
            } finally {
                kargatzen = false
            }
        }
    }

    LaunchedEffect(mahaiId) {
        kargatuDatuak(erakutsiKargatzen = true)
        while (true) {
            delay(3500)
            if (!ekintzanAri && !ordainketaDialogIrekita) {
                kargatuDatuak(erakutsiKargatzen = false)
            }
        }
    }

    fun aldatuMahaiarenEgoera(egoeraBerria: String) {
        scope.launch {
            ekintzanAri = true
            try {
                val res = ApiClient.service.eguneratuMahaiarenEgoera(mahaiId, EgoeraEguneratuRequest(egoeraBerria))
                if (res.isSuccessful) {
                    kargatuDatuak(erakutsiKargatzen = false)
                }
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    fun sortuEskaeraBerria() {
        val pax = pertsonaKopuruaInput.toIntOrNull() ?: 2
        scope.launch {
            ekintzanAri = true
            try {
                val req = SortuEskaeraRequest(
                    mahaiaId = mahaiId,
                    langileaId = zerbitzaria.id,
                    pertsonaKopurua = pax
                )
                val res = ApiClient.service.sortuEskaera(req)
                if (res.isSuccessful && res.body()?.data != null) {
                    sortuEskaeraDialogIrekita = false
                    kargatuDatuak(erakutsiKargatzen = false)
                }
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    fun bidaliSukaldera() {
        val esk = eskaeraAktiboa ?: return
        scope.launch {
            ekintzanAri = true
            try {
                val res = ApiClient.service.bidaliSukaldera(esk.id)
                if (res.isSuccessful) {
                    kargatuDatuak(erakutsiKargatzen = false)
                }
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    fun aldatuLerroEgoera(lerroId: Int, egoeraBerria: String) {
        scope.launch {
            ekintzanAri = true
            try {
                val res = ApiClient.service.eguneratuLerroEgoera(lerroId, EgoeraEguneratuRequest(egoeraBerria))
                if (res.isSuccessful) {
                    kargatuDatuak(erakutsiKargatzen = false)
                }
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    val unekoMahaia = mahaia
    val mahaiaZenb = unekoMahaia?.zenbakia ?: mahaiId

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "Mahaia $mahaiaZenb - Xehetasunak",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = TextPrimary
                        )
                        Text(
                            text = "${unekoMahaia?.kokapena ?: ""} · Zerbitzaria: ${zerbitzaria.izena}",
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
                actions = {
                    IconButton(onClick = { kargatuDatuak(erakutsiKargatzen = true) }) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = "Freskatu",
                            tint = AmberPrimary
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = DarkSurface)
            )
        },
        containerColor = DarkBackground,
        bottomBar = {
            // Beheko ekintza barrak (Sukaldera bidali, Gehitu platerak, Kobratu)
            if (eskaeraAktiboa != null) {
                val eskaera = eskaeraAktiboa!!
                Surface(
                    color = DarkSurface,
                    tonalElevation = 8.dp,
                    shadowElevation = 8.dp
                ) {
                    Column(modifier = Modifier.padding(14.dp)) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween,
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Column {
                                Text(
                                    text = "GUZTIRA (${eskaera.lerroak.sumOf { it.kantitatea }} plater)",
                                    fontSize = 11.sp,
                                    color = TextSecondary,
                                    fontWeight = FontWeight.SemiBold
                                )
                                Text(
                                    text = String.format(Locale.ROOT, "%.2f €", eskaera.guztira),
                                    fontSize = 22.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = AmberPrimary
                                )
                            }

                            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                                Button(
                                    onClick = { onGehituPlaterak(eskaera.id, mahaiId) },
                                    colors = ButtonDefaults.buttonColors(containerColor = DarkSurfaceVariant),
                                    shape = RoundedCornerShape(10.dp)
                                ) {
                                    Icon(imageVector = Icons.Default.Add, contentDescription = null, modifier = Modifier.size(16.dp))
                                    Spacer(modifier = Modifier.width(4.dp))
                                    Text("Platerak", fontSize = 13.sp)
                                }

                                Button(
                                    onClick = { ordainketaDialogIrekita = true },
                                    colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                                    shape = RoundedCornerShape(10.dp)
                                ) {
                                    Icon(imageVector = Icons.Default.Check, contentDescription = null, modifier = Modifier.size(16.dp))
                                    Spacer(modifier = Modifier.width(4.dp))
                                    Text("Kobratu", fontSize = 13.sp, fontWeight = FontWeight.Bold, color = DarkBackground)
                                }
                            }
                        }

                        // Sukaldera bidaltzeko botoi handia (zai dauden lerroak badaude)
                        val zaiKop = eskaera.lerroak.count { it.egoera.equals("Irekia", ignoreCase = true) || it.egoera.equals("Eskatua", ignoreCase = true) }
                        if (zaiKop > 0) {
                            Spacer(modifier = Modifier.height(10.dp))
                            Button(
                                onClick = { bidaliSukaldera() },
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .height(44.dp),
                                colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary),
                                shape = RoundedCornerShape(10.dp),
                                enabled = !ekintzanAri
                            ) {
                                Icon(
                                    imageVector = Icons.AutoMirrored.Filled.Send,
                                    contentDescription = null,
                                    tint = DarkBackground,
                                    modifier = Modifier.size(18.dp)
                                )
                                Spacer(modifier = Modifier.width(8.dp))
                                Text(
                                    text = "Bidali Sukaldera ($zaiKop plater zain)",
                                    color = DarkBackground,
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 14.sp
                                )
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
            if (kargatzen && mahaia == null) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator(color = AmberPrimary)
                }
            } else if (unekoMahaia == null) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text(text = erroreMezua ?: "Ez da mahaia aurkitu.", color = StatusBusy)
                }
            } else {
                // MAHAIAREN EGOERA TXARTELA
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(containerColor = DarkSurface),
                    shape = RoundedCornerShape(16.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween,
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Column {
                                Text(
                                    text = "Mahaia ${unekoMahaia.zenbakia} (${unekoMahaia.kokapena})",
                                    fontSize = 18.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = TextPrimary
                                )
                                Text(
                                    text = "Edukiera: ${unekoMahaia.edukiera} pertsona",
                                    fontSize = 13.sp,
                                    color = TextSecondary
                                )
                            }

                            val egoeraKolorea = when (unekoMahaia.egoera.lowercase()) {
                                "libre" -> StatusFree
                                "okupatuta" -> StatusBusy
                                "erreserbatuta" -> StatusPending
                                "garbitzen" -> StatusCleaning
                                else -> StatusPending
                            }

                            Surface(
                                color = egoeraKolorea.copy(alpha = 0.15f),
                                shape = RoundedCornerShape(20.dp),
                                border = ButtonDefaults.outlinedButtonBorder.copy(brush = androidx.compose.ui.graphics.SolidColor(egoeraKolorea))
                            ) {
                                Text(
                                    text = unekoMahaia.egoera.uppercase(),
                                    color = egoeraKolorea,
                                    fontSize = 12.sp,
                                    fontWeight = FontWeight.Bold,
                                    modifier = Modifier.padding(horizontal = 12.dp, vertical = 6.dp)
                                )
                            }
                        }

                        Spacer(modifier = Modifier.height(14.dp))

                        // Mahaiaren egoera azkar aldatzeko botoiak (Libre, Okupatu, Garbitzen)
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(8.dp)
                        ) {
                            listOf("Libre", "Okupatuta", "Garbitzen").forEach { egoera ->
                                val aktibo = unekoMahaia.egoera.equals(egoera, ignoreCase = true)
                                OutlinedButton(
                                    onClick = { aldatuMahaiarenEgoera(egoera) },
                                    modifier = Modifier.weight(1f),
                                    colors = ButtonDefaults.outlinedButtonColors(
                                        containerColor = if (aktibo) AmberPrimary.copy(alpha = 0.2f) else Color.Transparent
                                    ),
                                    border = ButtonDefaults.outlinedButtonBorder.copy(
                                        brush = androidx.compose.ui.graphics.SolidColor(if (aktibo) AmberPrimary else TextMuted.copy(alpha = 0.4f))
                                    ),
                                    shape = RoundedCornerShape(8.dp)
                                ) {
                                    Text(
                                        text = egoera,
                                        fontSize = 11.sp,
                                        color = if (aktibo) AmberPrimary else TextSecondary,
                                        fontWeight = if (aktibo) FontWeight.Bold else FontWeight.Normal
                                    )
                                }
                            }
                        }
                    }
                }

                Spacer(modifier = Modifier.height(16.dp))

                // ESKAERAREN EDUKIA / LERROAK
                if (eskaeraAktiboa == null) {
                    Card(
                        modifier = Modifier
                            .fillMaxWidth()
                            .weight(1f),
                        colors = CardDefaults.cardColors(containerColor = DarkSurface),
                        shape = RoundedCornerShape(16.dp)
                    ) {
                        Column(
                            modifier = Modifier
                                .fillMaxSize()
                                .padding(24.dp),
                            horizontalAlignment = Alignment.CenterHorizontally,
                            verticalArrangement = Arrangement.Center
                        ) {
                            Icon(
                                imageVector = Icons.Default.RestaurantMenu,
                                contentDescription = null,
                                tint = TextMuted,
                                modifier = Modifier.size(48.dp)
                            )
                            Spacer(modifier = Modifier.height(12.dp))
                            Text(
                                text = "Ez dago eskaera aktiborik mahai honetan",
                                fontSize = 15.sp,
                                color = TextSecondary,
                                textAlign = TextAlign.Center
                            )
                            Spacer(modifier = Modifier.height(16.dp))
                            Button(
                                onClick = { sortuEskaeraDialogIrekita = true },
                                colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary),
                                shape = RoundedCornerShape(12.dp)
                            ) {
                                Icon(imageVector = Icons.Default.Add, contentDescription = null, tint = DarkBackground)
                                Spacer(modifier = Modifier.width(6.dp))
                                Text("Sortu Eskaera Berria", color = DarkBackground, fontWeight = FontWeight.Bold)
                            }
                        }
                    }
                } else {
                    val eskaera = eskaeraAktiboa!!
                    Card(
                        modifier = Modifier
                            .fillMaxWidth()
                            .weight(1f),
                        colors = CardDefaults.cardColors(containerColor = DarkSurface),
                        shape = RoundedCornerShape(16.dp)
                    ) {
                        Column(
                            modifier = Modifier
                                .fillMaxSize()
                                .padding(16.dp)
                        ) {
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Text(
                                    text = "Eskaera #${eskaera.id} · ${eskaera.pertsonaKopurua} pax",
                                    fontSize = 15.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = TextPrimary
                                )
                                Text(
                                    text = eskaera.sortzeData.ifEmpty { "Gaur" },
                                    fontSize = 11.sp,
                                    color = TextMuted
                                )
                            }

                            HorizontalDivider(
                                modifier = Modifier.padding(vertical = 10.dp),
                                color = DarkSurfaceVariant
                            )

                            if (eskaera.lerroak.isEmpty()) {
                                Box(
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .weight(1f),
                                    contentAlignment = Alignment.Center
                                ) {
                                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                                        Text(
                                            text = "Ez dago platerik eskaera honetan.",
                                            color = TextSecondary,
                                            fontSize = 13.sp
                                        )
                                        Spacer(modifier = Modifier.height(8.dp))
                                        Button(
                                            onClick = { onGehituPlaterak(eskaera.id, mahaiId) },
                                            colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary)
                                        ) {
                                            Text("Gehitu Platerak Katalogoatik", color = DarkBackground, fontWeight = FontWeight.Bold)
                                        }
                                    }
                                }
                            } else {
                                LazyColumn(
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .weight(1f),
                                    verticalArrangement = Arrangement.spacedBy(8.dp)
                                ) {
                                    items(eskaera.lerroak) { lerro ->
                                        EskaeraLerroElementua(
                                            lerro = lerro,
                                            onZerbitzatu = {
                                                aldatuLerroEgoera(lerro.id, "Zerbitzatua")
                                            }
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

    // ESKAERA BERRIA SORTZEKO MODALA (Pertsona kopurua hautatzeko)
    if (sortuEskaeraDialogIrekita) {
        AlertDialog(
            onDismissRequest = { sortuEskaeraDialogIrekita = false },
            title = {
                Text(
                    text = "Mahaia $mahaiaZenb - Eskaera Berria",
                    color = TextPrimary,
                    fontWeight = FontWeight.Bold
                )
            },
            text = {
                Column {
                    Text(
                        text = "Aukeratu mahaiko mahaikide kopurua (pax):",
                        fontSize = 14.sp,
                        color = TextSecondary
                    )
                    Spacer(modifier = Modifier.height(14.dp))
                    OutlinedTextField(
                        value = pertsonaKopuruaInput,
                        onValueChange = { if (it.all { ch -> ch.isDigit() }) pertsonaKopuruaInput = it },
                        label = { Text("Pertsona kopurua") },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = AmberPrimary,
                            unfocusedBorderColor = TextMuted,
                            focusedTextColor = TextPrimary,
                            unfocusedTextColor = TextPrimary
                        ),
                        modifier = Modifier.fillMaxWidth()
                    )
                    Spacer(modifier = Modifier.height(10.dp))
                    // Botoi azkarrak (2, 4, 6, 8)
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        listOf("1", "2", "4", "6", "8").forEach { n ->
                            OutlinedButton(
                                onClick = { pertsonaKopuruaInput = n },
                                modifier = Modifier.weight(1f),
                                shape = RoundedCornerShape(8.dp)
                            ) {
                                Text(n, fontSize = 12.sp)
                            }
                        }
                    }
                }
            },
            confirmButton = {
                Button(
                    onClick = { sortuEskaeraBerria() },
                    colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary),
                    enabled = !ekintzanAri
                ) {
                    Text("Ireki Eskaera", color = DarkBackground, fontWeight = FontWeight.Bold)
                }
            },
            dismissButton = {
                TextButton(onClick = { sortuEskaeraDialogIrekita = false }) {
                    Text("Utzi", color = TextSecondary)
                }
            },
            containerColor = DarkSurface
        )
    }

    // ORDAINKETA MODALA (Eskudirua, Txartela, Bizum + Kanbioaren kalkulua)
    if (ordainketaDialogIrekita && eskaeraAktiboa != null) {
        val eskaera = eskaeraAktiboa!!
        var hautatutakoMota by remember { mutableStateOf("Eskudirua") }
        var jasotakoaText by remember { mutableStateOf(String.format(Locale.ROOT, "%.2f", eskaera.guztira)) }
        var ordaintzen by remember { mutableStateOf(false) }
        var ordainketaErrorea by remember { mutableStateOf<String?>(null) }

        val jasotakoaBalioa = jasotakoaText.toDoubleOrNull() ?: 0.0
        val kanbioa = (jasotakoaBalioa - eskaera.guztira).coerceAtLeast(0.0)

        Dialog(onDismissRequest = { if (!ordaintzen) ordainketaDialogIrekita = false }) {
            Surface(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(4.dp),
                shape = RoundedCornerShape(16.dp),
                color = DarkSurface,
                tonalElevation = 8.dp
            ) {
                Column(
                    modifier = Modifier.padding(16.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            Text(
                                text = "Kontua Kobratu",
                                fontSize = 18.sp,
                                fontWeight = FontWeight.Bold,
                                color = TextPrimary
                            )
                            Text(
                                text = "Mahaia $mahaiaZenb · Eskaera #${eskaera.id}",
                                fontSize = 12.sp,
                                color = TextSecondary
                            )
                        }

                        IconButton(onClick = { ordainketaDialogIrekita = false }, enabled = !ordaintzen) {
                            Icon(imageVector = Icons.Default.Close, contentDescription = null, tint = TextSecondary)
                        }
                    }

                    Spacer(modifier = Modifier.height(12.dp))

                    // Zenbateko osoa nabarmenduta
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(containerColor = DarkBackground),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Column(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(14.dp),
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Text(text = "ORDAINDU BEHARREKOA", fontSize = 11.sp, color = TextSecondary)
                            Text(
                                text = String.format(Locale.ROOT, "%.2f €", eskaera.guztira),
                                fontSize = 28.sp,
                                fontWeight = FontWeight.Bold,
                                color = AmberPrimary
                            )
                            val subtotal = eskaera.guztira / 1.10
                            val bez = eskaera.guztira - subtotal
                            Text(
                                text = String.format(Locale.ROOT, "BEZ (%10) barne: %.2f €", bez),
                                fontSize = 11.sp,
                                color = TextMuted
                            )
                        }
                    }

                    Spacer(modifier = Modifier.height(12.dp))

                    // Ordainketa motaren fitxak (Eskudirua, Txartela, Bizum)
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        listOf("Eskudirua", "Txartela", "Bizum").forEach { mota ->
                            val aktibo = hautatutakoMota == mota
                            Button(
                                onClick = { hautatutakoMota = mota },
                                modifier = Modifier.weight(1f),
                                colors = ButtonDefaults.buttonColors(
                                    containerColor = if (aktibo) AmberPrimary else DarkSurfaceVariant
                                ),
                                shape = RoundedCornerShape(8.dp),
                                contentPadding = PaddingValues(vertical = 8.dp)
                            ) {
                                Text(
                                    text = mota,
                                    fontSize = 12.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = if (aktibo) DarkBackground else TextPrimary
                                )
                            }
                        }
                    }

                    Spacer(modifier = Modifier.height(12.dp))

                    if (hautatutakoMota == "Eskudirua") {
                        Column(modifier = Modifier.fillMaxWidth()) {
                            Text(text = "Jasotako Eskudirua (€):", fontSize = 12.sp, color = TextSecondary)
                            Spacer(modifier = Modifier.height(6.dp))
                            OutlinedTextField(
                                value = jasotakoaText,
                                onValueChange = { jasotakoaText = it },
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                                colors = OutlinedTextFieldDefaults.colors(
                                    focusedBorderColor = AmberPrimary,
                                    unfocusedBorderColor = TextMuted,
                                    focusedTextColor = TextPrimary,
                                    unfocusedTextColor = TextPrimary
                                ),
                                modifier = Modifier.fillMaxWidth()
                            )

                            Spacer(modifier = Modifier.height(8.dp))

                            // Billete azkarrak
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.spacedBy(6.dp)
                            ) {
                                listOf(
                                    "Zehatza" to eskaera.guztira,
                                    "10€" to 10.0,
                                    "20€" to 20.0,
                                    "50€" to 50.0,
                                    "100€" to 100.0
                                ).forEach { (etiketa, balioa) ->
                                    OutlinedButton(
                                        onClick = {
                                            jasotakoaText = String.format(Locale.ROOT, "%.2f", balioa)
                                        },
                                        modifier = Modifier.weight(1f),
                                        shape = RoundedCornerShape(6.dp),
                                        contentPadding = PaddingValues(horizontal = 2.dp, vertical = 4.dp)
                                    ) {
                                        Text(etiketa, fontSize = 10.sp)
                                    }
                                }
                            }

                            Spacer(modifier = Modifier.height(10.dp))

                            // Itzuli beharreko kanbioa
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .clip(RoundedCornerShape(8.dp))
                                    .background(DarkBackground)
                                    .padding(horizontal = 12.dp, vertical = 8.dp),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Text(text = "Itzuli beharrekoa (Kanbioa):", fontSize = 13.sp, color = TextSecondary)
                                Text(
                                    text = String.format(Locale.ROOT, "%.2f €", kanbioa),
                                    fontSize = 18.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = if (jasotakoaBalioa >= eskaera.guztira) StatusFree else StatusBusy
                                )
                            }
                        }
                    } else if (hautatutakoMota == "Txartela") {
                        Card(
                            modifier = Modifier.fillMaxWidth(),
                            colors = CardDefaults.cardColors(containerColor = DarkBackground),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Column(modifier = Modifier.padding(12.dp)) {
                                Text("POS Datofonoa Konektatuta", color = StatusFree, fontSize = 13.sp, fontWeight = FontWeight.Bold)
                                Spacer(modifier = Modifier.height(4.dp))
                                Text("Txartela hurbildu edo txertatu kontakturik gabeko ordainketa egiteko.", color = TextSecondary, fontSize = 11.sp)
                            }
                        }
                    } else {
                        // Bizum
                        Card(
                            modifier = Modifier.fillMaxWidth(),
                            colors = CardDefaults.cardColors(containerColor = DarkBackground),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Column(modifier = Modifier.padding(12.dp)) {
                                Text("Bizum Ordainketa Instantaneoa", color = AmberPrimary, fontSize = 13.sp, fontWeight = FontWeight.Bold)
                                Spacer(modifier = Modifier.height(4.dp))
                                Text("Bezeroak Bizum bidez ordaindu du jatetxeko zenbakira.", color = TextSecondary, fontSize = 11.sp)
                            }
                        }
                    }

                    if (ordainketaErrorea != null) {
                        Spacer(modifier = Modifier.height(8.dp))
                        Text(text = ordainketaErrorea!!, color = StatusBusy, fontSize = 12.sp)
                    }

                    Spacer(modifier = Modifier.height(16.dp))

                    Button(
                        onClick = {
                            if (hautatutakoMota == "Eskudirua" && jasotakoaBalioa < eskaera.guztira) {
                                ordainketaErrorea = "Jasotako zenbatekoa ez da nahikoa kontua ordaintzeko."
                                return@Button
                            }

                            scope.launch {
                                ordaintzen = true
                                ordainketaErrorea = null
                                try {
                                    val ordReq = OrdainketaEskaeraDto(
                                        eskaeraId = eskaera.id,
                                        mota = hautatutakoMota,
                                        zenbatekoa = eskaera.guztira,
                                        jasotakoa = if (hautatutakoMota == "Eskudirua") jasotakoaBalioa else eskaera.guztira,
                                        langileaId = zerbitzaria.id
                                    )
                                    val res = ApiClient.service.prozesatuOrdainketa(ordReq)
                                    if (res.isSuccessful && res.body()?.data != null) {
                                        azkenTiketa = res.body()!!.data
                                        ordainketaDialogIrekita = false
                                        tiketModalIrekita = true
                                        // Mahaia libre edo garbitzen jarri
                                        ApiClient.service.eguneratuMahaiarenEgoera(mahaiId, EgoeraEguneratuRequest("Libre"))
                                        kargatuDatuak(erakutsiKargatzen = false)
                                    } else {
                                        ordainketaErrorea = res.body()?.message ?: "Ezin izan da ordainketa burutu."
                                    }
                                } catch (e: Exception) {
                                    ordainketaErrorea = "Errorea ordaintzean: ${e.localizedMessage}"
                                } finally {
                                    ordaintzen = false
                                }
                            }
                        },
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(50.dp),
                        colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary),
                        shape = RoundedCornerShape(10.dp),
                        enabled = !ordaintzen
                    ) {
                        if (ordaintzen) {
                            CircularProgressIndicator(color = DarkBackground, modifier = Modifier.size(20.dp))
                        } else {
                            Text("Ordainketa Burutu eta Tiketa Sortu", color = DarkBackground, fontWeight = FontWeight.Bold)
                        }
                    }
                }
            }
        }
    }

    // 80MM TIKETAREN BISTARATZE MODALA (Inprimaketa simulazioa)
    if (tiketModalIrekita && azkenTiketa != null) {
        val t = azkenTiketa!!
        Dialog(onDismissRequest = { tiketModalIrekita = false }) {
            Surface(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(8.dp),
                shape = RoundedCornerShape(12.dp),
                color = Color.White
            ) {
                Column(
                    modifier = Modifier.padding(20.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Text(
                        text = "1T JATETXEA",
                        fontSize = 18.sp,
                        fontWeight = FontWeight.Bold,
                        color = Color.Black,
                        fontFamily = FontFamily.Monospace
                    )
                    Text(
                        text = "Goierri Kalea 1, Ordizia",
                        fontSize = 11.sp,
                        color = Color.DarkGray,
                        fontFamily = FontFamily.Monospace
                    )
                    Text(
                        text = "IFZ: B-20123456 · Tel: 943 12 34 56",
                        fontSize = 10.sp,
                        color = Color.DarkGray,
                        fontFamily = FontFamily.Monospace
                    )

                    Spacer(modifier = Modifier.height(10.dp))
                    Text(
                        text = "--------------------------------",
                        color = Color.Gray,
                        fontFamily = FontFamily.Monospace
                    )
                    Text(
                        text = "TIKET ZENBAKIA: ${t.tiketZenbakia}",
                        fontSize = 13.sp,
                        fontWeight = FontWeight.Bold,
                        color = Color.Black,
                        fontFamily = FontFamily.Monospace
                    )
                    Text(
                        text = "Mahaia $mahaiaZenb · Zerbitzaria: ${zerbitzaria.izena}",
                        fontSize = 11.sp,
                        color = Color.DarkGray,
                        fontFamily = FontFamily.Monospace
                    )
                    Text(
                        text = "Mota: ${t.mota}",
                        fontSize = 11.sp,
                        color = Color.DarkGray,
                        fontFamily = FontFamily.Monospace
                    )
                    Text(
                        text = "--------------------------------",
                        color = Color.Gray,
                        fontFamily = FontFamily.Monospace
                    )

                    Spacer(modifier = Modifier.height(6.dp))

                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween
                    ) {
                        Text("GUZTIRA:", fontSize = 16.sp, fontWeight = FontWeight.Bold, color = Color.Black, fontFamily = FontFamily.Monospace)
                        Text(String.format(Locale.ROOT, "%.2f €", t.zenbatekoa), fontSize = 16.sp, fontWeight = FontWeight.Bold, color = Color.Black, fontFamily = FontFamily.Monospace)
                    }

                    if (t.jasotakoa != null) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text("Jasotakoa:", fontSize = 12.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                            Text(String.format(Locale.ROOT, "%.2f €", t.jasotakoa), fontSize = 12.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                        }
                    }

                    if (t.itzulitakoa != null) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text("Itzulitakoa (Kanbioa):", fontSize = 12.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                            Text(String.format(Locale.ROOT, "%.2f €", t.itzulitakoa), fontSize = 12.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                        }
                    }

                    Spacer(modifier = Modifier.height(10.dp))
                    Text(
                        text = "Eskerrik asko zure bisitagatik!",
                        fontSize = 11.sp,
                        color = Color.DarkGray,
                        fontFamily = FontFamily.Monospace,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(16.dp))

                    Button(
                        onClick = {
                            tiketModalIrekita = false
                            onAtzera()
                        },
                        colors = ButtonDefaults.buttonColors(containerColor = Color.Black),
                        shape = RoundedCornerShape(8.dp),
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text("Itxi eta Mahaietara Itzuli", color = Color.White, fontWeight = FontWeight.Bold)
                    }
                }
            }
        }
    }
}

@Composable
fun EskaeraLerroElementua(
    lerro: EskaeraLerroaDto,
    onZerbitzatu: () -> Unit
) {
    val egoeraKolorea = when (lerro.egoera.lowercase()) {
        "irekia", "eskatua" -> StatusPending
        "sukaldean" -> AmberPrimary
        "prestatua" -> StatusFree
        "zerbitzatua" -> TextMuted
        else -> TextSecondary
    }

    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = DarkSurfaceVariant),
        shape = RoundedCornerShape(10.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Kopurua karratutxo batean
            Box(
                modifier = Modifier
                    .size(32.dp)
                    .clip(RoundedCornerShape(6.dp))
                    .background(DarkBackground),
                contentAlignment = Alignment.Center
            ) {
                Text(
                    text = "${lerro.kantitatea}x",
                    fontSize = 13.sp,
                    fontWeight = FontWeight.Bold,
                    color = AmberPrimary
                )
            }

            Spacer(modifier = Modifier.width(12.dp))

            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = lerro.platerIzena,
                    fontSize = 14.sp,
                    fontWeight = FontWeight.SemiBold,
                    color = TextPrimary
                )
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Text(
                        text = "${lerro.kategoria} · ",
                        fontSize = 11.sp,
                        color = TextMuted
                    )
                    Text(
                        text = lerro.egoera,
                        fontSize = 11.sp,
                        fontWeight = FontWeight.Bold,
                        color = egoeraKolorea
                    )
                }
                if (!lerro.oharrak.isNullOrBlank()) {
                    Text(
                        text = "Oharra: ${lerro.oharrak}",
                        fontSize = 11.sp,
                        color = AmberPrimary
                    )
                }
            }

            Column(horizontalAlignment = Alignment.End) {
                Text(
                    text = String.format(Locale.ROOT, "%.2f €", lerro.azpitotala),
                    fontSize = 14.sp,
                    fontWeight = FontWeight.Bold,
                    color = TextPrimary
                )

                // Sukaldeak prestatu badu, Zerbitzatzeko botoia!
                if (lerro.egoera.equals("Prestatua", ignoreCase = true)) {
                    Spacer(modifier = Modifier.height(4.dp))
                    Button(
                        onClick = onZerbitzatu,
                        colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                        shape = RoundedCornerShape(6.dp),
                        contentPadding = PaddingValues(horizontal = 8.dp, vertical = 2.dp),
                        modifier = Modifier.height(28.dp)
                    ) {
                        Text("Zerbitzatu", fontSize = 10.sp, color = DarkBackground, fontWeight = FontWeight.Bold)
                    }
                }
            }
        }
    }
}
