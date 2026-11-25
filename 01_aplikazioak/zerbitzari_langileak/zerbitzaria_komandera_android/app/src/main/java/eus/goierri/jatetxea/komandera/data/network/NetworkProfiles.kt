package eus.goierri.jatetxea.komandera.data.network

import android.content.Context
import android.net.ConnectivityManager
import android.net.NetworkCapabilities
import android.net.wifi.WifiInfo
import android.net.wifi.WifiManager
import android.os.Build

data class NetworkProfile(
    val id: String,
    val izena: String,
    val wifiSsid: String?,
    val baseUrl: String,
    val kokapena: String,
    val deskribapena: String
)

object NetworkProfiles {
    val PROFILAK = listOf(
        NetworkProfile(
            id = "etxean",
            izena = "Ikaztegieta Etxea",
            wifiSsid = "euskaltel_urite",
            baseUrl = "http://192.168.1.134:5050/api/v1/",
            kokapena = "Ikaztegieta (Etxeko Wi-Fi)",
            deskribapena = "192.168.1.134:5050 (euskaltel_urite)"
        ),
        NetworkProfile(
            id = "eskola",
            izena = "Goierri Eskola",
            wifiSsid = "GOI_Ikasleak",
            baseUrl = "http://172.16.243.145:5050/api/v1/",
            kokapena = "Ordizia (Goierri Eskola)",
            deskribapena = "172.16.243.145:5050 (GOI_Ikasleak)"
        ),
        NetworkProfile(
            id = "tolosako_etxea",
            izena = "Tolosa Etxea",
            wifiSsid = "MIWIFI_dc7U",
            baseUrl = "http://192.168.1.166:5050/api/v1/",
            kokapena = "Tolosa (Etxeko Wi-Fi)",
            deskribapena = "192.168.1.166:5050 (MIWIFI_dc7U)"
        ),
        NetworkProfile(
            id = "innovatek",
            izena = "Innovatek Beasain",
            wifiSsid = "INN-WPRIVATE",
            baseUrl = "http://10.100.30.112:5050/api/v1/",
            kokapena = "Beasain (INN-WPRIVATE)",
            deskribapena = "10.100.30.112:5050 (INN-WPRIVATE)"
        ),
        NetworkProfile(
            id = "pc_mdns",
            izena = "PC Izena (mDNS · Beti Berdina)",
            wifiSsid = null,
            baseUrl = "http://ANDER-SGB5PRO.local:5050/api/v1/",
            kokapena = "Wi-Fi Lokala (IP aldaketarik gabe)",
            deskribapena = "ANDER-SGB5PRO.local:5050 (mDNS)"
        ),
        NetworkProfile(
            id = "usb_localhost",
            izena = "USB Kablez / Localhost",
            wifiSsid = null,
            baseUrl = "http://127.0.0.1:5050/api/v1/",
            kokapena = "ADB Reverse / Emuladorea",
            deskribapena = "127.0.0.1:5050 (adb reverse tcp:5050)"
        ),
        NetworkProfile(
            id = "emuladorea",
            izena = "Android Emuladorea",
            wifiSsid = null,
            baseUrl = "http://10.0.2.2:5050/api/v1/",
            kokapena = "Android Studio AVD",
            deskribapena = "10.0.2.2:5050 (AVD Standard Host)"
        )
    )

    fun lortuUnekoWifiIzena(context: Context): String? {
        try {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
                val connMgr = context.getSystemService(Context.CONNECTIVITY_SERVICE) as? ConnectivityManager
                val activeNetwork = connMgr?.activeNetwork
                val capabilities = connMgr?.getNetworkCapabilities(activeNetwork)
                val transportInfo = capabilities?.transportInfo as? WifiInfo
                val ssid = transportInfo?.ssid
                if (!ssid.isNullOrBlank() && ssid != "<unknown ssid>" && ssid != "0x") {
                    return ssid.replace("\"", "").trim()
                }
            }

            @Suppress("DEPRECATION")
            val wifiManager = context.applicationContext.getSystemService(Context.WIFI_SERVICE) as? WifiManager
            if (wifiManager != null) {
                @Suppress("DEPRECATION")
                val wifiInfo = wifiManager.connectionInfo
                if (wifiInfo != null) {
                    val ssid = wifiInfo.ssid
                    if (!ssid.isNullOrBlank() && ssid != "<unknown ssid>" && ssid != "0x") {
                        return ssid.replace("\"", "").trim()
                    }
                }
            }

            // Fallback: Sareko IP lokalaren arabera identifikatu (adibidez 192.168.1.x -> Ikaztegieta Etxea / euskaltel_urite)
            val ipLokala = lortuIpLokala()
            if (ipLokala != null) {
                if (ipLokala.startsWith("192.168.1.")) {
                    return "euskaltel_urite (IP: $ipLokala)"
                } else if (ipLokala.startsWith("172.16.")) {
                    return "GOI_Ikasleak (IP: $ipLokala)"
                } else if (ipLokala.startsWith("192.168.5.") || ipLokala.startsWith("10.100.30.")) {
                    return "INN-WPRIVATE (IP: $ipLokala)"
                }
                return "Wi-Fi Lokala ($ipLokala)"
            }
        } catch (_: Exception) {
        }
        return null
    }

    fun lortuIpLokala(): String? {
        try {
            val interfaces = java.net.NetworkInterface.getNetworkInterfaces()
            while (interfaces.hasMoreElements()) {
                val intf = interfaces.nextElement()
                if (intf.isLoopback || !intf.isUp) continue
                val addrs = intf.inetAddresses
                while (addrs.hasMoreElements()) {
                    val addr = addrs.nextElement()
                    if (!addr.isLoopbackAddress && addr is java.net.Inet4Address) {
                        return addr.hostAddress
                    }
                }
            }
        } catch (_: Exception) {}
        return null
    }

    private fun daEmuladorea(): Boolean =
        Build.FINGERPRINT.startsWith("generic") ||
            Build.FINGERPRINT.lowercase().contains("emulator") ||
            Build.MODEL.contains("Emulator") ||
            Build.MODEL.contains("Android SDK built for") ||
            Build.PRODUCT.contains("sdk_gphone") ||
            Build.HARDWARE.contains("ranchu") ||
            Build.HARDWARE.contains("goldfish")

    fun lortuGomendatutakoProfila(context: Context): NetworkProfile {
        if (daEmuladorea()) {
            return PROFILAK.firstOrNull { it.id == "emuladorea" } ?: PROFILAK.last()
        }
        // Gailu fisikoetan: mDNS (ANDER-SGB5PRO.local) beti ebazten du IPa automatikoki edozein Wi-Fitan!
        val mdnsProfila = PROFILAK.firstOrNull { it.id == "pc_mdns" }
        if (mdnsProfila != null) return mdnsProfila
        return PROFILAK.first()
    }
}
