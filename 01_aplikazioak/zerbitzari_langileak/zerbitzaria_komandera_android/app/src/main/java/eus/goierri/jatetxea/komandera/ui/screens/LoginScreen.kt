package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.animateDpAsState
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.AdminPanelSettings
import androidx.compose.material.icons.filled.ChevronRight
import androidx.compose.material.icons.filled.Close
import androidx.compose.material.icons.filled.LocalBar
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material.icons.filled.Restaurant
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material.icons.filled.Wifi
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import eus.goierri.jatetxea.komandera.R
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.model.LoginRequestDto
import eus.goierri.jatetxea.komandera.data.network.NetworkProfiles
import eus.goierri.jatetxea.komandera.ui.components.EzarpenakDialog
import eus.goierri.jatetxea.komandera.ui.state.AppPreferences
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
                        egoeraAzalpena = AppPreferences.tr(
                            "API Konektatuta",
                            "API Conectada",
                            "API Connected"
                        )
                    } else {
                        apiKonexioEgoera = "DB_ERROREA"
                        egoeraAzalpena = AppPreferences.tr(
                            "API martxan, DB deskonektatuta",
                            "API activa, BD desconectada",
                            "API active, DB disconnected"
                        )
                    }
                } else if (healthRes.code() == 503) {
                    apiKonexioEgoera = "DB_ERROREA"
                    egoeraAzalpena = "API martxan, DB errorea (503)"
                } else {
                    apiKonexioEgoera = "API_DESKONEKTATUTA"
                    egoeraAzalpena = "Errorea: HTTP ${healthRes.code()}"
                }
            } catch (_: Exception) {
                apiKonexioEgoera = "API_DESKONEKTATUTA"
                egoeraAzalpena = AppPreferences.tr(
                    "Ezin da APIra konektatu",
                    "Sin conexión con la API",
                    "Cannot connect to API"
                )
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
                    val zerbitzariRoolak = listOf("Zerbitzaria", "Barrakoa", "Zerbitzari Burua", "Kudeatzailea", "AdminIT", "Admin")
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
                    erroreMezua = AppPreferences.tr(
                        "Ezin izan dira langileak lortu API zerbitzaritik.",
                        "No se pudieron obtener los empleados del servidor.",
                        "Could not load staff from API server."
                    )
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

    LoginScreenContent(
        langileak = langileak,
        kargatzen = kargatzen,
        erroreMezua = erroreMezua,
        unekoWifiSsid = unekoWifiSsid,
        apiKonexioEgoera = apiKonexioEgoera,
        egoeraAzalpena = egoeraAzalpena,
        apiUrl = apiUrlState,
        onLangileaAukeratua = { langilea ->
            aukeratutakoLangilea = langilea
            unekoPina = ""
            pinErrorea = null
            pinModalIrekita = true
        },
        onFreskatu = { kargatuLangileak() },
        onIrekiEzarpenak = { configDialogIrekita = true },
        modifier = modifier
    )

    if (pinModalIrekita && aukeratutakoLangilea != null) {
        val langilea = aukeratutakoLangilea!!

        fun saioaHasi() {
            if (unekoPina.length < 4) {
                pinErrorea = AppPreferences.tr("Sartu 4 zifrako PINa.", "Introduce un PIN de 4 dígitos.", "Enter a 4-digit PIN.")
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
                        pinErrorea = res.body()?.message ?: AppPreferences.tr(
                            "PIN okerra. Saiatu berriro.",
                            "PIN incorrecto. Inténtalo de nuevo.",
                            "Incorrect PIN. Try again."
                        )
                        unekoPina = ""
                    }
                } catch (e: Exception) {
                    pinErrorea = "Konexio errorea: ${e.localizedMessage ?: "Ezin izan da egiaztatu"}"
                } finally {
                    saioaHasten = false
                }
            }
        }

        PinSarreraDialog(
            langilea = langilea,
            unekoPina = unekoPina,
            pinErrorea = pinErrorea,
            saioaHasten = saioaHasten,
            onPinTekla = { tekla ->
                if (saioaHasten) return@PinSarreraDialog
                when (tekla) {
                    "C" -> {
                        if (unekoPina.isNotEmpty()) unekoPina = unekoPina.dropLast(1)
                        pinErrorea = null
                    }
                    "OK" -> saioaHasi()
                    else -> {
                        if (unekoPina.length < 4) {
                            unekoPina += tekla
                            pinErrorea = null
                            if (unekoPina.length == 4) saioaHasi()
                        }
                    }
                }
            },
            onDismiss = { if (!saioaHasten) pinModalIrekita = false }
        )
    }

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

private fun lortuInicialak(izena: String): String {
    val zatiak = izena.trim().split(" ").filter { it.isNotBlank() }
    return when {
        zatiak.size >= 2 -> "${zatiak[0].first()}${zatiak[1].first()}".uppercase()
        zatiak.isNotEmpty() -> zatiak[0].take(2).uppercase()
        else -> "T1"
    }
}

@Composable
fun LoginScreenContent(
    langileak: List<LangileaDto>,
    kargatzen: Boolean,
    erroreMezua: String?,
    unekoWifiSsid: String?,
    apiKonexioEgoera: String,
    egoeraAzalpena: String,
    apiUrl: String,
    onLangileaAukeratua: (LangileaDto) -> Unit,
    onFreskatu: () -> Unit,
    onIrekiEzarpenak: () -> Unit,
    modifier: Modifier = Modifier
) {
    val isDark = AppPreferences.isDarkTheme
    val logoRes = if (isDark) R.drawable.ic_t1_logo_dark else R.drawable.ic_t1_logo
    val puntuaKolorea by animateColorAsState(
        targetValue = when (apiKonexioEgoera) {
            "KONEKTATUTA" -> if (isDark) StatusFreeDark else StatusFree
            "DB_ERROREA" -> if (isDark) GoldBorder else AntiqueGold
            "API_DESKONEKTATUTA" -> if (isDark) StatusOccupiedDark else StatusOccupied
            else -> TextMuted
        },
        animationSpec = tween(350),
        label = "apiStatusColor"
    )

    BoxWithConstraints(
        modifier = modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background)
    ) {
        val isTablet = maxWidth >= 800.dp

        if (isTablet) {
            // TABLET SPLIT-PANE LAYOUT (Figma T01_Login_Split)
            Row(modifier = Modifier.fillMaxSize()) {
                // Ezkerreko Marka Hero Panela (%40)
                Column(
                    modifier = Modifier
                        .weight(0.40f)
                        .fillMaxHeight()
                        .background(
                            Brush.linearGradient(
                                colors = listOf(Color(0xFF6B0F2B), ChiantiPrimary, BurgundyDark)
                            )
                        )
                        .padding(36.dp),
                    verticalArrangement = Arrangement.SpaceBetween
                ) {
                    Column(verticalArrangement = Arrangement.spacedBy(16.dp)) {
                        Image(
                            painter = painterResource(id = R.drawable.ic_t1_logo),
                            contentDescription = "T1 Jatetxea Logoa",
                            modifier = Modifier.size(84.dp)
                        )
                        Text(
                            text = "T1 JATETXEA",
                            fontFamily = CinzelFont,
                            fontWeight = FontWeight.Bold,
                            fontSize = 30.sp,
                            letterSpacing = 2.sp,
                            color = Color.White
                        )
                        Text(
                            text = "OSTERIA & PARRILLA · TABLET POS",
                            fontSize = 11.sp,
                            fontWeight = FontWeight.Bold,
                            letterSpacing = 2.sp,
                            color = AmberLight
                        )
                        Text(
                            text = AppPreferences.tr(
                                "Bi tradizio, artisautza bakarra.\nAretoko eta barrako zerbitzu azkarra ukipen bakarrean.",
                                "Dos tradiciones, una sola artesanía.\nServicio rápido de sala y barra en un toque.",
                                "Two traditions, one craft.\nFast dining & bar service in a single tap."
                            ),
                            fontSize = 14.sp,
                            lineHeight = 22.sp,
                            color = WarmLinen.copy(alpha = 0.88f)
                        )
                    }

                    ApiStatusFooterPill(
                        puntuaKolorea = StatusFreeDark,
                        egoeraAzalpena = egoeraAzalpena,
                        apiUrl = apiUrl,
                        unekoWifiSsid = unekoWifiSsid,
                        onDarkHero = true,
                        onClick = onIrekiEzarpenak
                    )
                }

                // Eskuineko Langile Direktorioa (%60)
                Column(
                    modifier = Modifier
                        .weight(0.60f)
                        .fillMaxHeight()
                        .padding(32.dp)
                ) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Text(
                            text = AppPreferences.tr("Hautatu langilea", "Selecciona tu usuario", "Select staff member"),
                            style = MaterialTheme.typography.headlineSmall,
                            color = MaterialTheme.colorScheme.onBackground
                        )
                        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                            Surface(
                                onClick = onFreskatu,
                                shape = RoundedCornerShape(12.dp),
                                color = MaterialTheme.colorScheme.surface,
                                border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                            ) {
                                Icon(
                                    imageVector = Icons.Default.Refresh,
                                    contentDescription = AppPreferences.tr("Freskatu", "Actualizar", "Refresh"),
                                    tint = MaterialTheme.colorScheme.secondary,
                                    modifier = Modifier.padding(10.dp).size(20.dp)
                                )
                            }
                            Surface(
                                onClick = onIrekiEzarpenak,
                                shape = RoundedCornerShape(12.dp),
                                color = MaterialTheme.colorScheme.surface,
                                border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                            ) {
                                Icon(
                                    imageVector = Icons.Default.Settings,
                                    contentDescription = AppPreferences.tr("Ezarpenak", "Ajustes", "Settings"),
                                    tint = MaterialTheme.colorScheme.onSurfaceVariant,
                                    modifier = Modifier.padding(10.dp).size(20.dp)
                                )
                            }
                        }
                    }

                    Spacer(modifier = Modifier.height(16.dp))

                    StaffDirectoryContent(
                        langileak = langileak,
                        kargatzen = kargatzen,
                        erroreMezua = erroreMezua,
                        onLangileaAukeratua = onLangileaAukeratua,
                        onFreskatu = onFreskatu,
                        modifier = Modifier.weight(1f)
                    )
                }
            }
        } else {
            // SMARTPHONE LAYOUT (Figma 01_Login — Flat Warm Hospitality, no Card-in-Card)
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .statusBarsPadding()
                    .navigationBarsPadding()
                    .padding(horizontal = 20.dp, vertical = 14.dp)
            ) {
                // Goiburu Editoriala: Logo B + T1 JATETXEA (Cinzel) + Ezarpenak
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        Image(
                            painter = painterResource(id = logoRes),
                            contentDescription = "T1 Jatetxea Logoa",
                            modifier = Modifier.size(54.dp)
                        )
                        Column(verticalArrangement = Arrangement.spacedBy(2.dp)) {
                            Text(
                                text = "T1 JATETXEA",
                                fontFamily = CinzelFont,
                                fontWeight = FontWeight.Bold,
                                fontSize = 20.sp,
                                letterSpacing = 1.6.sp,
                                color = MaterialTheme.colorScheme.onBackground
                            )
                            Text(
                                text = "OSTERIA & PARRILLA",
                                fontSize = 10.sp,
                                fontWeight = FontWeight.Bold,
                                letterSpacing = 2.2.sp,
                                color = MaterialTheme.colorScheme.secondary
                            )
                        }
                    }

                    Surface(
                        onClick = onIrekiEzarpenak,
                        shape = RoundedCornerShape(12.dp),
                        color = MaterialTheme.colorScheme.surface,
                        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                    ) {
                        Icon(
                            imageVector = Icons.Default.Settings,
                            contentDescription = AppPreferences.tr("Ezarpenak", "Ajustes", "Settings"),
                            tint = MaterialTheme.colorScheme.onSurfaceVariant,
                            modifier = Modifier.padding(10.dp).size(20.dp)
                        )
                    }
                }

                Spacer(modifier = Modifier.height(18.dp))

                // Sekzio-titulua + Freskatu
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = AppPreferences.tr(
                            "Hautatu zure erabiltzailea",
                            "Selecciona tu usuario",
                            "Select your profile"
                        ),
                        style = MaterialTheme.typography.titleLarge,
                        color = MaterialTheme.colorScheme.onBackground
                    )
                    IconButton(onClick = onFreskatu) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = AppPreferences.tr("Freskatu", "Actualizar", "Refresh"),
                            tint = MaterialTheme.colorScheme.secondary
                        )
                    }
                }

                Spacer(modifier = Modifier.height(6.dp))

                StaffDirectoryContent(
                    langileak = langileak,
                    kargatzen = kargatzen,
                    erroreMezua = erroreMezua,
                    onLangileaAukeratua = onLangileaAukeratua,
                    onFreskatu = onFreskatu,
                    modifier = Modifier.weight(1f)
                )

                Spacer(modifier = Modifier.height(12.dp))

                ApiStatusFooterPill(
                    puntuaKolorea = puntuaKolorea,
                    egoeraAzalpena = egoeraAzalpena,
                    apiUrl = apiUrl,
                    unekoWifiSsid = unekoWifiSsid,
                    onDarkHero = false,
                    onClick = onIrekiEzarpenak
                )
            }
        }
    }
}

@Composable
private fun StaffDirectoryContent(
    langileak: List<LangileaDto>,
    kargatzen: Boolean,
    erroreMezua: String?,
    onLangileaAukeratua: (LangileaDto) -> Unit,
    onFreskatu: () -> Unit,
    modifier: Modifier = Modifier
) {
    Column(modifier = modifier.fillMaxWidth()) {
        when {
            kargatzen -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator(color = MaterialTheme.colorScheme.primary)
                }
            }
            erroreMezua != null -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Surface(
                        shape = RoundedCornerShape(18.dp),
                        color = MaterialTheme.colorScheme.surface,
                        border = BorderStroke(1.dp, MaterialTheme.colorScheme.error.copy(alpha = 0.35f))
                    ) {
                        Column(
                            horizontalAlignment = Alignment.CenterHorizontally,
                            modifier = Modifier.padding(24.dp)
                        ) {
                            Text(
                                text = erroreMezua,
                                color = MaterialTheme.colorScheme.error,
                                textAlign = TextAlign.Center,
                                fontSize = 13.sp
                            )
                            Spacer(modifier = Modifier.height(14.dp))
                            Button(
                                onClick = onFreskatu,
                                shape = RoundedCornerShape(12.dp),
                                colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
                            ) {
                                Text(
                                    AppPreferences.tr("Saiatu Berriro", "Reintentar", "Retry"),
                                    color = MaterialTheme.colorScheme.onPrimary,
                                    fontWeight = FontWeight.Bold
                                )
                            }
                        }
                    }
                }
            }
            langileak.isEmpty() -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text(
                        text = AppPreferences.tr(
                            "Ez da zerbitzari aktiborik aurkitu datu-basean.",
                            "No se encontraron camareros activos.",
                            "No active waiters found."
                        ),
                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                        textAlign = TextAlign.Center
                    )
                }
            }
            else -> {
                var hautatutakoBlokea by rememberSaveable { mutableStateOf("Guztiak") }

                val jantokiaLangileak = remember(langileak) {
                    langileak.filter {
                        it.rola.equals("Zerbitzaria", ignoreCase = true) ||
                            it.rola.equals("Zerbitzari Burua", ignoreCase = true)
                    }
                }
                val barraLangileak = remember(langileak) {
                    langileak.filter { it.rola.equals("Barrakoa", ignoreCase = true) }
                }
                val adminLangileak = remember(langileak) {
                    langileak.filter {
                        it.rola.equals("AdminIT", ignoreCase = true) ||
                            it.rola.equals("Kudeatzailea", ignoreCase = true) ||
                            it.rola.equals("Admin", ignoreCase = true)
                    }
                }

                // 3-bloke pill iragazki garbia (Guztiak / Jantokia / Barra / Admin)
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(6.dp)
                ) {
                    listOf("Guztiak", "Jantokia", "Barra", "Admin").forEach { blokea ->
                        val aktibo = hautatutakoBlokea == blokea
                        val etiketa = when (blokea) {
                            "Guztiak" -> AppPreferences.tr("Guztiak", "Todos", "All")
                            "Jantokia" -> AppPreferences.tr("Jantokia", "Sala", "Dining")
                            "Barra" -> AppPreferences.tr("Barra", "Barra", "Bar")
                            else -> "Admin"
                        }
                        Surface(
                            modifier = Modifier
                                .weight(if (blokea == "Jantokia") 1.18f else 0.94f)
                                .clip(RoundedCornerShape(999.dp))
                                .clickable { hautatutakoBlokea = blokea },
                            shape = RoundedCornerShape(999.dp),
                            color = if (aktibo) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.surface,
                            border = BorderStroke(
                                1.dp,
                                if (aktibo) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.outline
                            )
                        ) {
                            Text(
                                text = etiketa,
                                fontSize = 12.sp,
                                fontWeight = if (aktibo) FontWeight.Bold else FontWeight.Medium,
                                color = if (aktibo) MaterialTheme.colorScheme.onPrimary else MaterialTheme.colorScheme.onSurfaceVariant,
                                maxLines = 1,
                                softWrap = false,
                                textAlign = TextAlign.Center,
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(horizontal = 4.dp, vertical = 9.dp)
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))

                LazyColumn(
                    modifier = Modifier.fillMaxSize(),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    if ((hautatutakoBlokea == "Guztiak" || hautatutakoBlokea == "Jantokia") && jantokiaLangileak.isNotEmpty()) {
                        item(key = "header_jantokia") {
                            LangileBlokeGoiburua(
                                titulua = AppPreferences.tr("Jantokia", "Sala", "Dining"),
                                kopurua = jantokiaLangileak.size,
                                ikonoa = Icons.Default.Restaurant,
                                kolorea = MaterialTheme.colorScheme.primary
                            )
                        }
                        items(jantokiaLangileak, key = { "j_${it.id}" }) { langilea ->
                            LangileaItem(
                                langilea = langilea,
                                azentuKolorea = MaterialTheme.colorScheme.primary,
                                onClick = { onLangileaAukeratua(langilea) }
                            )
                        }
                    }

                    if ((hautatutakoBlokea == "Guztiak" || hautatutakoBlokea == "Barra") && barraLangileak.isNotEmpty()) {
                        item(key = "header_barra") {
                            Spacer(modifier = Modifier.height(4.dp))
                            LangileBlokeGoiburua(
                                titulua = AppPreferences.tr("Barra", "Barra", "Bar"),
                                kopurua = barraLangileak.size,
                                ikonoa = Icons.Default.LocalBar,
                                kolorea = MaterialTheme.colorScheme.secondary
                            )
                        }
                        items(barraLangileak, key = { "b_${it.id}" }) { langilea ->
                            LangileaItem(
                                langilea = langilea,
                                azentuKolorea = MaterialTheme.colorScheme.secondary,
                                onClick = { onLangileaAukeratua(langilea) }
                            )
                        }
                    }

                    if ((hautatutakoBlokea == "Guztiak" || hautatutakoBlokea == "Admin") && adminLangileak.isNotEmpty()) {
                        item(key = "header_admin") {
                            Spacer(modifier = Modifier.height(4.dp))
                            LangileBlokeGoiburua(
                                titulua = "Admin",
                                kopurua = adminLangileak.size,
                                ikonoa = Icons.Default.AdminPanelSettings,
                                kolorea = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                        }
                        items(adminLangileak, key = { "a_${it.id}" }) { langilea ->
                            LangileaItem(
                                langilea = langilea,
                                azentuKolorea = MaterialTheme.colorScheme.onSurfaceVariant,
                                onClick = { onLangileaAukeratua(langilea) }
                            )
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun ApiStatusFooterPill(
    puntuaKolorea: Color,
    egoeraAzalpena: String,
    apiUrl: String,
    unekoWifiSsid: String?,
    onDarkHero: Boolean,
    onClick: () -> Unit
) {
    val cleanHost = apiUrl.removePrefix("http://").removePrefix("https://").removeSuffix("/api/v1/").removeSuffix("/")
    Surface(
        onClick = onClick,
        modifier = Modifier.fillMaxWidth(),
        color = if (onDarkHero) Color.White.copy(alpha = 0.12f) else MaterialTheme.colorScheme.surface,
        shape = RoundedCornerShape(14.dp),
        border = BorderStroke(
            1.dp,
            if (onDarkHero) puntuaKolorea.copy(alpha = 0.45f) else puntuaKolorea.copy(alpha = 0.35f)
        )
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(horizontal = 14.dp, vertical = 10.dp),
            verticalAlignment = Alignment.CenterVertically,
            horizontalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            Box(
                modifier = Modifier
                    .size(8.dp)
                    .clip(CircleShape)
                    .background(puntuaKolorea)
            )
            Text(
                text = egoeraAzalpena,
                fontSize = 11.sp,
                fontWeight = FontWeight.Bold,
                color = if (onDarkHero) WarmLinen else puntuaKolorea,
                maxLines = 1
            )
            Text(
                text = "· $cleanHost",
                fontFamily = JetBrainsMono,
                fontSize = 11.sp,
                fontWeight = FontWeight.Medium,
                color = if (onDarkHero) WarmLinen.copy(alpha = 0.78f) else MaterialTheme.colorScheme.onSurfaceVariant,
                maxLines = 1,
                overflow = TextOverflow.Ellipsis,
                modifier = Modifier.weight(1f)
            )
            if (unekoWifiSsid != null) {
                Icon(
                    imageVector = Icons.Default.Wifi,
                    contentDescription = "Wi-Fi",
                    tint = if (onDarkHero) AmberLight else StatusFree,
                    modifier = Modifier.size(14.dp)
                )
            }
        }
    }
}

@Composable
fun LangileBlokeGoiburua(
    titulua: String,
    kopurua: Int,
    ikonoa: ImageVector,
    kolorea: Color,
    modifier: Modifier = Modifier
) {
    Row(
        modifier = modifier
            .fillMaxWidth()
            .padding(top = 4.dp, bottom = 2.dp),
        verticalAlignment = Alignment.CenterVertically,
        horizontalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        Icon(
            imageVector = ikonoa,
            contentDescription = null,
            tint = kolorea,
            modifier = Modifier.size(14.dp)
        )
        Text(
            text = "${titulua.uppercase()} · $kopurua",
            fontSize = 11.sp,
            fontWeight = FontWeight.Bold,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
            letterSpacing = 1.3.sp
        )
        HorizontalDivider(
            modifier = Modifier.weight(1f),
            color = MaterialTheme.colorScheme.outline.copy(alpha = 0.65f)
        )
    }
}

@Composable
fun LangileaItem(
    langilea: LangileaDto,
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
    azentuKolorea: Color = ChiantiPrimary
) {
    val isDark = AppPreferences.isDarkTheme
    val avatarBg = when {
        langilea.rola.equals("Barrakoa", ignoreCase = true) -> if (isDark) Color(0xFF2B170B) else GoldTint
        langilea.rola.contains("Admin", ignoreCase = true) || langilea.rola.equals("Kudeatzailea", ignoreCase = true) ->
            MaterialTheme.colorScheme.surfaceVariant
        else -> if (isDark) ChiantiTintDark else ChiantiTint
    }

    Surface(
        onClick = onClick,
        modifier = modifier.fillMaxWidth(),
        color = MaterialTheme.colorScheme.surface,
        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline),
        shape = RoundedCornerShape(16.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(horizontal = 14.dp, vertical = 12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Box(
                modifier = Modifier
                    .size(44.dp)
                    .clip(CircleShape)
                    .background(avatarBg),
                contentAlignment = Alignment.Center
            ) {
                Text(
                    text = lortuInicialak(langilea.izena),
                    fontFamily = CinzelFont,
                    fontWeight = FontWeight.Bold,
                    fontSize = 14.sp,
                    color = azentuKolorea
                )
            }

            Spacer(modifier = Modifier.width(14.dp))

            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = langilea.izena,
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.onSurface
                )
                Text(
                    text = "${langilea.rola} · @${langilea.erabiltzailea}",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }

            Icon(
                imageVector = Icons.Default.ChevronRight,
                contentDescription = "PIN beharrezkoa",
                tint = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.7f),
                modifier = Modifier.size(20.dp)
            )
        }
    }
}

@Composable
fun PinSarreraDialog(
    langilea: LangileaDto,
    unekoPina: String,
    pinErrorea: String?,
    saioaHasten: Boolean,
    onPinTekla: (String) -> Unit,
    onDismiss: () -> Unit
) {
    val isDark = AppPreferences.isDarkTheme
    Dialog(onDismissRequest = onDismiss) {
        Surface(
            modifier = Modifier
                .fillMaxWidth()
                .padding(4.dp),
            shape = RoundedCornerShape(26.dp),
            color = MaterialTheme.colorScheme.surface,
            border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline),
            tonalElevation = 6.dp
        ) {
            Column(
                modifier = Modifier.padding(22.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        Box(
                            modifier = Modifier
                                .size(44.dp)
                                .clip(CircleShape)
                                .background(if (isDark) ChiantiTintDark else ChiantiTint),
                            contentAlignment = Alignment.Center
                        ) {
                            Text(
                                text = lortuInicialak(langilea.izena),
                                fontFamily = CinzelFont,
                                fontWeight = FontWeight.Bold,
                                fontSize = 15.sp,
                                color = MaterialTheme.colorScheme.primary
                            )
                        }
                        Column {
                            Text(
                                text = langilea.izena,
                                style = MaterialTheme.typography.titleMedium,
                                color = MaterialTheme.colorScheme.onSurface
                            )
                            Text(
                                text = AppPreferences.tr("Sartu 4 digituko PIN kodea", "Introduce tu PIN de 4 dígitos", "Enter 4-digit PIN"),
                                style = MaterialTheme.typography.bodySmall,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                        }
                    }

                    IconButton(onClick = onDismiss, enabled = !saioaHasten) {
                        Icon(
                            imageVector = Icons.Default.Close,
                            contentDescription = "Itxi",
                            tint = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }
                }

                Spacer(modifier = Modifier.height(18.dp))

                // PIN puntu animatuak
                Row(
                    horizontalArrangement = Arrangement.spacedBy(18.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    for (i in 0 until 4) {
                        val beteta = i < unekoPina.length
                        val dotSize by animateDpAsState(
                            targetValue = if (beteta) 18.dp else 14.dp,
                            animationSpec = tween(160),
                            label = "pinDotSize$i"
                        )
                        val dotColor by animateColorAsState(
                            targetValue = if (beteta) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.outline,
                            animationSpec = tween(160),
                            label = "pinDotColor$i"
                        )
                        Box(
                            modifier = Modifier
                                .size(dotSize)
                                .clip(CircleShape)
                                .background(dotColor)
                                .border(
                                    width = if (beteta) 2.dp else 0.dp,
                                    color = if (beteta) GoldBorder else Color.Transparent,
                                    shape = CircleShape
                                )
                        )
                    }
                }

                AnimatedVisibility(
                    visible = pinErrorea != null,
                    enter = fadeIn(tween(200)),
                    exit = fadeOut(tween(150))
                ) {
                    Column {
                        Spacer(modifier = Modifier.height(10.dp))
                        Text(
                            text = pinErrorea.orEmpty(),
                            color = MaterialTheme.colorScheme.error,
                            fontSize = 12.sp,
                            fontWeight = FontWeight.Medium,
                            textAlign = TextAlign.Center
                        )
                    }
                }

                Spacer(modifier = Modifier.height(20.dp))

                val teklak = listOf(
                    listOf("1", "2", "3"),
                    listOf("4", "5", "6"),
                    listOf("7", "8", "9"),
                    listOf("C", "0", "OK")
                )

                Column(verticalArrangement = Arrangement.spacedBy(10.dp)) {
                    for (lerro in teklak) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(10.dp)
                        ) {
                            for (tekla in lerro) {
                                val bg = when (tekla) {
                                    "OK" -> MaterialTheme.colorScheme.primary
                                    "C" -> if (isDark) Color(0xFF2D1214) else StatusOccupiedLight
                                    else -> MaterialTheme.colorScheme.background
                                }
                                val fg = when (tekla) {
                                    "OK" -> Color.White
                                    "C" -> MaterialTheme.colorScheme.error
                                    else -> MaterialTheme.colorScheme.onSurface
                                }
                                Surface(
                                    onClick = { onPinTekla(tekla) },
                                    modifier = Modifier
                                        .weight(1f)
                                        .height(56.dp),
                                    shape = RoundedCornerShape(16.dp),
                                    color = bg,
                                    border = if (tekla != "OK" && tekla != "C") {
                                        BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                                    } else null
                                ) {
                                    Box(contentAlignment = Alignment.Center) {
                                        if (tekla == "OK" && saioaHasten) {
                                            CircularProgressIndicator(
                                                modifier = Modifier.size(20.dp),
                                                color = Color.White,
                                                strokeWidth = 2.dp
                                            )
                                        } else {
                                            Text(
                                                text = tekla,
                                                fontFamily = JetBrainsMono,
                                                fontSize = 19.sp,
                                                fontWeight = FontWeight.Bold,
                                                color = fg
                                            )
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))
                Text(
                    text = AppPreferences.tr(
                        "Demo PINak: Maite (1002) · Gorka Barra (2001)",
                        "PINs demo: Maite (1002) · Gorka Barra (2001)",
                        "Demo PINs: Maite (1002) · Gorka Bar (2001)"
                    ),
                    fontFamily = JetBrainsMono,
                    fontSize = 10.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }
}

@Preview(showBackground = true, name = "LoginScreen - Argia")
@Composable
fun LoginScreenLightPreview() {
    KomanderaTheme(darkTheme = false) {
        LoginScreenContent(
            langileak = listOf(
                LangileaDto(1, "maite_zerb", "Maite Etxebarria", "Zerbitzaria", true),
                LangileaDto(2, "gorka_barra", "Gorka Zubizarreta", "Barrakoa", true),
                LangileaDto(3, "ander_admin", "Ander Urien", "Admin", true)
            ),
            kargatzen = false,
            erroreMezua = null,
            unekoWifiSsid = "Goierri_WiFi",
            apiKonexioEgoera = "KONEKTATUTA",
            egoeraAzalpena = "API Konektatuta",
            apiUrl = "http://10.100.30.101:5050/api/v1/",
            onLangileaAukeratua = {},
            onFreskatu = {},
            onIrekiEzarpenak = {}
        )
    }
}

@Preview(showBackground = true, name = "LoginScreen - Iluna")
@Composable
fun LoginScreenDarkPreview() {
    KomanderaTheme(darkTheme = true) {
        LoginScreenContent(
            langileak = listOf(
                LangileaDto(1, "maite_zerb", "Maite Etxebarria", "Zerbitzaria", true),
                LangileaDto(2, "gorka_barra", "Gorka Zubizarreta", "Barrakoa", true)
            ),
            kargatzen = false,
            erroreMezua = null,
            unekoWifiSsid = "Goierri_WiFi",
            apiKonexioEgoera = "KONEKTATUTA",
            egoeraAzalpena = "API Konektatuta",
            apiUrl = "http://10.100.30.101:5050/api/v1/",
            onLangileaAukeratua = {},
            onFreskatu = {},
            onIrekiEzarpenak = {}
        )
    }
}
