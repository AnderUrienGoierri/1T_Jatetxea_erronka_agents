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
import eus.goierri.jatetxea.komandera.data.api.ApiClient
import eus.goierri.jatetxea.komandera.data.model.Eskaera
import eus.goierri.jatetxea.komandera.data.model.Langilea
import eus.goierri.jatetxea.komandera.data.model.Mahaia
import eus.goierri.jatetxea.komandera.data.model.Platera
import eus.goierri.jatetxea.komandera.ui.navigation.Screen
import eus.goierri.jatetxea.komandera.ui.screens.EskaeraScreen
import eus.goierri.jatetxea.komandera.ui.screens.LoginScreen
import eus.goierri.jatetxea.komandera.ui.screens.MahaiakScreen
import eus.goierri.jatetxea.komandera.ui.theme.DarkBackground
import eus.goierri.jatetxea.komandera.ui.theme.KomanderaTheme
import kotlinx.coroutines.launch

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
    val scope = rememberCoroutineScope()

    // Oinarrizko datuen egoera (remember)
    var unekoZerbitzaria by remember { mutableStateOf<Langilea?>(null) }
    var mahaiak by remember {
        mutableStateOf(
            listOf(
                Mahaia(1, 1, "Mahaia 1", 4, "Libre", "Areto Nagusia"),
                Mahaia(2, 2, "Mahaia 2", 2, "Okupatuta", "Areto Nagusia", guztira = 42.50),
                Mahaia(3, 3, "Mahaia 3", 6, "Eskaeran", "Areto Nagusia", guztira = 95.00),
                Mahaia(4, 4, "Mahaia 4", 4, "Libre", "Terraza"),
                Mahaia(5, 5, "Mahaia 5", 4, "Fakturatua", "Terraza", guztira = 58.20),
                Mahaia(6, 6, "Mahaia 6", 8, "Libre", "VIP Gunea")
            )
        )
    }

    val platerKatalogoa = remember {
        listOf(
            Platera(1, "Iberiko Urdaiazpikoa", "Guijuelo ezkurrezkoa ogi txigortuarekin", 18.50, "Hasierakoak"),
            Platera(2, "Ohol Gazta Aukeratuak", "Idiazabal, Erronkari eta Aralar", 14.00, "Hasierakoak"),
            Platera(3, "Otsoko Txuleta Errea", "1kg Euskal Okela parrillan", 42.00, "Plater Nagusiak"),
            Platera(4, "Bakailaoa Bizkaitar Erara", "Txorizero piper saltsan", 22.50, "Plater Nagusiak"),
            Platera(5, "Legatza Koskera Erara", "Legatza, zainzuriak eta txirlak", 24.00, "Plater Nagusiak"),
            Platera(6, "Goxua", "Etxeko errezeta krema eta karameluarekin", 6.50, "Postreak"),
            Platera(7, "Etxeko Gazta Tarta", "Labean errea fruitu gorriekin", 7.00, "Postreak"),
            Platera(8, "Errioxa Ardoa", "Krianza Haro 2021", 16.00, "Edariak"),
            Platera(9, "Sagardo Naturala", "Gipuzkoako botila", 5.00, "Edariak"),
            Platera(10, "Ura / Errefreskoa", "33cl", 2.50, "Edariak")
        )
    }

    val langileZerrenda = remember {
        listOf(
            Langilea(1, "Ander", "Urien", "Zerbitzari Burua"),
            Langilea(2, "Mikel", "Goikoetxea", "Zerbitzaria"),
            Langilea(3, "Ane", "Zubizarreta", "Zerbitzaria"),
            Langilea(4, "Julen", "Arrieta", "Barra / Edariak")
        )
    }

    NavHost(
        navController = navController,
        startDestination = Screen.Login.route
    ) {
        // 1. PANTAILA: LOGIN / ZERBITZARIA AUKERATU
        composable(Screen.Login.route) {
            LoginScreen(
                langileak = langileZerrenda,
                onLangileaAukeratua = { langilea ->
                    unekoZerbitzaria = langilea
                    onMezuaBistaratu("Ongi etorri, ${langilea.izena}!")
                    navController.navigate(Screen.Mahaiak.route)
                }
            )
        }

        // 2. PANTAILA: MAHAIEN MAPA ETA EGOERA
        composable(Screen.Mahaiak.route) {
            val zerbitzaria = unekoZerbitzaria ?: langileZerrenda.first()
            MahaiakScreen(
                zerbitzaria = zerbitzaria,
                mahaiak = mahaiak,
                onMahaiaAukeratua = { mahaia ->
                    navController.navigate(Screen.Eskaera.sortuRuta(mahaia.id))
                },
                onAtzera = {
                    navController.popBackStack()
                },
                onEguneratu = {
                    onMezuaBistaratu("Mahaien egoerak eguneratuta.")
                }
            )
        }

        // 3. PANTAILA: ESKAERA BERRIA / KOMANDA SORTZEA
        composable(
            route = Screen.Eskaera.route,
            arguments = listOf(navArgument("mahaiId") { type = NavType.IntType })
        ) { backStackEntry ->
            val mahaiId = backStackEntry.arguments?.getInt("mahaiId") ?: 1
            val mahaia = mahaiak.find { it.id == mahaiId } ?: mahaiak.first()

            EskaeraScreen(
                mahaia = mahaia,
                platerak = platerKatalogoa,
                onAtzera = {
                    navController.popBackStack()
                },
                onEskaeraBidalita = { lerroak ->
                    val eskaeraBerria = Eskaera(
                        mahaiId = mahaia.id,
                        mahaiZenbakia = mahaia.zenbakia,
                        zerbitzariId = unekoZerbitzaria?.id ?: 1,
                        zerbitzariIzena = unekoZerbitzaria?.izena ?: "Zerbitzaria",
                        lerroak = lerroak
                    )

                    // Mahaiaren egoera eguneratu lokalki
                    mahaiak = mahaiak.map {
                        if (it.id == mahaia.id) {
                            it.copy(
                                egoera = "Eskaeran",
                                guztira = it.guztira + eskaeraBerria.guztira
                            )
                        } else it
                    }

                    // REST APIra deia egiten saiatu (Aukerako Komunikazio Erronka)
                    scope.launch {
                        try {
                            val api = ApiClient.createService()
                            val response = api.sortuEskaera(eskaeraBerria)
                            if (response.isSuccessful) {
                                onMezuaBistaratu("Komanda arrakastaz bidali da sukaldera (REST API)!")
                            } else {
                                onMezuaBistaratu("Komanda lokalki erregistratuta (Mahaia ${mahaia.zenbakia})")
                            }
                        } catch (e: Exception) {
                            onMezuaBistaratu("Komanda sukaldera bidalita (Tokiko modua)")
                        }
                    }

                    navController.popBackStack()
                }
            )
        }
    }
}
