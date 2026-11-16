package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material.icons.filled.Remove
import androidx.compose.material.icons.filled.Send
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.model.EskaeraLerroa
import eus.goierri.jatetxea.komandera.data.model.Mahaia
import eus.goierri.jatetxea.komandera.data.model.Platera
import eus.goierri.jatetxea.komandera.ui.theme.*

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun EskaeraScreen(
    mahaia: Mahaia,
    platerak: List<Platera>,
    onEskaeraBidalita: (List<EskaeraLerroa>) -> Unit,
    onAtzera: () -> Unit,
    modifier: Modifier = Modifier
) {
    // Uneko eskaeraren egoera erreaktiboa (remember)
    var lerroak by remember { mutableStateOf(listOf<EskaeraLerroa>()) }
    var hautatutakoKategoria by remember { mutableStateOf("Guztiak") }

    val kategoriak = listOf("Guztiak", "Hasierakoak", "Plater Nagusiak", "Postreak", "Edariak")

    val filtratutakoPlaterak = if (hautatutakoKategoria == "Guztiak") {
        platerak
    } else {
        platerak.filter { it.kategoria.equals(hautatutakoKategoria, ignoreCase = true) }
    }

    val guztira = lerroak.sumOf { it.guztira }

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "Mahaia ${mahaia.zenbakia} - Komanda Berria",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = TextPrimary
                        )
                        Text(
                            text = "${mahaia.eremua} • ${lerroak.sumOf { it.kopurua }} plater hautatuta",
                            fontSize = 12.sp,
                            color = AmberPrimary
                        )
                    }
                },
                navigationIcon = {
                    IconButton(onClick = onAtzera) {
                        Icon(
                            imageVector = Icons.Default.ArrowBack,
                            contentDescription = "Atzera",
                            tint = TextPrimary
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = DarkSurface)
            )
        },
        containerColor = DarkBackground,
        bottomBar = {
            // Beheko barra: Guztira eta Bidalteko Botoia
            Surface(
                color = DarkSurface,
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
                            text = "GUZTIRA",
                            fontSize = 12.sp,
                            color = TextSecondary,
                            fontWeight = FontWeight.SemiBold
                        )
                        Text(
                            text = "%.2f €".format(guztira),
                            fontSize = 22.sp,
                            fontWeight = FontWeight.Bold,
                            color = AmberPrimary
                        )
                    }

                    Button(
                        onClick = { onEskaeraBidalita(lerroak) },
                        enabled = lerroak.isNotEmpty(),
                        colors = ButtonDefaults.buttonColors(
                            containerColor = AmberPrimary,
                            disabledContainerColor = DarkSurfaceVariant
                        ),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Icon(
                            imageVector = Icons.Default.Send,
                            contentDescription = null,
                            tint = if (lerroak.isNotEmpty()) DarkBackground else TextMuted
                        )
                        Spacer(modifier = Modifier.width(8.dp))
                        Text(
                            text = "Bidali Sukaldera",
                            fontWeight = FontWeight.Bold,
                            color = if (lerroak.isNotEmpty()) DarkBackground else TextMuted
                        )
                    }
                }
            }
        }
    ) { innerPadding ->
        Column(
            modifier = modifier
                .fillMaxSize()
                .padding(innerPadding)
                .padding(horizontal = 16.dp)
        ) {
            Spacer(modifier = Modifier.height(12.dp))

            // Kategoria-zerrenda horizontala (Chips)
            LazyRow(
                horizontalArrangement = Arrangement.spacedBy(8.dp),
                modifier = Modifier.fillMaxWidth()
            ) {
                items(kategoriak) { kategoria ->
                    FilterChip(
                        selected = hautatutakoKategoria == kategoria,
                        onClick = { hautatutakoKategoria = kategoria },
                        label = { Text(kategoria, fontSize = 13.sp) },
                        colors = FilterChipDefaults.filterChipColors(
                            selectedContainerColor = AmberPrimary,
                            selectedLabelColor = DarkBackground,
                            containerColor = DarkSurfaceVariant,
                            labelColor = TextPrimary
                        )
                    )
                }
            }

            Spacer(modifier = Modifier.height(12.dp))

            // Zatiketa: Goian Plateren Katalogoa, Behean Hautatutako Komanda
            Text(
                text = "Plateren Katalogoa:",
                fontSize = 15.sp,
                fontWeight = FontWeight.SemiBold,
                color = TextSecondary
            )

            Spacer(modifier = Modifier.height(8.dp))

            LazyColumn(
                modifier = Modifier
                    .weight(1f)
                    .fillMaxWidth(),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(filtratutakoPlaterak) { platera ->
                    PlaterElementua(
                        platera = platera,
                        onGehitu = {
                            val aurkitua = lerroak.find { it.platerId == platera.id }
                            lerroak = if (aurkitua != null) {
                                lerroak.map {
                                    if (it.platerId == platera.id) it.copy(kopurua = it.kopurua + 1) else it
                                }
                            } else {
                                lerroak + EskaeraLerroa(
                                    platerId = platera.id,
                                    platerIzena = platera.izena,
                                    kopurua = 1,
                                    prezioUnitarioa = platera.prezioa
                                )
                            }
                        }
                    )
                }
            }

            Divider(color = DarkSurfaceVariant, modifier = Modifier.padding(vertical = 8.dp))

            // Hautatutako lerroen zerrenda
            Text(
                text = "Komandaren Zerrenda (${lerroak.size}):",
                fontSize = 15.sp,
                fontWeight = FontWeight.SemiBold,
                color = TextSecondary
            )

            Spacer(modifier = Modifier.height(8.dp))

            LazyColumn(
                modifier = Modifier
                    .weight(0.9f)
                    .fillMaxWidth(),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                if (lerroak.isEmpty()) {
                    item {
                        Box(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(24.dp),
                            contentAlignment = Alignment.Center
                        ) {
                            Text(
                                text = "Ez dago platerrik hautatuta oraindik.",
                                fontSize = 14.sp,
                                color = TextMuted
                            )
                        }
                    }
                } else {
                    items(lerroak) { lerroa ->
                        KomandaLerroElementua(
                            lerroa = lerroa,
                            onGehitu = {
                                lerroak = lerroak.map {
                                    if (it.platerId == lerroa.platerId) it.copy(kopurua = it.kopurua + 1) else it
                                }
                            },
                            onKendu = {
                                lerroak = if (lerroa.kopurua > 1) {
                                    lerroak.map {
                                        if (it.platerId == lerroa.platerId) it.copy(kopurua = it.kopurua - 1) else it
                                    }
                                } else {
                                    lerroak.filter { it.platerId != lerroa.platerId }
                                }
                            },
                            onEzabatu = {
                                lerroak = lerroak.filter { it.platerId != lerroa.platerId }
                            }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun PlaterElementua(
    platera: Platera,
    onGehitu: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        colors = CardDefaults.cardColors(containerColor = DarkSurface)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = platera.izena,
                    fontSize = 15.sp,
                    fontWeight = FontWeight.Medium,
                    color = TextPrimary
                )
                Text(
                    text = "${platera.kategoria} • ${platera.deskribapena}",
                    fontSize = 12.sp,
                    color = TextSecondary,
                    maxLines = 1
                )
            }

            Spacer(modifier = Modifier.width(8.dp))

            Text(
                text = "%.2f €".format(platera.prezioa),
                fontSize = 15.sp,
                fontWeight = FontWeight.Bold,
                color = AmberPrimary
            )

            Spacer(modifier = Modifier.width(12.dp))

            IconButton(
                onClick = onGehitu,
                colors = IconButtonDefaults.iconButtonColors(
                    containerColor = AmberPrimary,
                    contentColor = DarkBackground
                ),
                modifier = Modifier.size(36.dp)
            ) {
                Icon(imageVector = Icons.Default.Add, contentDescription = "Gehitu")
            }
        }
    }
}

@Composable
fun KomandaLerroElementua(
    lerroa: EskaeraLerroa,
    onGehitu: () -> Unit,
    onKendu: () -> Unit,
    onEzabatu: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(10.dp),
        colors = CardDefaults.cardColors(containerColor = DarkSurfaceVariant)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(10.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = lerroa.platerIzena,
                    fontSize = 14.sp,
                    fontWeight = FontWeight.SemiBold,
                    color = TextPrimary
                )
                Text(
                    text = "%.2f € / unitatea".format(lerroa.prezioUnitarioa),
                    fontSize = 12.sp,
                    color = TextSecondary
                )
            }

            Row(verticalAlignment = Alignment.CenterVertically) {
                IconButton(onClick = onKendu, modifier = Modifier.size(28.dp)) {
                    Icon(imageVector = Icons.Default.Remove, contentDescription = "Kendu", tint = TextPrimary)
                }

                Text(
                    text = "${lerroa.kopurua}",
                    fontSize = 15.sp,
                    fontWeight = FontWeight.Bold,
                    color = AmberPrimary,
                    modifier = Modifier.padding(horizontal = 8.dp)
                )

                IconButton(onClick = onGehitu, modifier = Modifier.size(28.dp)) {
                    Icon(imageVector = Icons.Default.Add, contentDescription = "Gehitu", tint = TextPrimary)
                }

                Spacer(modifier = Modifier.width(8.dp))

                Text(
                    text = "%.2f €".format(lerroa.guztira),
                    fontSize = 14.sp,
                    fontWeight = FontWeight.Bold,
                    color = TextPrimary
                )

                IconButton(onClick = onEzabatu, modifier = Modifier.size(28.dp)) {
                    Icon(imageVector = Icons.Default.Delete, contentDescription = "Ezabatu", tint = StatusOccupied)
                }
            }
        }
    }
}

@Preview(showBackground = true, widthDp = 360, heightDp = 640)
@Composable
fun EskaeraScreenPreview() {
    KomanderaTheme {
        EskaeraScreen(
            mahaia = Mahaia(1, 4, "Mahaia 4", 4, "Libre", "Areto Nagusia"),
            platerak = listOf(
                Platera(1, "Iberiko Urdaiazpikoa", "Guijuelo ezkurrezkoa", 18.50, "Hasierakoak"),
                Platera(2, "Txuleta Errea", "1kg Euskal Okela", 38.00, "Plater Nagusiak"),
                Platera(3, "Bakailaoa Bizkaitar Erara", "Tradizionala", 22.00, "Plater Nagusiak"),
                Platera(4, "Goxua", "Etxekoa", 6.50, "Postreak"),
                Platera(5, "Errioxa Ardoa", "Krianza", 14.00, "Edariak")
            ),
            onEskaeraBidalita = {},
            onAtzera = {}
        )
    }
}
