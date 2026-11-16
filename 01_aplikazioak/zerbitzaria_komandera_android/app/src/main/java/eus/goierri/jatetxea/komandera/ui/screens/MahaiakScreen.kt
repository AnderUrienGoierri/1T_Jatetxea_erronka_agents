package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.model.Langilea
import eus.goierri.jatetxea.komandera.data.model.Mahaia
import eus.goierri.jatetxea.komandera.ui.theme.*

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MahaiakScreen(
    zerbitzaria: Langilea,
    mahaiak: List<Mahaia>,
    onMahaiaAukeratua: (Mahaia) -> Unit,
    onAtzera: () -> Unit,
    onEguneratu: () -> Unit,
    modifier: Modifier = Modifier
) {
    var eremuFiltroa by remember { mutableStateOf("Guztiak") }
    val eremuak = listOf("Guztiak", "Areto Nagusia", "Terraza", "VIP Gunea")

    val filtratuak = if (eremuFiltroa == "Guztiak") {
        mahaiak
    } else {
        mahaiak.filter { it.eremua.equals(eremuFiltroa, ignoreCase = true) }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "Aretoko Mahaiak",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = TextPrimary
                        )
                        Text(
                            text = "Zerbitzaria: ${zerbitzaria.izena} (${zerbitzaria.rola})",
                            fontSize = 12.sp,
                            color = AmberPrimary
                        )
                    }
                },
                navigationIcon = {
                    IconButton(onClick = onAtzera) {
                        Icon(
                            imageVector = Icons.AutoMirrored.Filled.ArrowBack,
                            contentDescription = "Atzera",
                            tint = TextPrimary
                        )
                    }
                },
                actions = {
                    IconButton(onClick = onEguneratu) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = "Freskatu",
                            tint = AmberPrimary
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = DarkSurface
                )
            )
        },
        containerColor = DarkBackground
    ) { innerPadding ->
        Column(
            modifier = modifier
                .fillMaxSize()
                .padding(innerPadding)
                .padding(16.dp)
        ) {
            // Eremuen botoi-filtroa (Pills)
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                eremuak.forEach { eremua ->
                    FilterChip(
                        selected = eremuFiltroa == eremua,
                        onClick = { eremuFiltroa = eremua },
                        label = { Text(eremua, fontSize = 13.sp) },
                        colors = FilterChipDefaults.filterChipColors(
                            selectedContainerColor = AmberPrimary,
                            selectedLabelColor = DarkBackground,
                            containerColor = DarkSurfaceVariant,
                            labelColor = TextPrimary
                        )
                    )
                }
            }

            Spacer(modifier = Modifier.height(16.dp))

            // Egoera Laburpena
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                EgoeraBadge("Libre", mahaiak.count { it.egoera.equals("Libre", true) }, StatusFree)
                EgoeraBadge("Okupatuta", mahaiak.count { it.egoera.equals("Okupatuta", true) }, StatusOccupied)
                EgoeraBadge("Eskaeran", mahaiak.count { it.egoera.equals("Eskaeran", true) }, StatusOrdering)
            }

            Spacer(modifier = Modifier.height(16.dp))

            // Mahaien Sarea (Responsive: Mugikorrean 2 zutabe, Tabletan 4 zutabe)
            LazyVerticalGrid(
                columns = GridCells.Adaptive(minSize = 150.dp),
                verticalArrangement = Arrangement.spacedBy(12.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp),
                modifier = Modifier.fillMaxSize()
            ) {
                items(filtratuak) { mahaia ->
                    MahaiTxartela(
                        mahaia = mahaia,
                        onClick = { onMahaiaAukeratua(mahaia) }
                    )
                }
            }
        }
    }
}

@Composable
fun MahaiTxartela(
    mahaia: Mahaia,
    onClick: () -> Unit
) {
    val egoeraKolorea = when (mahaia.egoera.lowercase()) {
        "libre" -> StatusFree
        "okupatuta" -> StatusOccupied
        "eskaeran" -> StatusOrdering
        "fakturatua" -> StatusBill
        else -> TextSecondary
    }

    Card(
        modifier = Modifier
            .fillMaxWidth()
            .height(130.dp)
            .clickable(onClick = onClick),
        shape = RoundedCornerShape(16.dp),
        colors = CardDefaults.cardColors(containerColor = DarkSurface),
        elevation = CardDefaults.cardElevation(defaultElevation = 3.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(12.dp),
            verticalArrangement = Arrangement.SpaceBetween
        ) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = "Mahaia ${mahaia.zenbakia}",
                    fontSize = 17.sp,
                    fontWeight = FontWeight.Bold,
                    color = TextPrimary
                )
                Box(
                    modifier = Modifier
                        .size(12.dp)
                        .clip(CircleShape)
                        .background(egoeraKolorea)
                )
            }

            Text(
                text = mahaia.eremua,
                fontSize = 12.sp,
                color = TextSecondary
            )

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.Bottom
            ) {
                Text(
                    text = "${mahaia.kapazitatea} lagun",
                    fontSize = 12.sp,
                    color = TextMuted
                )
                if (mahaia.guztira > 0) {
                    Text(
                        text = "%.2f €".format(mahaia.guztira),
                        fontSize = 14.sp,
                        fontWeight = FontWeight.Bold,
                        color = AmberPrimary
                    )
                } else {
                    Text(
                        text = mahaia.egoera,
                        fontSize = 12.sp,
                        fontWeight = FontWeight.Medium,
                        color = egoeraKolorea
                    )
                }
            }
        }
    }
}

@Composable
fun EgoeraBadge(etiketa: String, kopurua: Int, kolorea: Color) {
    Row(
        verticalAlignment = Alignment.CenterVertically,
        horizontalArrangement = Arrangement.spacedBy(6.dp)
    ) {
        Box(
            modifier = Modifier
                .size(10.dp)
                .clip(CircleShape)
                .background(kolorea)
        )
        Text(
            text = "$etiketa: $kopurua",
            fontSize = 13.sp,
            color = TextSecondary
        )
    }
}

@Preview(showBackground = true, widthDp = 360, heightDp = 640)
@Composable
fun MahaiakScreenPreview() {
    KomanderaTheme {
        MahaiakScreen(
            zerbitzaria = Langilea(1, "Ander", "Urien", "Zerbitzari Burua"),
            mahaiak = listOf(
                Mahaia(1, 1, "Mahaia 1", 4, "Libre", "Areto Nagusia"),
                Mahaia(2, 2, "Mahaia 2", 2, "Okupatuta", "Areto Nagusia", guztira = 34.50),
                Mahaia(3, 3, "Mahaia 3", 6, "Eskaeran", "Areto Nagusia", guztira = 78.00),
                Mahaia(4, 4, "Mahaia 4", 4, "Libre", "Terraza"),
                Mahaia(5, 5, "Mahaia 5", 4, "Fakturatua", "Terraza", guztira = 45.20)
            ),
            onMahaiaAukeratua = {},
            onAtzera = {},
            onEguneratu = {}
        )
    }
}
