package eus.goierri.jatetxea.komandera.ui.components

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import androidx.compose.ui.window.DialogProperties
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.ErreserbaDto
import eus.goierri.jatetxea.komandera.data.model.ErreserbaSortuDto
import eus.goierri.jatetxea.komandera.data.model.MahaiaDto
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch
import java.text.SimpleDateFormat
import java.util.*

@Composable
fun ErreserbakDialog(
    mahaiak: List<MahaiaDto>,
    onDismiss: () -> Unit,
    onErreserbaSortua: () -> Unit
) {
    val scope = rememberCoroutineScope()
    var fitxaAktiboa by remember { mutableStateOf("ZERRENDA") } // "ZERRENDA" edo "BERRIA"

    val gaur = remember {
        val sdf = SimpleDateFormat("yyyy-MM-dd", Locale.getDefault())
        sdf.format(Date())
    }

    var hautatutakoData by remember { mutableStateOf(gaur) }
    var erreserbak by remember { mutableStateOf<List<ErreserbaDto>>(emptyList()) }
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

    fun kargatuErreserbak() {
        scope.launch {
            kargatzen = true
            errorea = null
            try {
                val res = ApiClient.service.getErreserbak(data = hautatutakoData)
                if (res.isSuccessful && res.body()?.dataList != null) {
                    erreserbak = res.body()!!.dataList!!
                } else {
                    erreserbak = emptyList()
                }
            } catch (e: Exception) {
                errorea = "Errorea: ${e.localizedMessage}"
            } finally {
                kargatzen = false
            }
        }
    }

    LaunchedEffect(hautatutakoData) {
        kargatuErreserbak()
    }

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
                            text = "Aretoko Erreserbak",
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

                Spacer(modifier = Modifier.height(14.dp))

                // Nabigazio Fitxak (Zerrenda vs Erreserba Berria)
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .clip(RoundedCornerShape(10.dp))
                        .background(MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f))
                        .padding(4.dp)
                ) {
                    TabItem(
                        titulua = "Gaurko Erreserbak (${erreserbak.size})",
                        aktibo = fitxaAktiboa == "ZERRENDA",
                        modifier = Modifier.weight(1f),
                        onClick = { fitxaAktiboa = "ZERRENDA" }
                    )
                    TabItem(
                        titulua = "+ Sortu Erreserba",
                        aktibo = fitxaAktiboa == "BERRIA",
                        modifier = Modifier.weight(1f),
                        onClick = { fitxaAktiboa = "BERRIA" }
                    )
                }

                Spacer(modifier = Modifier.height(14.dp))

                if (fitxaAktiboa == "ZERRENDA") {
                    // Data hautatua eta freskatu
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Text(
                            text = "Data: $hautatutakoData",
                            fontSize = 13.sp,
                            fontWeight = FontWeight.SemiBold,
                            color = AmberAccent
                        )
                        IconButton(onClick = { kargatuErreserbak() }, modifier = Modifier.size(28.dp)) {
                            Icon(Icons.Default.Refresh, contentDescription = "Freskatu", tint = AmberAccent)
                        }
                    }

                    Spacer(modifier = Modifier.height(8.dp))

                    if (kargatzen) {
                        Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                            CircularProgressIndicator(color = AmberAccent)
                        }
                    } else if (erreserbak.isEmpty()) {
                        Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                            Column(horizontalAlignment = Alignment.CenterHorizontally) {
                                Icon(Icons.Default.EventBusy, contentDescription = null, tint = TextMuted, modifier = Modifier.size(48.dp))
                                Spacer(modifier = Modifier.height(8.dp))
                                Text("Ez dago erreserbarik data honetarako", color = TextMuted, fontSize = 14.sp)
                                Spacer(modifier = Modifier.height(12.dp))
                                Button(
                                    onClick = { fitxaAktiboa = "BERRIA" },
                                    colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary)
                                ) {
                                    Text("Gehitu Lehen Erreserba", color = Color.White)
                                }
                            }
                        }
                    } else {
                        LazyColumn(
                            verticalArrangement = Arrangement.spacedBy(8.dp),
                            modifier = Modifier.fillMaxSize()
                        ) {
                            items(erreserbak) { erreserba ->
                                ErreserbaKarta(erreserba)
                            }
                        }
                    }
                } else {
                    // BERRIA FORMULARIOA
                    LazyColumn(
                        verticalArrangement = Arrangement.spacedBy(10.dp),
                        modifier = Modifier.weight(1f)
                    ) {
                        item {
                            OutlinedTextField(
                                value = bezeroIzena,
                                onValueChange = { bezeroIzena = it },
                                label = { Text("Bezeroaren Izena *") },
                                modifier = Modifier.fillMaxWidth(),
                                singleLine = true
                            )
                        }
                        item {
                            OutlinedTextField(
                                value = bezeroEmaila,
                                onValueChange = { bezeroEmaila = it },
                                label = { Text("Emaila (erregistro automatikorako) *") },
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                                modifier = Modifier.fillMaxWidth(),
                                singleLine = true
                            )
                        }
                        item {
                            OutlinedTextField(
                                value = bezeroTelefonoa,
                                onValueChange = { bezeroTelefonoa = it },
                                label = { Text("Telefonoa (aukerakoa)") },
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                                modifier = Modifier.fillMaxWidth(),
                                singleLine = true
                            )
                        }
                        item {
                            // Txanda
                            Text("Txanda:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = AmberAccent)
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
                                        border = androidx.compose.foundation.BorderStroke(1.dp, if (sel) GoldBorder else Color.Transparent)
                                    ) {
                                        Text(
                                            text = label,
                                            fontSize = 11.sp,
                                            fontWeight = if (sel) FontWeight.Bold else FontWeight.Normal,
                                            color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface,
                                            modifier = Modifier.padding(vertical = 8.dp),
                                            textAlign = androidx.compose.ui.text.style.TextAlign.Center
                                        )
                                    }
                                }
                            }
                        }
                        item {
                            // Pertsona kopurua
                            Text("Pertsona kopurua:", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = AmberAccent)
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
                                        border = androidx.compose.foundation.BorderStroke(1.dp, if (sel) GoldBorder else Color.Transparent)
                                    ) {
                                        Text(
                                            text = "$p pax",
                                            fontSize = 11.sp,
                                            fontWeight = if (sel) FontWeight.Bold else FontWeight.Normal,
                                            color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface,
                                            modifier = Modifier.padding(vertical = 8.dp),
                                            textAlign = androidx.compose.ui.text.style.TextAlign.Center
                                        )
                                    }
                                }
                            }
                        }
                        item {
                            // Mahaia esleitu (Aukerakoa: sistemak auto-esleituko du hutsik utziz gero)
                            Text("Mahaia (Aukeratu edo Utzi Libre automatikoki esleitzeko):", fontSize = 12.sp, fontWeight = FontWeight.Bold, color = AmberAccent)
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(vertical = 4.dp),
                                horizontalArrangement = Arrangement.spacedBy(6.dp)
                            ) {
                                Surface(
                                    modifier = Modifier
                                        .weight(1f)
                                        .clickable { hautatutakoMahaia = null },
                                    shape = RoundedCornerShape(8.dp),
                                    color = if (hautatutakoMahaia == null) AmberAccent else MaterialTheme.colorScheme.surfaceVariant
                                ) {
                                    Text(
                                        text = "Auto-Esleitu",
                                        fontSize = 11.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = if (hautatutakoMahaia == null) Color.Black else MaterialTheme.colorScheme.onSurface,
                                        modifier = Modifier.padding(vertical = 8.dp),
                                        textAlign = androidx.compose.ui.text.style.TextAlign.Center
                                    )
                                }
                                mahaiak.take(4).forEach { m ->
                                    val sel = hautatutakoMahaia?.id == m.id
                                    Surface(
                                        modifier = Modifier
                                            .weight(1f)
                                            .clickable { hautatutakoMahaia = m },
                                        shape = RoundedCornerShape(8.dp),
                                        color = if (sel) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant
                                    ) {
                                        Text(
                                            text = "M.${m.zenbakia}",
                                            fontSize = 11.sp,
                                            fontWeight = if (sel) FontWeight.Bold else FontWeight.Normal,
                                            color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface,
                                            modifier = Modifier.padding(vertical = 8.dp),
                                            textAlign = androidx.compose.ui.text.style.TextAlign.Center
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
                        Text(errorea!!, color = StatusBusy, fontSize = 12.sp)
                        Spacer(modifier = Modifier.height(4.dp))
                    }
                    if (mezua != null) {
                        Text(mezua!!, color = StatusFree, fontSize = 12.sp)
                        Spacer(modifier = Modifier.height(4.dp))
                    }

                    // Gorde Botoia
                    Button(
                        onClick = {
                            if (bezeroIzena.isBlank() || bezeroEmaila.isBlank()) {
                                errorea = "Izena eta Emaila derrigorrezkoak dira."
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
                                        data = hautatutakoData,
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
            modifier = Modifier.padding(vertical = 8.dp),
            textAlign = androidx.compose.ui.text.style.TextAlign.Center
        )
    }
}

@Composable
fun ErreserbaKarta(erreserba: ErreserbaDto) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f)),
        border = androidx.compose.foundation.BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.2f))
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
                            modifier = Modifier.padding(horizontal = 4.dp, vertical = 2.dp)
                        )
                    }
                }
                Spacer(modifier = Modifier.height(3.dp))
                Text(
                    text = "Mahaia ${erreserba.mahaiZenbakia} (${erreserba.kokapena})  ·  ${erreserba.txanda}",
                    fontSize = 12.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                if (!erreserba.oharrak.isNullOrBlank()) {
                    Text(
                        text = "Oharra: ${erreserba.oharrak}",
                        fontSize = 11.sp,
                        color = TextMuted
                    )
                }
            }

            Surface(
                color = StatusFree.copy(alpha = 0.15f),
                shape = RoundedCornerShape(8.dp),
                border = androidx.compose.foundation.BorderStroke(1.dp, StatusFree)
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
