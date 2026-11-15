package eus.goierri.jatetxea.adminzentrala

import android.annotation.SuppressLint
import android.content.Context
import android.content.SharedPreferences
import android.graphics.Bitmap
import android.os.Bundle
import android.view.View
import android.webkit.*
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout

class MainActivity : AppCompatActivity() {

    private lateinit var webView: WebView
    private lateinit var swipeRefresh: SwipeRefreshLayout
    private lateinit var progressBar: ProgressBar
    private lateinit var layoutError: LinearLayout
    private lateinit var tvCurrentHost: TextView
    private lateinit var dotStatus: View
    private lateinit var tvStatus: TextView
    private lateinit var btnSettings: ImageButton
    private lateinit var btnRefresh: ImageButton
    private lateinit var btnConnectUsb: Button
    private lateinit var btnConnectWifi: Button
    private lateinit var btnRetry: Button

    private lateinit var prefs: SharedPreferences

    companion object {
        private const val PREFS_NAME = "admin_it_prefs"
        private const val KEY_SERVER_URL = "server_url"
        private const val DEFAULT_USB_URL = "http://localhost:5000"
        private const val DEFAULT_WIFI_URL = "http://192.168.1.168:5000"
    }

    private var currentUrl: String = DEFAULT_USB_URL

    @SuppressLint("SetJavaScriptEnabled")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        prefs = getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        currentUrl = prefs.getString(KEY_SERVER_URL, DEFAULT_USB_URL) ?: DEFAULT_USB_URL

        initViews()
        setupWebView()
        setupListeners()

        loadUrl(currentUrl)
    }

    private fun initViews() {
        webView = findViewById(R.id.webView)
        swipeRefresh = findViewById(R.id.swipeRefresh)
        progressBar = findViewById(R.id.progressBar)
        layoutError = findViewById(R.id.layoutError)
        tvCurrentHost = findViewById(R.id.tvCurrentHost)
        dotStatus = findViewById(R.id.dotStatus)
        tvStatus = findViewById(R.id.tvStatus)
        btnSettings = findViewById(R.id.btnSettings)
        btnRefresh = findViewById(R.id.btnRefresh)
        btnConnectUsb = findViewById(R.id.btnConnectUsb)
        btnConnectWifi = findViewById(R.id.btnConnectWifi)
        btnRetry = findViewById(R.id.btnRetry)

        tvCurrentHost.text = currentUrl
        swipeRefresh.setColorSchemeResources(R.color.primary)
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun setupWebView() {
        val s = webView.settings
        s.javaScriptEnabled = true
        s.domStorageEnabled = true
        s.databaseEnabled = true
        s.useWideViewPort = true
        s.loadWithOverviewMode = true
        s.setSupportZoom(true)
        s.builtInZoomControls = true
        s.displayZoomControls = false
        s.mixedContentMode = WebSettings.MIXED_CONTENT_ALWAYS_ALLOW
        s.cacheMode = WebSettings.LOAD_DEFAULT

        webView.webViewClient = object : WebViewClient() {
            override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
                super.onPageStarted(view, url, favicon)
                progressBar.visibility = View.VISIBLE
                setOnlineStatus(true, "Kargatzen...")
            }

            override fun onPageFinished(view: WebView?, url: String?) {
                super.onPageFinished(view, url)
                progressBar.visibility = View.GONE
                swipeRefresh.isRefreshing = false
                layoutError.visibility = View.GONE
                webView.visibility = View.VISIBLE
                setOnlineStatus(true, "LINEAN")
            }

            override fun onReceivedError(view: WebView?, request: WebResourceRequest?, error: WebResourceError?) {
                super.onReceivedError(view, request, error)
                if (request?.isForMainFrame == true) {
                    progressBar.visibility = View.GONE
                    swipeRefresh.isRefreshing = false
                    webView.visibility = View.GONE
                    layoutError.visibility = View.VISIBLE
                    setOnlineStatus(false, "DESKONEKTATUTA")
                }
            }
        }

        webView.webChromeClient = object : WebChromeClient() {
            override fun onProgressChanged(view: WebView?, newProgress: Int) {
                super.onProgressChanged(view, newProgress)
                progressBar.progress = newProgress
                if (newProgress >= 100) {
                    progressBar.visibility = View.GONE
                }
            }

            override fun onJsAlert(view: WebView?, url: String?, message: String?, result: JsResult?): Boolean {
                AlertDialog.Builder(this@MainActivity)
                    .setTitle("IT Kontrol Zentrala")
                    .setMessage(message ?: "")
                    .setPositiveButton("Ados") { _, _ -> result?.confirm() }
                    .setCancelable(false)
                    .create()
                    .show()
                return true
            }

            override fun onJsConfirm(view: WebView?, url: String?, message: String?, result: JsResult?): Boolean {
                AlertDialog.Builder(this@MainActivity)
                    .setTitle("Berrespena")
                    .setMessage(message ?: "")
                    .setPositiveButton("Bai") { _, _ -> result?.confirm() }
                    .setNegativeButton("Ez") { _, _ -> result?.cancel() }
                    .setCancelable(false)
                    .create()
                    .show()
                return true
            }
        }
    }

    private fun setupListeners() {
        swipeRefresh.setOnRefreshListener {
            webView.reload()
        }

        btnRefresh.setOnClickListener {
            progressBar.visibility = View.VISIBLE
            webView.reload()
        }

        btnSettings.setOnClickListener {
            showServerDialog()
        }

        btnConnectUsb.setOnClickListener {
            switchServer(DEFAULT_USB_URL)
        }

        btnConnectWifi.setOnClickListener {
            switchServer(DEFAULT_WIFI_URL)
        }

        btnRetry.setOnClickListener {
            loadUrl(currentUrl)
        }
    }

    private fun setOnlineStatus(online: Boolean, text: String) {
        val color = if (online) getColor(R.color.status_green) else getColor(R.color.status_red)
        dotStatus.setBackgroundColor(color)
        tvStatus.setTextColor(color)
        tvStatus.text = text
    }

    private fun loadUrl(url: String) {
        currentUrl = url
        tvCurrentHost.text = url
        prefs.edit().putString(KEY_SERVER_URL, url).apply()
        layoutError.visibility = View.GONE
        webView.visibility = View.VISIBLE
        progressBar.visibility = View.VISIBLE
        webView.loadUrl(url)
    }

    private fun switchServer(url: String) {
        Toast.makeText(this, "Zerbitzaria aldatzen: $url", Toast.LENGTH_SHORT).show()
        loadUrl(url)
    }

    private fun showServerDialog() {
        val options = arrayOf(
            "USB Kablea / ADB (http://localhost:5000)",
            "WiFi Sarea (http://192.168.1.168:5000)",
            "Beste IP edo ataka pertsonalizatua..."
        )

        AlertDialog.Builder(this)
            .setTitle("Hautatu IT Kontrol Zerbitzaria")
            .setItems(options) { _, which ->
                when (which) {
                    0 -> switchServer(DEFAULT_USB_URL)
                    1 -> switchServer(DEFAULT_WIFI_URL)
                    2 -> showCustomIpDialog()
                }
            }
            .setNegativeButton("Utzi", null)
            .show()
    }

    private fun showCustomIpDialog() {
        val input = EditText(this)
        input.hint = "http://192.168.1.XXX:5000"
        input.setText(currentUrl)

        AlertDialog.Builder(this)
            .setTitle("Sartu Zerbitzariaren Helbidea")
            .setView(input)
            .setPositiveButton("Konektatu") { _, _ ->
                val text = input.text.toString().trim()
                if (text.isNotEmpty()) {
                    val fullUrl = if (!text.startsWith("http://") && !text.startsWith("https://")) {
                        "http://$text"
                    } else text
                    switchServer(fullUrl)
                }
            }
            .setNegativeButton("Utzi", null)
            .show()
    }

    override fun onBackPressed() {
        if (webView.canGoBack()) {
            webView.goBack()
        } else {
            super.onBackPressed()
        }
    }
}
