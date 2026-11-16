package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Close
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material.icons.filled.Restaurant
import androidx.compose.material.icons.filled.Settings
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
import androidx.compose.ui.window.Dialog
import androidx.compose.material.icons.filled.Wifi
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.ui.platform.LocalContext
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.network.NetworkProfile
import eus.goierri.jatetxea.komandera.data.network.NetworkProfiles
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.model.LoginRequestDto
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch

@Composable
fun LoginScreen(
    onLoginSuccess: (LangileaDto) -> Unit,
    modifier: Modifier = Modifier
) {
    val context = LocalContext.current
    val scope = rememberCoroutineScope()
    var langileak by remember { mutableStateOf<List<LangileaDto>>(emptyList()) }
    var kargatzen by remember { mutableStateOf(true) }
    var erroreMezua by remember { mutableStateOf<String?>(null) }

    var aukeratutakoLangilea by remember { mutableStateOf<LangileaDto?>(null) }
    var pinModalIrekita by remember { mutableStateOf(false) }
    var unekoPina by remember { mutableStateOf("") }
    var pinErrorea by remember { mutableStateOf<String?>(null) }
    var saioaHasten by remember { mutableStateOf(false) }

    var configDialogIrekita by remember { mutableStateOf(false) }
    var apiUrlState by remember { mutableStateOf(ApiClient.baseUrl) }
    var unekoWifiSsid by remember { mutableStateOf(NetworkProfiles.lortuUnekoWifiIzena(context)) }

    fun kargatuLangileak() {
        scope.launch {
            kargatzen = true
            erroreMezua = null
            try {
                val res = ApiClient.createService(apiUrlState).getLangileak()
                if (res.isSuccessful && res.body()?.dataList != null) {
                    val guztiak = res.body()!!.dataList!!
                    // Zerbitzariak eta Barrakoak lehenetsi zerrendaren goialdean
                    val zerbitzariRoolak = listOf("Zerbitzaria", "Barrakoa", "Zerbitzari Burua", "Kudeatzailea", "AdminIT")
                    langileak = guztiak
                        .filter { it.aktibo && it.rola in zerbitzariRoolak }
                        .sortedBy { lang ->
                            when (lang.rola) {
                                "Zerbitzaria" -> 1
                                "Barrakoa" -> 2
                                "Zerbitzari Burua" -> 3
                                else -> 4
                            }
                        }
                } else {
                    erroreMezua = "Ezin izan dira langileak lortu API zerbitzaritik."
                }
            } catch (e: Exception) {
                erroreMezua = "Konexio errorea (${ApiClient.baseUrl}): ${e.localizedMessage ?: "Zerbitzaria ez dago eskuragarri"}"
            } finally {
                kargatzen = false
            }
        }
    }

    LaunchedEffect(Unit) {
        kargatuLangileak()
    }

    Column(
        modifier = modifier
            .fillMaxSize()
            .background(DarkBackground)
            .padding(20.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Spacer(modifier = Modifier.height(16.dp))

        // Goiburua eta Ikonoa
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Box(
                modifier = Modifier
                    .size(56.dp)
                    .clip(CircleShape)
                    .background(AmberPrimary.copy(alpha = 0.2f)),
                contentAlignment = Alignment.Center
            ) {
                Icon(
                    imageVector = Icons.Default.Restaurant,
                    contentDescription = "Logo",
                    tint = AmberPrimary,
                    modifier = Modifier.size(32.dp)
                )
            }

            IconButton(onClick = { configDialogIrekita = true }) {
                Icon(
                    imageVector = Icons.Default.Settings,
                    contentDescription = "Ezarpenak",
                    tint = TextSecondary
                )
            }
        }

        Spacer(modifier = Modifier.height(12.dp))

        Text(
            text = "1T JATETXEA",
            fontSize = 26.sp,
            fontWeight = FontWeight.Bold,
            color = AmberPrimary,
            letterSpacing = 2.sp
        )

        Text(
            text = "Zerbitzarien Komandera Mugikorra (Android)",
            fontSize = 14.sp,
            color = TextSecondary,
            textAlign = TextAlign.Center
        )

        Spacer(modifier = Modifier.height(20.dp))

        // Langileen Zerrenda Txartela
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
                        text = "Hautatu zure erabiltzailea:",
                        fontSize = 16.sp,
                        fontWeight = FontWeight.SemiBold,
                        color = TextPrimary
                    )

                    IconButton(onClick = { kargatuLangileak() }) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = "Freskatu",
                            tint = AmberPrimary
                        )
                    }
                }

                Spacer(modifier = Modifier.height(8.dp))

                if (kargatzen) {
                    Box(
                        modifier = Modifier.fillMaxSize(),
                        contentAlignment = Alignment.Center
                    ) {
                        CircularProgressIndicator(color = AmberPrimary)
                    }
                } else if (erroreMezua != null) {
                    Box(
                        modifier = Modifier.fillMaxSize(),
                        contentAlignment = Alignment.Center
                    ) {
                        Column(
                            horizontalAlignment = Alignment.CenterHorizontally,
                            modifier = Modifier.padding(16.dp)
                        ) {
                            Text(
                                text = erroreMezua!!,
                                color = StatusBusy,
                                textAlign = TextAlign.Center,
                                fontSize = 14.sp
                            )
                            Spacer(modifier = Modifier.height(12.dp))
                            Button(
                                onClick = { kargatuLangileak() },
                                colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary)
                            ) {
                                Text("Saiatu Berriro", color = DarkBackground, fontWeight = FontWeight.Bold)
                            }
                        }
                    }
                } else if (langileak.isEmpty()) {
                    Box(
                        modifier = Modifier.fillMaxSize(),
                        contentAlignment = Alignment.Center
                    ) {
                        Text(
                            text = "Ez da zerbitzari aktiborik aurkitu datu-basean.",
                            color = TextSecondary,
                            textAlign = TextAlign.Center
                        )
                    }
                } else {
                    LazyColumn(
                        modifier = Modifier.fillMaxSize(),
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        items(langileak) { langilea ->
                            Card(
                                onClick = {
                                    aukeratutakoLangilea = langilea
                                    unekoPina = ""
                                    pinErrorea = null
                                    pinModalIrekita = true
                                },
                                modifier = Modifier.fillMaxWidth(),
                                colors = CardDefaults.cardColors(
                                    containerColor = DarkSurfaceVariant
                                ),
                                shape = RoundedCornerShape(12.dp)
                            ) {
                                Row(
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .padding(12.dp),
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    Box(
                                        modifier = Modifier
                                            .size(42.dp)
                                            .clip(CircleShape)
                                            .background(
                                                if (langilea.rola == "Barrakoa") StatusCleaning.copy(alpha = 0.2f)
                                                else AmberPrimary.copy(alpha = 0.2f)
                                            ),
                                        contentAlignment = Alignment.Center
                                    ) {
                                        Icon(
                                            imageVector = Icons.Default.Person,
                                            contentDescription = null,
                                            tint = if (langilea.rola == "Barrakoa") StatusCleaning else AmberPrimary,
                                            modifier = Modifier.size(24.dp)
                                        )
                                    }

                                    Spacer(modifier = Modifier.width(14.dp))

                                    Column(modifier = Modifier.weight(1f)) {
                                        Text(
                                            text = langilea.izena,
                                            fontSize = 16.sp,
                                            fontWeight = FontWeight.SemiBold,
                                            color = TextPrimary
                                        )
                                        Text(
                                            text = "${langilea.rola} · @${langilea.erabiltzailea}",
                                            fontSize = 13.sp,
                                            color = TextSecondary
                                        )
                                    }

                                    Icon(
                                        imageVector = Icons.Default.Lock,
                                        contentDescription = "PIN beharrezkoa",
                                        tint = TextMuted,
                                        modifier = Modifier.size(20.dp)
                                    )
                                }
                            }
                        }
                    }
                }
            }
        }

        Spacer(modifier = Modifier.height(16.dp))
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.Center,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Icon(
                imageVector = Icons.Default.Wifi,
                contentDescription = "Wi-Fi egoera",
                tint = if (unekoWifiSsid != null) StatusReady else TextMuted,
                modifier = Modifier.size(16.dp)
            )
            Spacer(modifier = Modifier.width(6.dp))
            Text(
                text = if (unekoWifiSsid != null) "Wi-Fi: $unekoWifiSsid" else "Ez da Wi-Fi sarerik hauteman",
                fontSize = 12.sp,
                color = if (unekoWifiSsid != null) StatusReady else TextMuted,
                fontWeight = FontWeight.Medium
            )
            Spacer(modifier = Modifier.width(12.dp))
            Text(
                text = "·  API: ${ApiClient.baseUrl}",
                fontSize = 11.sp,
                color = TextSecondary,
                maxLines = 1
            )
        }
    }

    // 4 ZIFRAKO PIN SARRERA MODALA (C# TPV BEZALAKOA)
    if (pinModalIrekita && aukeratutakoLangilea != null) {
        val langilea = aukeratutakoLangilea!!

        fun saioaHasi() {
            if (unekoPina.length < 4) {
                pinErrorea = "Sartu 4 zifrako PINa."
                return
            }
            scope.launch {
                saioaHasten = true
                pinErrorea = null
                try {
                    val req = LoginRequestDto(
                        erabiltzailea = langilea.erabiltzailea,
                        pasahitza = unekoPina
                    )
                    val res = ApiClient.createService(ApiClient.baseUrl).login(req)
                    if (res.isSuccessful && res.body()?.data?.arrakasta == true) {
                        val saioLangilea = res.body()?.data?.langilea ?: langilea
                        pinModalIrekita = false
                        onLoginSuccess(saioLangilea)
                    } else {
                        pinErrorea = res.body()?.message ?: "PIN okerra. Saiatu berriro."
                        unekoPina = ""
                    }
                } catch (e: Exception) {
                    pinErrorea = "Konexio errorea: ${e.localizedMessage ?: "Ezin izan da egiaztatu"}"
                } finally {
                    saioaHasten = false
                }
            }
        }

        Dialog(onDismissRequest = { if (!saioaHasten) pinModalIrekita = false }) {
            Surface(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(8.dp),
                shape = RoundedCornerShape(20.dp),
                color = DarkSurface,
                tonalElevation = 8.dp
            ) {
                Column(
                    modifier = Modifier.padding(20.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            Text(
                                text = "Sartu PINa",
                                fontSize = 18.sp,
                                fontWeight = FontWeight.Bold,
                                color = TextPrimary
                            )
                            Text(
                                text = "${langilea.izena} (${langilea.rola})",
                                fontSize = 13.sp,
                                color = AmberPrimary
                            )
                        }

                        IconButton(
                            onClick = { pinModalIrekita = false },
                            enabled = !saioaHasten
                        ) {
                            Icon(
                                imageVector = Icons.Default.Close,
                                contentDescription = "Itxi",
                                tint = TextSecondary
                            )
                        }
                    }

                    Spacer(modifier = Modifier.height(16.dp))

                    // PIN Zirkuluen adierazlea (● ● ● ●)
                    Row(
                        horizontalArrangement = Arrangement.spacedBy(16.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        for (i in 0 until 4) {
                            val beteta = i < unekoPina.length
                            Box(
                                modifier = Modifier
                                    .size(20.dp)
                                    .clip(CircleShape)
                                    .background(if (beteta) AmberPrimary else DarkSurfaceVariant)
                                    .border(
                                        width = 1.5.dp,
                                        color = if (beteta) AmberPrimary else TextMuted,
                                        shape = CircleShape
                                    )
                            )
                        }
                    }

                    if (pinErrorea != null) {
                        Spacer(modifier = Modifier.height(10.dp))
                        Text(
                            text = pinErrorea!!,
                            color = StatusBusy,
                            fontSize = 12.sp,
                            textAlign = TextAlign.Center
                        )
                    }

                    Spacer(modifier = Modifier.height(20.dp))

                    // Zenbaki Teklatua (1etik 9ra, 0, Garbitu, Sartu)
                    val teklak = listOf(
                        listOf("1", "2", "3"),
                        listOf("4", "5", "6"),
                        listOf("7", "8", "9"),
                        listOf("C", "0", "OK")
                    )

                    for (lerro in teklak) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(10.dp)
                        ) {
                            for (tekla in lerro) {
                                Button(
                                    onClick = {
                                        if (saioaHasten) return@Button
                                        when (tekla) {
                                            "C" -> {
                                                if (unekoPina.isNotEmpty()) {
                                                    unekoPina = unekoPina.dropLast(1)
                                                }
                                                pinErrorea = null
                                            }
                                            "OK" -> {
                                                saioaHasi()
                                            }
                                            else -> {
                                                if (unekoPina.length < 4) {
                                                    unekoPina += tekla
                                                    pinErrorea = null
                                                    if (unekoPina.length == 4) {
                                                        saioaHasi()
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    modifier = Modifier
                                        .weight(1f)
                                        .height(52.dp)
                                        .padding(vertical = 4.dp),
                                    colors = ButtonDefaults.buttonColors(
                                        containerColor = when (tekla) {
                                            "OK" -> AmberPrimary
                                            "C" -> StatusBusy.copy(alpha = 0.25f)
                                            else -> DarkSurfaceVariant
                                        }
                                    ),
                                    shape = RoundedCornerShape(12.dp)
                                ) {
                                    if (tekla == "OK" && saioaHasten) {
                                        CircularProgressIndicator(
                                            modifier = Modifier.size(20.dp),
                                            color = DarkBackground,
                                            strokeWidth = 2.dp
                                        )
                                    } else {
                                        Text(
                                            text = tekla,
                                            fontSize = 18.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = when (tekla) {
                                                "OK" -> DarkBackground
                                                "C" -> StatusBusy
                                                else -> TextPrimary
                                            }
                                        )
                                    }
                                }
                            }
                        }
                    }

                    Spacer(modifier = Modifier.height(10.dp))
                    Text(
                        text = "Lehenetsitako demo PINa: 1234",
                        fontSize = 11.sp,
                        color = TextMuted
                    )
                }
            }
        }
    }

    // API URL ETA SARE PRESETAK KONFIGURATZEKO MODALA
    if (configDialogIrekita) {
        var behinBehinekoUrl by remember { mutableStateOf(ApiClient.baseUrl) }
        var testEmaitza by remember { mutableStateOf<String?>(null) }
        var testKargatzen by remember { mutableStateOf(false) }

        AlertDialog(
            onDismissRequest = { configDialogIrekita = false },
            title = {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Icon(
                        imageVector = Icons.Default.Settings,
                        contentDescription = null,
                        tint = AmberPrimary,
                        modifier = Modifier.size(22.dp)
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text("Sare Ezarpenak & API Profilak", color = TextPrimary, fontWeight = FontWeight.Bold, fontSize = 18.sp)
                }
            },
            text = {
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .heightIn(max = 480.dp)
                ) {
                    // Wi-Fi detektatua
                    Surface(
                        color = DarkSurfaceVariant,
                        shape = RoundedCornerShape(8.dp),
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Row(
                            modifier = Modifier.padding(10.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Icon(
                                imageVector = Icons.Default.Wifi,
                                contentDescription = null,
                                tint = if (unekoWifiSsid != null) StatusReady else TextMuted,
                                modifier = Modifier.size(18.dp)
                            )
                            Spacer(modifier = Modifier.width(8.dp))
                            Column {
                                Text(
                                    text = if (unekoWifiSsid != null) "Hautemandako Wi-Fi: $unekoWifiSsid" else "Ez da Wi-Fi sarerik hauteman",
                                    fontSize = 12.sp,
                                    fontWeight = FontWeight.SemiBold,
                                    color = if (unekoWifiSsid != null) StatusReady else TextSecondary
                                )
                                Text(
                                    text = "Klikatu profil batean URL egokia automatikoki ezartzeko",
                                    fontSize = 10.sp,
                                    color = TextMuted
                                )
                            }
                        }
                    }

                    Spacer(modifier = Modifier.height(10.dp))
                    Text(
                        text = "Aukeratu Ingurunea (Profil Azkarrak):",
                        fontSize = 12.sp,
                        fontWeight = FontWeight.Bold,
                        color = TextSecondary
                    )
                    Spacer(modifier = Modifier.height(6.dp))

                    LazyColumn(
                        modifier = Modifier
                            .fillMaxWidth()
                            .weight(1f, fill = false)
                    ) {
                        items(NetworkProfiles.PROFILAK) { profila ->
                            val hautatuta = behinBehinekoUrl.trimEnd('/') == profila.baseUrl.trimEnd('/')
                            val unekoWifiaBera = unekoWifiSsid != null && profila.wifiSsid.equals(unekoWifiSsid, ignoreCase = true)

                            Surface(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(vertical = 3.dp)
                                    .clickable {
                                        behinBehinekoUrl = profila.baseUrl
                                        testEmaitza = null
                                    },
                                color = if (hautatuta) AmberPrimary.copy(alpha = 0.15f) else DarkSurfaceVariant,
                                shape = RoundedCornerShape(8.dp),
                                border = if (hautatuta) androidx.compose.foundation.BorderStroke(1.dp, AmberPrimary) else null
                            ) {
                                Row(
                                    modifier = Modifier.padding(horizontal = 10.dp, vertical = 8.dp),
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    Column(modifier = Modifier.weight(1f)) {
                                        Row(verticalAlignment = Alignment.CenterVertically) {
                                            Text(
                                                text = profila.izena,
                                                fontWeight = FontWeight.Bold,
                                                fontSize = 13.sp,
                                                color = if (hautatuta) AmberPrimary else TextPrimary
                                            )
                                            if (unekoWifiaBera) {
                                                Spacer(modifier = Modifier.width(6.dp))
                                                Surface(
                                                    color = StatusReady.copy(alpha = 0.2f),
                                                    shape = RoundedCornerShape(4.dp)
                                                ) {
                                                    Text(
                                                        text = "Uneko Wi-Fi",
                                                        fontSize = 9.sp,
                                                        color = StatusReady,
                                                        fontWeight = FontWeight.Bold,
                                                        modifier = Modifier.padding(horizontal = 4.dp, vertical = 2.dp)
                                                    )
                                                }
                                            }
                                        }
                                        Text(
                                            text = profila.deskribapena,
                                            fontSize = 11.sp,
                                            color = TextSecondary
                                        )
                                    }
                                    if (hautatuta) {
                                        Icon(
                                            imageVector = Icons.Default.CheckCircle,
                                            contentDescription = "Hautatua",
                                            tint = AmberPrimary,
                                            modifier = Modifier.size(18.dp)
                                        )
                                    }
                                }
                            }
                        }
                    }

                    Spacer(modifier = Modifier.height(10.dp))
                    OutlinedTextField(
                        value = behinBehinekoUrl,
                        onValueChange = {
                            behinBehinekoUrl = it
                            testEmaitza = null
                        },
                        label = { Text("API Base URL eskuz") },
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = AmberPrimary,
                            unfocusedBorderColor = TextMuted,
                            focusedTextColor = TextPrimary,
                            unfocusedTextColor = TextPrimary
                        ),
                        modifier = Modifier.fillMaxWidth()
                    )

                    Spacer(modifier = Modifier.height(8.dp))
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        OutlinedButton(
                            onClick = {
                                scope.launch {
                                    testKargatzen = true
                                    testEmaitza = null
                                    try {
                                        val testSrv = ApiClient.createService(behinBehinekoUrl)
                                        val res = testSrv.getHealth()
                                        if (res.isSuccessful) {
                                            testEmaitza = "OK: Zerbitzaria konektatuta!"
                                        } else {
                                            testEmaitza = "Errorea: HTTP ${res.code()}"
                                        }
                                    } catch (e: Exception) {
                                        testEmaitza = "Errorea: ${e.localizedMessage ?: "Ezin izan da konektatu"}"
                                    } finally {
                                        testKargatzen = false
                                    }
                                }
                            },
                            enabled = !testKargatzen,
                            colors = ButtonDefaults.outlinedButtonColors(contentColor = AmberPrimary)
                        ) {
                            if (testKargatzen) {
                                CircularProgressIndicator(modifier = Modifier.size(16.dp), color = AmberPrimary, strokeWidth = 2.dp)
                            } else {
                                Text("Proba Konexioa", fontSize = 12.sp)
                            }
                        }

                        if (testEmaitza != null) {
                            Text(
                                text = testEmaitza!!,
                                fontSize = 11.sp,
                                color = if (testEmaitza!!.startsWith("OK")) StatusReady else StatusBusy,
                                fontWeight = FontWeight.SemiBold,
                                modifier = Modifier.weight(1f).padding(start = 8.dp)
                            )
                        }
                    }
                }
            },
            confirmButton = {
                Button(
                    onClick = {
                        ApiClient.saveBaseUrl(context, behinBehinekoUrl)
                        apiUrlState = ApiClient.baseUrl
                        unekoWifiSsid = NetworkProfiles.lortuUnekoWifiIzena(context)
                        configDialogIrekita = false
                        kargatuLangileak()
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = AmberPrimary)
                ) {
                    Text("Gorde eta Freskatu", color = DarkBackground, fontWeight = FontWeight.Bold)
                }
            },
            dismissButton = {
                TextButton(onClick = { configDialogIrekita = false }) {
                    Text("Utzi", color = TextSecondary)
                }
            },
            containerColor = DarkSurface
        )
    }
}
