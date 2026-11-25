package eus.goierri.jatetxea.komandera.ui.components

import androidx.compose.animation.AnimatedContent
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.togetherWith
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import androidx.compose.ui.window.DialogProperties
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.network.NetworkProfiles
import eus.goierri.jatetxea.komandera.ui.state.AppPreferences
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch

@Composable
fun EzarpenakDialog(
    zerbitzaria: LangileaDto?,
    onDismiss: () -> Unit,
    onApiConfigAldatua: (() -> Unit)? = null
) {
    val context = LocalContext.current
    val scope = rememberCoroutineScope()
    var fitxaAktiboa by remember { mutableStateOf(if (zerbitzaria != null) "PREF" else "SAREA") }

    // Sare aldagaiak
    var unekoWifiSsid by remember { mutableStateOf(NetworkProfiles.lortuUnekoWifiIzena(context)) }
    var behinBehinekoUrl by remember { mutableStateOf(ApiClient.baseUrl) }
    var testEmaitza by remember { mutableStateOf<String?>(null) }
    var testKargatzen by remember { mutableStateOf(false) }

    Dialog(
        onDismissRequest = onDismiss,
        properties = DialogProperties(usePlatformDefaultWidth = false)
    ) {
        Card(
            modifier = Modifier
                .fillMaxWidth(0.94f)
                .fillMaxHeight(0.85f),
            shape = RoundedCornerShape(20.dp),
            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
            border = androidx.compose.foundation.BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.4f)),
            elevation = CardDefaults.cardElevation(defaultElevation = 8.dp)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(18.dp)
            ) {
                // Titulua eta Itxi
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
                                imageVector = Icons.Default.Settings,
                                contentDescription = null,
                                tint = AmberAccent,
                                modifier = Modifier.size(20.dp)
                            )
                        }
                        Text(
                            text = "Aplikazioaren Ezarpenak",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.onSurface
                        )
                    }

                    IconButton(onClick = onDismiss, modifier = Modifier.size(28.dp)) {
                        Icon(
                            imageVector = Icons.Default.Close,
                            contentDescription = "Itxi",
                            tint = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))

                // Fitxak (Nahi bada Sare Profilak edo Itxura/Hizkuntza)
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .clip(RoundedCornerShape(10.dp))
                        .background(MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f))
                        .padding(4.dp)
                ) {
                    Surface(
                        modifier = Modifier
                            .weight(1f)
                            .clip(RoundedCornerShape(8.dp))
                            .clickable { fitxaAktiboa = "PREF" },
                        color = if (fitxaAktiboa == "PREF") ChiantiPrimary else Color.Transparent
                    ) {
                        Text(
                            text = "Itxura & Hizkuntza",
                            fontSize = 12.sp,
                            fontWeight = if (fitxaAktiboa == "PREF") FontWeight.Bold else FontWeight.Medium,
                            color = if (fitxaAktiboa == "PREF") Color.White else MaterialTheme.colorScheme.onSurfaceVariant,
                            modifier = Modifier.padding(vertical = 8.dp),
                            textAlign = androidx.compose.ui.text.style.TextAlign.Center
                        )
                    }

                    Surface(
                        modifier = Modifier
                            .weight(1f)
                            .clip(RoundedCornerShape(8.dp))
                            .clickable { fitxaAktiboa = "SAREA" },
                        color = if (fitxaAktiboa == "SAREA") ChiantiPrimary else Color.Transparent
                    ) {
                        Text(
                            text = "Sarea & API IP",
                            fontSize = 12.sp,
                            fontWeight = if (fitxaAktiboa == "SAREA") FontWeight.Bold else FontWeight.Medium,
                            color = if (fitxaAktiboa == "SAREA") Color.White else MaterialTheme.colorScheme.onSurfaceVariant,
                            modifier = Modifier.padding(vertical = 8.dp),
                            textAlign = androidx.compose.ui.text.style.TextAlign.Center
                        )
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))

                AnimatedContent(
                    targetState = fitxaAktiboa,
                    modifier = Modifier.weight(1f),
                    transitionSpec = { fadeIn(tween(200)) togetherWith fadeOut(tween(150)) },
                    label = "settingsTab"
                ) { tab ->
                if (tab == "PREF") {
                    Column(
                        modifier = Modifier
                            .fillMaxSize()
                            .verticalScroll(rememberScrollState())
                    ) {
                        // 1. ERABILTZAILEAREN DATUAK
                        if (zerbitzaria != null) {
                            Text(
                                text = "SAIOA HASITA",
                                fontSize = 11.sp,
                                fontWeight = FontWeight.Bold,
                                color = AmberAccent,
                                letterSpacing = 1.sp
                            )
                            Spacer(modifier = Modifier.height(8.dp))

                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .clip(RoundedCornerShape(12.dp))
                                    .background(MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f))
                                    .padding(12.dp),
                                verticalAlignment = Alignment.CenterVertically,
                                horizontalArrangement = Arrangement.spacedBy(12.dp)
                            ) {
                                Box(
                                    modifier = Modifier
                                        .size(44.dp)
                                        .clip(CircleShape)
                                        .background(ChiantiPrimary),
                                    contentAlignment = Alignment.Center
                                ) {
                                    Icon(
                                        imageVector = Icons.Default.Person,
                                        contentDescription = null,
                                        tint = Color.White,
                                        modifier = Modifier.size(24.dp)
                                    )
                                }

                                Column(modifier = Modifier.weight(1f)) {
                                    Text(
                                        text = zerbitzaria.izena,
                                        fontSize = 16.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = MaterialTheme.colorScheme.onSurface
                                    )
                                    Text(
                                        text = "Erabiltzailea: @${zerbitzaria.erabiltzailea}",
                                        fontSize = 12.sp,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                    Text(
                                        text = "Rola: ${zerbitzaria.rola}  ·  ID: #${zerbitzaria.id}",
                                        fontSize = 11.sp,
                                        color = TextMuted
                                    )
                                }
                            }

                            Spacer(modifier = Modifier.height(16.dp))
                        }

                        // 2. MODO ARGIA / ILUNA (GAUA / EGUNA)
                        Text(
                            text = "ITXURA ETA TEMA",
                            fontSize = 11.sp,
                            fontWeight = FontWeight.Bold,
                            color = AmberAccent,
                            letterSpacing = 1.sp
                        )
                        Spacer(modifier = Modifier.height(8.dp))

                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .clip(RoundedCornerShape(12.dp))
                                .background(MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f))
                                .padding(horizontal = 12.dp, vertical = 6.dp),
                            horizontalArrangement = Arrangement.SpaceBetween,
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Row(
                                modifier = Modifier.weight(1f),
                                verticalAlignment = Alignment.CenterVertically,
                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                Icon(
                                    imageVector = if (AppPreferences.isDarkTheme) Icons.Default.DarkMode else Icons.Default.LightMode,
                                    contentDescription = null,
                                    tint = if (AppPreferences.isDarkTheme) AmberAccent else Color(0xFFF59E0B),
                                    modifier = Modifier.size(20.dp)
                                )
                                Text(
                                    text = if (AppPreferences.isDarkTheme) "Modu Iluna (OLED)" else "Modu Argia (Marmol)",
                                    fontSize = 13.sp,
                                    fontWeight = FontWeight.Medium,
                                    color = MaterialTheme.colorScheme.onSurface,
                                    maxLines = 1
                                )
                            }

                            Switch(
                                checked = AppPreferences.isDarkTheme,
                                onCheckedChange = { AppPreferences.setDarkTheme(context, it) },
                                modifier = Modifier.scale(0.85f),
                                colors = SwitchDefaults.colors(
                                    checkedThumbColor = Color.White,
                                    checkedTrackColor = ChiantiPrimary,
                                    uncheckedThumbColor = Color.White,
                                    uncheckedTrackColor = Color.Gray
                                )
                            )
                        }

                        Spacer(modifier = Modifier.height(16.dp))

                        // 3. HIZKUNTZA ALDATZEA
                        Text(
                            text = "HIZKUNTZA (IDIOMA)",
                            fontSize = 11.sp,
                            fontWeight = FontWeight.Bold,
                            color = AmberAccent,
                            letterSpacing = 1.sp
                        )
                        Spacer(modifier = Modifier.height(8.dp))

                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(8.dp)
                        ) {
                            val hizkuntzak = listOf("EU" to "Euskara", "ES" to "Castellano", "EN" to "English")
                            hizkuntzak.forEach { (kod, izena) ->
                                val aktibo = AppPreferences.hizkuntza == kod
                                Surface(
                                    modifier = Modifier
                                        .weight(1f)
                                        .clip(RoundedCornerShape(10.dp))
                                        .clickable { AppPreferences.setHizkuntza(context, kod) },
                                    color = if (aktibo) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.6f),
                                    border = androidx.compose.foundation.BorderStroke(
                                        1.dp,
                                        if (aktibo) GoldBorder else Color.Transparent
                                    )
                                ) {
                                    Column(
                                        modifier = Modifier.padding(vertical = 10.dp),
                                        horizontalAlignment = Alignment.CenterHorizontally
                                    ) {
                                        Text(
                                            text = kod,
                                            fontSize = 14.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = if (aktibo) Color.White else MaterialTheme.colorScheme.onSurface
                                        )
                                        Text(
                                            text = izena,
                                            fontSize = 10.sp,
                                            color = if (aktibo) AmberLight else MaterialTheme.colorScheme.onSurfaceVariant
                                        )
                                    }
                                }
                            }
                        }
                    }
                } else {
                    // SAREA & API IP KONFIGURAZIOA
                    Column(
                        modifier = Modifier
                            .fillMaxSize()
                            .verticalScroll(rememberScrollState())
                    ) {
                        Surface(
                            color = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.6f),
                            shape = RoundedCornerShape(10.dp),
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
                                        color = if (unekoWifiSsid != null) StatusReady else MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                    Text(
                                        text = "Klikatu profil batean URL egokia automatikoki ezartzeko",
                                        fontSize = 10.sp,
                                        color = TextMuted
                                    )
                                }
                            }
                        }

                        Spacer(modifier = Modifier.height(12.dp))
                        Text(
                            text = "Profil Azkarrak:",
                            fontSize = 12.sp,
                            fontWeight = FontWeight.Bold,
                            color = AmberAccent
                        )
                        Spacer(modifier = Modifier.height(6.dp))

                        NetworkProfiles.PROFILAK.forEach { profila ->
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
                                color = if (hautatuta) ChiantiPrimary.copy(alpha = 0.15f) else MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.4f),
                                shape = RoundedCornerShape(8.dp),
                                border = androidx.compose.foundation.BorderStroke(
                                    1.dp,
                                    if (hautatuta) ChiantiPrimary else Color.Transparent
                                )
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
                                                color = if (hautatuta) ChiantiPrimary else MaterialTheme.colorScheme.onSurface
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
                                            color = MaterialTheme.colorScheme.onSurfaceVariant
                                        )
                                    }
                                    if (hautatuta) {
                                        Icon(
                                            imageVector = Icons.Default.CheckCircle,
                                            contentDescription = "Hautatua",
                                            tint = ChiantiPrimary,
                                            modifier = Modifier.size(18.dp)
                                        )
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
                                                testEmaitza = "OK: Konektatuta!"
                                            } else {
                                                testEmaitza = "Errorea: HTTP ${res.code()}"
                                            }
                                        } catch (e: Exception) {
                                            testEmaitza = "Errorea: ${e.localizedMessage ?: "Ezin da konektatu"}"
                                        } finally {
                                            testKargatzen = false
                                        }
                                    }
                                },
                                enabled = !testKargatzen
                            ) {
                                if (testKargatzen) {
                                    CircularProgressIndicator(modifier = Modifier.size(16.dp), strokeWidth = 2.dp)
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

                        Spacer(modifier = Modifier.height(6.dp))
                    }
                }
                }

                Spacer(modifier = Modifier.height(12.dp))

                // BEHEKO BOTOIAK (Beti ikusgai — inoiz ez moztuta!)
                if (fitxaAktiboa == "SAREA") {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(10.dp)
                    ) {
                        OutlinedButton(
                            onClick = onDismiss,
                            modifier = Modifier
                                .weight(0.9f)
                                .height(48.dp),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Text(
                                AppPreferences.tr("Itxi", "Cerrar", "Close"),
                                color = MaterialTheme.colorScheme.onSurface,
                                fontWeight = FontWeight.SemiBold
                            )
                        }
                        Button(
                            onClick = {
                                ApiClient.saveBaseUrl(context, behinBehinekoUrl)
                                onApiConfigAldatua?.invoke()
                                onDismiss()
                            },
                            modifier = Modifier
                                .weight(1.4f)
                                .height(48.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Text(
                                AppPreferences.tr("Gorde eta Aplikatu", "Guardar y Aplicar", "Save & Apply"),
                                color = Color.White,
                                fontWeight = FontWeight.Bold
                            )
                        }
                    }
                } else {
                    Button(
                        onClick = onDismiss,
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(48.dp),
                        colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Text(
                            AppPreferences.tr("Itxi", "Cerrar", "Close"),
                            color = Color.White,
                            fontWeight = FontWeight.Bold
                        )
                    }
                }
            }
        }
    }
}

@Preview(showBackground = true, name = "EzarpenakDialog")
@Composable
fun EzarpenakDialogPreview() {
    KomanderaTheme(darkTheme = true) {
        EzarpenakDialog(
            zerbitzaria = LangileaDto(1, "ane", "Ane Etxeberria", "Zerbitzaria"),
            onDismiss = {}
        )
    }
}
