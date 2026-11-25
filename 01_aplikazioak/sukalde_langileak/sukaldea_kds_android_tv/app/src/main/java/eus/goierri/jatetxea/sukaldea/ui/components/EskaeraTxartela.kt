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
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import eus.goierri.jatetxea.sukaldea.data.model.Eskaera
import eus.goierri.jatetxea.sukaldea.data.model.EskaeraLerroa
import eus.goierri.jatetxea.sukaldea.ui.theme.AntiqueGold
import eus.goierri.jatetxea.sukaldea.ui.theme.CharcoalWine
import eus.goierri.jatetxea.sukaldea.ui.theme.ChiantiDark
import eus.goierri.jatetxea.sukaldea.ui.theme.ChiantiPrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.DarkHairline
import eus.goierri.jatetxea.sukaldea.ui.theme.EveningEspresso
import eus.goierri.jatetxea.sukaldea.ui.theme.GoldBorder
import eus.goierri.jatetxea.sukaldea.ui.theme.GoldTintDark
import eus.goierri.jatetxea.sukaldea.ui.theme.JetBrainsMono
import eus.goierri.jatetxea.sukaldea.ui.theme.PlusJakartaSans
import eus.goierri.jatetxea.sukaldea.ui.theme.RoseLight
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusFree
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusFreeBright
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusFreeDark
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusInfo
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusInfoBright
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusInfoDark
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusPendingBright
import eus.goierri.jatetxea.sukaldea.ui.theme.StatusPendingDark
import eus.goierri.jatetxea.sukaldea.ui.theme.TextMuted
import eus.goierri.jatetxea.sukaldea.ui.theme.TextPrimary
import eus.goierri.jatetxea.sukaldea.ui.theme.TextSecondary
import eus.goierri.jatetxea.sukaldea.ui.theme.WarmGraphite

@Composable
fun EskaeraTxartela(
    eskaera: Eskaera,
    isHistorialMode: Boolean = false,
    onMarkatuLerroa: (Int) -> Unit,
    onMarkatuEskaera: (Int) -> Unit,
    onMarkatuZerbitzatua: (Int) -> Unit = {},
    onBerreskuratu: (Int) -> Unit = {}
) {
    var isFocused by remember { mutableStateOf(false) }

    val isKobratua = eskaera.egoera.equals("Kobratua", ignoreCase = true)
    val isGuztiakEginda = eskaera.egoera.equals("Prestatua", ignoreCase = true) ||
        (eskaera.lerroak.isNotEmpty() && eskaera.lerroak.all { it.isEginda })

    val borderColor = when {
        isFocused -> GoldBorder
        isHistorialMode -> if (isKobratua) StatusFree else StatusInfo
        isGuztiakEginda -> StatusFree
        else -> DarkHairline
    }

    Box(
        modifier = Modifier
            .width(356.dp)
            .clip(RoundedCornerShape(14.dp))
            .background(CharcoalWine)
            .border(
                width = if (isFocused) 2.5.dp else 1.dp,
                color = borderColor,
                shape = RoundedCornerShape(14.dp)
            )
            .onFocusChanged { isFocused = it.isFocused }
            .focusable()
            .padding(16.dp)
    ) {
        Column(modifier = Modifier.fillMaxWidth()) {

            // TXARTELAREN GOIBURUA: ID, Mahaia eta Egoera
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Box(
                        modifier = Modifier
                            .clip(RoundedCornerShape(6.dp))
                            .background(ChiantiDark)
                            .padding(horizontal = 8.dp, vertical = 4.dp)
                    ) {
                        Text(
                            text = "#${eskaera.id}",
                            color = Color.White,
                            fontWeight = FontWeight.Bold,
                            fontSize = 13.sp,
                            fontFamily = JetBrainsMono
                        )
                    }
                    Spacer(modifier = Modifier.width(10.dp))
                    Text(
                        text = eskaera.mahaiaErakutsi,
                        color = TextPrimary,
                        fontSize = 17.sp,
                        fontFamily = PlusJakartaSans,
                        fontWeight = FontWeight.ExtraBold
                    )
                }

                // Egoera Txipa
                val (badgeBg, badgeText, badgeLabel) = when {
                    isHistorialMode && isKobratua -> Triple(StatusFreeDark, StatusFreeBright, "KOBRATUA")
                    isHistorialMode -> Triple(StatusInfoDark, StatusInfoBright, "ZERBITZATUA")
                    isGuztiakEginda -> Triple(StatusFreeDark, StatusFreeBright, "PRESTATUA")
                    else -> Triple(StatusPendingDark, StatusPendingBright, eskaera.egoera.uppercase())
                }
                Box(
                    modifier = Modifier
                        .clip(RoundedCornerShape(6.dp))
                        .background(badgeBg)
                        .padding(horizontal = 8.dp, vertical = 4.dp)
                ) {
                    Text(
                        text = badgeLabel,
                        color = badgeText,
                        fontSize = 10.sp,
                        fontFamily = JetBrainsMono,
                        fontWeight = FontWeight.Bold
                    )
                }
            }

            Spacer(modifier = Modifier.height(8.dp))

            // Datu Lagungarriak: Zerbitzaria eta Ordua
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = "Zerbitzaria: ${eskaera.langileaErakutsi}",
                    color = TextSecondary,
                    fontSize = 12.sp,
                    fontFamily = PlusJakartaSans
                )
                Box(
                    modifier = Modifier
                        .clip(RoundedCornerShape(6.dp))
                        .background(GoldTintDark)
                        .padding(horizontal = 7.dp, vertical = 2.dp)
                ) {
                    Text(
                        text = eskaera.orduaErakutsi.ifBlank { "--:--" },
                        color = GoldBorder,
                        fontSize = 11.sp,
                        fontFamily = JetBrainsMono,
                        fontWeight = FontWeight.Bold
                    )
                }
            }

            Spacer(modifier = Modifier.height(10.dp))
            Divider(color = DarkHairline, thickness = 1.dp)
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
                            isReadOnly = isHistorialMode,
                            onEgindaKlik = { onMarkatuLerroa(lerroa.id) }
                        )
                    }
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            // BEHEKO EKINTZA BOTOIA
            if (isHistorialMode) {
                if (!isKobratua) {
                    Button(
                        onClick = { onBerreskuratu(eskaera.id) },
                        colors = ButtonDefaults.buttonColors(containerColor = AntiqueGold),
                        shape = RoundedCornerShape(10.dp),
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text(
                            text = "Berreskuratu pantailara",
                            fontSize = 12.sp,
                            fontFamily = PlusJakartaSans,
                            fontWeight = FontWeight.Bold,
                            color = Color.White
                        )
                    }
                }
            } else if (isGuztiakEginda) {
                Button(
                    onClick = { onMarkatuZerbitzatua(eskaera.id) },
                    colors = ButtonDefaults.buttonColors(containerColor = StatusInfo),
                    shape = RoundedCornerShape(10.dp),
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Text(
                        text = "✓ Aterata (Historialera)",
                        fontSize = 12.sp,
                        fontFamily = PlusJakartaSans,
                        fontWeight = FontWeight.Bold,
                        color = Color.White
                    )
                }
            } else {
                Button(
                    onClick = { onMarkatuEskaera(eskaera.id) },
                    colors = ButtonDefaults.buttonColors(containerColor = StatusFree),
                    shape = RoundedCornerShape(10.dp),
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Text(
                        text = "Eskaera Osoa Eginda",
                        fontSize = 12.sp,
                        fontFamily = PlusJakartaSans,
                        fontWeight = FontWeight.Bold,
                        color = Color.White
                    )
                }
            }
        }
    }
}

@Composable
fun PlaterLerroItem(
    lerroa: EskaeraLerroa,
    isReadOnly: Boolean = false,
    onEgindaKlik: () -> Unit
) {
    val isEginda = lerroa.isEginda || isReadOnly
    val bg = if (isEginda) EveningEspresso.copy(alpha = 0.65f) else WarmGraphite

    Row(
        modifier = Modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(10.dp))
            .background(bg)
            .border(
                1.dp,
                if (isEginda) StatusFree else DarkHairline,
                RoundedCornerShape(10.dp)
            )
            .padding(10.dp),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        Row(
            modifier = Modifier.weight(1f),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Box(
                modifier = Modifier
                    .size(26.dp)
                    .clip(RoundedCornerShape(6.dp))
                    .background(if (isEginda) StatusFree else ChiantiPrimary),
                contentAlignment = Alignment.Center
            ) {
                Text(
                    text = "${lerroa.kantitatea}x",
                    color = Color.White,
                    fontFamily = JetBrainsMono,
                    fontWeight = FontWeight.Bold,
                    fontSize = 11.sp
                )
            }

            Spacer(modifier = Modifier.width(10.dp))

            Column {
                Text(
                    text = lerroa.platerIzena,
                    color = if (isEginda) TextSecondary else TextPrimary,
                    fontSize = 14.sp,
                    fontFamily = PlusJakartaSans,
                    fontWeight = FontWeight.SemiBold,
                    maxLines = 2,
                    overflow = TextOverflow.Ellipsis
                )
                if (!lerroa.oharrak.isNullOrBlank()) {
                    Spacer(modifier = Modifier.height(3.dp))
                    Box(
                        modifier = Modifier
                            .clip(RoundedCornerShape(5.dp))
                            .background(StatusPendingDark)
                            .padding(horizontal = 6.dp, vertical = 2.dp)
                    ) {
                        Text(
                            text = "Oharra: ${lerroa.oharrak}",
                            color = StatusPendingBright,
                            fontSize = 11.sp,
                            fontFamily = PlusJakartaSans,
                            fontWeight = FontWeight.SemiBold
                        )
                    }
                }
            }
        }

        Spacer(modifier = Modifier.width(8.dp))

        if (isEginda) {
            Box(
                modifier = Modifier
                    .clip(RoundedCornerShape(6.dp))
                    .background(StatusFreeDark)
                    .padding(horizontal = 8.dp, vertical = 4.dp)
            ) {
                Text(
                    text = "✓ Eginda",
                    color = StatusFreeBright,
                    fontSize = 11.sp,
                    fontFamily = PlusJakartaSans,
                    fontWeight = FontWeight.Bold
                )
            }
        } else {
            Button(
                onClick = onEgindaKlik,
                colors = ButtonDefaults.buttonColors(containerColor = CharcoalWine),
                shape = RoundedCornerShape(8.dp),
                modifier = Modifier.border(1.dp, ChiantiDark, RoundedCornerShape(8.dp))
            ) {
                Text(
                    text = "Eginda",
                    color = RoseLight,
                    fontSize = 11.sp,
                    fontFamily = PlusJakartaSans,
                    fontWeight = FontWeight.Bold
                )
            }
        }
    }
}
