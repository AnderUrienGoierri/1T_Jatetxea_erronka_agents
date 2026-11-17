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
import eus.goierri.jatetxea.komandera.ui.components.EzarpenakDialog
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.network.NetworkProfile
import eus.goierri.jatetxea.komandera.data.network.NetworkProfiles
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.model.LoginRequestDto
import androidx.compose.ui.res.painterResource
import eus.goierri.jatetxea.komandera.R
import eus.goierri.jatetxea.komandera.ui.theme.AmberAccent
import eus.goierri.jatetxea.komandera.ui.theme.AmberPrimary
import eus.goierri.jatetxea.komandera.ui.theme.GoldBorder
import eus.goierri.jatetxea.komandera.ui.theme.DarkBackground
import eus.goierri.jatetxea.komandera.ui.theme.DarkSurface
import eus.goierri.jatetxea.komandera.ui.theme.DarkSurfaceVariant
import eus.goierri.jatetxea.komandera.ui.theme.StatusBusy
import eus.goierri.jatetxea.komandera.ui.theme.StatusCleaning
import eus.goierri.jatetxea.komandera.ui.theme.StatusFree
import eus.goierri.jatetxea.komandera.ui.theme.StatusReady
import eus.goierri.jatetxea.komandera.ui.theme.TextMuted
import eus.goierri.jatetxea.komandera.ui.theme.TextPrimary
import eus.goierri.jatetxea.komandera.ui.theme.TextSecondary
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

    // API eta Datu-basearen konexio egoera
    // "KONEKTATUTA", "DB_ERROREA", "API_DESKONEKTATUTA", "KARGATZEN"
    var apiKonexioEgoera by remember { mutableStateOf("KARGATZEN") }
    var egoeraAzalpena by remember { mutableStateOf("Egiaztatzen...") }

    fun egiaztatuSareEgoera() {
        scope.launch {
            unekoWifiSsid = NetworkProfiles.lortuUnekoWifiIzena(context)
            try {
                val service = ApiClient.createService(apiUrlState)
                val healthRes = service.getHealth()
                if (healthRes.isSuccessful && healthRes.body() != null) {
                    val body = healthRes.body()!!
                    val dbStatus = body["database"]?.toString() ?: ""
                    val apiStatus = body["status"]?.toString() ?: ""

                    if (dbStatus.equals("Connected", ignoreCase = true) || apiStatus.equals("Healthy", ignoreCase = true)) {
                        apiKonexioEgoera = "KONEKTATUTA"
                        egoeraAzalpena = "API eta DB konektatuta"
                    } else {
                        apiKonexioEgoera = "DB_ERROREA"
                        egoeraAzalpena = "API martxan, DB deskonektatuta"
                    }
                } else if (healthRes.code() == 503) {
                    apiKonexioEgoera = "DB_ERROREA"
                    egoeraAzalpena = "API martxan, DB errorea (503)"
                } else {
                    apiKonexioEgoera = "API_DESKONEKTATUTA"
                    egoeraAzalpena = "Errorea: HTTP ${healthRes.code()}"
                }
            } catch (e: Exception) {
                apiKonexioEgoera = "API_DESKONEKTATUTA"
                egoeraAzalpena = "Ezin da APIra konektatu"
            }
        }
    }

    fun kargatuLangileak() {
        egiaztatuSareEgoera()
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
                    .size(64.dp)
                    .clip(CircleShape)
                    .background(Color.Transparent),
                contentAlignment = Alignment.Center
            ) {
                androidx.compose.foundation.Image(
                    painter = painterResource(id = R.drawable.ic_t1_logo),
                    contentDescription = "T1 Jatetxea Logoa",
                    modifier = Modifier.size(64.dp)
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

        Spacer(modifier = Modifier.height(14.dp))

        Row(
            verticalAlignment = Alignment.CenterVertically,
            horizontalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            Text(
                text = "T1 JATETXEA",
                fontSize = 28.sp,
                fontWeight = FontWeight.Black,
                color = TextPrimary,
                letterSpacing = 2.5.sp
            )
            Surface(
                color = AmberAccent.copy(alpha = 0.2f),
                shape = RoundedCornerShape(6.dp),
                border = androidx.compose.foundation.BorderStroke(1.dp, GoldBorder.copy(alpha = 0.5f))
            ) {
                Text(
                    text = "OSTERIA & PARRILLA",
                    color = GoldBorder,
                    fontSize = 10.sp,
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.padding(horizontal = 6.dp, vertical = 2.dp),
                    letterSpacing = 1.sp
                )
            }
        }

        Spacer(modifier = Modifier.height(2.dp))

        Text(
            text = "Zerbitzarien Komandera Mugikorra · Euskal Herria & Italia",
            fontSize = 13.sp,
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

        Spacer(modifier = Modifier.height(14.dp))

        // FOOTER: Wi-Fi eta API / Datu-Basearen Egoera Argia
        Surface(
            modifier = Modifier.fillMaxWidth(),
            color = DarkSurfaceVariant.copy(alpha = 0.6f),
            shape = RoundedCornerShape(10.dp)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(horizontal = 12.dp, vertical = 8.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                // 1. Lerroa: Wi-Fi egoera
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.Center
                ) {
                    Icon(
                        imageVector = Icons.Default.Wifi,
                        contentDescription = "Wi-Fi egoera",
                        tint = if (unekoWifiSsid != null) StatusReady else TextMuted,
                        modifier = Modifier.size(15.dp)
                    )
                    Spacer(modifier = Modifier.width(6.dp))
                    Text(
                        text = if (unekoWifiSsid != null) "Wi-Fi: $unekoWifiSsid" else "Wi-Fi: Ez da sarerik hauteman",
                        fontSize = 12.sp,
                        color = if (unekoWifiSsid != null) StatusReady else TextMuted,
                        fontWeight = FontWeight.SemiBold
                    )
                }

                Spacer(modifier = Modifier.height(4.dp))

                // 2. Lerroa: API eta Datu-basearen konexio egoera
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.Center
                ) {
                    val (puntuaKolorea, testuaKolorea) = when (apiKonexioEgoera) {
                        "KONEKTATUTA" -> StatusReady to StatusReady
                        "DB_ERROREA" -> AmberPrimary to AmberPrimary
                        "API_DESKONEKTATUTA" -> StatusBusy to StatusBusy
                        else -> TextMuted to TextSecondary
                    }

                    Box(
                        modifier = Modifier
                            .size(8.dp)
                            .clip(CircleShape)
                            .background(puntuaKolorea)
                    )
                    Spacer(modifier = Modifier.width(6.dp))
                    Text(
                        text = egoeraAzalpena,
                        fontSize = 11.sp,
                        color = testuaKolorea,
                        fontWeight = FontWeight.Bold
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = "(${ApiClient.baseUrl})",
                        fontSize = 10.sp,
                        color = TextMuted,
                        maxLines = 1
                    )
                }
            }
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
        EzarpenakDialog(
            zerbitzaria = aukeratutakoLangilea,
            onDismiss = { configDialogIrekita = false },
            onApiConfigAldatua = {
                apiUrlState = ApiClient.baseUrl
                unekoWifiSsid = NetworkProfiles.lortuUnekoWifiIzena(context)
                kargatuLangileak()
                egiaztatuSareEgoera()
            }
        )
    }
}
