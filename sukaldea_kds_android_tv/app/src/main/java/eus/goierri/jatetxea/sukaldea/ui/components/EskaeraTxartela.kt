package eus.goierri.jatetxea.sukaldea.ui.components

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.focusable
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
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Divider
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.focus.onFocusChanged
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.sukaldea.data.model.Eskaera
import eus.goierri.jatetxea.sukaldea.data.model.EskaeraLerroa
import eus.goierri.jatetxea.sukaldea.ui.theme.AmberDark
import eus.goierri.jatetxea.sukaldea.ui.theme.AmberLight
import eus.goierri.jatetxea.sukaldea.ui.theme.AmberWarning
import eus.goierri.jatetxea.sukaldea.ui.theme.CardBackground
import eus.goierri.jatetxea.sukaldea.ui.theme.CardBorder
import eus.goierri.jatetxea.sukaldea.ui.theme.DarkSurface
import eus.goierri.jatetxea.sukaldea.ui.theme.EmeraldDark
import eus.goierri.jatetxea.sukaldea.ui.theme.EmeraldLight
import eus.goierri.jatetxea.sukaldea.ui.theme.EmeraldSuccess
import eus.goierri.jatetxea.sukaldea.ui.theme.RoseDark
import eus.goierri.jatetxea.sukaldea.ui.theme.RosePrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.SkyInfo
import eus.goierri.jatetxea.sukaldea.ui.theme.TextMuted
import eus.goierri.jatetxea.sukaldea.ui.theme.TextPrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.TextSecondary

@Composable
fun EskaeraTxartela(
    eskaera: Eskaera,
    onMarkatuLerroa: (Int) -> Unit,
    onMarkatuEskaera: (Int) -> Unit
) {
    var isFocused by remember { mutableStateOf(false) }

    val isGuztiakEginda = eskaera.lerroak.isNotEmpty() && eskaera.lerroak.all { it.isEginda }
    val borderColor = when {
        isFocused -> RosePrimary
        isGuztiakEginda -> EmeraldSuccess
        else -> CardBorder
    }

    Box(
        modifier = Modifier
            .width(340.dp)
            .clip(RoundedCornerShape(14.dp))
            .background(CardBackground)
            .border(
                width = if (isFocused) 2.5.dp else 1.dp,
                color = borderColor,
                shape = RoundedCornerShape(14.dp)
            )
            .onFocusChanged { isFocused = it.isFocused }
            .focusable()
            .padding(14.dp)
    ) {
        Column(modifier = Modifier.fillMaxWidth()) {

            // TXARTELAREN GOIBURUA: Mahaia, Eskaera ID eta Egoera
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Box(
                        modifier = Modifier
                            .clip(RoundedCornerShape(6.dp))
                            .background(RoseDark)
                            .padding(horizontal = 8.dp, vertical = 4.dp)
                    ) {
                        Text(
                            text = "#${eskaera.id}",
                            color = Color.White,
                            fontWeight = FontWeight.Bold,
                            fontSize = 13.sp,
                            fontFamily = FontFamily.Monospace
                        )
                    }
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = eskaera.mahaiaErakutsi,
                        color = TextPrimary,
                        fontSize = 16.sp,
                        fontWeight = FontWeight.ExtraBold
                    )
                }

                // Egoera Txipa
                val badgeBg = if (isGuztiakEginda) EmeraldDark else AmberDark
                val badgeText = if (isGuztiakEginda) EmeraldLight else AmberLight
                Box(
                    modifier = Modifier
                        .clip(RoundedCornerShape(6.dp))
                        .background(badgeBg)
                        .padding(horizontal = 8.dp, vertical = 3.dp)
                ) {
                    Text(
                        text = if (isGuztiakEginda) "PRESTATUA" else eskaera.egoera.uppercase(),
                        color = badgeText,
                        fontSize = 11.sp,
                        fontWeight = FontWeight.Bold
                    )
                }
            }

            Spacer(modifier = Modifier.height(8.dp))

            // Datu Lagungarriak: Zerbitzaria eta Ordua
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text(
                    text = "Zerbitzaria: ${eskaera.langileaErakutsi}",
                    color = TextSecondary,
                    fontSize = 11.sp
                )
                Text(
                    text = eskaera.orduaErakutsi,
                    color = TextMuted,
                    fontSize = 11.sp,
                    fontFamily = FontFamily.Monospace
                )
            }

            Spacer(modifier = Modifier.height(10.dp))
            Divider(color = Color(0xFF334155), thickness = 0.8.dp)
            Spacer(modifier = Modifier.height(10.dp))

            // PLATEREN ZERRENDA
            Column(
                modifier = Modifier.fillMaxWidth(),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                if (eskaera.lerroak.isEmpty()) {
                    Text(
                        text = "Ez dago platerik erregistratuta.",
                        color = TextMuted,
                        fontSize = 12.sp
                    )
                } else {
                    eskaera.lerroak.forEach { lerroa ->
                        PlaterLerroItem(
                            lerroa = lerroa,
                            onEgindaKlik = { onMarkatuLerroa(lerroa.id) }
                        )
                    }
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            // BEHEKO BOTOIA: Eskaera Osoa Prestatuta Jarri
            Button(
                onClick = { onMarkatuEskaera(eskaera.id) },
                enabled = !isGuztiakEginda,
                colors = ButtonDefaults.buttonColors(
                    containerColor = if (isGuztiakEginda) Color(0xFF334155) else EmeraldSuccess,
                    disabledContainerColor = Color(0xFF1E293B)
                ),
                shape = RoundedCornerShape(8.dp),
                modifier = Modifier.fillMaxWidth()
            ) {
                Text(
                    text = if (isGuztiakEginda) "Eskaera Prestatuta Dago" else "Eskaera Osoa Eginda",
                    fontSize = 12.sp,
                    fontWeight = FontWeight.Bold,
                    color = if (isGuztiakEginda) TextMuted else Color.White
                )
            }
        }
    }
}

@Composable
fun PlaterLerroItem(
    lerroa: EskaeraLerroa,
    onEgindaKlik: () -> Unit
) {
    val isEginda = lerroa.isEginda
    val bg = if (isEginda) DarkSurface.copy(alpha = 0.5f) else DarkSurface

    Row(
        modifier = Modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(8.dp))
            .background(bg)
            .border(
                1.dp,
                if (isEginda) EmeraldDark else Color(0xFF334155),
                RoundedCornerShape(8.dp)
            )
            .padding(8.dp),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        Row(
            modifier = Modifier.weight(1f),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Kantitate zirkulua
            Box(
                modifier = Modifier
                    .size(24.dp)
                    .clip(RoundedCornerShape(6.dp))
                    .background(if (isEginda) EmeraldDark else RoseDark),
                contentAlignment = Alignment.Center
            ) {
                Text(
                    text = "${lerroa.kantitatea}",
                    color = Color.White,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp
                )
            }

            Spacer(modifier = Modifier.width(8.dp))

            Column {
                Text(
                    text = lerroa.platerIzena,
                    color = if (isEginda) TextMuted else TextPrimary,
                    fontSize = 13.sp,
                    fontWeight = FontWeight.SemiBold,
                    maxLines = 2,
                    overflow = TextOverflow.Ellipsis
                )
                if (!lerroa.oharrak.isNullOrBlank()) {
                    Text(
                        text = "Oharra: ${lerroa.oharrak}",
                        color = AmberWarning,
                        fontSize = 11.sp,
                        fontWeight = FontWeight.Medium
                    )
                }
            }
        }

        Spacer(modifier = Modifier.width(8.dp))

        // Ekintza Botoia
        if (isEginda) {
            Box(
                modifier = Modifier
                    .clip(RoundedCornerShape(6.dp))
                    .background(EmeraldDark)
                    .padding(horizontal = 8.dp, vertical = 4.dp)
            ) {
                Text(
                    text = "Eginda",
                    color = EmeraldLight,
                    fontSize = 11.sp,
                    fontWeight = FontWeight.Bold
                )
            }
        } else {
            Button(
                onClick = onEgindaKlik,
                colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF1E293B)),
                shape = RoundedCornerShape(6.dp),
                modifier = Modifier.border(1.dp, RoseDark, RoundedCornerShape(6.dp))
            ) {
                Text(
                    text = "Eginda",
                    color = RoseLight,
                    fontSize = 11.sp,
                    fontWeight = FontWeight.Bold
                )
            }
        }
    }
}
