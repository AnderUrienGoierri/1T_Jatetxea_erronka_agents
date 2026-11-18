package eus.goierri.jatetxea.sukaldea.ui.components

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
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
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
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
    onFiltroAldatu: (String) -> Unit,
    onFreskatu: () -> Unit,
    onServerUrlAldatu: (String) -> Unit
) {
    var showIpDialog by remember { mutableStateOf(false) }

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

                // IP aldatzeko botoia
                Box(
                    modifier = Modifier
                        .clip(RoundedCornerShape(8.dp))
                        .background(Color(0xFF1E293B))
                        .border(1.dp, Color(0xFF334155), RoundedCornerShape(8.dp))
                        .clickable { showIpDialog = true }
                        .padding(horizontal = 10.dp, vertical = 6.dp)
                ) {
                    Text(
                        text = "IP: $serverUrl",
                        color = SkyInfo,
                        fontSize = 11.sp,
                        fontFamily = FontFamily.Monospace
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

    if (showIpDialog) {
        var newUrl by remember { mutableStateOf(serverUrl) }
        AlertDialog(
            onDismissRequest = { showIpDialog = false },
            title = { Text("Konfiguratu Backend API Helbidea", fontWeight = FontWeight.Bold) },
            text = {
                Column {
                    Text(
                        "Android Emuladorean: http://10.0.2.2:5000/api/v1/\nTelebista fisikoan edo LANean: http://192.168.1.166:5000/api/v1/",
                        fontSize = 12.sp,
                        color = TextSecondary
                    )
                    Spacer(modifier = Modifier.size(10.dp))
                    OutlinedTextField(
                        value = newUrl,
                        onValueChange = { newUrl = it },
                        label = { Text("API Base URL") },
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )
                }
            },
            confirmButton = {
                Button(onClick = {
                    onServerUrlAldatu(newUrl.trim())
                    showIpDialog = false
                }) {
                    Text("Gorde eta Konektatu")
                }
            },
            dismissButton = {
                TextButton(onClick = { showIpDialog = false }) {
                    Text("Utzi")
                }
            }
        )
    }
}
