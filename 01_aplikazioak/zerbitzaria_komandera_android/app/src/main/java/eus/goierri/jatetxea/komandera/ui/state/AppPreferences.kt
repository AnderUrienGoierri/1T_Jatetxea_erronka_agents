package eus.goierri.jatetxea.komandera.ui.state

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue

object AppPreferences {
    var isDarkTheme by mutableStateOf(true)
    var hizkuntza by mutableStateOf("EU") // "EU", "ES", "EN"
}
