package eus.goierri.jatetxea.komandera.ui.components

import android.content.Context
import android.graphics.BitmapFactory
import android.util.LruCache
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.LocalBar
import androidx.compose.material.icons.filled.Restaurant
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import eus.goierri.jatetxea.komandera.data.model.KategoriaLaguntzailea
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import java.text.Normalizer
import java.util.Locale

/**
 * Produktu-irudien (platerak eta edariak) cache eta kargatzaile lokala (%100 offline).
 * desktop_tpv_csharp (ProductImageCache.cs) eta web_php_bezeroak-eko 3 mailako hierarkia bera jarraitzen du:
 *   1. images/Platerak/platera_{id}.{jpg|jpeg|png|webp}
 *   2. images/Platerak/{slug} edo gako-hitz espezifikoa
 *   3. images/Kategoriak/{kategoria} edo alias-a
 */
object ProductImageLoader {
    private val bitmapCache = LruCache<String, ImageBitmap>(80)
    @Volatile
    private var platerakFiles: Set<String>? = null
    @Volatile
    private var kategoriakFiles: Set<String>? = null

    private val extensions = listOf(".jpg", ".jpeg", ".png", ".webp")

    private fun ensureIndex(context: Context) {
        if (platerakFiles == null) {
            synchronized(this) {
                if (platerakFiles == null) {
                    platerakFiles = runCatching {
                        context.assets.list("images/Platerak")?.toSet() ?: emptySet()
                    }.getOrDefault(emptySet())
                }
                if (kategoriakFiles == null) {
                    kategoriakFiles = runCatching {
                        context.assets.list("images/Kategoriak")?.toSet() ?: emptySet()
                    }.getOrDefault(emptySet())
                }
            }
        }
    }

    fun resolveAssetPath(context: Context, plateraId: Int, izena: String, kategoria: String): String? {
        ensureIndex(context)
        val pFiles = platerakFiles.orEmpty()
        val kFiles = kategoriakFiles.orEmpty()

        fun findIn(files: Set<String>, folder: String, baseName: String?): String? {
            if (baseName.isNullOrBlank()) return null
            for (ext in extensions) {
                val fileName = "$baseName$ext"
                if (files.contains(fileName)) return "$folder/$fileName"
            }
            return null
        }

        // 1. Maila: ID zehatza (platera_{id}.jpg)
        if (plateraId > 0) {
            findIn(pFiles, "images/Platerak", "platera_$plateraId")?.let { return it }
        }

        // 2. Maila: Izenaren slug-a edo gako-hitz espezifikoa
        val slug = toSlug(izena)
        findIn(pFiles, "images/Platerak", slug)?.let { return it }

        val keywordFile = mapKeywordImage(slug)
        findIn(pFiles, "images/Platerak", keywordFile)?.let { return it }
        findIn(kFiles, "images/Kategoriak", keywordFile)?.let { return it }

        // 3. Maila: Kategoriaren irudia
        val katSlug = toSlug(kategoria)
        findIn(kFiles, "images/Kategoriak", katSlug)?.let { return it }

        val katAlias = mapCategoryAlias(katSlug)
        findIn(kFiles, "images/Kategoriak", katAlias)?.let { return it }

        return null
    }

    fun getCached(path: String): ImageBitmap? = bitmapCache.get(path)

    suspend fun loadBitmap(context: Context, assetPath: String, targetPx: Int = 256): ImageBitmap? {
        bitmapCache.get(assetPath)?.let { return it }
        return withContext(Dispatchers.IO) {
            runCatching {
                val boundsOpts = BitmapFactory.Options().apply { inJustDecodeBounds = true }
                context.assets.open(assetPath).use { BitmapFactory.decodeStream(it, null, boundsOpts) }

                var sampleSize = 1
                val maxDim = maxOf(boundsOpts.outWidth, boundsOpts.outHeight)
                while (maxDim / sampleSize > targetPx * 2) {
                    sampleSize *= 2
                }

                val decodeOpts = BitmapFactory.Options().apply { inSampleSize = sampleSize }
                val bmp = context.assets.open(assetPath).use { BitmapFactory.decodeStream(it, null, decodeOpts) }
                bmp?.asImageBitmap()?.also { bitmapCache.put(assetPath, it) }
            }.getOrNull()
        }
    }

    private fun mapKeywordImage(s: String): String? = when {
        s.contains("txuleta") || s.contains("entrekot") || s.contains("solomillo") || s.contains("arkume") || s.contains("haragi") -> "bigarrena"
        s.contains("bakailao") || s.contains("legatz") || s.contains("txipiroi") || s.contains("arrain") || s.contains("izokin") || s.contains("pulpo") || s.contains("olagarro") -> "bakailaoa"
        s.contains("entsalada") || s.contains("burrata") || s.contains("barazki") -> "sarrerakoa"
        s.contains("kroket") || s.contains("urdaiazpik") || s.contains("txistor") || s.contains("pintxo") || s.contains("rabas") -> "sarrerakoa"
        s.contains("pasta") || s.contains("risotto") || s.contains("zopa") || s.contains("babarrun") || s.contains("porrusalda") -> "lehena"
        s.contains("gazta") || s.contains("tarta") || s.contains("tiramisu") || s.contains("txokolate") || s.contains("izozki") || s.contains("flan") || s.contains("mamia") || s.contains("pantxineta") -> "postrea"
        s.contains("txakolin") || s.contains("zuri") || s.contains("prosecco") -> "ardo_zuria"
        s.contains("ardo") || s.contains("chianti") || s.contains("errioxa") -> "ardoak"
        s.contains("sagardo") -> "sagardoak"
        s.contains("garagardo") || s.contains("peroni") || s.contains("basqueland") || s.contains("zurito") || s.contains("kana") -> "garagardoak"
        s.contains("cappuccino") || s.contains("espresso") || s.contains("kafe") || s.contains("carajillo") -> "kafeak"
        s.contains("te_") || s.startsWith("te_") || s.contains("kamamila") || s.contains("poleo") || s.contains("infusio") -> "infusioak"
        s.contains("gin") || s.contains("ron") || s.contains("vodka") || s.contains("whisky") -> "konbinatuak"
        s.contains("mojito") || s.contains("aperol") || s.contains("negroni") || s.contains("martini") || s.contains("koktail") -> "koktailak"
        s.contains("patxaran") || s.contains("orujo") || s.contains("limoncello") || s.contains("amaretto") || s.contains("likore") -> "likoreak"
        s.contains("limonada") || s.contains("zuku") || s.contains("mosto") || s.contains("tonika") || s.contains("freskagarri") || s.contains("kola") -> "freskagarriak"
        s.contains("ur_") || s.contains("ura") || s.contains("alzola") || s.contains("pellegrino") -> "urak"
        else -> null
    }

    private fun mapCategoryAlias(k: String): String = when {
        k.contains("sarrera") || k.contains("hastek") -> "sarrerakoa"
        k.contains("lehen") -> "lehena"
        k.contains("bigarren") || k.contains("haragi") || k.contains("arrain") -> "bigarrena"
        k.contains("postre") -> "postrea"
        else -> k
    }

    fun toSlug(text: String?): String {
        if (text.isNullOrBlank()) return ""
        val normalized = Normalizer.normalize(text.trim(), Normalizer.Form.NFD)
            .replace(Regex("\\p{InCombiningDiacriticalMarks}+"), "")
            .lowercase(Locale.ROOT)
        return normalized
            .replace(Regex("[^a-z0-9]+"), "_")
            .trim('_')
    }
}

@Composable
fun PlateraIrudia(
    plateraId: Int,
    izena: String,
    kategoria: String,
    modifier: Modifier = Modifier,
    cornerRadius: Dp = 10.dp
) {
    val context = LocalContext.current
    val assetPath = remember(plateraId, izena, kategoria) {
        ProductImageLoader.resolveAssetPath(context, plateraId, izena, kategoria)
    }
    var imageBitmap by remember(assetPath) {
        mutableStateOf(assetPath?.let { ProductImageLoader.getCached(it) })
    }

    LaunchedEffect(assetPath) {
        if (assetPath != null && imageBitmap == null) {
            imageBitmap = ProductImageLoader.loadBitmap(context, assetPath)
        }
    }

    val shape = RoundedCornerShape(cornerRadius)
    Box(
        modifier = modifier
            .clip(shape)
            .background(MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.6f))
            .border(1.dp, MaterialTheme.colorScheme.outline.copy(alpha = 0.25f), shape),
        contentAlignment = Alignment.Center
    ) {
        val bmp = imageBitmap
        if (bmp != null) {
            Image(
                bitmap = bmp,
                contentDescription = izena,
                contentScale = ContentScale.Crop,
                modifier = Modifier.fillMaxSize()
            )
        } else {
            Icon(
                imageVector = if (KategoriaLaguntzailea.daBarrakoProduktua(kategoria)) {
                    Icons.Default.LocalBar
                } else {
                    Icons.Default.Restaurant
                },
                contentDescription = izena,
                tint = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.6f),
                modifier = Modifier.size(26.dp)
            )
        }
    }
}
