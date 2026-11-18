package eus.goierri.jatetxea.komandera.ui.navigation

sealed class Screen(val route: String) {
    object Login : Screen("login")
    object Mahaiak : Screen("mahaiak")
    object MahaiaXehetasunak : Screen("mahaia_xehetasunak/{mahaiId}") {
        fun sortuRuta(mahaiId: Int) = "mahaia_xehetasunak/$mahaiId"
    }
    object Eskaera : Screen("eskaera/{eskaeraId}/{mahaiId}") {
        fun sortuRuta(eskaeraId: Int, mahaiId: Int) = "eskaera/$eskaeraId/$mahaiId"
    }
}
