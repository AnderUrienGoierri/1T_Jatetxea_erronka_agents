<?php
/**
 * JATETXEA - Karta eta Plateren Atala (karta.php)
 *
 * @var array $platerak
 * @var array $ahalmenaMap
 */
$platerak = $platerak ?? [];
$ahalmenaMap = $ahalmenaMap ?? [];
?>
        <!-- 1. KARTA ATALA (PHP ZERBITZARI-MAILAKO ERRENDERIZAZIOA) -->
        <section id="karta">
            <div class="section-header">
                <h2>Gure Karta</h2>
                <p>Egunero bertako baserritar eta arraindegietatik jasotako lehengaiekin prestatua (Guztira: <?php echo count($platerak); ?> plater eta edari)</p>
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

            <!-- Plateren Grid-a (PHP bidez zerbitzarian sortua) -->
            <div id="platerak-zerrenda" class="platerak-grid">
                <?php if (empty($platerak)): ?>
                    <p class="oharra-testua">Ez dago platerik erabilgarri edo REST APIa ez dago martxan.</p>
                <?php else: ?>
                    <?php foreach ($platerak as $p): ?>
                        <?php
                            $plateraId = (int)$p['id'];
                            $kategoria = htmlspecialchars($p['kategoria'], ENT_QUOTES, 'UTF-8');
                            $izena = htmlspecialchars($p['izena'], ENT_QUOTES, 'UTF-8');
                            $deskribapena = htmlspecialchars($p['deskribapena'] ?? '', ENT_QUOTES, 'UTF-8');
                            $alergenoak = htmlspecialchars($p['alergenoak'] ?? '', ENT_QUOTES, 'UTF-8');
                            $prezioZenb = (float)($p['prezioa'] ?? $p['salmentaPrezioa'] ?? 0);
                            $prezioa = number_format($prezioZenb, 2, ',', '.') . ' €';
                            $denbora = (int)($p['prestatzeDenboraMin'] ?? 15);
                            
                            $stockAhalmena = isset($ahalmenaMap[$plateraId]) ? $ahalmenaMap[$plateraId] : null;
                            $agortuta = ($stockAhalmena !== null && $stockAhalmena === 0);
                        ?>
                        <div class="plater-card <?php echo $agortuta ? 'agortuta' : ''; ?>" data-kategoria="<?php echo $kategoria; ?>">
                            <div class="plater-header">
                                <h3 class="plater-izena"><?php echo $izena; ?></h3>
                                <span class="plater-prezioa"><?php echo $prezioa; ?></span>
                            </div>
                            <p class="plater-deskribapena"><?php echo $deskribapena; ?></p>
                            
                            <?php if (!empty($alergenoak)): ?>
                                <p style="font-size: 0.8rem; color: #d97706; margin-bottom: 0.5rem;">
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
                                <span class="prestatze-denbora" style="font-size: 0.8rem; color: #64748b;"><?php echo $denbora; ?> min</span>
                                <?php if ($agortuta): ?>
                                    <span class="stock-tag agortuta">Agortuta</span>
                                <?php elseif ($stockAhalmena !== null): ?>
                                    <span class="stock-tag eskuragarri"><?php echo $stockAhalmena; ?> anoa</span>
                                <?php endif; ?>
                            </div>
                        </div>
                    <?php endforeach; ?>
                <?php endif; ?>
            </div>
        </section>
