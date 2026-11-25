package eus.goierri.jatetxea.komandera.ui.screens

import android.content.Context
import android.print.PrintAttributes
import android.print.PrintManager
import android.webkit.WebResourceRequest
import android.webkit.WebView
import android.webkit.WebViewClient
import android.widget.Toast
import androidx.compose.animation.AnimatedContent
import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.togetherWith
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ReceiptLong
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import androidx.compose.ui.window.DialogProperties
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.EskaeraDto
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.data.model.MahaiaDto
import eus.goierri.jatetxea.komandera.data.model.OrdainketaErantzunaDto
import eus.goierri.jatetxea.komandera.data.model.OrdainketaEskaeraDto
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch
import java.io.File
import java.util.Locale

/**
 * Kontua Eskatzeko / Proforma Aurrebista Modala (Desktop C# TPV bezala).
 * Mahaiko bezeroek kontua eskatzen dutenean, zerbitzariak aurre-tiketa (proforma)
 * atera eta bezeroari eraman diezaioke, baita zuzenean ordainketara pasa ere.
 */
@Composable
fun KontuaEskatuDialog(
    mahaia: MahaiaDto,
    eskaera: EskaeraDto,
    zerbitzaria: LangileaDto,
    onOrdainketaraJoan: () -> Unit,
    onDismiss: () -> Unit
) {
    val context = LocalContext.current
    val subtotal = eskaera.guztira / 1.10
    val bez = eskaera.guztira - subtotal

    Dialog(
        onDismissRequest = onDismiss,
        properties = DialogProperties(usePlatformDefaultWidth = false)
    ) {
        Surface(
            modifier = Modifier
                .fillMaxWidth(0.94f)
                .fillMaxHeight(0.90f),
            shape = RoundedCornerShape(16.dp),
            color = MaterialTheme.colorScheme.surface,
            tonalElevation = 8.dp
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(20.dp)
            ) {
                // Goiburua
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column {
                        Row(verticalAlignment = Alignment.CenterVertically) {
                            Icon(
                                imageVector = Icons.AutoMirrored.Filled.ReceiptLong,
                                contentDescription = null,
                                tint = AmberAccent,
                                modifier = Modifier.size(26.dp)
                            )
                            Spacer(modifier = Modifier.width(8.dp))
                            Text(
                                text = "KONTUA ESKATU · PROFORMA",
                                fontSize = 17.sp,
                                fontWeight = FontWeight.Bold,
                                color = MaterialTheme.colorScheme.onSurface
                            )
                        }
                        Text(
                            text = "Mahaia ${mahaia.zenbakia} (${mahaia.kokapena}) · Eskaera #${eskaera.id}",
                            fontSize = 12.sp,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }

                    IconButton(onClick = onDismiss) {
                        Icon(imageVector = Icons.Default.Close, contentDescription = "Itxi", tint = MaterialTheme.colorScheme.onSurfaceVariant)
                    }
                }

                HorizontalDivider(modifier = Modifier.padding(vertical = 12.dp), color = MaterialTheme.colorScheme.outline.copy(alpha = 0.25f))

                // Tiket proforma termikoaren simulazio papera
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .weight(1f),
                    colors = CardDefaults.cardColors(containerColor = Color.White),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Column(
                        modifier = Modifier
                            .fillMaxSize()
                            .verticalScroll(rememberScrollState())
                            .padding(16.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Text(
                            text = "T1 JATETXEA",
                            fontSize = 20.sp,
                            fontWeight = FontWeight.Bold,
                            color = EspressoInk,
                            fontFamily = CinzelFont,
                            letterSpacing = 2.sp
                        )
                        Text(
                            text = "OSTERIA & PARRILLA · GOIERRI",
                            fontSize = 10.sp,
                            fontWeight = FontWeight.Bold,
                            color = AntiqueGold,
                            letterSpacing = 1.6.sp
                        )
                        Text(
                            text = "Goierri Kalea 1, Ordizia · IFZ: B-20123456 · Tel: 943 12 34 56",
                            fontSize = 10.sp,
                            color = WarmSlate,
                            fontFamily = JetBrainsMono,
                            textAlign = TextAlign.Center
                        )
                        Spacer(modifier = Modifier.height(6.dp))
                        Text(
                            text = "================================",
                            color = Color.Gray,
                            fontFamily = FontFamily.Monospace
                        )
                        Text(
                            text = "*** KONTU PROFORMA / AURRE-TIKETA ***",
                            fontSize = 11.sp,
                            fontWeight = FontWeight.Bold,
                            color = Color(0xFF881337), // Granate C# TPV
                            fontFamily = FontFamily.Monospace,
                            textAlign = TextAlign.Center
                        )
                        Text(
                            text = "Mahaia ${mahaia.zenbakia} · Pax: ${eskaera.pertsonaKopurua} · Zerbitzaria: ${zerbitzaria.izena}",
                            fontSize = 10.sp,
                            color = Color.DarkGray,
                            fontFamily = FontFamily.Monospace
                        )
                        Text(
                            text = "Data: ${eskaera.sortzeData.ifEmpty { "Gaur" }}",
                            fontSize = 10.sp,
                            color = Color.DarkGray,
                            fontFamily = FontFamily.Monospace
                        )
                        Text(
                            text = "================================",
                            color = Color.Gray,
                            fontFamily = FontFamily.Monospace
                        )

                        Spacer(modifier = Modifier.height(6.dp))

                        // Lerroak
                        eskaera.lerroak.forEach { lerro ->
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(vertical = 2.dp),
                                horizontalArrangement = Arrangement.SpaceBetween
                            ) {
                                Text(
                                    text = "${lerro.kantitatea}x ${lerro.platerIzena}",
                                    fontSize = 11.sp,
                                    color = Color.Black,
                                    fontFamily = FontFamily.Monospace,
                                    modifier = Modifier.weight(1f)
                                )
                                Text(
                                    text = String.format(Locale.ROOT, "%.2f €", lerro.azpitotala),
                                    fontSize = 11.sp,
                                    fontWeight = FontWeight.SemiBold,
                                    color = Color.Black,
                                    fontFamily = FontFamily.Monospace
                                )
                            }
                        }

                        Spacer(modifier = Modifier.height(8.dp))
                        Text(
                            text = "--------------------------------",
                            color = Color.Gray,
                            fontFamily = FontFamily.Monospace
                        )

                        // Zergak eta Totala
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text("Azpitotala:", fontSize = 11.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                            Text(String.format(Locale.ROOT, "%.2f €", subtotal), fontSize = 11.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                        }
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text("BEZ (%10):", fontSize = 11.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                            Text(String.format(Locale.ROOT, "%.2f €", bez), fontSize = 11.sp, color = Color.DarkGray, fontFamily = FontFamily.Monospace)
                        }

                        Spacer(modifier = Modifier.height(4.dp))
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text(
                                "GUZTIRA ORD.:",
                                fontSize = 15.sp,
                                fontWeight = FontWeight.Bold,
                                color = Color.Black,
                                fontFamily = FontFamily.Monospace
                            )
                            Text(
                                String.format(Locale.ROOT, "%.2f €", eskaera.guztira),
                                fontSize = 16.sp,
                                fontWeight = FontWeight.Bold,
                                color = Color.Black,
                                fontFamily = FontFamily.Monospace
                            )
                        }

                        Spacer(modifier = Modifier.height(8.dp))
                        Text(
                            text = "(Dokumentu informatiboa, ez du balio fiskalik)",
                            fontSize = 9.sp,
                            color = Color.Gray,
                            fontFamily = FontFamily.Monospace,
                            textAlign = TextAlign.Center
                        )
                    }
                }

                Spacer(modifier = Modifier.height(14.dp))

                // Botoiak: Proforma Inprimatu eta KOBRATZERA PASATU
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(10.dp)
                ) {
                    OutlinedButton(
                        onClick = {
                            inprimatuHTML(
                                context = context,
                                lanIzena = "Proforma_Mahaia_${mahaia.zenbakia}",
                                html = sortuProformaHtml(mahaia, eskaera, zerbitzaria)
                            )
                        },
                        modifier = Modifier
                            .weight(1f)
                            .height(48.dp),
                        shape = RoundedCornerShape(10.dp),
                        colors = ButtonDefaults.outlinedButtonColors(contentColor = MaterialTheme.colorScheme.onSurface),
                        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline)
                    ) {
                        Icon(imageVector = Icons.Default.Print, contentDescription = null, modifier = Modifier.size(18.dp))
                        Spacer(modifier = Modifier.width(6.dp))
                        Text("Inprimatu", fontSize = 13.sp)
                    }

                    Button(
                        onClick = onOrdainketaraJoan,
                        modifier = Modifier
                            .weight(1.3f)
                            .height(48.dp),
                        colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Icon(imageVector = Icons.Default.Payment, contentDescription = null, tint = Color.White, modifier = Modifier.size(18.dp))
                        Spacer(modifier = Modifier.width(6.dp))
                        Text("Kobratu Orain", color = Color.White, fontWeight = FontWeight.Bold, fontSize = 13.sp)
                    }
                }
            }
        }
    }
}

/**
 * Ordainketa Pantaila Osoa (Desktop C# OrdainketaForm bezalakoa):
 * Fitxak: Eskudirua (billete azkarrekin eta kanbioaren kalkulu automatikoarekin),
 * Txartela (EMV / Contactless datofonoa), Bizum (telefono balidazioarekin).
 */
@Composable
fun OrdainketaPantailaDialog(
    mahaia: MahaiaDto,
    eskaera: EskaeraDto,
    zerbitzaria: LangileaDto,
    onOrdainketaBurutua: (OrdainketaErantzunaDto) -> Unit,
    onDismiss: () -> Unit
) {
    val scope = rememberCoroutineScope()
    var hautatutakoMota by remember { mutableStateOf("Eskudirua") } // Eskudirua, Txartela, Bizum

    // Eskudirua kontrolak
    var jasotakoaText by remember { mutableStateOf(String.format(Locale.ROOT, "%.2f", eskaera.guztira)) }
    val jasotakoaBalioa = jasotakoaText.replace(",", ".").toDoubleOrNull() ?: 0.0
    val kanbioa = (jasotakoaBalioa - eskaera.guztira).coerceAtLeast(0.0)
    val eskudiruaNahikoa = jasotakoaBalioa >= eskaera.guztira

    // Txartela kontrolak (C# Form bezala)
    var txartelZenbakia by remember { mutableStateOf("4532 •••• •••• 8892") }
    var titularra by remember { mutableStateOf("BEZEROA") }
    var iraungitzea by remember { mutableStateOf("12/28") }
    var cvv by remember { mutableStateOf("•••") }

    // Bizum kontrolak (C# Form bezala)
    var bizumTelefonoa by remember { mutableStateOf("600123456") }
    val bizumBaliozkoa = bizumTelefonoa.trim().matches(Regex("^[67]\\d{8}$"))

    var ordaintzen by remember { mutableStateOf(false) }
    var ordainketaErrorea by remember { mutableStateOf<String?>(null) }

    val subtotal = eskaera.guztira / 1.10
    val bez = eskaera.guztira - subtotal

    Dialog(
        onDismissRequest = { if (!ordaintzen) onDismiss() },
        properties = DialogProperties(usePlatformDefaultWidth = false)
    ) {
        Surface(
            modifier = Modifier
                .fillMaxWidth(0.95f)
                .fillMaxHeight(0.92f),
            shape = RoundedCornerShape(18.dp),
            color = MaterialTheme.colorScheme.surface,
            tonalElevation = 10.dp
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(18.dp)
            ) {
                // 1. Goiburua: Mahaia, Eskaera eta Ordaindu beharreko zenbateko handia
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column {
                        Text(
                            text = "KONTUA KOBRATU · MAHAIA ${mahaia.zenbakia}",
                            fontSize = 17.sp,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.onSurface
                        )
                        Text(
                            text = "Eskaera #${eskaera.id} · ${eskaera.lerroak.sumOf { it.kantitatea }} plater · Zerbitzaria: ${zerbitzaria.izena}",
                            fontSize = 12.sp,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }

                    IconButton(onClick = onDismiss, enabled = !ordaintzen) {
                        Icon(imageVector = Icons.Default.Close, contentDescription = "Itxi", tint = MaterialTheme.colorScheme.onSurfaceVariant)
                    }
                }

                Spacer(modifier = Modifier.height(10.dp))

                // Txartel handia Zenbatekoa eta BEZarekin
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF881337)), // Granate korporatiboa C# TPV
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(horizontal = 16.dp, vertical = 12.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            Text("ORDAINDU BEHARREKO GUZTIRA", fontSize = 10.sp, color = Color(0xFFFECDD3), fontWeight = FontWeight.Bold)
                            Text(
                                text = String.format(Locale.ROOT, "Azpitotala: %.2f € · BEZ (%%10): %.2f €", subtotal, bez),
                                fontSize = 11.sp,
                                color = Color(0xFFFFE4E6)
                            )
                        }
                        Text(
                            text = String.format(Locale.ROOT, "%.2f €", eskaera.guztira),
                            fontSize = 26.sp,
                            fontWeight = FontWeight.Bold,
                            color = Color.White
                        )
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))

                // 2. Fitxak (Tabs): Eskudirua, Txartela, Bizum (Warm Hospitality Light & Dark)
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
                                containerColor = if (aktibo) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.surfaceVariant
                            ),
                            shape = RoundedCornerShape(12.dp),
                            contentPadding = PaddingValues(vertical = 12.dp)
                        ) {
                            Text(
                                text = when (mota) {
                                    "Eskudirua" -> "ESKUDIRUA"
                                    "Txartela" -> "TXARTELA"
                                    else -> "BIZUM"
                                },
                                fontSize = 12.sp,
                                fontWeight = FontWeight.Bold,
                                color = if (aktibo) Color.White else MaterialTheme.colorScheme.onSurface
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))

                // 3. Gorputza aukeratutako motaren arabera (animazioarekin)
                AnimatedContent(
                    targetState = hautatutakoMota,
                    modifier = Modifier
                        .fillMaxWidth()
                        .weight(1f),
                    transitionSpec = { fadeIn(tween(200)) togetherWith fadeOut(tween(150)) },
                    label = "paymentMethodTab"
                ) { mota ->
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                        .verticalScroll(rememberScrollState())
                ) {
                    when (mota) {
                        "Eskudirua" -> {
                            Column(modifier = Modifier.fillMaxWidth()) {
                                Text(
                                    text = "Bezeroak emandako dirua (Jasotakoa):",
                                    fontSize = 13.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = MaterialTheme.colorScheme.onSurface
                                )
                                Spacer(modifier = Modifier.height(6.dp))

                                OutlinedTextField(
                                    value = jasotakoaText,
                                    onValueChange = { jasotakoaText = it },
                                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                                    textStyle = LocalTextStyle.current.copy(
                                        fontFamily = JetBrainsMono,
                                        fontSize = 22.sp,
                                        fontWeight = FontWeight.Bold,
                                        textAlign = TextAlign.End
                                    ),
                                    trailingIcon = { Text("€", fontFamily = JetBrainsMono, fontSize = 18.sp, fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.primary) },
                                    colors = OutlinedTextFieldDefaults.colors(
                                        focusedBorderColor = MaterialTheme.colorScheme.primary,
                                        unfocusedBorderColor = MaterialTheme.colorScheme.outline,
                                        focusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        unfocusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        focusedContainerColor = MaterialTheme.colorScheme.background,
                                        unfocusedContainerColor = MaterialTheme.colorScheme.background
                                    ),
                                    shape = RoundedCornerShape(14.dp),
                                    modifier = Modifier.fillMaxWidth()
                                )

                                Spacer(modifier = Modifier.height(10.dp))

                                Row(
                                    modifier = Modifier.fillMaxWidth(),
                                    horizontalArrangement = Arrangement.spacedBy(6.dp)
                                ) {
                                    listOf(
                                        "Zehatza" to eskaera.guztira,
                                        "+5€" to (Math.ceil(eskaera.guztira / 5.0) * 5.0),
                                        "+10€" to (Math.ceil(eskaera.guztira / 10.0) * 10.0),
                                        "20€" to 20.0,
                                        "50€" to 50.0,
                                        "100€" to 100.0
                                    ).forEach { (etiketa, balioa) ->
                                        OutlinedButton(
                                            onClick = {
                                                jasotakoaText = String.format(Locale.ROOT, "%.2f", balioa)
                                            },
                                            modifier = Modifier.weight(1f),
                                            shape = RoundedCornerShape(10.dp),
                                            border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline),
                                            contentPadding = PaddingValues(horizontal = 2.dp, vertical = 8.dp)
                                        ) {
                                            Text(etiketa, fontFamily = JetBrainsMono, fontSize = 10.sp, fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.onSurface)
                                        }
                                    }
                                }

                                Spacer(modifier = Modifier.height(14.dp))

                                val kanbioKolorea by animateColorAsState(
                                    targetValue = if (eskudiruaNahikoa) StatusFree else StatusBusy,
                                    animationSpec = tween(250),
                                    label = "changeBoxColor"
                                )
                                Card(
                                    modifier = Modifier.fillMaxWidth(),
                                    colors = CardDefaults.cardColors(
                                        containerColor = if (eskudiruaNahikoa) StatusFree.copy(alpha = 0.12f) else StatusBusy.copy(alpha = 0.12f)
                                    ),
                                    shape = RoundedCornerShape(14.dp),
                                    border = BorderStroke(1.5.dp, kanbioKolorea.copy(alpha = 0.5f))
                                ) {
                                    Column(modifier = Modifier.padding(14.dp)) {
                                        Text(
                                            text = "BEZEROARI ITZULI BEHARREKO KANBIOA:",
                                            fontSize = 11.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = MaterialTheme.colorScheme.onSurfaceVariant
                                        )
                                        Spacer(modifier = Modifier.height(4.dp))
                                        Text(
                                            text = if (eskudiruaNahikoa) {
                                                String.format(Locale.ROOT, "%.2f €", kanbioa)
                                            } else {
                                                String.format(Locale.ROOT, "Falta dira: %.2f €", eskaera.guztira - jasotakoaBalioa)
                                            },
                                            fontFamily = JetBrainsMono,
                                            fontSize = 24.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = kanbioKolorea
                                        )
                                    }
                                }
                            }
                        }

                        "Txartela" -> {
                            Column(modifier = Modifier.fillMaxWidth()) {
                                Text(
                                    text = "Kreditu / Zordunketa Txartela (EMV / Contactless):",
                                    fontSize = 13.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = MaterialTheme.colorScheme.onSurface
                                )
                                Spacer(modifier = Modifier.height(8.dp))

                                OutlinedTextField(
                                    value = txartelZenbakia,
                                    onValueChange = { txartelZenbakia = it },
                                    label = { Text("Txartel Zenbakia") },
                                    leadingIcon = { Icon(Icons.Default.CreditCard, contentDescription = null, tint = MaterialTheme.colorScheme.primary) },
                                    colors = OutlinedTextFieldDefaults.colors(
                                        focusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        unfocusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        focusedContainerColor = MaterialTheme.colorScheme.background,
                                        unfocusedContainerColor = MaterialTheme.colorScheme.background
                                    ),
                                    modifier = Modifier.fillMaxWidth()
                                )

                                Spacer(modifier = Modifier.height(8.dp))

                                Row(
                                    modifier = Modifier.fillMaxWidth(),
                                    horizontalArrangement = Arrangement.spacedBy(8.dp)
                                ) {
                                    OutlinedTextField(
                                        value = iraungitzea,
                                        onValueChange = { iraungitzea = it },
                                        label = { Text("Iraungitzea") },
                                        modifier = Modifier.weight(1f),
                                        colors = OutlinedTextFieldDefaults.colors(
                                            focusedTextColor = MaterialTheme.colorScheme.onSurface,
                                            unfocusedTextColor = MaterialTheme.colorScheme.onSurface,
                                            focusedContainerColor = MaterialTheme.colorScheme.background,
                                            unfocusedContainerColor = MaterialTheme.colorScheme.background
                                        )
                                    )
                                    OutlinedTextField(
                                        value = cvv,
                                        onValueChange = { cvv = it },
                                        label = { Text("CVV") },
                                        modifier = Modifier.weight(1f),
                                        colors = OutlinedTextFieldDefaults.colors(
                                            focusedTextColor = MaterialTheme.colorScheme.onSurface,
                                            unfocusedTextColor = MaterialTheme.colorScheme.onSurface,
                                            focusedContainerColor = MaterialTheme.colorScheme.background,
                                            unfocusedContainerColor = MaterialTheme.colorScheme.background
                                        )
                                    )
                                }

                                Spacer(modifier = Modifier.height(8.dp))

                                OutlinedTextField(
                                    value = titularra,
                                    onValueChange = { titularra = it },
                                    label = { Text("Titularra") },
                                    modifier = Modifier.fillMaxWidth(),
                                    colors = OutlinedTextFieldDefaults.colors(
                                        focusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        unfocusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        focusedContainerColor = MaterialTheme.colorScheme.background,
                                        unfocusedContainerColor = MaterialTheme.colorScheme.background
                                    )
                                )

                                Spacer(modifier = Modifier.height(10.dp))
                                Row(verticalAlignment = Alignment.CenterVertically) {
                                    Icon(Icons.Default.Nfc, contentDescription = null, tint = StatusFree, modifier = Modifier.size(18.dp))
                                    Spacer(modifier = Modifier.width(6.dp))
                                    Text("POS / Datofono kontaktugabea prest dago.", fontSize = 11.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                                }
                            }
                        }

                        "Bizum" -> {
                            Column(modifier = Modifier.fillMaxWidth()) {
                                Text(
                                    text = "Bizum Berehalako Ordainketa:",
                                    fontSize = 13.sp,
                                    fontWeight = FontWeight.Bold,
                                    color = MaterialTheme.colorScheme.onSurface
                                )
                                Spacer(modifier = Modifier.height(8.dp))

                                OutlinedTextField(
                                    value = bizumTelefonoa,
                                    onValueChange = { if (it.all { ch -> ch.isDigit() || ch == ' ' }) bizumTelefonoa = it },
                                    label = { Text("Bezeroaren Telefonoa (9 digitu)") },
                                    leadingIcon = { Icon(Icons.Default.PhoneAndroid, contentDescription = null, tint = MaterialTheme.colorScheme.primary) },
                                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                                    colors = OutlinedTextFieldDefaults.colors(
                                        focusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        unfocusedTextColor = MaterialTheme.colorScheme.onSurface,
                                        focusedContainerColor = MaterialTheme.colorScheme.background,
                                        unfocusedContainerColor = MaterialTheme.colorScheme.background
                                    ),
                                    modifier = Modifier.fillMaxWidth()
                                )

                                Spacer(modifier = Modifier.height(8.dp))
                                Text(
                                    text = if (bizumBaliozkoa) {
                                        "Telefono baliozkoa (Berehalako Bizum push jakinarazpena bidaliko da)"
                                    } else {
                                        "Telefonoak 9 digitu izan behar ditu (6 edo 7z hasita)"
                                    },
                                    fontSize = 11.sp,
                                    color = if (bizumBaliozkoa) StatusFree else StatusBusy
                                )

                                Spacer(modifier = Modifier.height(12.dp))
                                Card(
                                    modifier = Modifier.fillMaxWidth(),
                                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.background),
                                    border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline),
                                    shape = RoundedCornerShape(12.dp)
                                ) {
                                    Column(modifier = Modifier.padding(12.dp)) {
                                        Text("Bizum Segurua", color = MaterialTheme.colorScheme.primary, fontSize = 12.sp, fontWeight = FontWeight.Bold)
                                        Text(
                                            "Bezeroak bere bankuko aplikazioan jakinarazpena jasoko du eta baieztatzean transakzioa berehala itxiko da.",
                                            fontSize = 11.sp,
                                            color = MaterialTheme.colorScheme.onSurfaceVariant
                                        )
                                    }
                                }
                            }
                        }
                    }
                }
                }

                if (ordainketaErrorea != null) {
                    Spacer(modifier = Modifier.height(6.dp))
                    Text(text = ordainketaErrorea!!, color = StatusBusy, fontSize = 12.sp)
                }

                Spacer(modifier = Modifier.height(12.dp))

                // 4. Beheko Botoiak: Utzi eta KOBRATU ETA TIKETA SORTU
                val gaituKobratu = when (hautatutakoMota) {
                    "Eskudirua" -> eskudiruaNahikoa && !ordaintzen
                    "Txartela" -> !ordaintzen
                    "Bizum" -> bizumBaliozkoa && !ordaintzen
                    else -> false
                }

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(10.dp)
                ) {
                    OutlinedButton(
                        onClick = onDismiss,
                        enabled = !ordaintzen,
                        modifier = Modifier
                            .weight(1f)
                            .height(50.dp),
                        shape = RoundedCornerShape(10.dp)
                    ) {
                        Text("Utzi", color = TextSecondary)
                    }

                    Button(
                        onClick = {
                            scope.launch {
                                ordaintzen = true
                                ordainketaErrorea = null
                                try {
                                    val ordReq = OrdainketaEskaeraDto(
                                        eskaeraId = eskaera.id,
                                        mota = hautatutakoMota,
                                        zenbatekoa = eskaera.guztira,
                                        jasotakoa = if (hautatutakoMota == "Eskudirua") jasotakoaBalioa else eskaera.guztira,
                                        txartelZenbakia = if (hautatutakoMota == "Txartela") txartelZenbakia else null,
                                        titularra = if (hautatutakoMota == "Txartela") titularra else null,
                                        iraungitzea = if (hautatutakoMota == "Txartela") iraungitzea else null,
                                        cvv = if (hautatutakoMota == "Txartela") cvv else null,
                                        bizumTelefonoa = if (hautatutakoMota == "Bizum") bizumTelefonoa else null,
                                        langileaId = zerbitzaria.id
                                    )

                                    val res = ApiClient.service.prozesatuOrdainketa(ordReq)
                                    if (res.isSuccessful && res.body()?.data != null) {
                                        val emaitza = res.body()!!.data!!
                                        // Mahaia Garbitzen egoeran jarri (C# TPV bezala)
                                        ApiClient.service.eguneratuMahaiarenEgoera(mahaia.id, eus.goierri.jatetxea.komandera.data.model.EgoeraEguneratuRequest("Garbitzen"))
                                        onOrdainketaBurutua(emaitza)
                                    } else {
                                        ordainketaErrorea = res.body()?.message ?: "Ezin izan da ordainketa burutu."
                                    }
                                } catch (e: Exception) {
                                    ordainketaErrorea = "Errorea ordaintzean: ${e.localizedMessage ?: "Konexio akatsa"}"
                                } finally {
                                    ordaintzen = false
                                }
                            }
                        },
                        enabled = gaituKobratu,
                        modifier = Modifier
                            .weight(2f)
                            .height(50.dp),
                        colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        if (ordaintzen) {
                            CircularProgressIndicator(color = Color.White, modifier = Modifier.size(22.dp))
                        } else {
                            Icon(imageVector = Icons.Default.Receipt, contentDescription = null, tint = Color.White, modifier = Modifier.size(18.dp))
                            Spacer(modifier = Modifier.width(6.dp))
                            Text(
                                text = "KOBRATU ETA TIKETA SORTU",
                                fontSize = 12.sp,
                                fontWeight = FontWeight.Bold,
                                color = Color.White
                            )
                        }
                    }
                }
            }
        }
    }
}

/**
 * Tiket Fiskalaren Bistaratzeko eta Inprimatzeko Modala (C# TiketIkusteaForm bezalakoa).
 * - ESC/POS / Android Print inprimaketa
 * - Testua (.txt) fitxategira esportatzea
 * - Tiket fisikoaren itxura zehatza
 */
@Composable
fun TiketFiskalaDialog(
    ordainketa: OrdainketaErantzunaDto,
    onItxi: () -> Unit
) {
    val context = LocalContext.current

    Dialog(
        onDismissRequest = onItxi,
        properties = DialogProperties(usePlatformDefaultWidth = false)
    ) {
        Surface(
            modifier = Modifier
                .fillMaxWidth(0.94f)
                .fillMaxHeight(0.92f),
            shape = RoundedCornerShape(16.dp),
            color = MaterialTheme.colorScheme.surface,
            tonalElevation = 8.dp
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(18.dp)
            ) {
                // Goiburua: Berdea (Arrakastatsua)
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF166534)), // Berde dotorea
                    shape = RoundedCornerShape(10.dp)
                ) {
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(horizontal = 16.dp, vertical = 12.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            Text("ORDAINKETA ONARTUA - TIKET FISKALA", fontSize = 13.sp, fontWeight = FontWeight.Bold, color = Color.White)
                            Text(
                                text = "Tiket zk: ${ordainketa.tiketZenbakia} · ${String.format(Locale.ROOT, "%.2f €", ordainketa.zenbatekoa)} (${ordainketa.mota})",
                                fontSize = 11.sp,
                                color = Color(0xFFDCFCE7)
                            )
                        }
                        Icon(imageVector = Icons.Default.CheckCircle, contentDescription = null, tint = Color(0xFF86EFAC), modifier = Modifier.size(28.dp))
                    }
                }

                Spacer(modifier = Modifier.height(12.dp))

                // Tiketaren testua (Consolas/Monospace 80mm formatu errealean)
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .weight(1f),
                    colors = CardDefaults.cardColors(containerColor = Color.White),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Column(
                        modifier = Modifier
                            .fillMaxSize()
                            .verticalScroll(rememberScrollState())
                            .padding(16.dp)
                    ) {
                        if (ordainketa.tiketTestua.isNotBlank()) {
                            Text(
                                text = ordainketa.tiketTestua,
                                fontSize = 11.sp,
                                color = Color.Black,
                                fontFamily = FontFamily.Monospace
                            )
                        } else {
                            // Fallback formatua testurik ez badator
                            Text(
                                text = sortuFallbackTiketTestua(ordainketa),
                                fontSize = 11.sp,
                                color = Color.Black,
                                fontFamily = FontFamily.Monospace
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(14.dp))

                // Ekintza botoiak: 1. lerroa (Gorde .txt + Inprimatu 80mm), 2. lerroa (Itxi)
                Column(
                    modifier = Modifier.fillMaxWidth(),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        OutlinedButton(
                            onClick = {
                                gordeTiketaFitxategian(context, ordainketa)
                            },
                            modifier = Modifier
                                .weight(1f)
                                .height(46.dp),
                            shape = RoundedCornerShape(10.dp),
                            contentPadding = PaddingValues(horizontal = 10.dp, vertical = 0.dp)
                        ) {
                            Icon(
                                imageVector = Icons.Default.Save,
                                contentDescription = null,
                                modifier = Modifier.size(16.dp)
                            )
                            Spacer(modifier = Modifier.width(6.dp))
                            Text(
                                text = "Gorde (.txt)",
                                fontSize = 12.sp,
                                fontWeight = FontWeight.SemiBold,
                                maxLines = 1,
                                softWrap = false,
                                overflow = TextOverflow.Ellipsis
                            )
                        }

                        Button(
                            onClick = {
                                inprimatuHTML(
                                    context = context,
                                    lanIzena = "Tiket_${ordainketa.tiketZenbakia}",
                                    html = sortuTiketHtml(ordainketa)
                                )
                            },
                            colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF881337)),
                            modifier = Modifier
                                .weight(1.15f)
                                .height(46.dp),
                            shape = RoundedCornerShape(10.dp),
                            contentPadding = PaddingValues(horizontal = 10.dp, vertical = 0.dp)
                        ) {
                            Icon(
                                imageVector = Icons.Default.Print,
                                contentDescription = null,
                                tint = Color.White,
                                modifier = Modifier.size(16.dp)
                            )
                            Spacer(modifier = Modifier.width(6.dp))
                            Text(
                                text = "Inprimatu (80mm)",
                                fontSize = 12.sp,
                                fontWeight = FontWeight.Bold,
                                color = Color.White,
                                maxLines = 1,
                                softWrap = false,
                                overflow = TextOverflow.Ellipsis
                            )
                        }
                    }

                    Button(
                        onClick = onItxi,
                        colors = ButtonDefaults.buttonColors(containerColor = DarkSurfaceVariant),
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(46.dp),
                        shape = RoundedCornerShape(10.dp),
                        contentPadding = PaddingValues(horizontal = 16.dp, vertical = 0.dp)
                    ) {
                        Text(
                            text = "Itxi",
                            fontSize = 13.sp,
                            fontWeight = FontWeight.SemiBold,
                            color = TextPrimary,
                            maxLines = 1,
                            softWrap = false
                        )
                    }
                }
            }
        }
    }
}

// ============================================================================
// INPRIMATZEKO ETA GORDETZEKO LAGUNTZAILEAK
// ============================================================================

private fun gordeTiketaFitxategian(context: Context, ordainketa: OrdainketaErantzunaDto) {
    try {
        val testua = if (ordainketa.tiketTestua.isNotBlank()) ordainketa.tiketTestua else sortuFallbackTiketTestua(ordainketa)
        val dir = context.getExternalFilesDir(null) ?: context.filesDir
        val fitxategia = File(dir, "Tiket_${ordainketa.tiketZenbakia.replace("/", "_")}.txt")
        fitxategia.writeText(testua)
        Toast.makeText(context, "Tiketa ondo gorde da: ${fitxategia.name}", Toast.LENGTH_LONG).show()
    } catch (e: Exception) {
        Toast.makeText(context, "Errorea gordetzean: ${e.localizedMessage}", Toast.LENGTH_SHORT).show()
    }
}

private fun inprimatuHTML(context: Context, lanIzena: String, html: String) {
    try {
        val webView = WebView(context)
        webView.webViewClient = object : WebViewClient() {
            override fun shouldOverrideUrlLoading(view: WebView?, request: WebResourceRequest?) = false

            override fun onPageFinished(view: WebView?, url: String?) {
                val printManager = context.getSystemService(Context.PRINT_SERVICE) as? PrintManager
                val printAdapter = webView.createPrintDocumentAdapter(lanIzena)
                printManager?.print(lanIzena, printAdapter, PrintAttributes.Builder().build())
            }
        }
        webView.loadDataWithBaseURL(null, html, "text/html", "UTF-8", null)
    } catch (e: Exception) {
        Toast.makeText(context, "Inprimaketa akatsa: ${e.localizedMessage}", Toast.LENGTH_SHORT).show()
    }
}

private fun sortuProformaHtml(mahaia: MahaiaDto, eskaera: EskaeraDto, zerbitzaria: LangileaDto): String {
    val subtotal = eskaera.guztira / 1.10
    val bez = eskaera.guztira - subtotal
    val lerroakHtml = eskaera.lerroak.joinToString("") { l ->
        "<tr><td>${l.kantitatea}x ${l.platerIzena}</td><td style='text-align:right;'>${String.format(Locale.ROOT, "%.2f €", l.azpitotala)}</td></tr>"
    }

    return """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'/>
            <style>
                body { font-family: monospace; font-size: 12px; margin: 0; padding: 10px; width: 280px; }
                h2, h3 { text-align: center; margin: 4px 0; }
                table { width: 100%; border-collapse: collapse; margin-top: 8px; }
                hr { border: none; border-top: 1px dashed #444; margin: 6px 0; }
                .center { text-align: center; }
                .right { text-align: right; }
                .bold { font-weight: bold; }
            </style>
        </head>
        <body>
            <h2>1T JATETXEA</h2>
            <div class='center'>Goierri Kalea 1, Ordizia<br/>IFZ: B-20123456 · Tel: 943 12 34 56</div>
            <hr/>
            <div class='center bold'>*** PROFORMA / KONTU ESKAERA ***</div>
            <div>Mahaia: ${mahaia.zenbakia} (${mahaia.kokapena})</div>
            <div>Zerbitzaria: ${zerbitzaria.izena}</div>
            <div>Eskaera #: ${eskaera.id} · Pax: ${eskaera.pertsonaKopurua}</div>
            <hr/>
            <table>
                $lerroakHtml
            </table>
            <hr/>
            <table>
                <tr><td>Azpitotala:</td><td class='right'>${String.format(Locale.ROOT, "%.2f €", subtotal)}</td></tr>
                <tr><td>BEZ (%10):</td><td class='right'>${String.format(Locale.ROOT, "%.2f €", bez)}</td></tr>
                <tr class='bold' style='font-size: 14px;'><td>GUZTIRA:</td><td class='right'>${String.format(Locale.ROOT, "%.2f €", eskaera.guztira)}</td></tr>
            </table>
            <hr/>
            <div class='center' style='font-size:10px;'>Ez du balio fiskalik.<br/>Eskerrik asko etortzeagatik!</div>
        </body>
        </html>
    """.trimIndent()
}

private fun sortuTiketHtml(ordainketa: OrdainketaErantzunaDto): String {
    val testua = if (ordainketa.tiketTestua.isNotBlank()) ordainketa.tiketTestua else sortuFallbackTiketTestua(ordainketa)
    return """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'/>
            <style>
                body { font-family: monospace; font-size: 11px; margin: 0; padding: 8px; width: 280px; white-space: pre-wrap; }
            </style>
        </head>
        <body>$testua</body>
        </html>
    """.trimIndent()
}

private fun sortuFallbackTiketTestua(o: OrdainketaErantzunaDto): String {
    return """
        ========================================
                      1T JATETXEA
               Goierri Kalea 1, Ordizia
           IFZ: B-20123456 · Tel: 943 12 34 56
        ========================================
        TIKET ZENBAKIA: ${o.tiketZenbakia}
        DATA: ${o.data.ifEmpty { "Gaur" }}
        ORDAINKETA MOTA: ${o.mota}
        OPERAZIOA: ${o.operazioKodea}
        ----------------------------------------
        GUZTIRA:           ${String.format(Locale.ROOT, "%8.2f €", o.zenbatekoa)}
        JASOTAKOA:         ${String.format(Locale.ROOT, "%8.2f €", o.jasotakoa ?: o.zenbatekoa)}
        ITZULITAKOA:       ${String.format(Locale.ROOT, "%8.2f €", o.itzulitakoa ?: 0.0)}
        ========================================
             Eskerrik asko zure bisitagatik!
        ========================================
    """.trimIndent()
}

@Preview(showBackground = true, name = "OrdainketaPantailaDialog")
@Composable
fun OrdainketaPantailaDialogPreview() {
    KomanderaTheme(darkTheme = true) {
        OrdainketaPantailaDialog(
            mahaia = MahaiaDto(1, 1, 4, "Jantoki Nagusia", "Okupatuta"),
            eskaera = EskaeraDto(id = 42, mahaiaId = 1, mahaiZenbakia = 1, pertsonaKopurua = 2, guztira = 38.50),
            zerbitzaria = LangileaDto(1, "ane", "Ane Etxeberria", "Zerbitzaria"),
            onOrdainketaBurutua = {},
            onDismiss = {}
        )
    }
}
