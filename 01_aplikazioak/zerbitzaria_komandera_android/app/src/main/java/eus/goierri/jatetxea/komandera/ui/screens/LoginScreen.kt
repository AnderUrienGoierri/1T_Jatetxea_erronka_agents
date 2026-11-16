package eus.goierri.jatetxea.komandera.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Restaurant
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.komandera.data.model.Langilea
import eus.goierri.jatetxea.komandera.ui.theme.*

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun LoginScreen(
    langileak: List<Langilea>,
    onLangileaAukeratua: (Langilea) -> Unit,
    modifier: Modifier = Modifier
) {
    var hautatuaId by remember { mutableStateOf<Int?>(null) }

    Column(
        modifier = modifier
            .fillMaxSize()
            .background(DarkBackground)
            .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Spacer(modifier = Modifier.height(32.dp))

        // Goiburuko Ikonoa eta Izenburua
        Box(
            modifier = Modifier
                .size(72.dp)
                .clip(CircleShape)
                .background(AmberPrimary.copy(alpha = 0.2f)),
            contentAlignment = Alignment.Center
        ) {
            Icon(
                imageVector = Icons.Default.Restaurant,
                contentDescription = "Logo",
                tint = AmberPrimary,
                modifier = Modifier.size(40.dp)
            )
        }

        Spacer(modifier = Modifier.height(16.dp))

        Text(
            text = "1T JATETXEA",
            fontSize = 28.sp,
            fontWeight = FontWeight.Bold,
            color = AmberPrimary,
            letterSpacing = 2.sp
        )

        Text(
            text = "Zerbitzarien Komandera Mugikorra",
            fontSize = 15.sp,
            color = TextSecondary,
            textAlign = TextAlign.Center
        )

        Spacer(modifier = Modifier.height(32.dp))

        Card(
            modifier = Modifier.fillMaxWidth(),
            colors = CardDefaults.cardColors(containerColor = DarkSurface),
            shape = RoundedCornerShape(16.dp)
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(
                    text = "Aukeratu zure zerbitzari-rola:",
                    fontSize = 16.sp,
                    fontWeight = FontWeight.SemiBold,
                    color = TextPrimary
                )

                Spacer(modifier = Modifier.height(12.dp))

                LazyColumn(
                    modifier = Modifier.heightIn(max = 300.dp),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    items(langileak) { langilea ->
                        LangileElementua(
                            langilea = langilea,
                            hautatua = langilea.id == hautatuaId,
                            onClick = { hautatuaId = langilea.id }
                        )
                    }
                }
            }
        }

        Spacer(modifier = Modifier.weight(1f))

        Button(
            onClick = {
                val aukeratua = langileak.find { it.id == hautatuaId }
                if (aukeratua != null) {
                    onLangileaAukeratua(aukeratua)
                }
            },
            enabled = hautatuaId != null,
            modifier = Modifier
                .fillMaxWidth()
                .height(56.dp),
            colors = ButtonDefaults.buttonColors(
                containerColor = AmberPrimary,
                disabledContainerColor = DarkSurfaceVariant
            ),
            shape = RoundedCornerShape(12.dp)
        ) {
            Text(
                text = "Hasi Txanda eta Ireki Mahaiak",
                fontSize = 16.sp,
                fontWeight = FontWeight.Bold,
                color = if (hautatuaId != null) DarkBackground else TextMuted
            )
        }

        Spacer(modifier = Modifier.height(16.dp))
    }
}

@Composable
fun LangileElementua(
    langilea: Langilea,
    hautatua: Boolean,
    onClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable(onClick = onClick),
        colors = CardDefaults.cardColors(
            containerColor = if (hautatua) AmberPrimary.copy(alpha = 0.15f) else DarkSurfaceVariant
        ),
        shape = RoundedCornerShape(12.dp),
        border = if (hautatua) CardDefaults.outlinedCardBorder().copy(width = 2.dp) else null
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(14.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Box(
                modifier = Modifier
                    .size(44.dp)
                    .clip(CircleShape)
                    .background(if (hautatua) AmberPrimary else DarkSurface),
                contentAlignment = Alignment.Center
            ) {
                Icon(
                    imageVector = Icons.Default.Person,
                    contentDescription = null,
                    tint = if (hautatua) DarkBackground else TextSecondary,
                    modifier = Modifier.size(24.dp)
                )
            }

            Spacer(modifier = Modifier.width(16.dp))

            Column {
                Text(
                    text = "${langilea.izena} ${langilea.abizena}".trim(),
                    fontSize = 16.sp,
                    fontWeight = FontWeight.Medium,
                    color = if (hautatua) AmberPrimary else TextPrimary
                )
                Text(
                    text = langilea.rola,
                    fontSize = 13.sp,
                    color = TextSecondary
                )
            }
        }
    }
}

@Preview(showBackground = true, widthDp = 360, heightDp = 640)
@Composable
fun LoginScreenPreview() {
    KomanderaTheme {
        LoginScreen(
            langileak = listOf(
                Langilea(1, "Ander", "Urien", "Zerbitzari Burua"),
                Langilea(2, "Mikel", "Goikoetxea", "Zerbitzaria"),
                Langilea(3, "Ane", "Zubizarreta", "Zerbitzaria"),
                Langilea(4, "Julen", "Arrieta", "Barra / Edariak")
            ),
            onLangileaAukeratua = {}
        )
    }
}
