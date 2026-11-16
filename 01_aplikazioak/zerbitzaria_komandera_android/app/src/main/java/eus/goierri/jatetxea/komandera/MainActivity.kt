package eus.goierri.jatetxea.komandera

import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Surface
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import eus.goierri.jatetxea.komandera.data.model.LangileaDto
import eus.goierri.jatetxea.komandera.ui.navigation.Screen
import eus.goierri.jatetxea.komandera.ui.screens.EskaeraScreen
import eus.goierri.jatetxea.komandera.ui.screens.LoginScreen
import eus.goierri.jatetxea.komandera.ui.screens.MahaiakScreen
import eus.goierri.jatetxea.komandera.ui.screens.MahaiaXehetasunakScreen
import eus.goierri.jatetxea.komandera.ui.theme.DarkBackground
import eus.goierri.jatetxea.komandera.ui.theme.KomanderaTheme

class MainActivity : ComponentActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            KomanderaTheme {
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = DarkBackground
                ) {
                    KomanderaApp(
                        onMezuaBistaratu = { mezua ->
                            Toast.makeText(this@MainActivity, mezua, Toast.LENGTH_SHORT).show()
                        }
                    )
                }
            }
        }
    }
}

@Composable
fun KomanderaApp(onMezuaBistaratu: (String) -> Unit) {
    val navController = rememberNavController()

    // Uneko zerbitzariaren saio egoera (PIN bidez egiaztatua)
    var unekoZerbitzaria by remember { mutableStateOf<LangileaDto?>(null) }

    NavHost(
        navController = navController,
        startDestination = Screen.Login.route
    ) {
        // 1. PANTAILA: LOGIN / PIN BIDEZKO SAIO-HASIERA (Zerbitzaria / Barrakoa)
        composable(Screen.Login.route) {
            LoginScreen(
                onLoginSuccess = { langilea ->
                    unekoZerbitzaria = langilea
                    onMezuaBistaratu("Ongi etorri, ${langilea.izena} (${langilea.rola})!")
                    navController.navigate(Screen.Mahaiak.route) {
                        popUpTo(Screen.Login.route) { inclusive = true }
                    }
                }
            )
        }

        // 2. PANTAILA: MAHAIEN MAPA ETA EGOERA (Libre, Okupatuta, Garbitzen...)
        composable(Screen.Mahaiak.route) {
            val zerbitzaria = unekoZerbitzaria ?: LangileaDto(id = 1, izena = "Zerbitzaria", rola = "Zerbitzaria")
            MahaiakScreen(
                zerbitzaria = zerbitzaria,
                onMahaiaAukeratua = { mahaia ->
                    navController.navigate(Screen.MahaiaXehetasunak.sortuRuta(mahaia.id))
                },
                onSaioaItxi = {
                    unekoZerbitzaria = null
                    onMezuaBistaratu("Saioa itxi da.")
                    navController.navigate(Screen.Login.route) {
                        popUpTo(Screen.Mahaiak.route) { inclusive = true }
                    }
                }
            )
        }

        // 3. PANTAILA: MAHAIAREN XEHETASUNAK (Eskaerak, egoerak, sukaldera bidali, ordainketa/kobratu)
        composable(
            route = Screen.MahaiaXehetasunak.route,
            arguments = listOf(navArgument("mahaiId") { type = NavType.IntType })
        ) { backStackEntry ->
            val mahaiId = backStackEntry.arguments?.getInt("mahaiId") ?: 1
            val zerbitzaria = unekoZerbitzaria ?: LangileaDto(id = 1, izena = "Zerbitzaria", rola = "Zerbitzaria")

            MahaiaXehetasunakScreen(
                mahaiId = mahaiId,
                zerbitzaria = zerbitzaria,
                onGehituPlaterak = { eskaeraId, mid ->
                    navController.navigate(Screen.Eskaera.sortuRuta(eskaeraId, mid))
                },
                onAtzera = {
                    navController.popBackStack()
                }
            )
        }

        // 4. PANTAILA: PLATER KATALOGOA ETA ESKAERA LERROAK GEHITZEA
        composable(
            route = Screen.Eskaera.route,
            arguments = listOf(
                navArgument("eskaeraId") { type = NavType.IntType },
                navArgument("mahaiId") { type = NavType.IntType }
            )
        ) { backStackEntry ->
            val eskaeraId = backStackEntry.arguments?.getInt("eskaeraId") ?: 1
            val mahaiId = backStackEntry.arguments?.getInt("mahaiId") ?: 1

            EskaeraScreen(
                eskaeraId = eskaeraId,
                mahaiId = mahaiId,
                onAtzera = {
                    navController.popBackStack()
                }
            )
        }
    }
}
