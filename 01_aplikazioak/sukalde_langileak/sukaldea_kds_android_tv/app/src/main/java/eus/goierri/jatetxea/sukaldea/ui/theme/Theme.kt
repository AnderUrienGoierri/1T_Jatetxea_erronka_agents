package eus.goierri.jatetxea.sukaldea.ui.theme

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color

private val T1KdsDarkColorScheme = darkColorScheme(
    primary = ChiantiDark,
    onPrimary = Color.White,
    primaryContainer = BurgundyDark,
    onPrimaryContainer = AmberLight,
    secondary = GoldBorder,
    onSecondary = EveningEspresso,
    secondaryContainer = WarmGraphite,
    onSecondaryContainer = AmberLight,
    tertiary = StatusFree,
    onTertiary = Color.White,
    background = EveningEspresso,
    onBackground = WarmLinen,
    surface = CharcoalWine,
    onSurface = WarmLinen,
    surfaceVariant = WarmGraphite,
    onSurfaceVariant = TextSecondary,
    outline = DarkHairline,
    error = StatusDangerBright,
    onError = EveningEspresso
)

@Composable
fun SukaldeaKdsTheme(content: @Composable () -> Unit) {
    MaterialTheme(
        colorScheme = T1KdsDarkColorScheme,
        typography = KdsTypography,
        content = content
    )
}
