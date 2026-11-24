package eus.goierri.jatetxea.komandera.ui.screens

import android.app.Activity
import android.content.ActivityNotFoundException
import android.content.Intent
import android.speech.RecognizerIntent
import android.widget.Toast
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.slideInVertically
import androidx.compose.animation.slideOutVertically
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Check
import androidx.compose.material.icons.filled.Close
import androidx.compose.material.icons.filled.LocalBar
import androidx.compose.material.icons.filled.Mic
import androidx.compose.material.icons.filled.Remove
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.GehituPlateraRequest
import eus.goierri.jatetxea.komandera.data.model.KategoriaLaguntzailea
import eus.goierri.jatetxea.komandera.data.model.PlateraDto
import eus.goierri.jatetxea.komandera.data.model.SortuEskaeraRequest
import eus.goierri.jatetxea.komandera.ui.state.AppPreferences
import eus.goierri.jatetxea.komandera.ui.theme.*
import kotlinx.coroutines.launch
import java.text.Normalizer
import java.util.Locale

data class SaskikoElementua(
    val platera: PlateraDto,
    val kopurua: Int = 1,
    val oharrak: String = ""
) {
    val azpitotala: Double get() = platera.prezioa * kopurua
}

@Composable
fun EskaeraScreen(
    eskaeraId: Int,
    mahaiId: Int,
    pax: Int = 2,
    langileaId: Int = 1,
    zerbitzariRola: String = "Zerbitzaria",
    onAtzera: () -> Unit,
    modifier: Modifier = Modifier
) {
    val daBarrakoa = zerbitzariRola.equals("Barrakoa", ignoreCase = true)
    val context = LocalContext.current
    val scope = rememberCoroutineScope()
    var platerak by remember { mutableStateOf<List<PlateraDto>>(emptyList()) }
    var kargatzen by remember { mutableStateOf(true) }
    var erroreMezua by remember { mutableStateOf<String?>(null) }
    var gehitzen by remember { mutableStateOf(false) }

    var hautatutakoKategoria by rememberSaveable { mutableStateOf("Guztiak") }
    var bilaketaTestua by rememberSaveable { mutableStateOf("") }
    var saskia by remember { mutableStateOf<List<SaskikoElementua>>(emptyList()) }

    val ahotsBilaketaLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.StartActivityForResult()
    ) { result ->
        if (result.resultCode == Activity.RESULT_OK) {
            val emaitzak = result.data?.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS)
            val esandakoa = emaitzak?.firstOrNull()?.trim()
            if (!esandakoa.isNullOrBlank()) {
                bilaketaTestua = esandakoa
                hautatutakoKategoria = "Guztiak"
            }
        }
    }

    fun hasiAhotsBilaketa() {
        val hizkuntzaKodea = when (AppPreferences.hizkuntza) {
            "ES" -> "es-ES"
            "EN" -> "en-US"
            else -> "eu-ES"
        }
        val intent = Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH).apply {
            putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM)
            putExtra(RecognizerIntent.EXTRA_LANGUAGE, hizkuntzaKodea)
            putExtra(
                RecognizerIntent.EXTRA_PROMPT,
                if (daBarrakoa) {
                    AppPreferences.tr("Esan edariaren izena...", "Di el nombre de la bebida...", "Say the drink name...")
                } else {
                    AppPreferences.tr("Esan plater edo edariaren izena...", "Di el nombre del plato o bebida...", "Say the dish or drink name...")
                }
            )
        }
        try {
            ahotsBilaketaLauncher.launch(intent)
        } catch (_: ActivityNotFoundException) {
            Toast.makeText(
                context,
                AppPreferences.tr(
                    "Gailu honek ez du ahots-ezagutza zerbitzurik instalatuta.",
                    "Este dispositivo no tiene reconocimiento de voz instalado.",
                    "Speech recognition is not available on this device."
                ),
                Toast.LENGTH_SHORT
            ).show()
        }
    }

    fun kargatuKatalogoa() {
        scope.launch {
            kargatzen = true
            try {
                val platerRes = ApiClient.service.getPlaterak()
                val stockRes = ApiClient.service.getPlaterenStockAhalmena()

                val stockMap = mutableMapOf<Int, Int>()
                if (stockRes.isSuccessful && stockRes.body()?.dataList != null) {
                    stockRes.body()!!.dataList!!.forEach {
                        stockMap[it.plateraId] = it.eginDaitezkeenPlaterak
                    }
                }

                if (platerRes.isSuccessful && platerRes.body()?.dataList != null) {
                    val lista = platerRes.body()!!.dataList!!
                        .filter { it.aktibo }
                        .filter { !daBarrakoa || KategoriaLaguntzailea.daBarrakoProduktua(it.kategoria) }
                        .sortedWith(
                            compareBy<PlateraDto> { KategoriaLaguntzailea.ordenaIndizea(it.kategoria) }
                                .thenBy { it.izena }
                        )
                    lista.forEach { p -> p.stockAhalmena = stockMap[p.id] ?: 999 }
                    platerak = lista
                    erroreMezua = null
                } else {
                    erroreMezua = AppPreferences.tr(
                        "Ezin izan da plater katalogoa kargatu.",
                        "No se pudo cargar el catálogo de platos.",
                        "Could not load dish menu."
                    )
                }
            } catch (e: Exception) {
                erroreMezua = "Konexio errorea: ${e.localizedMessage ?: "Zerbitzaria ez dago eskuragarri"}"
            } finally {
                kargatzen = false
            }
        }
    }

    LaunchedEffect(Unit) {
        kargatuKatalogoa()
    }

    fun gehituSaskira(platera: PlateraDto) {
        val existitzen = saskia.find { it.platera.id == platera.id }
        if (existitzen != null) {
            if (existitzen.kopurua < platera.stockAhalmena) {
                saskia = saskia.map {
                    if (it.platera.id == platera.id) it.copy(kopurua = it.kopurua + 1) else it
                }
            }
        } else {
            saskia = saskia + SaskikoElementua(platera = platera, kopurua = 1)
        }
    }

    fun kenduSaskitik(plateraId: Int) {
        val existitzen = saskia.find { it.platera.id == plateraId }
        if (existitzen != null) {
            saskia = if (existitzen.kopurua > 1) {
                saskia.map {
                    if (it.platera.id == plateraId) it.copy(kopurua = it.kopurua - 1) else it
                }
            } else {
                saskia.filter { it.platera.id != plateraId }
            }
        }
    }

    fun gordePlaterakEskaeran() {
        if (saskia.isEmpty()) return
        scope.launch {
            gehitzen = true
            try {
                // Eskaera berria bada (eskaeraId == 0), orain sortzen da produktuekin batera (inoiz ez hutsik!)
                val helburuEskaeraId = if (eskaeraId <= 0) {
                    val sortuRes = ApiClient.service.sortuEskaera(
                        SortuEskaeraRequest(
                            mahaiaId = mahaiId,
                            langileaId = langileaId,
                            pertsonaKopurua = pax
                        )
                    )
                    if (sortuRes.isSuccessful && sortuRes.body()?.data != null) {
                        sortuRes.body()!!.data!!.id
                    } else {
                        val msg = sortuRes.body()?.message ?: AppPreferences.tr(
                            "Ezin izan da eskaera sortu (egiaztatu mahaiaren aforoa).",
                            "No se pudo crear el pedido (comprueba el aforo de la mesa).",
                            "Could not create order (check table capacity)."
                        )
                        Toast.makeText(context, msg, Toast.LENGTH_LONG).show()
                        return@launch
                    }
                } else {
                    eskaeraId
                }

                for (item in saskia) {
                    val req = GehituPlateraRequest(
                        plateraId = item.platera.id,
                        kantitatea = item.kopurua,
                        oharrak = item.oharrak.ifBlank { null }
                    )
                    ApiClient.service.gehituPlatera(helburuEskaeraId, req)
                }
                onAtzera()
            } catch (e: Exception) {
                Toast.makeText(
                    context,
                    "Errorea: ${e.localizedMessage ?: "Konexio akatsa"}",
                    Toast.LENGTH_SHORT
                ).show()
            } finally {
                gehitzen = false
            }
        }
    }

    EskaeraScreenContent(
        eskaeraId = eskaeraId,
        daBarrakoa = daBarrakoa,
        platerak = platerak,
        kargatzen = kargatzen,
        erroreMezua = erroreMezua,
        gehitzen = gehitzen,
        hautatutakoKategoria = hautatutakoKategoria,
        bilaketaTestua = bilaketaTestua,
        saskia = saskia,
        onKategoriaAldatu = { hautatutakoKategoria = it },
        onBilaketaAldatu = { bilaketaTestua = it },
        onAhotsBilaketa = { hasiAhotsBilaketa() },
        onGehituSaskira = { gehituSaskira(it) },
        onKenduSaskitik = { kenduSaskitik(it) },
        onGordeEskaeran = { gordePlaterakEskaeran() },
        onFreskatu = { kargatuKatalogoa() },
        onAtzera = onAtzera,
        modifier = modifier
    )
}

private fun normalizatuBilaketa(testua: String): String =
    Normalizer.normalize(testua.trim().lowercase(Locale.ROOT), Normalizer.Form.NFD)
        .replace(Regex("\\p{InCombiningDiacriticalMarks}+"), "")

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun EskaeraScreenContent(
    eskaeraId: Int,
    daBarrakoa: Boolean = false,
    platerak: List<PlateraDto>,
    kargatzen: Boolean,
    erroreMezua: String?,
    gehitzen: Boolean,
    hautatutakoKategoria: String,
    bilaketaTestua: String = "",
    saskia: List<SaskikoElementua>,
    onKategoriaAldatu: (String) -> Unit,
    onBilaketaAldatu: (String) -> Unit = {},
    onAhotsBilaketa: () -> Unit = {},
    onGehituSaskira: (PlateraDto) -> Unit,
    onKenduSaskitik: (Int) -> Unit,
    onGordeEskaeran: () -> Unit,
    onFreskatu: () -> Unit,
    onAtzera: () -> Unit,
    modifier: Modifier = Modifier
) {
    val kategoriak = remember(platerak) {
        val katSet = platerak
            .map { it.kategoria }
            .distinct()
            .filter { it.isNotBlank() }
            .sortedBy { KategoriaLaguntzailea.ordenaIndizea(it) }
        listOf("Guztiak") + katSet
    }

    val filtratutakoPlaterak = remember(platerak, hautatutakoKategoria, bilaketaTestua) {
        val kategoriaFiltroa = if (hautatutakoKategoria == "Guztiak") platerak
        else platerak.filter { it.kategoria.equals(hautatutakoKategoria, ignoreCase = true) }

        val kontsulta = normalizatuBilaketa(bilaketaTestua)
        if (kontsulta.isBlank()) {
            kategoriaFiltroa
        } else {
            kategoriaFiltroa.filter { p ->
                normalizatuBilaketa(p.izena).contains(kontsulta) ||
                    normalizatuBilaketa(p.kategoria).contains(kontsulta) ||
                    normalizatuBilaketa(p.deskribapena.orEmpty()).contains(kontsulta)
            }
        }
    }

    val guztira by remember(saskia) { derivedStateOf { saskia.sumOf { it.azpitotala } } }
    val platerKop by remember(saskia) { derivedStateOf { saskia.sumOf { it.kopurua } } }

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = when {
                                eskaeraId <= 0 && daBarrakoa -> AppPreferences.tr("Barra Eskaera Berria", "Nuevo Pedido Barra", "New Bar Order")
                                eskaeraId <= 0 -> AppPreferences.tr("Eskaera Berria", "Nuevo Pedido", "New Order")
                                daBarrakoa -> "${AppPreferences.tr("Barra Katalogoa", "Catálogo de Barra", "Bar Menu")} - #$eskaeraId"
                                else -> "${AppPreferences.tr("Gehitu Platerak", "Añadir Platos", "Add Dishes")} - #$eskaeraId"
                            },
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.onSurface
                        )
                        Text(
                            text = "${AppPreferences.tr("Saskian", "En carrito", "In cart")}: $platerKop · ${String.format(Locale.ROOT, "%.2f €", guztira)}",
                            fontFamily = JetBrainsMono,
                            fontSize = 12.sp,
                            fontWeight = FontWeight.Bold,
                            color = if (daBarrakoa) MaterialTheme.colorScheme.secondary else MaterialTheme.colorScheme.primary
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
                colors = TopAppBarDefaults.topAppBarColors(containerColor = MaterialTheme.colorScheme.surface)
            )
        },
        containerColor = MaterialTheme.colorScheme.background,
        bottomBar = {
            AnimatedVisibility(
                visible = saskia.isNotEmpty(),
                enter = slideInVertically(initialOffsetY = { it }, animationSpec = tween(250)) + fadeIn(),
                exit = slideOutVertically(targetOffsetY = { it }, animationSpec = tween(200)) + fadeOut()
            ) {
                Surface(
                    color = MaterialTheme.colorScheme.surface,
                    tonalElevation = 8.dp,
                    shadowElevation = 8.dp
                ) {
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(16.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            Text(
                                text = AppPreferences.tr("SASKIA GUZTIRA", "TOTAL CARRITO", "CART TOTAL"),
                                fontSize = 11.sp,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Text(
                                text = String.format(Locale.ROOT, "%.2f €", guztira),
                                fontSize = 22.sp,
                                fontWeight = FontWeight.Bold,
                                color = AmberAccent
                            )
                        }

                        Button(
                            onClick = onGordeEskaeran,
                            colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary),
                            shape = RoundedCornerShape(12.dp),
                            enabled = !gehitzen,
                            modifier = Modifier.height(48.dp)
                        ) {
                            if (gehitzen) {
                                CircularProgressIndicator(color = Color.White, modifier = Modifier.size(20.dp))
                            } else {
                                Icon(imageVector = Icons.Default.Check, contentDescription = null, tint = Color.White)
                                Spacer(modifier = Modifier.width(6.dp))
                                Text(
                                    if (eskaeraId <= 0) {
                                        AppPreferences.tr("Sortu Eskaera", "Crear Pedido", "Create Order")
                                    } else {
                                        AppPreferences.tr("Gehitu Eskaerara", "Añadir al Pedido", "Add to Order")
                                    },
                                    color = Color.White,
                                    fontWeight = FontWeight.Bold
                                )
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
            if (daBarrakoa) {
                Surface(
                    color = StatusCleaning.copy(alpha = 0.14f),
                    shape = RoundedCornerShape(10.dp),
                    border = BorderStroke(1.dp, StatusCleaning.copy(alpha = 0.5f)),
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Row(
                        modifier = Modifier.padding(horizontal = 12.dp, vertical = 8.dp),
                        verticalAlignment = Alignment.CenterVertically,
                        horizontalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        Icon(
                            imageVector = Icons.Default.LocalBar,
                            contentDescription = null,
                            tint = StatusCleaning,
                            modifier = Modifier.size(18.dp)
                        )
                        Text(
                            text = AppPreferences.tr(
                                "BARRA MODUA · Edariak, kafeak, infusioak eta koktailak soilik",
                                "MODO BARRA · Solo bebidas, cafés, infusiones y cócteles",
                                "BAR MODE · Drinks, coffees, infusions & cocktails only"
                            ),
                            fontSize = 11.sp,
                            fontWeight = FontWeight.SemiBold,
                            color = StatusCleaning
                        )
                    }
                }
                Spacer(modifier = Modifier.height(10.dp))
            }

            // Bilatzailea (testuz edo ahotsez)
            OutlinedTextField(
                value = bilaketaTestua,
                onValueChange = onBilaketaAldatu,
                modifier = Modifier.fillMaxWidth(),
                singleLine = true,
                shape = RoundedCornerShape(12.dp),
                placeholder = {
                    Text(
                        text = if (daBarrakoa) {
                            AppPreferences.tr(
                                "Bilatu edaria, kafea, koktaila...",
                                "Buscar bebida, café, cóctel...",
                                "Search drink, coffee, cocktail..."
                            )
                        } else {
                            AppPreferences.tr(
                                "Bilatu platera, edaria edo kategoria...",
                                "Buscar plato, bebida o categoría...",
                                "Search dish, drink or category..."
                            )
                        },
                        fontSize = 13.sp,
                        maxLines = 1
                    )
                },
                leadingIcon = {
                    Icon(
                        imageVector = Icons.Default.Search,
                        contentDescription = AppPreferences.tr("Bilatu", "Buscar", "Search"),
                        tint = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                },
                trailingIcon = {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        if (bilaketaTestua.isNotEmpty()) {
                            IconButton(onClick = { onBilaketaAldatu("") }) {
                                Icon(
                                    imageVector = Icons.Default.Close,
                                    contentDescription = AppPreferences.tr("Garbitu", "Limpiar", "Clear"),
                                    tint = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                        }
                        IconButton(onClick = onAhotsBilaketa) {
                            Icon(
                                imageVector = Icons.Default.Mic,
                                contentDescription = AppPreferences.tr("Ahots bidez bilatu", "Buscar por voz", "Voice search"),
                                tint = if (daBarrakoa) StatusCleaning else ChiantiPrimary
                            )
                        }
                    }
                },
                colors = OutlinedTextFieldDefaults.colors(
                    focusedBorderColor = if (daBarrakoa) StatusCleaning else ChiantiPrimary,
                    unfocusedBorderColor = MaterialTheme.colorScheme.outline.copy(alpha = 0.5f),
                    focusedContainerColor = MaterialTheme.colorScheme.surface,
                    unfocusedContainerColor = MaterialTheme.colorScheme.surface
                )
            )

            Spacer(modifier = Modifier.height(10.dp))

            LazyRow(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(kategoriak) { kategoria ->
                    val hautatua = hautatutakoKategoria == kategoria
                    val etiketa = if (kategoria == "Guztiak") {
                        AppPreferences.tr("Guztiak", "Todos", "All")
                    } else kategoria

                    FilterChip(
                        selected = hautatua,
                        onClick = { onKategoriaAldatu(kategoria) },
                        label = { Text(etiketa, fontSize = 12.sp) },
                        colors = FilterChipDefaults.filterChipColors(
                            containerColor = MaterialTheme.colorScheme.surface,
                            labelColor = MaterialTheme.colorScheme.onSurfaceVariant,
                            selectedContainerColor = ChiantiPrimary,
                            selectedLabelColor = Color.White
                        ),
                        border = FilterChipDefaults.filterChipBorder(
                            borderColor = if (hautatua) ChiantiPrimary else MaterialTheme.colorScheme.outline.copy(alpha = 0.4f),
                            selectedBorderColor = ChiantiPrimary,
                            enabled = true,
                            selected = hautatua
                        )
                    )
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            when {
                kargatzen -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        CircularProgressIndicator(color = MaterialTheme.colorScheme.primary)
                    }
                }
                erroreMezua != null -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                            Text(text = erroreMezua, color = StatusBusy, textAlign = TextAlign.Center)
                            Spacer(modifier = Modifier.height(12.dp))
                            Button(
                                onClick = onFreskatu,
                                colors = ButtonDefaults.buttonColors(containerColor = ChiantiPrimary)
                            ) {
                                Text(AppPreferences.tr("Saiatu Berriro", "Reintentar", "Retry"), color = Color.White)
                            }
                        }
                    }
                }
                filtratutakoPlaterak.isEmpty() -> {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                            Icon(
                                imageVector = Icons.Default.Search,
                                contentDescription = null,
                                tint = MaterialTheme.colorScheme.onSurfaceVariant,
                                modifier = Modifier.size(40.dp)
                            )
                            Spacer(modifier = Modifier.height(8.dp))
                            Text(
                                text = AppPreferences.tr(
                                    "Ez da produkturik aurkitu \"$bilaketaTestua\" bilaketarekin.",
                                    "No se encontraron productos para \"$bilaketaTestua\".",
                                    "No products found for \"$bilaketaTestua\"."
                                ),
                                color = MaterialTheme.colorScheme.onSurfaceVariant,
                                textAlign = TextAlign.Center,
                                fontSize = 14.sp
                            )
                            if (bilaketaTestua.isNotEmpty() || hautatutakoKategoria != "Guztiak") {
                                Spacer(modifier = Modifier.height(10.dp))
                                OutlinedButton(
                                    onClick = {
                                        onBilaketaAldatu("")
                                        onKategoriaAldatu("Guztiak")
                                    }
                                ) {
                                    Text(AppPreferences.tr("Garbitu bilaketa", "Limpiar búsqueda", "Clear search"))
                                }
                            }
                        }
                    }
                }
                else -> {
                    LazyColumn(
                        modifier = Modifier.fillMaxSize(),
                        verticalArrangement = Arrangement.spacedBy(10.dp)
                    ) {
                        items(filtratutakoPlaterak, key = { it.id }) { platera ->
                            val kopurua = saskia.find { it.platera.id == platera.id }?.kopurua ?: 0
                            PlateraItem(
                                platera = platera,
                                kopurua = kopurua,
                                onGehitu = { onGehituSaskira(platera) },
                                onKendu = { onKenduSaskitik(platera.id) }
                            )
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun PlateraItem(
    platera: PlateraDto,
    kopurua: Int,
    onGehitu: () -> Unit,
    onKendu: () -> Unit,
    modifier: Modifier = Modifier
) {
    val stocka = platera.stockAhalmena
    val agortuta = stocka <= 0
    val botoiKolorea by animateColorAsState(
        targetValue = if (kopurua > 0) ChiantiPrimary else MaterialTheme.colorScheme.surfaceVariant,
        animationSpec = tween(220),
        label = "dishBtnColor"
    )

    Card(
        modifier = modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        border = BorderStroke(
            1.dp,
            if (kopurua > 0) ChiantiPrimary.copy(alpha = 0.6f)
            else MaterialTheme.colorScheme.outline.copy(alpha = 0.2f)
        ),
        shape = RoundedCornerShape(12.dp)
    ) {
        Column(modifier = Modifier.padding(14.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.Top
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        text = platera.izena,
                        fontSize = 15.sp,
                        fontWeight = FontWeight.SemiBold,
                        color = if (agortuta) MaterialTheme.colorScheme.onSurfaceVariant else MaterialTheme.colorScheme.onSurface
                    )
                    if (!platera.deskribapena.isNullOrBlank()) {
                        Text(
                            text = platera.deskribapena,
                            fontSize = 12.sp,
                            color = MaterialTheme.colorScheme.onSurfaceVariant,
                            maxLines = 2
                        )
                    }
                    if (!platera.alergenoak.isNullOrBlank()) {
                        Text(
                            text = "Alergenoak: ${platera.alergenoak}",
                            fontSize = 11.sp,
                            color = AmberAccent
                        )
                    }
                }

                Spacer(modifier = Modifier.width(8.dp))

                Column(horizontalAlignment = Alignment.End) {
                    Text(
                        text = String.format(Locale.ROOT, "%.2f €", platera.prezioa),
                        fontSize = 16.sp,
                        fontWeight = FontWeight.Bold,
                        color = AmberAccent
                    )

                    Text(
                        text = if (agortuta) AppPreferences.tr("Agortuta", "Agotado", "Sold out") else "Stock: $stocka",
                        fontSize = 11.sp,
                        color = if (agortuta) StatusBusy else StatusFree,
                        fontWeight = FontWeight.Medium
                    )
                }
            }

            Spacer(modifier = Modifier.height(10.dp))

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.End,
                verticalAlignment = Alignment.CenterVertically
            ) {
                AnimatedVisibility(visible = kopurua > 0) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        IconButton(
                            onClick = onKendu,
                            modifier = Modifier
                                .size(32.dp)
                                .clip(CircleShape)
                                .background(MaterialTheme.colorScheme.surfaceVariant)
                        ) {
                            Icon(
                                imageVector = Icons.Default.Remove,
                                contentDescription = "Kendu",
                                tint = MaterialTheme.colorScheme.onSurface,
                                modifier = Modifier.size(16.dp)
                            )
                        }

                        Text(
                            text = "$kopurua",
                            fontSize = 15.sp,
                            fontWeight = FontWeight.Bold,
                            color = AmberAccent,
                            modifier = Modifier.padding(horizontal = 14.dp)
                        )
                    }
                }

                Button(
                    onClick = onGehitu,
                    enabled = !agortuta && kopurua < stocka,
                    colors = ButtonDefaults.buttonColors(containerColor = botoiKolorea),
                    shape = RoundedCornerShape(8.dp),
                    contentPadding = PaddingValues(horizontal = 12.dp, vertical = 6.dp),
                    modifier = Modifier.height(36.dp)
                ) {
                    Icon(
                        imageVector = Icons.Default.Add,
                        contentDescription = "Gehitu",
                        tint = if (kopurua > 0) Color.White else MaterialTheme.colorScheme.onSurface,
                        modifier = Modifier.size(16.dp)
                    )
                    Spacer(modifier = Modifier.width(4.dp))
                    Text(
                        text = if (kopurua > 0) {
                            AppPreferences.tr("Gehiago", "Más", "More")
                        } else {
                            AppPreferences.tr("Hautatu", "Añadir", "Select")
                        },
                        fontSize = 12.sp,
                        fontWeight = FontWeight.Bold,
                        color = if (kopurua > 0) Color.White else MaterialTheme.colorScheme.onSurface
                    )
                }
            }
        }
    }
}

@Preview(showBackground = true, name = "EskaeraScreen - Barrakoa")
@Composable
fun EskaeraScreenBarrakoaPreview() {
    val edariak = listOf(
        PlateraDto(1, "Aperol Spritz Italiarra", "Prosecco, Aperol, soda eta laranja xerra", "Koktailak", 7.50, "Sulfitoak", 5, true, 40),
        PlateraDto(2, "Cappuccino Krematsua", "Espressoa, esne aparra eta kakao ukitua", "Kafeak", 2.50, "Laktosa", 3, true, 90),
        PlateraDto(3, "Chianti Classico D.O.C.G. (Kopa)", "Toscana eskualdeko ardo beltza", "Ardoak", 4.80, "Sulfitoak", 2, true, 60)
    )
    KomanderaTheme(darkTheme = true) {
        EskaeraScreenContent(
            eskaeraId = 42,
            daBarrakoa = true,
            platerak = edariak,
            kargatzen = false,
            erroreMezua = null,
            gehitzen = false,
            hautatutakoKategoria = "Guztiak",
            saskia = listOf(SaskikoElementua(edariak[0], kopurua = 2)),
            onKategoriaAldatu = {},
            onGehituSaskira = {},
            onKenduSaskitik = {},
            onGordeEskaeran = {},
            onFreskatu = {},
            onAtzera = {}
        )
    }
}

@Preview(showBackground = true, name = "EskaeraScreen - Iluna")
@Composable
fun EskaeraScreenDarkPreview() {
    val platerak = listOf(
        PlateraDto(1, "Burrata & Tomate Euskalduna", "Albahaka olioa eta baratxuri txigortua", "Hasierakoak", 14.50, "Esnea", 10, true, 8),
        PlateraDto(2, "Txuleta Parrillan (1kg)", "Basa-haragia piper gorriekin", "Plater Nagusiak", 48.00, null, 20, true, 3),
        PlateraDto(3, "Tiramisu Etxekoa", "Mascarpone eta kafe espresoa", "Postreak", 7.50, "Glutena, Arrautza", 5, true, 0)
    )
    KomanderaTheme(darkTheme = true) {
        EskaeraScreenContent(
            eskaeraId = 42,
            platerak = platerak,
            kargatzen = false,
            erroreMezua = null,
            gehitzen = false,
            hautatutakoKategoria = "Guztiak",
            saskia = listOf(SaskikoElementua(platerak[0], kopurua = 2)),
            onKategoriaAldatu = {},
            onGehituSaskira = {},
            onKenduSaskitik = {},
            onGordeEskaeran = {},
            onFreskatu = {},
            onAtzera = {}
        )
    }
}
