<?php
/**
 * JATETXEA - Karta eta Plateren Atala (karta.php)
 *
 * @var array $platerak
 * @var array $ahalmenaMap
 */
$platerak = $platerak ?? [];
$ahalmenaMap = $ahalmenaMap ?? [];

$katFallbackMap = [
    'Sarrerakoa'    => 'sarrerakoa.jpg',
    'Lehena'        => 'lehena.jpg',
    'Bigarrena'     => 'bigarrena.jpg',
    'Postrea'       => 'postrea.jpg',
    'Edaria'        => 'ardoak.jpg',
    'Urak'          => 'urak.jpg',
    'Freskagarriak' => 'freskagarriak.jpg',
    'Garagardoak'   => 'garagardoak.jpg',
    'Sagardoak'     => 'sagardoak.jpg',
    'Ardoak'        => 'ardoak.jpg',
    'Kafeak'        => 'kafeak.jpg',
    'Infusioak'     => 'infusioak.jpg',
    'Likoreak'      => 'likoreak.jpg',
    'Konbinatuak'   => 'konbinatuak.jpg',
    'Koktailak'     => 'koktailak.jpg',
];

$edariKategoriak = [
    'Urak', 'Freskagarriak', 'Garagardoak', 'Sagardoak', 'Ardoak',
    'Kafeak', 'Infusioak', 'Likoreak', 'Konbinatuak', 'Koktailak'
];
$daudenEdariKat = [];
foreach ($platerak as $pl) {
    $k = (string)($pl['kategoria'] ?? '');
    if (in_array($k, $edariKategoriak, true)) {
        $daudenEdariKat[$k] = true;
    }
}
?>
        <!-- 1. KARTA ATALA (PHP ZERBITZARI-MAILAKO ERRENDERIZAZIOA) -->
        <section id="karta">
            <div class="section-header">
                <h2>Gure Karta</h2>
                <p>Bi munduetako lehengai gorenekin prestatua: euskal parrillako haragi eta arrainak, italiar pasta freskoak eta ardo hautatuak (Guztira: <?php echo count($platerak); ?> aukera)</p>
            </div>

            <!-- Kategoria-iragazkiak -->
            <div class="kategoria-filters">
                <button type="button" class="filter-btn active" data-kategoria="Guztiak">Guztiak</button>
                <button type="button" class="filter-btn" data-kategoria="Sarrerakoa">Sarrerakoak</button>
                <button type="button" class="filter-btn" data-kategoria="Lehena">Lehen Platerak</button>
                <button type="button" class="filter-btn" data-kategoria="Bigarrena">Bigarren Platerak</button>
                <button type="button" class="filter-btn" data-kategoria="Postrea">Postreak</button>
                <button type="button" class="filter-btn" data-kategoria="Edaria">Edariak</button>
            </div>

            <!-- Edarien Azpikategoria-iragazkiak (Edariak hautatzean ikusgai) -->
            <div id="edari-azpikategoriak" class="kategoria-subfilters" style="display: none;">
                <button type="button" class="subfilter-btn active" data-edari-kat="Guztiak">Edari Guztiak</button>
                <?php foreach ($edariKategoriak as $ek): ?>
                    <?php if (empty($daudenEdariKat) || isset($daudenEdariKat[$ek])): ?>
                        <button type="button" class="subfilter-btn" data-edari-kat="<?php echo htmlspecialchars($ek, ENT_QUOTES, 'UTF-8'); ?>"><?php echo htmlspecialchars($ek, ENT_QUOTES, 'UTF-8'); ?></button>
                    <?php endif; ?>
                <?php endforeach; ?>
            </div>

            <!-- Plateren Grid-a (PHP bidez zerbitzarian sortua) -->
            <div id="platerak-zerrenda" class="platerak-grid">
                <?php if (empty($platerak)): ?>
                    <p class="oharra-testua">Ez dago platerik erabilgarri edo REST APIa ez dago martxan.</p>
                <?php else: ?>
                    <?php foreach ($platerak as $p): ?>
                        <?php
                            $plateraId = (int)$p['id'];
                            $kategoriaRaw = (string)($p['kategoria'] ?? '');
                            $kategoria = htmlspecialchars($kategoriaRaw, ENT_QUOTES, 'UTF-8');
                            $izena = htmlspecialchars($p['izena'], ENT_QUOTES, 'UTF-8');
                            $deskribapena = htmlspecialchars($p['deskribapena'] ?? '', ENT_QUOTES, 'UTF-8');
                            $alergenoak = htmlspecialchars($p['alergenoak'] ?? '', ENT_QUOTES, 'UTF-8');
                            $prezioZenb = (float)($p['prezioa'] ?? $p['salmentaPrezioa'] ?? 0);
                            $prezioa = number_format($prezioZenb, 2, ',', '.') . ' €';
                            $denbora = (int)($p['prestatzeDenboraMin'] ?? 15);

                            $stockAhalmena = isset($ahalmenaMap[$plateraId]) ? $ahalmenaMap[$plateraId] : null;
                            $agortuta = ($stockAhalmena !== null && $stockAhalmena === 0);

                            // Plateraren irudi lokala (ID zehatza lehenik, kategoria fallback gisa)
                            $platerImgRel = "assets/img/platerak/platera_{$plateraId}.jpg";
                            $platerImgAbs = __DIR__ . "/../img/platerak/platera_{$plateraId}.jpg";
                            if (!file_exists($platerImgAbs)) {
                                $katFile = $katFallbackMap[$kategoriaRaw] ?? 'bigarrena.jpg';
                                $platerImgRel = "assets/img/kategoriak/{$katFile}";
                            }
                        ?>
                        <div class="plater-card <?php echo $agortuta ? 'agortuta' : ''; ?>" data-kategoria="<?php echo $kategoria; ?>">
                            <div class="plater-img-wrap">
                                <img src="<?php echo htmlspecialchars($platerImgRel, ENT_QUOTES, 'UTF-8'); ?>"
                                     alt="<?php echo $izena; ?>"
                                     class="plater-img"
                                     loading="lazy"
                                     decoding="async">
                                <?php if ($agortuta): ?>
                                    <span class="plater-agortuta-overlay">Agortuta</span>
                                <?php endif; ?>
                            </div>
                            <div class="plater-body">
                                <div class="plater-header">
                                    <h3 class="plater-izena"><?php echo $izena; ?></h3>
                                    <span class="plater-prezioa"><?php echo $prezioa; ?></span>
                                </div>
                                <p class="plater-deskribapena"><?php echo $deskribapena; ?></p>

                                <?php if (!empty($alergenoak)): ?>
                                    <p class="plater-alergenoak">
                                        <strong>Alergenoak:</strong> <?php echo $alergenoak; ?>
                                    </p>
                                <?php endif; ?>

                                <?php if (!empty($p['osagaiak']) && is_array($p['osagaiak'])): ?>
                                    <div class="plater-osagaiak">
                                        <strong>Osagaiak:</strong>
                                        <ul>
                                            <?php foreach ($p['osagaiak'] as $osagaia): ?>
                                                <li><?php echo htmlspecialchars($osagaia['lehengaiaIzena'], ENT_QUOTES, 'UTF-8'); ?></li>
                                            <?php endforeach; ?>
                                        </ul>
                                    </div>
                                <?php endif; ?>

                                <div class="plater-meta">
                                    <span class="kategoria-tag"><?php echo $kategoria; ?></span>
                                    <span class="prestatze-denbora"><?php echo $denbora; ?> min</span>
                                    <?php if ($agortuta): ?>
                                        <span class="stock-tag agortuta">Agortuta</span>
                                    <?php elseif ($stockAhalmena !== null): ?>
                                        <span class="stock-tag eskuragarri"><?php echo $stockAhalmena; ?> anoa</span>
                                    <?php endif; ?>
                                </div>
                            </div>
                        </div>
                    <?php endforeach; ?>
                <?php endif; ?>
            </div>
        </section>
