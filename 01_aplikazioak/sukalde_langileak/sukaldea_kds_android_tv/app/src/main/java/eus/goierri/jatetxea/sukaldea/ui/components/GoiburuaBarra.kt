package eus.goierri.jatetxea.sukaldea.ui.components

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.sukaldea.R
import eus.goierri.jatetxea.sukaldea.data.api.RetrofitClient
import eus.goierri.jatetxea.sukaldea.ui.theme.BurgundyDark
import eus.goierri.jatetxea.sukaldea.ui.theme.CharcoalWine
import eus.goierri.jatetxea.sukaldea.ui.theme.ChiantiDark
import eus.goierri.jatetxea.sukaldea.ui.theme.ChiantiTintDark
import eus.goierri.jatetxea.sukaldea.ui.theme.CinzelFont
import eus.goierri.jatetxea.sukaldea.ui.theme.DarkHairline
import eus.goierri.jatetxea.sukaldea.ui.theme.EveningEspresso
import eus.goierri.jatetxea.sukaldea.ui.theme.GoldBorder
import eus.goierri.jatetxea.sukaldea.ui.theme.JetBrainsMono
import eus.goierri.jatetxea.sukaldea.ui.theme.PlusJakartaSans
import eus.goierri.jatetxea.sukaldea.ui.theme.RoseLight
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusFree
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusFreeBright
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusFreeDark
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusInfoBright
import eus.goierri.jatetxea.sukaldea.ui.theme.TextPrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.TextSecondary
import eus.goierri.jatetxea.sukaldea.ui.theme.WarmGraphite

@Composable
fun GoiburuaBarra(
    isOnline: Boolean,
    azkenOrdua: String,
    eskaeraKopurua: Int,
    prestatuaKopurua: Int,
    historialaKopurua: Int,
    unekoFiltroa: String,
    serverUrl: String,
    showConfigExternal: Boolean = false,
    onDismissConfig: () -> Unit = {},
    onFiltroAldatu: (String) -> Unit,
    onGarbituPrestatutakoak: () -> Unit,
    onFreskatu: () -> Unit,
    onServerUrlAldatu: (String) -> Unit
) {
    var showIpDialog by remember { mutableStateOf(false) }
    var showClearConfirmDialog by remember { mutableStateOf(false) }
    val shouldShowDialog = showIpDialog || showConfigExternal

    Column(
        modifier = Modifier
            .fillMaxWidth()
            .background(CharcoalWine)
            .border(width = 1.dp, color = DarkHairline)
            .padding(horizontal = 24.dp, vertical = 14.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Ezkerrean: T1 Logo B eta Marka Titulua (Cinzel)
            Row(verticalAlignment = Alignment.CenterVertically) {
                Image(
                    painter = painterResource(id = R.drawable.ic_t1_logo_dark),
                    contentDescription = "T1 Jatetxea Logo",
                    modifier = Modifier.size(46.dp)
                )
                Spacer(modifier = Modifier.width(14.dp))
                Column {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Text(
                            text = "T1 JATETXEA · KDS",
                            color = TextPrimary,
                            fontSize = 20.sp,
                            fontFamily = CinzelFont,
                            fontWeight = FontWeight.Bold,
                            letterSpacing = 1.2.sp
                        )
                        Spacer(modifier = Modifier.width(10.dp))
                        Box(
                            modifier = Modifier
                                .clip(RoundedCornerShape(999.dp))
                                .background(ChiantiDark)
                                .padding(horizontal = 10.dp, vertical = 3.dp)
                        ) {
                            Text(
                                text = "SUKALDEA TV",
                                color = Color.White,
                                fontSize = 10.sp,
                                fontFamily = PlusJakartaSans,
                                fontWeight = FontWeight.ExtraBold,
                                letterSpacing = 0.8.sp
                            )
                        }
                    }
                    Text(
                        text = "Osteria & Parrilla · Android TV Pantaila Interaktiboa",
                        color = TextSecondary,
                        fontSize = 12.sp,
                        fontFamily = PlusJakartaSans
                    )
                }
            }

            // Eskuinean: Konexio Egoera, IP, Erlojua eta Freskatu
            Row(
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                // Konexio Badge
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    modifier = Modifier
                        .clip(RoundedCornerShape(999.dp))
                        .background(if (isOnline) StatusFreeDark else BurgundyDark)
                        .border(
                            1.dp,
                            if (isOnline) StatusFree else ChiantiDark,
                            RoundedCornerShape(999.dp)
                        )
                        .clickable { showIpDialog = true }
                        .padding(horizontal = 12.dp, vertical = 6.dp)
                ) {
                    Box(
                        modifier = Modifier
                            .size(8.dp)
                            .clip(CircleShape)
                            .background(if (isOnline) StatusFreeBright else RoseLight)
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = if (isOnline) "API ONLINE" else "API OFFLINE",
                        color = Color.White,
                        fontSize = 12.sp,
                        fontWeight = FontWeight.Bold,
                        fontFamily = JetBrainsMono
                    )
                }

                // IP eta Ataka aldatzeko botoia
                Button(
                    onClick = { showIpDialog = true },
                    colors = ButtonDefaults.buttonColors(containerColor = WarmGraphite),
                    border = BorderStroke(1.dp, if (!isOnline) ChiantiDark else DarkHairline),
                    shape = RoundedCornerShape(10.dp)
                ) {
                    Text(
                        text = serverUrl,
                        color = if (!isOnline) RoseLight else GoldBorder,
                        fontSize = 11.sp,
                        fontFamily = JetBrainsMono,
                        fontWeight = FontWeight.Bold
                    )
                }

                // Erloju ordua (Gold JetBrains Mono)
                Box(
                    modifier = Modifier
                        .clip(RoundedCornerShape(10.dp))
                        .background(WarmGraphite)
                        .border(1.dp, DarkHairline, RoundedCornerShape(10.dp))
                        .padding(horizontal = 12.dp, vertical = 8.dp)
                ) {
                    Text(
                        text = azkenOrdua,
                        color = GoldBorder,
                        fontSize = 14.sp,
                        fontFamily = JetBrainsMono,
                        fontWeight = FontWeight.Bold
                    )
                }

                // Freskatu botoia
                Button(
                    onClick = onFreskatu,
                    colors = ButtonDefaults.buttonColors(containerColor = WarmGraphite),
                    border = BorderStroke(1.dp, DarkHairline),
                    shape = RoundedCornerShape(10.dp)
                ) {
                    Text(
                        text = "Freskatu",
                        color = TextPrimary,
                        fontSize = 12.sp,
                        fontFamily = PlusJakartaSans,
                        fontWeight = FontWeight.Bold
                    )
                }
            }
        }

        Spacer(modifier = Modifier.height(12.dp))

        // Behean: Filtro Txipak (Guztiak, Sukaldean, Prestatua, Historiala) + Garbitu + Kontagailua
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Row(
                horizontalArrangement = Arrangement.spacedBy(8.dp),
                verticalAlignment = Alignment.CenterVertically
            ) {
                listOf("Guztiak", "Sukaldean", "Prestatua").forEach { filtro ->
                    val isSelected = unekoFiltroa.equals(filtro, ignoreCase = true)
                    Box(
                        modifier = Modifier
                            .clip(RoundedCornerShape(999.dp))
                            .background(if (isSelected) ChiantiDark else WarmGraphite)
                            .border(
                                1.dp,
                                if (isSelected) GoldBorder else DarkHairline,
                                RoundedCornerShape(999.dp)
                            )
                            .clickable { onFiltroAldatu(filtro) }
                            .padding(horizontal = 16.dp, vertical = 6.dp)
                    ) {
                        Text(
                            text = filtro,
                            color = if (isSelected) Color.White else TextSecondary,
                            fontSize = 12.sp,
                            fontFamily = PlusJakartaSans,
                            fontWeight = if (isSelected) FontWeight.Bold else FontWeight.SemiBold
                        )
                    }
                }

                // HISTORIALA Txip berezia (kontagailuarekin)
                val isHistoriala = unekoFiltroa.equals("Historiala", ignoreCase = true)
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.spacedBy(8.dp),
                    modifier = Modifier
                        .clip(RoundedCornerShape(999.dp))
                        .background(if (isHistoriala) ChiantiDark else ChiantiTintDark)
                        .border(
                            1.dp,
                            if (isHistoriala) GoldBorder else ChiantiDark,
                            RoundedCornerShape(999.dp)
                        )
                        .clickable { onFiltroAldatu("Historiala") }
                        .padding(horizontal = 14.dp, vertical = 6.dp)
                ) {
                    Text(
                        text = "Historiala",
                        color = Color.White,
                        fontSize = 12.sp,
                        fontFamily = PlusJakartaSans,
                        fontWeight = FontWeight.Bold
                    )
                    Box(
                        modifier = Modifier
                            .clip(RoundedCornerShape(999.dp))
                            .background(GoldBorder)
                            .padding(horizontal = 7.dp, vertical = 1.dp)
                    ) {
                        Text(
                            text = "$historialaKopurua",
                            color = EveningEspresso,
                            fontSize = 11.sp,
                            fontFamily = JetBrainsMono,
                            fontWeight = FontWeight.Bold
                        )
                    }
                }
            }

            Row(
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                if (prestatuaKopurua > 0 && !unekoFiltroa.equals("Historiala", ignoreCase = true)) {
                    Button(
                        onClick = { showClearConfirmDialog = true },
                        colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                        shape = RoundedCornerShape(8.dp)
                    ) {
                        Text(
                            text = "Garbitu prestatuak ($prestatuaKopurua)",
                            color = Color.White,
                            fontSize = 12.sp,
                            fontFamily = PlusJakartaSans,
                            fontWeight = FontWeight.Bold
                        )
                    }
                }

                Text(
                    text = if (unekoFiltroa.equals("Historiala", ignoreCase = true)) {
                        "$eskaeraKopurua eskaera historialean"
                    } else {
                        "$eskaeraKopurua eskaera aktibo"
                    },
                    color = if (unekoFiltroa.equals("Historiala", ignoreCase = true)) StatusInfoBright else GoldBorder,
                    fontSize = 13.sp,
                    fontFamily = JetBrainsMono,
                    fontWeight = FontWeight.Bold
                )
            }
        }
    }

    if (showClearConfirmDialog) {
        AlertDialog(
            onDismissRequest = { showClearConfirmDialog = false },
            containerColor = CharcoalWine,
            title = {
                Text(
                    text = "Prestatutakoak garbitu?",
                    fontFamily = CinzelFont,
                    fontWeight = FontWeight.Bold,
                    color = GoldBorder,
                    fontSize = 18.sp
                )
            },
            text = {
                Text(
                    text = "Prestatuta dauden $prestatuaKopurua eskaerak zerbitzatuta bezala markatu eta 'Historiala' atalera mugituko dira.",
                    color = TextPrimary,
                    fontFamily = PlusJakartaSans,
                    fontSize = 14.sp
                )
            },
            confirmButton = {
                Button(
                    onClick = {
                        onGarbituPrestatutakoak()
                        showClearConfirmDialog = false
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = StatusFree)
                ) {
                    Text("Mugitu historialera", color = Color.White, fontWeight = FontWeight.Bold)
                }
            },
            dismissButton = {
                TextButton(onClick = { showClearConfirmDialog = false }) {
                    Text("Utzi", color = TextSecondary)
                }
            }
        )
    }

    if (shouldShowDialog) {
        var host by remember(serverUrl) { mutableStateOf(RetrofitClient.lortuHost(serverUrl)) }
        var portStr by remember(serverUrl) { mutableStateOf(RetrofitClient.lortuPort(serverUrl).toString()) }

        val unekoPreviewUrl = remember(host, portStr) {
            val p = portStr.toIntOrNull() ?: 5050
            RetrofitClient.eraikiUrl(host, p)
        }

        AlertDialog(
            onDismissRequest = {
                showIpDialog = false
                onDismissConfig()
            },
            containerColor = CharcoalWine,
            title = {
                Text(
                    text = "Konfiguratu API Zerbitzaria eta Ataka",
                    fontFamily = CinzelFont,
                    fontWeight = FontWeight.Bold,
                    fontSize = 18.sp,
                    color = GoldBorder
                )
            },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(10.dp)) {
                    Text(
                        text = "Aukeratu aurre-ezarpen azkar bat edo pertsonalizatu IP-a eta Ataka:",
                        fontSize = 12.sp,
                        color = TextSecondary
                    )

                    Text("Konfigurazio azkarrak:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = GoldBorder)
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(6.dp)
                    ) {
                        Button(
                            onClick = {
                                host = "10.100.30.112"
                                portStr = "5050"
                            },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (host == "10.100.30.112" && portStr == "5050") ChiantiDark else WarmGraphite
                            ),
                            shape = RoundedCornerShape(8.dp),
                            modifier = Modifier.weight(1f)
                        ) {
                            Text("Innovatek\n5050", fontSize = 11.sp, textAlign = TextAlign.Center)
                        }
                        Button(
                            onClick = {
                                host = "172.16.243.150"
                                portStr = "5050"
                            },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (host == "172.16.243.150" && portStr == "5050") ChiantiDark else WarmGraphite
                            ),
                            shape = RoundedCornerShape(8.dp),
                            modifier = Modifier.weight(1f)
                        ) {
                            Text("Eskola\n5050", fontSize = 11.sp, textAlign = TextAlign.Center)
                        }
                        Button(
                            onClick = {
                                host = "10.0.2.2"
                                portStr = "5050"
                            },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (host == "10.0.2.2") ChiantiDark else WarmGraphite
                            ),
                            shape = RoundedCornerShape(8.dp),
                            modifier = Modifier.weight(1f)
                        ) {
                            Text("Emuladorea\n10.0.2.2", fontSize = 11.sp, textAlign = TextAlign.Center)
                        }
                    }

                    Text("Zerbitzariaren IP-a edo Ostalaria:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = TextPrimary)
                    OutlinedTextField(
                        value = host,
                        onValueChange = { host = it },
                        label = { Text("IP / Host") },
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )

                    Text("API Ataka (Portua):", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = TextPrimary)
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(8.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        OutlinedTextField(
                            value = portStr,
                            onValueChange = { if (it.all { ch -> ch.isDigit() }) portStr = it },
                            label = { Text("Ataka") },
                            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                            singleLine = true,
                            modifier = Modifier.weight(1f)
                        )
                        Button(
                            onClick = { portStr = "5050" },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (portStr == "5050") StatusFreeDark else WarmGraphite
                            ),
                            border = BorderStroke(1.dp, if (portStr == "5050") StatusFree else DarkHairline),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("5050 (Docker)", fontSize = 11.sp)
                        }
                        Button(
                            onClick = { portStr = "5000" },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (portStr == "5000") StatusFreeDark else WarmGraphite
                            ),
                            border = BorderStroke(1.dp, if (portStr == "5000") StatusFree else DarkHairline),
                            shape = RoundedCornerShape(8.dp)
                        ) {
                            Text("5000 (Zuzena)", fontSize = 11.sp)
                        }
                    }

                    Box(
                        modifier = Modifier
                            .fillMaxWidth()
                            .clip(RoundedCornerShape(8.dp))
                            .background(EveningEspresso)
                            .border(1.dp, DarkHairline, RoundedCornerShape(8.dp))
                            .padding(10.dp)
                    ) {
                        Text(
                            text = "Helbide Osoa: $unekoPreviewUrl",
                            color = GoldBorder,
                            fontSize = 11.sp,
                            fontFamily = JetBrainsMono
                        )
                    }
                }
            },
            confirmButton = {
                Button(
                    onClick = {
                        val p = portStr.toIntOrNull() ?: 5050
                        val berria = RetrofitClient.eraikiUrl(host, p)
                        onServerUrlAldatu(berria)
                        showIpDialog = false
                        onDismissConfig()
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = StatusFree)
                ) {
                    Text("Gorde eta Konektatu", fontWeight = FontWeight.Bold, color = Color.White)
                }
            },
            dismissButton = {
                TextButton(onClick = {
                    showIpDialog = false
                    onDismissConfig()
                }) {
                    Text("Utzi", color = TextSecondary)
                }
            }
        )
    }
}
