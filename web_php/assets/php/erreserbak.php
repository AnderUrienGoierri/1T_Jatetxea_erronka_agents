        <!-- 2. MAHAI ERRESERBA ATALA -->
        <section id="erreserba">
            <div class="section-header">
                <h2>Mahai Erreserba</h2>
                <p>Aukeratu zure data, txanda eta mahaia unean bertako baieztapenarekin</p>
            </div>

            <div class="erreserba-wrapper">
                <!-- Erreserba inprimakia -->
                <div class="erreserba-form-area">
                    <h3>Erreserbaren Datuak</h3>
                    
                    <!-- PHP bidezko Alertak -->
                    <?php if ($flash_success): ?>
                        <div class="alert-kaxa alert-success" style="display: block;">
                            <strong>Baieztatuta:</strong> <?php echo htmlspecialchars($flash_success, ENT_QUOTES, 'UTF-8'); ?>
                        </div>
                    <?php endif; ?>

                    <?php if ($flash_conflict): ?>
                        <div class="alert-kaxa alert-error" style="display: block; border-left: 4px solid #dc2626;">
                            <strong>Gatazka (409):</strong> <?php echo htmlspecialchars($flash_conflict, ENT_QUOTES, 'UTF-8'); ?>
                        </div>
                    <?php endif; ?>

                    <?php if ($flash_error): ?>
                        <div class="alert-kaxa alert-error" style="display: block;">
                            <?php echo htmlspecialchars($flash_error, ENT_QUOTES, 'UTF-8'); ?>
                        </div>
                    <?php endif; ?>

                    <div id="erreserba-alerta" class="alert-kaxa"></div>

                    <form id="erreserba-form" method="POST" action="index.php#erreserba">
                        <input type="hidden" name="action" value="erreserbatu">

                        <div class="form-row">
                            <div class="form-group">
                                <label for="erreserba-data">Data (*):</label>
                                <input type="date" name="data" id="erreserba-data" class="form-control" value="<?php echo date('Y-m-d'); ?>" required>
                            </div>
                            <div class="form-group">
                                <label for="erreserba-txanda">Txanda (*):</label>
                                <select name="txanda" id="erreserba-txanda" class="form-control" required>
                                    <option value="Bazkaria_1">Bazkaria 1 (13:00 - 14:30)</option>
                                    <option value="Bazkaria_2">Bazkaria 2 (14:45 - 16:30)</option>
                                    <option value="Afaria_1">Afaria 1 (20:30 - 22:00)</option>
                                    <option value="Afaria_2">Afaria 2 (22:15 - 00:00)</option>
                                </select>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="erreserba-pertsonak">Mahaikide Kopurua (*):</label>
                                <input type="number" name="pertsona_kopurua" id="erreserba-pertsonak" class="form-control" min="1" max="20" value="2" required>
                            </div>
                            <div class="form-group">
                                <label for="erreserba-mahaia">Mahaia (*):</label>
                                <select name="mahaia_id" id="erreserba-mahaia" class="form-control" required>
                                    <?php if (empty($mahaiak)): ?>
                                        <option value="">Mahaiak kargatzen edo zerbitzaria itzalita...</option>
                                    <?php else: ?>
                                        <option value="">-- Hautatu mahaia --</option>
                                        <?php foreach ($mahaiak as $m): ?>
                                            <option value="<?php echo (int)$m['id']; ?>">
                                                Mahaia #<?php echo (int)$m['zenbakia']; ?> (<?php echo (int)$m['edukiera']; ?> pertsona - <?php echo htmlspecialchars($m['kokapena'] ?? 'Jantokia', ENT_QUOTES, 'UTF-8'); ?>)
                                            </option>
                                        <?php endforeach; ?>
                                    <?php endif; ?>
                                </select>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="bezero-izena">Izena (*):</label>
                                <input type="text" name="bezero_izena" id="bezero-izena" class="form-control" placeholder="Adib. Ane" required>
                            </div>
                            <div class="form-group">
                                <label for="bezero-abizena">Abizena:</label>
                                <input type="text" name="bezero_abizena" id="bezero-abizena" class="form-control" placeholder="Adib. Iparragirre">
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="bezero-emaila">E-posta helbidea (*):</label>
                                <input type="email" name="bezero_emaila" id="bezero-emaila" class="form-control" placeholder="ane@adibidea.eus" required>
                            </div>
                            <div class="form-group">
                                <label for="bezero-telefonoa">Telefonoa:</label>
                                <input type="tel" name="bezero_telefonoa" id="bezero-telefonoa" class="form-control" placeholder="600112233">
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="erreserba-oharrak">Oharrak edo Alergiak:</label>
                            <textarea name="erreserba_oharrak" id="erreserba-oharrak" class="form-control" rows="2" placeholder="Aulki gehigarria, ospakizuna, gluten intolerantea..."></textarea>
                        </div>

                        <button type="submit" id="btn-erreserbatu" class="btn btn-primary" style="width: 100%;">Erreserba Baieztatu (PHP bidez)</button>
                    </form>
                </div>

                <!-- Informazio osagarria eta arauak -->
                <div class="erreserba-info-area">
                    <div>
                        <h3>Jantokiko Arauak eta Ordutegiak</h3>
                        <p style="font-size: 0.9rem; color: #64748b; margin-bottom: 1rem;">
                            Gure zerbitzua ahalik eta ondoen antolatzeko eta elikagaien alferrik galtzea saihesteko, mahai bakoitza txandaka kudeatzen da:
                        </p>
                        <ul class="txanden-ordutegiak">
                            <li><span>Bazkaria 1:</span> <strong>13:00 - 14:30</strong></li>
                            <li><span>Bazkaria 2:</span> <strong>14:45 - 16:30</strong></li>
                            <li><span>Afaria 1:</span> <strong>20:30 - 22:00</strong></li>
                            <li><span>Afaria 2:</span> <strong>22:15 - 00:00</strong></li>
                        </ul>
                    </div>

                    <div class="oharra-kaxa">
                        <strong>Garrantzitsua:</strong> Mahai bererako eta txanda bererako eskaera bat baino gehiago jasotzen bada aldi berean, C# APIak lehentasuna lehenengoari emango dio (409 Conflict errorearen kudeaketa transakzionala). Gatazka gertatuz gero, sistemak abisu garbia erakutsiko du.
                    </div>
                </div>
            </div>
        </section>
