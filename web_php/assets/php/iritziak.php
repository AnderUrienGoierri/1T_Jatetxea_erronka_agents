        <!-- 3. BEZEROEN IRITZIAK ATALA (PHP ZERBITZARI-MAILAKO ERRENDERIZAZIOA) -->
        <section id="iritziak">
            <div class="section-header">
                <h2>Bezeroen Balorazioak</h2>
                <p>Gure bezeroek partekatutako esperientziak eta iritziak</p>
            </div>

            <div class="iritziak-grid">
                <!-- Ezkerra: Estatistika orokorrak -->
                <div id="balorazio-estatistikak" class="estatistikak-kaxa">
                    <?php
                        $media = number_format((float)($estatistikak['batazBestekoBalorazioa'] ?? 0), 1);
                        $guztira = (int)($estatistikak['guztiraIritziak'] ?? 0);
                    ?>
                    <div class="estatistika-puntuazioa"><?php echo $media; ?></div>
                    <div class="estatistika-izarrak">
                        <?php
                            $borobildua = (int)round((float)$media);
                            for ($i = 1; $i <= 5; $i++) {
                                echo ($i <= $borobildua) ? '★' : '☆';
                            }
                        ?>
                    </div>
                    <div class="estatistika-guztira"><?php echo $guztira; ?> bezeroren balorazioak</div>

                    <div class="izar-distribuzioa">
                        <?php
                            $mailak = [
                                5 => $estatistikak['bostIzarrak'] ?? 0,
                                4 => $estatistikak['lauIzarrak'] ?? 0,
                                3 => $estatistikak['hiruIzarrak'] ?? 0,
                                2 => $estatistikak['biIzarrak'] ?? 0,
                                1 => $estatistikak['izarBat'] ?? 0,
                                0 => $estatistikak['zeroIzar'] ?? 0
                            ];
                            foreach ($mailak as $izar => $kopurua):
                                $portzentaia = $guztira > 0 ? round(($kopurua / $guztira) * 100) : 0;
                        ?>
                            <div class="distribuzio-lerroa">
                                <span><?php echo $izar; ?> ★</span>
                                <div class="barra-hutsik">
                                    <div class="barra-beteta" style="width: <?php echo $portzentaia; ?>%;"></div>
                                </div>
                                <span class="lerro-kopurua"><?php echo $kopurua; ?></span>
                            </div>
                        <?php endforeach; ?>
                    </div>
                </div>

                <!-- Eskuina: Iritzi zerrenda eta iritzi berria idazteko inprimakia -->
                <div>
                    <div class="iritziak-zerrenda-kaxa">
                        <h3 style="margin-bottom: 1rem; color: #1e293b; font-size: 1.15rem;">Azken Iritziak (<?php echo count($iritziak); ?>)</h3>
                        <div id="iritziak-zerrenda">
                            <?php if (empty($iritziak)): ?>
                                <p class="oharra-testua">Oraindik ez dago iritzirik. Izan zaitez lehena zure esperientzia partekatzen!</p>
                            <?php else: ?>
                                <?php foreach ($iritziak as $i): ?>
                                    <?php
                                        $bezeroa = htmlspecialchars($i['bezeroIzena'] ?? 'Bezeroa', ENT_QUOTES, 'UTF-8');
                                        $dataIritzi = htmlspecialchars(substr($i['sortzeData'] ?? '', 0, 10), ENT_QUOTES, 'UTF-8');
                                        $puntuak = (int)($i['balorazioa'] ?? 5);
                                        $testua = htmlspecialchars($i['testua'] ?? '', ENT_QUOTES, 'UTF-8');
                                    ?>
                                    <div class="iritzi-txartela">
                                        <div class="iritzi-goiburua">
                                            <strong><?php echo $bezeroa; ?></strong>
                                            <span class="iritzi-data"><?php echo $dataIritzi; ?></span>
                                        </div>
                                        <div class="iritzi-izarrak">
                                            <?php
                                                for ($x = 1; $x <= 5; $x++) {
                                                    echo ($x <= $puntuak) ? '★' : '☆';
                                                }
                                            ?>
                                        </div>
                                        <?php if (!empty($testua)): ?>
                                            <p class="iritzi-testua"><?php echo $testua; ?></p>
                                        <?php endif; ?>
                                    </div>
                                <?php endforeach; ?>
                            <?php endif; ?>
                        </div>
                    </div>

                    <!-- Iritzia uzteko inprimakia -->
                    <div class="erreserba-form-area" style="margin-top: 1.5rem; padding: 1.5rem; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px;">
                        <h4 style="margin-bottom: 1rem; color: #1e293b;">Utzi Zure Balorazioa</h4>
                        
                        <?php if ($flash_iritzi_success): ?>
                            <div class="alert-kaxa alert-success" style="display: block;">
                                <?php echo htmlspecialchars($flash_iritzi_success, ENT_QUOTES, 'UTF-8'); ?>
                            </div>
                        <?php endif; ?>

                        <?php if ($flash_iritzi_error): ?>
                            <div class="alert-kaxa alert-error" style="display: block;">
                                <?php echo htmlspecialchars($flash_iritzi_error, ENT_QUOTES, 'UTF-8'); ?>
                            </div>
                        <?php endif; ?>

                        <div id="iritzi-alerta" class="alert-kaxa"></div>

                        <form id="iritzi-form" method="POST" action="index.php#iritziak">
                            <input type="hidden" name="action" value="iritzia">

                            <div class="form-row">
                                <div class="form-group">
                                    <label for="iritzi-emaila">E-posta helbidea (*):</label>
                                    <input type="email" name="iritzi_emaila" id="iritzi-emaila" class="form-control" placeholder="zure.emaila@adibidea.eus" required>
                                </div>
                                <div class="form-group">
                                    <label for="iritzi-izena">Izena:</label>
                                    <input type="text" name="iritzi_izena" id="iritzi-izena" class="form-control" placeholder="Zure izena">
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="iritzi-balorazioa">Puntuazioa (0 eta 5 artean) (*):</label>
                                <select name="iritzi_balorazioa" id="iritzi-balorazioa" class="form-control" required>
                                    <option value="5">5 izar - Bikaina</option>
                                    <option value="4">4 izar - Oso ona</option>
                                    <option value="3">3 izar - Ona</option>
                                    <option value="2">2 izar - Hobetzekoa</option>
                                    <option value="1">1 izar - Eskasa</option>
                                    <option value="0">0 izar - Oso txarra</option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label for="iritzi-testua">Iruzkina:</label>
                                <textarea name="iritzi_testua" id="iritzi-testua" class="form-control" rows="2" placeholder="Nola joan da zure esperientzia?"></textarea>
                            </div>
                            <button type="submit" class="btn btn-secondary" style="width: 100%;">Iritzia Bidali (PHP bidez)</button>
                        </form>
                    </div>
                </div>
            </div>
        </section>
