package eus.goierri.jatetxea.komandera.ui.theme

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color

private val T1DarkColorScheme = darkColorScheme(
    primary = ChiantiDark,
    onPrimary = TextOnBrand,
    primaryContainer = BurgundyDark,
    onPrimaryContainer = AmberLight,
    secondary = GoldBorder,
    onSecondary = EveningEspresso,
    secondaryContainer = WarmGraphite,
    onSecondaryContainer = AmberLight,
    background = EveningEspresso,
    onBackground = WarmLinen,
    surface = CharcoalWine,
    onSurface = WarmLinen,
    surfaceVariant = WarmGraphite,
    onSurfaceVariant = TextSecondary,
    outline = DarkHairline,
    error = StatusOccupiedDark,
    onError = EveningEspresso
)

private val T1LightColorScheme = lightColorScheme(
    primary = ChiantiPrimary,
    onPrimary = TextOnBrand,
    primaryContainer = ChiantiTint,
    onPrimaryContainer = BurgundyDark,
    secondary = AntiqueGold,
    onSecondary = Color.White,
    secondaryContainer = GoldTint,
    onSecondaryContainer = BurgundyDark,
    background = WarmLinen,
    onBackground = EspressoInk,
    surface = MarbleWhite,
    onSurface = EspressoInk,
    surfaceVariant = StoneMist,
    onSurfaceVariant = WarmSlate,
    outline = WarmTaupe,
    error = StatusOccupied,
    onError = TextOnBrand
)

@Composable
fun KomanderaTheme(
    darkTheme: Boolean = false,
    content: @Composable () -> Unit
) {
    val colorScheme = if (darkTheme) T1DarkColorScheme else T1LightColorScheme
    MaterialTheme(
        colorScheme = colorScheme,
        typography = KomanderaTypography,
        content = content
    )
}
