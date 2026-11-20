package eus.goierri.jatetxea.sukaldea.ui.components

import androidx.compose.foundation.BorderStroke
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
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.sukaldea.data.api.RetrofitClient
import eus.goierri.jatetxea.sukaldea.ui.theme.DarkSurface
import eus.goierri.jatetxea.sukaldea.ui.theme.EmeraldDark
import eus.goierri.jatetxea.sukaldea.ui.theme.EmeraldLight
import eus.goierri.jatetxea.sukaldea.ui.theme.EmeraldSuccess
import eus.goierri.jatetxea.sukaldea.ui.theme.RoseDark
import eus.goierri.jatetxea.sukaldea.ui.theme.RoseLight
import eus.goierri.jatetxea.sukaldea.ui.theme.RosePrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.SkyInfo
import eus.goierri.jatetxea.sukaldea.ui.theme.TextMuted
import eus.goierri.jatetxea.sukaldea.ui.theme.TextPrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.TextSecondary

@Composable
fun GoiburuaBarra(
    isOnline: Boolean,
    azkenOrdua: String,
    eskaeraKopurua: Int,
    unekoFiltroa: String,
    serverUrl: String,
    showConfigExternal: Boolean = false,
    onDismissConfig: () -> Unit = {},
    onFiltroAldatu: (String) -> Unit,
    onFreskatu: () -> Unit,
    onServerUrlAldatu: (String) -> Unit
) {
    var showIpDialog by remember { mutableStateOf(false) }
    val shouldShowDialog = showIpDialog || showConfigExternal

    Column(
        modifier = Modifier
            .fillMaxWidth()
            .background(DarkSurface)
            .border(width = 1.dp, color = Color(0xFF1E293B))
            .padding(horizontal = 24.dp, vertical = 14.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Ezkerrean: Titulua eta Ikonoa
            Row(verticalAlignment = Alignment.CenterVertically) {
                Box(
                    modifier = Modifier
                        .size(44.dp)
                        .clip(RoundedCornerShape(10.dp))
                        .background(RoseDark)
                        .border(1.dp, RosePrimary, RoundedCornerShape(10.dp)),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        text = "KDS",
                        color = Color.White,
                        fontWeight = FontWeight.Bold,
                        fontSize = 15.sp
                    )
                }
                Spacer(modifier = Modifier.width(14.dp))
                Column {
                    Text(
                        text = "1T JATETXEA · SUKALDEKO KDS PANTAILA",
                        color = TextPrimary,
                        fontSize = 18.sp,
                        fontWeight = FontWeight.ExtraBold,
                        letterSpacing = 0.5.sp
                    )
                    Text(
                        text = "Goierri Eskola · Android TV Pantaila Interaktiboa",
                        color = TextSecondary,
                        fontSize = 12.sp
                    )
                }
            }

            // Eskuinean: Konexio Egoera, Freskatze Ordua eta Zerbitzariaren IP
            Row(
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.spacedBy(14.dp)
            ) {
                // Konexio Badge
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    modifier = Modifier
                        .clip(RoundedCornerShape(8.dp))
                        .background(if (isOnline) EmeraldDark else RoseDark)
                        .border(
                            1.dp,
                            if (isOnline) EmeraldSuccess else RosePrimary,
                            RoundedCornerShape(8.dp)
                        )
                        .clickable { showIpDialog = true }
                        .padding(horizontal = 10.dp, vertical = 6.dp)
                ) {
                    Box(
                        modifier = Modifier
                            .size(8.dp)
                            .clip(CircleShape)
                            .background(if (isOnline) EmeraldLight else RoseLight)
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = if (isOnline) "API ONLINE" else "API OFFLINE",
                        color = Color.White,
                        fontSize = 12.sp,
                        fontWeight = FontWeight.Bold,
                        fontFamily = FontFamily.Monospace
                    )
                }

                // IP eta Ataka aldatzeko botoi nabarmena
                Button(
                    onClick = { showIpDialog = true },
                    colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF1E293B)),
                    border = BorderStroke(1.dp, if (!isOnline) RosePrimary else Color(0xFF334155)),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Text(
                        text = "⚙️ $serverUrl",
                        color = if (!isOnline) RoseLight else SkyInfo,
                        fontSize = 11.sp,
                        fontFamily = FontFamily.Monospace,
                        fontWeight = FontWeight.Bold
                    )
                }

                // Erloju ordua
                Text(
                    text = azkenOrdua,
                    color = TextMuted,
                    fontSize = 13.sp,
                    fontFamily = FontFamily.Monospace,
                    fontWeight = FontWeight.SemiBold
                )

                // Freskatu botoia
                Button(
                    onClick = onFreskatu,
                    colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF334155)),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Text("Freskatu", fontSize = 12.sp, fontWeight = FontWeight.Bold)
                }
            }
        }

        Spacer(modifier = Modifier.size(10.dp))

        // Behean: Filtro Txipak eta Eskaera Kopurua
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                listOf("Guztiak", "Sukaldean", "Prestatua").forEach { filtro ->
                    val isSelected = unekoFiltroa.equals(filtro, ignoreCase = true)
                    Box(
                        modifier = Modifier
                            .clip(RoundedCornerShape(20.dp))
                            .background(if (isSelected) RosePrimary else Color(0xFF1E293B))
                            .border(
                                1.dp,
                                if (isSelected) RoseDark else Color(0xFF334155),
                                RoundedCornerShape(20.dp)
                            )
                            .clickable { onFiltroAldatu(filtro) }
                            .padding(horizontal = 14.dp, vertical = 5.dp)
                    ) {
                        Text(
                            text = filtro,
                            color = if (isSelected) Color.White else TextSecondary,
                            fontSize = 12.sp,
                            fontWeight = if (isSelected) FontWeight.Bold else FontWeight.Medium
                        )
                    }
                }
            }

            Text(
                text = "$eskaeraKopurua eskaera aktibo",
                color = SkyInfo,
                fontSize = 13.sp,
                fontWeight = FontWeight.Bold
            )
        }
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
            title = {
                Text(
                    text = "⚙️ Konfiguratu API Zerbitzaria eta Ataka",
                    fontWeight = FontWeight.Bold,
                    fontSize = 18.sp,
                    color = TextPrimary
                )
            },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(10.dp)) {
                    Text(
                        text = "Aukeratu aurre-ezarpen azkar bat edo pertsonalizatu IP-a eta Ataka:",
                        fontSize = 12.sp,
                        color = TextSecondary
                    )

                    // 1. Aurre-ezarpen azkarrak (Presets)
                    Text("Konfigurazio azkarrak:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = SkyInfo)
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(6.dp)
                    ) {
                        Button(
                            onClick = {
                                host = "172.16.243.102"
                                portStr = "5050"
                            },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (host == "172.16.243.102" && portStr == "5050") RosePrimary else Color(0xFF1E293B)
                            ),
                            shape = RoundedCornerShape(6.dp),
                            modifier = Modifier.weight(1f)
                        ) {
                            Text("TV · 5050\n(Docker)", fontSize = 11.sp, textAlign = TextAlign.Center)
                        }
                        Button(
                            onClick = {
                                host = "172.16.243.102"
                                portStr = "5000"
                            },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (host == "172.16.243.102" && portStr == "5000") RosePrimary else Color(0xFF1E293B)
                            ),
                            shape = RoundedCornerShape(6.dp),
                            modifier = Modifier.weight(1f)
                        ) {
                            Text("TV · 5000\n(Zuzenekoa)", fontSize = 11.sp, textAlign = TextAlign.Center)
                        }
                        Button(
                            onClick = {
                                host = "10.0.2.2"
                                portStr = "5050"
                            },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (host == "10.0.2.2") RosePrimary else Color(0xFF1E293B)
                            ),
                            shape = RoundedCornerShape(6.dp),
                            modifier = Modifier.weight(1f)
                        ) {
                            Text("Emuladorea\n(10.0.2.2)", fontSize = 11.sp, textAlign = TextAlign.Center)
                        }
                    }

                    Spacer(modifier = Modifier.height(2.dp))

                    // 2. IP / Ostalaria
                    Text("Zerbitzariaren IP-a edo Ostalaria:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = TextPrimary)
                    OutlinedTextField(
                        value = host,
                        onValueChange = { host = it },
                        label = { Text("IP / Host") },
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )

                    // 3. Ataka (Portua)
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
                                containerColor = if (portStr == "5050") EmeraldDark else Color(0xFF1E293B)
                            ),
                            border = BorderStroke(1.dp, if (portStr == "5050") EmeraldSuccess else Color(0xFF334155)),
                            shape = RoundedCornerShape(6.dp)
                        ) {
                            Text("5050 (Docker)", fontSize = 11.sp)
                        }
                        Button(
                            onClick = { portStr = "5000" },
                            colors = ButtonDefaults.buttonColors(
                                containerColor = if (portStr == "5000") EmeraldDark else Color(0xFF1E293B)
                            ),
                            border = BorderStroke(1.dp, if (portStr == "5000") EmeraldSuccess else Color(0xFF334155)),
                            shape = RoundedCornerShape(6.dp)
                        ) {
                            Text("5000 (Zuzena)", fontSize = 11.sp)
                        }
                    }

                    Spacer(modifier = Modifier.height(2.dp))

                    // 4. URL Aurrebista
                    Box(
                        modifier = Modifier
                            .fillMaxWidth()
                            .background(Color(0xFF0F172A))
                            .border(1.dp, Color(0xFF334155), RoundedCornerShape(6.dp))
                            .padding(8.dp)
                    ) {
                        Text(
                            text = "Helbide Osoa: $unekoPreviewUrl",
                            color = SkyInfo,
                            fontSize = 11.sp,
                            fontFamily = FontFamily.Monospace
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
                    colors = ButtonDefaults.buttonColors(containerColor = EmeraldSuccess)
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
