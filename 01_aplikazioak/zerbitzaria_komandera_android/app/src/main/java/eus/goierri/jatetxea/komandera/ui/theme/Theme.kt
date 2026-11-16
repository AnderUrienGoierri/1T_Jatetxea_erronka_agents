package eus.goierri.jatetxea.komandera.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.runtime.Composable

private val DarkColorScheme = darkColorScheme(
    primary = AmberPrimary,
    onPrimary = DarkBackground,
    primaryContainer = AmberDark,
    onPrimaryContainer = TextPrimary,
    secondary = AmberLight,
    onSecondary = DarkBackground,
    background = DarkBackground,
    onBackground = TextPrimary,
    surface = DarkSurface,
    onSurface = TextPrimary,
    surfaceVariant = DarkSurfaceVariant,
    onSurfaceVariant = TextSecondary
)

@Composable
fun KomanderaTheme(
    @Suppress("UNUSED_PARAMETER") darkTheme: Boolean = true, // Lehenetsi tema iluna jatetxeko ingurunerako
    content: @Composable () -> Unit
) {
    MaterialTheme(
        colorScheme = DarkColorScheme,
        content = content
    )
}
