package eus.goierri.jatetxea.komandera.ui.navigation

sealed class Screen(val route: String) {
    object Login : Screen("login")
    object Mahaiak : Screen("mahaiak")
    object Eskaera : Screen("eskaera/{mahaiId}") {
        fun sortuRuta(mahaiId: Int) = "eskaera/$mahaiId"
    }
}
