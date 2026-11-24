package eus.goierri.jatetxea.komandera.ui.screens

import android.widget.Toast
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.tween
import androidx.compose.animation.expandVertically
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.shrinkVertically
import androidx.compose.animation.slideInVertically
import androidx.compose.animation.slideOutVertically
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.automirrored.filled.ReceiptLong
import androidx.compose.material.icons.automirrored.filled.Send
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Check
import androidx.compose.material.icons.filled.DeleteOutline
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Payment
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material.icons.filled.RestaurantMenu
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.*
import eus.goierri.jatetxea.komandera.ui.state.AppPreferences
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import java.util.Locale

@Composable
fun MahaiaXehetasunakScreen(
    mahaiId: Int,
    zerbitzaria: LangileaDto,
    onGehituPlaterak: (eskaeraId: Int, mahaiId: Int, pax: Int) -> Unit,
    onAtzera: () -> Unit,
    modifier: Modifier = Modifier
) {
    val context = LocalContext.current
    val scope = rememberCoroutineScope()
    var mahaia by remember { mutableStateOf<MahaiaDto?>(null) }
    var eskaeraAktiboa by remember { mutableStateOf<EskaeraDto?>(null) }
    var kargatzen by remember { mutableStateOf(true) }
    var erroreMezua by remember { mutableStateOf<String?>(null) }
    var ekintzanAri by remember { mutableStateOf(false) }

    var sortuEskaeraDialogIrekita by remember { mutableStateOf(false) }
    var pertsonaKopuruaInput by remember { mutableStateOf("2") }

    var kontuaEskatuDialogIrekita by remember { mutableStateOf(false) }
    var ordainketaDialogIrekita by remember { mutableStateOf(false) }
    var tiketModalIrekita by remember { mutableStateOf(false) }
    var azkenTiketa by remember { mutableStateOf<OrdainketaErantzunaDto?>(null) }

    fun kargatuDatuak(erakutsiKargatzen: Boolean = true) {
        scope.launch {
            if (erakutsiKargatzen) kargatzen = true
            try {
                val mahaiRes = ApiClient.service.getMahaiak()
                if (mahaiRes.isSuccessful && mahaiRes.body()?.dataList != null) {
                    mahaiRes.body()!!.dataList!!.find { it.id == mahaiId }?.let { mahaia = it }
                }

                val eskaeraRes = ApiClient.service.getEskaerakMahaia(mahaiId)
                if (eskaeraRes.isSuccessful && eskaeraRes.body()?.dataList != null) {
                    val eskaerak = eskaeraRes.body()!!.dataList!!
                    val aktiboa = eskaerak.firstOrNull { it.egoera != "Kobratua" && it.egoera != "Ezeztatua" }
                    if (aktiboa != null) {
                        val xehRes = ApiClient.service.getEskaeraXehetasunak(aktiboa.id)
                        eskaeraAktiboa = if (xehRes.isSuccessful && xehRes.body()?.data != null) {
                            xehRes.body()!!.data
                        } else {
                            aktiboa
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
        val kobratzekoKontuaDago = eskaeraAktiboa != null
        if (kobratzekoKontuaDago &&
            (egoeraBerria.equals("Libre", ignoreCase = true) || egoeraBerria.equals("Garbitzen", ignoreCase = true))
        ) {
            Toast.makeText(
                context,
                AppPreferences.tr(
                    "Ezin da '$egoeraBerria' jarri: kontua kobratu edo eskaera ezabatu behar da lehenik.",
                    "No se puede poner '$egoeraBerria': cobra la cuenta o elimina el pedido primero.",
                    "Cannot set '$egoeraBerria': pay or delete the active order first."
                ),
                Toast.LENGTH_LONG
            ).show()
            return
        }
        scope.launch {
            ekintzanAri = true
            try {
                val res = ApiClient.service.eguneratuMahaiarenEgoera(mahaiId, EgoeraEguneratuRequest(egoeraBerria))
                if (res.isSuccessful) {
                    kargatuDatuak(erakutsiKargatzen = false)
                } else {
                    val msg = res.body()?.message ?: AppPreferences.tr(
                        "Ezin izan da mahaiaren egoera aldatu.",
                        "No se pudo cambiar el estado de la mesa.",
                        "Could not update table status."
                    )
                    Toast.makeText(context, msg, Toast.LENGTH_LONG).show()
                }
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    fun hasiEskaeraBerriaKatalogoan() {
        val pax = (pertsonaKopuruaInput.toIntOrNull() ?: 2).coerceAtLeast(1)
        val gehienez = mahaia?.edukiera ?: 20
        if (pax > gehienez) {
            Toast.makeText(
                context,
                AppPreferences.tr(
                    "Gehienez $gehienez pertsona sartzen dira mahai honetan.",
                    "Máximo $gehienez personas en esta mesa.",
                    "Maximum $gehienez guests for this table."
                ),
                Toast.LENGTH_SHORT
            ).show()
            return
        }
        // Ez dugu eskaera hutsik sortzen DBn; zuzenean katalogora goaz produktuak aukeratzera!
        sortuEskaeraDialogIrekita = false
        onGehituPlaterak(0, mahaiId, pax)
    }

    fun ezabatuLerroa(lerroId: Int) {
        scope.launch {
            ekintzanAri = true
            try {
                val res = ApiClient.service.ezabatuLerroa(lerroId)
                if (res.isSuccessful) {
                    Toast.makeText(
                        context,
                        AppPreferences.tr("Lerroa ezabatu da eta stocka leheneratu da.", "Línea eliminada y stock restaurado.", "Item removed & stock restored."),
                        Toast.LENGTH_SHORT
                    ).show()
                    kargatuDatuak(erakutsiKargatzen = false)
                } else {
                    val msg = res.body()?.message ?: AppPreferences.tr(
                        "Ezin da lerro hau ezabatu (sukaldean edo zerbitzatuta dago).",
                        "No se puede eliminar esta línea.",
                        "Cannot delete this item."
                    )
                    Toast.makeText(context, msg, Toast.LENGTH_SHORT).show()
                }
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    fun ezabatuEskaeraHutsa() {
        val esk = eskaeraAktiboa ?: return
        if (esk.lerroak.isNotEmpty()) {
            Toast.makeText(
                context,
                AppPreferences.tr("Produktuak dituen eskaera ezin da huts gisa ezabatu.", "No se puede eliminar un pedido con productos.", "Cannot delete an order with items."),
                Toast.LENGTH_SHORT
            ).show()
            return
        }
        scope.launch {
            ekintzanAri = true
            try {
                val res = ApiClient.service.ezabatuEskaera(esk.id)
                if (res.isSuccessful) {
                    ApiClient.service.eguneratuMahaiarenEgoera(mahaiId, EgoeraEguneratuRequest("Libre"))
                    Toast.makeText(
                        context,
                        AppPreferences.tr("Eskaera hutsa ezabatu da eta mahaia libre geratu da.", "Pedido vacío eliminado y mesa liberada.", "Empty order deleted & table freed."),
                        Toast.LENGTH_SHORT
                    ).show()
                    eskaeraAktiboa = null
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
                if (res.isSuccessful) kargatuDatuak(erakutsiKargatzen = false)
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
                if (res.isSuccessful) kargatuDatuak(erakutsiKargatzen = false)
            } catch (_: Exception) {
            } finally {
                ekintzanAri = false
            }
        }
    }

    val unekoMahaia = mahaia
    val mahaiaZenb = unekoMahaia?.zenbakia ?: mahaiId

    MahaiaXehetasunakContent(
        mahaiId = mahaiId,
        mahaia = unekoMahaia,
        eskaeraAktiboa = eskaeraAktiboa,
        zerbitzaria = zerbitzaria,
        kargatzen = kargatzen,
        erroreMezua = erroreMezua,
        ekintzanAri = ekintzanAri,
        onAtzera = onAtzera,
        onFreskatu = { kargatuDatuak(erakutsiKargatzen = true) },
        onAldatuMahaiEgoera = { aldatuMahaiarenEgoera(it) },
        onIrekiSortuEskaera = { sortuEskaeraDialogIrekita = true },
        onGehituPlaterak = { eskaeraId, mid ->
            onGehituPlaterak(eskaeraId, mid, eskaeraAktiboa?.pertsonaKopurua ?: 2)
        },
        onBidaliSukaldera = { bidaliSukaldera() },
        onZerbitzatuLerroa = { lerroId -> aldatuLerroEgoera(lerroId, "Zerbitzatua") },
        onEzabatuLerroa = { lerroId -> ezabatuLerroa(lerroId) },
        onEzabatuEskaeraHutsa = { ezabatuEskaeraHutsa() },
        onIrekiKontua = { kontuaEskatuDialogIrekita = true },
        onIrekiOrdainketa = { ordainketaDialogIrekita = true },
        modifier = modifier
    )

    // ESKAERA BERRIA SORTZEKO MODALA
    if (sortuEskaeraDialogIrekita) {
        AlertDialog(
            onDismissRequest = { sortuEskaeraDialogIrekita = false },
            title = {
                Text(
                    text = "${AppPreferences.tr("Mahaia", "Mesa", "Table")} $mahaiaZenb - ${AppPreferences.tr("Eskaera Berria", "Nuevo Pedido", "New Order")}",
                    color = MaterialTheme.colorScheme.onSurface,
                    fontWeight = FontWeight.Bold
                )
            },
            text = {
                Column {
                    Text(
                        text = AppPreferences.tr(
                            "Aukeratu mahaiko mahaikide kopurua (pax):",
                            "Selecciona el número de comensales (pax):",
                            "Select number of guests (pax):"
                        ),
                        fontSize = 14.sp,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                    Spacer(modifier = Modifier.height(14.dp))
                    OutlinedTextField(
                        value = pertsonaKopuruaInput,
                        onValueChange = { if (it.all { ch -> ch.isDigit() }) pertsonaKopuruaInput = it },
                        label = { Text(AppPreferences.tr("Pertsona kopurua", "Comensales", "Guests")) },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = ChiantiPrimary,
                            unfocusedBorderColor = MaterialTheme.colorScheme.outline,
                            focusedTextColor = MaterialTheme.colorScheme.onSurface,
                            unfocusedTextColor = MaterialTheme.colorScheme.onSurface
                        ),
                        modifier = Modifier.fillMaxWidth()
                    )
                    Spacer(modifier = Modifier.height(10.dp))
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        listOf("1", "2", "4", "6", "8").forEach { n ->
                            val sel = pertsonaKopuruaInput == n
                            OutlinedButton(
                                onClick = { pertsonaKopuruaInput = n },
                                modifier = Modifier.weight(1f),
                                colors = ButtonDefaults.outlinedButtonColors(
                                    containerColor = if (sel) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f)
                                ),
                                border = BorderStroke(
                                    1.dp,
                                    if (sel) GoldBorder else MaterialTheme.colorScheme.outline.copy(alpha = 0.4f)
                                ),
                                shape = RoundedCornerShape(8.dp),
                                contentPadding = PaddingValues(0.dp)
                            ) {
                                Text(
                                    text = n,
                                    fontSize = 13.sp,
                                    fontWeight = if (sel) FontWeight.Bold else FontWeight.Medium,
                                    color = if (sel) Color.White else MaterialTheme.colorScheme.onSurface
                                )
                            }
                        }
                    }
                }
            },
            confirmButton = {
                Button(
                    onClick = { hasiEskaeraBerriaKatalogoan() },
                    colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                    enabled = !ekintzanAri
                ) {
                    Text(
                        AppPreferences.tr("Aukeratu Produktuak", "Elegir Productos", "Choose Products"),
                        color = Color.White,
                        fontWeight = FontWeight.Bold
                    )
                }
            },
            dismissButton = {
                TextButton(onClick = { sortuEskaeraDialogIrekita = false }) {
                    Text(AppPreferences.tr("Utzi", "Cancelar", "Cancel"), color = MaterialTheme.colorScheme.onSurfaceVariant)
                }
            },
            containerColor = MaterialTheme.colorScheme.surface
        )
    }

    if (kontuaEskatuDialogIrekita && eskaeraAktiboa != null && unekoMahaia != null) {
        KontuaEskatuDialog(
            mahaia = unekoMahaia,
            eskaera = eskaeraAktiboa!!,
            zerbitzaria = zerbitzaria,
            onOrdainketaraJoan = {
                kontuaEskatuDialogIrekita = false
                ordainketaDialogIrekita = true
            },
            onDismiss = { kontuaEskatuDialogIrekita = false }
        )
    }

    if (ordainketaDialogIrekita && eskaeraAktiboa != null && unekoMahaia != null) {
        OrdainketaPantailaDialog(
            mahaia = unekoMahaia,
            eskaera = eskaeraAktiboa!!,
            zerbitzaria = zerbitzaria,
            onOrdainketaBurutua = { emaitza ->
                azkenTiketa = emaitza
                ordainketaDialogIrekita = false
                tiketModalIrekita = true
                kargatuDatuak(erakutsiKargatzen = false)
            },
            onDismiss = { ordainketaDialogIrekita = false }
        )
    }

    if (tiketModalIrekita && azkenTiketa != null) {
        TiketFiskalaDialog(
            ordainketa = azkenTiketa!!,
            onItxi = {
                tiketModalIrekita = false
                onAtzera()
            }
        )
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MahaiaXehetasunakContent(
    mahaiId: Int,
    mahaia: MahaiaDto?,
    eskaeraAktiboa: EskaeraDto?,
    zerbitzaria: LangileaDto,
    kargatzen: Boolean,
    erroreMezua: String?,
    ekintzanAri: Boolean,
    onAtzera: () -> Unit,
    onFreskatu: () -> Unit,
    onAldatuMahaiEgoera: (String) -> Unit,
    onIrekiSortuEskaera: () -> Unit,
    onGehituPlaterak: (eskaeraId: Int, mahaiId: Int) -> Unit,
    onBidaliSukaldera: () -> Unit,
    onZerbitzatuLerroa: (Int) -> Unit,
    onEzabatuLerroa: (Int) -> Unit = {},
    onEzabatuEskaeraHutsa: () -> Unit = {},
    onIrekiKontua: () -> Unit,
    onIrekiOrdainketa: () -> Unit,
    modifier: Modifier = Modifier
) {
    val mahaiaZenb = mahaia?.zenbakia ?: mahaiId

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "${AppPreferences.tr("Mahaia", "Mesa", "Table")} $mahaiaZenb - ${AppPreferences.tr("Xehetasunak", "Detalles", "Details")}",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.onSurface
                        )
                        Text(
                            text = "${mahaia?.kokapena ?: ""} · ${zerbitzaria.izena}",
                            fontSize = 12.sp,
                            color = AmberAccent
                        )
                    }
                },
                navigationIcon = {
                    IconButton(onClick = onAtzera) {
                        Icon(
                            imageVector = Icons.AutoMirrored.Filled.ArrowBack,
                            contentDescription = AppPreferences.tr("Atzera", "Atrás", "Back"),
                            tint = MaterialTheme.colorScheme.onSurface
                        )
                    }
                },
                actions = {
                    IconButton(onClick = onFreskatu) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = AppPreferences.tr("Freskatu", "Actualizar", "Refresh"),
                            tint = AmberAccent
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = MaterialTheme.colorScheme.surface)
            )
        },
        containerColor = MaterialTheme.colorScheme.background,
        bottomBar = {
            AnimatedVisibility(
                visible = eskaeraAktiboa != null,
                enter = slideInVertically(initialOffsetY = { it }, animationSpec = tween(260)) + fadeIn(),
                exit = slideOutVertically(targetOffsetY = { it }, animationSpec = tween(220)) + fadeOut()
            ) {
                eskaeraAktiboa?.let { eskaera ->
                    Surface(
                        color = MaterialTheme.colorScheme.surface,
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
                                        text = "${AppPreferences.tr("GUZTIRA", "TOTAL", "TOTAL")} (${eskaera.lerroak.sumOf { it.kantitatea }})",
                                        fontSize = 11.sp,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                                        fontWeight = FontWeight.SemiBold
                                    )
                                    Text(
                                        text = String.format(Locale.ROOT, "%.2f €", eskaera.guztira),
                                        fontFamily = JetBrainsMono,
                                        fontSize = 21.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = MaterialTheme.colorScheme.primary
                                    )
                                }

                                Row(horizontalArrangement = Arrangement.spacedBy(6.dp)) {
                                    Button(
                                        onClick = { onGehituPlaterak(eskaera.id, mahaiId) },
                                        colors = ButtonDefaults.buttonColors(
                                            containerColor = MaterialTheme.colorScheme.surfaceVariant,
                                            contentColor = MaterialTheme.colorScheme.onSurface
                                        ),
                                        shape = RoundedCornerShape(10.dp),
                                        contentPadding = PaddingValues(horizontal = 10.dp, vertical = 6.dp)
                                    ) {
                                        Icon(imageVector = Icons.Default.Add, contentDescription = null, modifier = Modifier.size(15.dp))
                                        Spacer(modifier = Modifier.width(3.dp))
                                        val daBarrakoa = zerbitzaria.rola.equals("Barrakoa", ignoreCase = true)
                                        Text(
                                            if (daBarrakoa) AppPreferences.tr("Edariak", "Bebidas", "Drinks")
                                            else AppPreferences.tr("Platerak", "Platos", "Dishes"),
                                            fontSize = 12.sp
                                        )
                                    }

                                    Button(
                                        onClick = onIrekiKontua,
                                        enabled = eskaera.lerroak.isNotEmpty(),
                                        colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                                        shape = RoundedCornerShape(10.dp),
                                        contentPadding = PaddingValues(horizontal = 10.dp, vertical = 6.dp)
                                    ) {
                                        Icon(
                                            imageVector = Icons.AutoMirrored.Filled.ReceiptLong,
                                            contentDescription = null,
                                            tint = Color.White,
                                            modifier = Modifier.size(15.dp)
                                        )
                                        Spacer(modifier = Modifier.width(3.dp))
                                        Text(
                                            AppPreferences.tr("Kontua", "Cuenta", "Bill"),
                                            fontSize = 12.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = Color.White
                                        )
                                    }

                                    Button(
                                        onClick = onIrekiOrdainketa,
                                        enabled = eskaera.lerroak.isNotEmpty(),
                                        colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                                        shape = RoundedCornerShape(10.dp),
                                        contentPadding = PaddingValues(horizontal = 10.dp, vertical = 6.dp)
                                    ) {
                                        Icon(
                                            imageVector = Icons.Default.Payment,
                                            contentDescription = null,
                                            tint = Color.White,
                                            modifier = Modifier.size(15.dp)
                                        )
                                        Spacer(modifier = Modifier.width(3.dp))
                                        Text(
                                            AppPreferences.tr("Kobratu", "Cobrar", "Pay"),
                                            fontSize = 12.sp,
                                            fontWeight = FontWeight.Bold,
                                            color = Color.White
                                        )
                                    }
                                }
                            }

                            val zaiKop = eskaera.lerroak.count {
                                !KategoriaLaguntzailea.daBarrakoProduktua(it.kategoria) &&
                                    (it.egoera.equals("Irekia", ignoreCase = true) ||
                                        it.egoera.equals("Eskatua", ignoreCase = true))
                            }
                            AnimatedVisibility(
                                visible = zaiKop > 0,
                                enter = expandVertically(tween(240)) + fadeIn(),
                                exit = shrinkVertically(tween(200)) + fadeOut()
                            ) {
                                Column {
                                    Spacer(modifier = Modifier.height(10.dp))
                                    Button(
                                        onClick = onBidaliSukaldera,
                                        modifier = Modifier
                                            .fillMaxWidth()
                                            .height(48.dp),
                                        colors = ButtonDefaults.buttonColors(containerColor = StatusPending),
                                        shape = RoundedCornerShape(12.dp),
                                        enabled = !ekintzanAri
                                    ) {
                                        Icon(
                                            imageVector = Icons.AutoMirrored.Filled.Send,
                                            contentDescription = null,
                                            tint = Color.White,
                                            modifier = Modifier.size(18.dp)
                                        )
                                        Spacer(modifier = Modifier.width(8.dp))
                                        Text(
                                            text = AppPreferences.tr(
                                                "Bidali Sukaldera ($zaiKop plater zain)",
                                                "Enviar a Cocina ($zaiKop platos pendientes)",
                                                "Send to Kitchen ($zaiKop pending)"
                                            ),
                                            color = Color.White,
                                            fontWeight = FontWeight.Bold,
                                            fontSize = 14.sp
                                        )
                                    }
                                }
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
            when {
                kargatzen && mahaia == null -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        CircularProgressIndicator(color = MaterialTheme.colorScheme.primary)
                    }
                }
                mahaia == null -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Text(text = erroreMezua ?: "Ez da mahaia aurkitu.", color = StatusBusy)
                    }
                }
                else -> {
                    val egoeraKolorea by animateColorAsState(
                        targetValue = when (mahaia.egoera.lowercase()) {
                            "libre" -> StatusFree
                            "okupatuta" -> StatusBusy
                            "erreserbatuta" -> StatusPending
                            "garbitzen" -> StatusCleaning
                            else -> StatusPending
                        },
                        animationSpec = tween(300),
                        label = "detailStatusColor"
                    )

                    // MAHAIAREN EGOERA TXARTELA
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
                        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.3f)),
                        shape = RoundedCornerShape(16.dp)
                    ) {
                        Column(modifier = Modifier.padding(16.dp)) {
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Column(modifier = Modifier.weight(1f, fill = false)) {
                                    Text(
                                        text = "${AppPreferences.tr("Mahaia", "Mesa", "Table")} ${mahaia.zenbakia} (${mahaia.kokapena})",
                                        fontSize = 17.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = MaterialTheme.colorScheme.onSurface
                                    )
                                    Text(
                                        text = "${AppPreferences.tr("Edukiera", "Capacidad", "Capacity")}: ${mahaia.edukiera} pax",
                                        fontSize = 13.sp,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }

                                Spacer(modifier = Modifier.width(8.dp))

                                Surface(
                                    color = egoeraKolorea.copy(alpha = 0.15f),
                                    shape = RoundedCornerShape(20.dp),
                                    border = BorderStroke(1.dp, egoeraKolorea)
                                ) {
                                    Text(
                                        text = mahaia.egoera.uppercase(),
                                        color = egoeraKolorea,
                                        fontSize = 11.sp,
                                        fontWeight = FontWeight.Bold,
                                        maxLines = 1,
                                        softWrap = false,
                                        modifier = Modifier.padding(horizontal = 10.dp, vertical = 6.dp)
                                    )
                                }
                            }

                            Spacer(modifier = Modifier.height(14.dp))

                            val kontuaKobratzeko = eskaeraAktiboa != null

                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                listOf("Libre", "Okupatuta", "Garbitzen").forEach { egoera ->
                                    val aktibo = mahaia.egoera.equals(egoera, ignoreCase = true)
                                    val blokeatuta = kontuaKobratzeko && !egoera.equals("Okupatuta", ignoreCase = true)
                                    val egoeraKoloreBotoi = when (egoera.lowercase()) {
                                        "libre" -> StatusFree
                                        "okupatuta" -> StatusBusy
                                        "garbitzen" -> StatusCleaning
                                        else -> AmberAccent
                                    }
                                    val btnBg by animateColorAsState(
                                        targetValue = if (aktibo) egoeraKoloreBotoi.copy(alpha = 0.2f) else Color.Transparent,
                                        animationSpec = tween(250),
                                        label = "statusBtn$egoera"
                                    )
                                    OutlinedButton(
                                        onClick = { onAldatuMahaiEgoera(egoera) },
                                        enabled = !ekintzanAri && !blokeatuta,
                                        modifier = Modifier.weight(1f),
                                        colors = ButtonDefaults.outlinedButtonColors(
                                            containerColor = btnBg,
                                            disabledContainerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.35f)
                                        ),
                                        border = BorderStroke(
                                            1.dp,
                                            when {
                                                aktibo -> egoeraKoloreBotoi
                                                blokeatuta -> MaterialTheme.colorScheme.outline.copy(alpha = 0.2f)
                                                else -> MaterialTheme.colorScheme.outline.copy(alpha = 0.4f)
                                            }
                                        ),
                                        shape = RoundedCornerShape(10.dp),
                                        contentPadding = PaddingValues(horizontal = 4.dp, vertical = 6.dp)
                                    ) {
                                        if (blokeatuta) {
                                            Icon(
                                                imageVector = Icons.Default.Lock,
                                                contentDescription = null,
                                                tint = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.45f),
                                                modifier = Modifier.size(12.dp)
                                            )
                                            Spacer(modifier = Modifier.width(4.dp))
                                        }
                                        Text(
                                            text = egoera,
                                            fontSize = 11.sp,
                                            color = when {
                                                aktibo -> egoeraKoloreBotoi
                                                blokeatuta -> MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.45f)
                                                else -> MaterialTheme.colorScheme.onSurfaceVariant
                                            },
                                            fontWeight = if (aktibo) FontWeight.Bold else FontWeight.Medium,
                                            maxLines = 1,
                                            softWrap = false
                                        )
                                    }
                                }
                            }

                            if (kontuaKobratzeko) {
                                Spacer(modifier = Modifier.height(8.dp))
                                Text(
                                    text = AppPreferences.tr(
                                        "Kontua kobratu behar da mahaia Libre edo Garbitzen jartzeko.",
                                        "Debes cobrar la cuenta antes de liberar o limpiar la mesa.",
                                        "Pay the bill before marking this table Free or Cleaning."
                                    ),
                                    fontSize = 11.sp,
                                    color = StatusPending,
                                    fontWeight = FontWeight.Medium
                                )
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
                            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
                            border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.2f)),
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
                                    tint = MaterialTheme.colorScheme.onSurfaceVariant,
                                    modifier = Modifier.size(48.dp)
                                )
                                Spacer(modifier = Modifier.height(12.dp))
                                Text(
                                    text = AppPreferences.tr(
                                        "Ez dago eskaera aktiborik mahai honetan",
                                        "No hay ningún pedido activo en esta mesa",
                                        "No active order for this table"
                                    ),
                                    fontSize = 15.sp,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                                    textAlign = TextAlign.Center
                                )
                                Spacer(modifier = Modifier.height(16.dp))
                                Button(
                                    onClick = onIrekiSortuEskaera,
                                    colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                                    shape = RoundedCornerShape(12.dp)
                                ) {
                                    Icon(imageVector = Icons.Default.Add, contentDescription = null, tint = Color.White)
                                    Spacer(modifier = Modifier.width(6.dp))
                                    Text(
                                        AppPreferences.tr("Sortu Eskaera Berria", "Crear Nuevo Pedido", "Create New Order"),
                                        color = Color.White,
                                        fontWeight = FontWeight.Bold
                                    )
                                }
                            }
                        }
                    } else {
                        Card(
                            modifier = Modifier
                                .fillMaxWidth()
                                .weight(1f),
                            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
                            border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.2f)),
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
                                        text = "${AppPreferences.tr("Eskaera", "Pedido", "Order")} #${eskaeraAktiboa.id} · ${eskaeraAktiboa.pertsonaKopurua} pax",
                                        fontSize = 15.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = MaterialTheme.colorScheme.onSurface
                                    )
                                    Text(
                                        text = eskaeraAktiboa.sortzeData.ifEmpty { "Gaur" },
                                        fontSize = 11.sp,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }

                                HorizontalDivider(
                                    modifier = Modifier.padding(vertical = 10.dp),
                                    color = MaterialTheme.colorScheme.outline.copy(alpha = 0.25f)
                                )

                                if (eskaeraAktiboa.lerroak.isEmpty()) {
                                    Box(
                                        modifier = Modifier
                                            .fillMaxWidth()
                                            .weight(1f),
                                        contentAlignment = Alignment.Center
                                    ) {
                                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                                            Text(
                                                text = AppPreferences.tr(
                                                    "Ez dago platerik eskaera honetan.",
                                                    "No hay platos en este pedido.",
                                                    "No dishes in this order yet."
                                                ),
                                                color = MaterialTheme.colorScheme.onSurfaceVariant,
                                                fontSize = 13.sp
                                            )
                                            Spacer(modifier = Modifier.height(8.dp))
                                            Button(
                                                onClick = { onGehituPlaterak(eskaeraAktiboa.id, mahaiId) },
                                                colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary)
                                            ) {
                                                val daBarrakoa = zerbitzaria.rola.equals("Barrakoa", ignoreCase = true)
                                                Text(
                                                    if (daBarrakoa) AppPreferences.tr("Gehitu Edariak eta Kafeak", "Añadir Bebidas y Cafés", "Add Drinks & Coffees")
                                                    else AppPreferences.tr("Gehitu Platerak Katalogoatik", "Añadir Platos del Catálogo", "Add Dishes from Menu"),
                                                    color = Color.White,
                                                    fontWeight = FontWeight.Bold
                                                )
                                            }
                                            Spacer(modifier = Modifier.height(10.dp))
                                            OutlinedButton(
                                                onClick = onEzabatuEskaeraHutsa,
                                                enabled = !ekintzanAri,
                                                colors = ButtonDefaults.outlinedButtonColors(contentColor = StatusBusy),
                                                border = BorderStroke(1.dp, StatusBusy),
                                                shape = RoundedCornerShape(10.dp)
                                            ) {
                                                Icon(
                                                    imageVector = Icons.Default.DeleteOutline,
                                                    contentDescription = null,
                                                    tint = StatusBusy,
                                                    modifier = Modifier.size(18.dp)
                                                )
                                                Spacer(modifier = Modifier.width(6.dp))
                                                Text(
                                                    AppPreferences.tr("Ezabatu Eskaera Hutsa", "Eliminar Pedido Vacío", "Delete Empty Order"),
                                                    fontWeight = FontWeight.Bold,
                                                    color = StatusBusy
                                                )
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
                                        items(eskaeraAktiboa.lerroak, key = { it.id }) { lerro ->
                                            EskaeraLerroElementua(
                                                lerro = lerro,
                                                ekintzanAri = ekintzanAri,
                                                onZerbitzatu = { onZerbitzatuLerroa(lerro.id) },
                                                onEzabatu = { onEzabatuLerroa(lerro.id) }
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
    }
}

@Composable
fun EskaeraLerroElementua(
    lerro: EskaeraLerroaDto,
    onZerbitzatu: () -> Unit,
    modifier: Modifier = Modifier,
    ekintzanAri: Boolean = false,
    onEzabatu: () -> Unit = {}
) {
    val egoeraKolorea = when (lerro.egoera.lowercase()) {
        "irekia", "eskatua" -> StatusPending
        "sukaldean" -> AmberAccent
        "prestatua" -> StatusFree
        "zerbitzatua" -> MaterialTheme.colorScheme.onSurfaceVariant
        else -> MaterialTheme.colorScheme.onSurfaceVariant
    }
    // Sukaldera bidali gabe edo zerbitzatu gabe dauden lerroak soilik ezaba daitezke
    val ezabaDaiteke = lerro.egoera.equals("Eskatua", ignoreCase = true) ||
        lerro.egoera.equals("Irekia", ignoreCase = true)

    val zerbitzaDaiteke = lerro.egoera.equals("Prestatua", ignoreCase = true) ||
        (KategoriaLaguntzailea.daBarrakoProduktua(lerro.kategoria) &&
            (lerro.egoera.equals("Eskatua", ignoreCase = true) || lerro.egoera.equals("Irekia", ignoreCase = true)))

    Surface(
        modifier = modifier.fillMaxWidth(),
        color = MaterialTheme.colorScheme.surface,
        border = BorderStroke(1.dp, MaterialTheme.colorScheme.outline),
        shape = RoundedCornerShape(14.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp)
        ) {
            // Goiko lerroa: kopurua, izena/egoera eta prezioa garbi
            Row(
                modifier = Modifier.fillMaxWidth(),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Box(
                    modifier = Modifier
                        .size(34.dp)
                        .clip(RoundedCornerShape(8.dp))
                        .background(MaterialTheme.colorScheme.background),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        text = "${lerro.kantitatea}x",
                        fontFamily = JetBrainsMono,
                        fontSize = 13.sp,
                        fontWeight = FontWeight.Bold,
                        color = MaterialTheme.colorScheme.secondary
                    )
                }

                Spacer(modifier = Modifier.width(12.dp))

                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        text = lerro.platerIzena,
                        fontSize = 14.sp,
                        fontWeight = FontWeight.SemiBold,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Text(
                            text = "${lerro.kategoria} · ",
                            fontSize = 11.sp,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
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
                            color = MaterialTheme.colorScheme.secondary
                        )
                    }
                }

                Spacer(modifier = Modifier.width(8.dp))

                Text(
                    text = String.format(Locale.ROOT, "%.2f €", lerro.azpitotala),
                    fontFamily = JetBrainsMono,
                    fontSize = 14.sp,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.onSurface
                )
            }

            // Ekintza barra bereizia: Ezabatu EZKERREAN, Zerbitzatu ESKUINEAN (inoiz ez nahasteko!)
            if (ezabaDaiteke || zerbitzaDaiteke) {
                Spacer(modifier = Modifier.height(10.dp))
                HorizontalDivider(color = MaterialTheme.colorScheme.outline.copy(alpha = 0.2f))
                Spacer(modifier = Modifier.height(8.dp))

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    if (ezabaDaiteke) {
                        OutlinedButton(
                            onClick = onEzabatu,
                            enabled = !ekintzanAri,
                            colors = ButtonDefaults.outlinedButtonColors(contentColor = StatusBusy),
                            border = BorderStroke(1.dp, StatusBusy.copy(alpha = 0.6f)),
                            shape = RoundedCornerShape(8.dp),
                            contentPadding = PaddingValues(horizontal = 10.dp, vertical = 4.dp),
                            modifier = Modifier.height(32.dp)
                        ) {
                            Icon(
                                imageVector = Icons.Default.DeleteOutline,
                                contentDescription = null,
                                tint = StatusBusy,
                                modifier = Modifier.size(15.dp)
                            )
                            Spacer(modifier = Modifier.width(4.dp))
                            Text(
                                text = AppPreferences.tr("Ezabatu", "Eliminar", "Remove"),
                                fontSize = 11.sp,
                                fontWeight = FontWeight.SemiBold,
                                color = StatusBusy
                            )
                        }
                    } else {
                        Spacer(modifier = Modifier.width(1.dp))
                    }

                    if (zerbitzaDaiteke) {
                        Button(
                            onClick = onZerbitzatu,
                            enabled = !ekintzanAri,
                            colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                            shape = RoundedCornerShape(8.dp),
                            contentPadding = PaddingValues(horizontal = 12.dp, vertical = 4.dp),
                            modifier = Modifier.height(32.dp)
                        ) {
                            Icon(
                                imageVector = Icons.Default.Check,
                                contentDescription = null,
                                tint = Color.White,
                                modifier = Modifier.size(15.dp)
                            )
                            Spacer(modifier = Modifier.width(4.dp))
                            Text(
                                text = AppPreferences.tr("Zerbitzatu", "Servir", "Serve"),
                                fontSize = 11.sp,
                                color = Color.White,
                                fontWeight = FontWeight.Bold
                            )
                        }
                    }
                }
            }
        }
    }
}

@Preview(showBackground = true, name = "MahaiaXehetasunak - Iluna")
@Composable
fun MahaiaXehetasunakDarkPreview() {
    KomanderaTheme(darkTheme = true) {
        MahaiaXehetasunakContent(
            mahaiId = 1,
            mahaia = MahaiaDto(1, 1, 4, "Jantoki Nagusia", "Okupatuta"),
            eskaeraAktiboa = EskaeraDto(
                id = 42,
                mahaiaId = 1,
                mahaiZenbakia = 1,
                pertsonaKopurua = 4,
                egoera = "Irekia",
                guztira = 44.50,
                sortzeData = "14:15",
                lerroak = listOf(
                    EskaeraLerroaDto(1, 42, 10, "Txuleta Parrillan", "Bigarrena", 1, 28.00, "Prestatua"),
                    EskaeraLerroaDto(2, 42, 12, "Risotto Funghi", "Lehena", 1, 16.50, "Irekia", "Gaztarik gabe")
                )
            ),
            zerbitzaria = LangileaDto(1, "ane", "Ane Etxeberria", "Zerbitzaria"),
            kargatzen = false,
            erroreMezua = null,
            ekintzanAri = false,
            onAtzera = {},
            onFreskatu = {},
            onAldatuMahaiEgoera = {},
            onIrekiSortuEskaera = {},
            onGehituPlaterak = { _, _ -> },
            onBidaliSukaldera = {},
            onZerbitzatuLerroa = {},
            onIrekiKontua = {},
            onIrekiOrdainketa = {}
        )
    }
}
