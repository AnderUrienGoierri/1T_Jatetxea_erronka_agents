package eus.goierri.jatetxea.komandera.ui.state

import android.content.Context
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue

object AppPreferences {
    private const val PREFS_NAME = "komandera_prefs"
    private const val KEY_DARK_THEME = "pref_dark_theme"
    private const val KEY_HIZKUNTZA = "pref_hizkuntza"

    var isDarkTheme by mutableStateOf(false)
        private set

    var hizkuntza by mutableStateOf("EU") // "EU", "ES", "EN"
        private set

    fun init(context: Context) {
        val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        isDarkTheme = prefs.getBoolean(KEY_DARK_THEME, false)
        hizkuntza = prefs.getString(KEY_HIZKUNTZA, "EU") ?: "EU"
    }

    fun setDarkTheme(context: Context, enabled: Boolean) {
        isDarkTheme = enabled
        context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
            .edit()
            .putBoolean(KEY_DARK_THEME, enabled)
            .apply()
    }

    fun setHizkuntza(context: Context, kod: String) {
        hizkuntza = kod
        context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
            .edit()
            .putString(KEY_HIZKUNTZA, kod)
            .apply()
    }

    /** Testu eleaniztuna (EU / ES / EN) Compose-ren egoera erreaktiboarekin. */
    fun tr(eu: String, es: String, en: String): String = when (hizkuntza) {
        "ES" -> es
        "EN" -> en
        else -> eu
    }
}
