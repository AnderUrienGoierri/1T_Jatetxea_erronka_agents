package eus.goierri.jatetxea.komandera.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color

private val T1DarkColorScheme = darkColorScheme(
    primary = ChiantiPrimary,
    onPrimary = TextOnBrand,
    primaryContainer = PomodoroHover,
    onPrimaryContainer = TextPrimary,
    secondary = AmberAccent,
    onSecondary = DeepBackground,
    secondaryContainer = BurgundyDark,
    onSecondaryContainer = AmberLight,
    background = DeepBackground,
    onBackground = TextPrimary,
    surface = SurfaceDark,
    onSurface = TextPrimary,
    surfaceVariant = SurfaceVariantDark,
    onSurfaceVariant = TextSecondary,
    outline = SurfaceBorder,
    error = StatusOccupied,
    onError = TextOnBrand
)

private val T1LightColorScheme = lightColorScheme(
    primary = ChiantiPrimary,
    onPrimary = TextOnBrand,
    primaryContainer = PomodoroHover,
    onPrimaryContainer = TextPrimary,
    secondary = AmberAccent,
    onSecondary = Color.White,
    secondaryContainer = AmberLight,
    onSecondaryContainer = BurgundyDark,
    background = Color(0xFFF8FAFC),      // Irina Zuria
    onBackground = Color(0xFF1E293B),    // Arbel Iluna
    surface = Color(0xFFFFFFFF),         // Zuri Garbia
    onSurface = Color(0xFF1E293B),
    surfaceVariant = Color(0xFFE2E8F0),
    onSurfaceVariant = Color(0xFF475569),
    outline = Color(0xFFCBD5E1),
    error = StatusOccupied,
    onError = TextOnBrand
)

@Composable
fun KomanderaTheme(
    darkTheme: Boolean = true,
    content: @Composable () -> Unit
) {
    val colorScheme = if (darkTheme) T1DarkColorScheme else T1LightColorScheme
    MaterialTheme(
        colorScheme = colorScheme,
        content = content
    )
}
