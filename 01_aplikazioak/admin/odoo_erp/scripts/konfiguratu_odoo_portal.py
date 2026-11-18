import sys

# Company eguneratu
company = env['res.company'].browse(1)
company.write({
    'name': '1T Jatetxea S.L.',
    'email': 'info@t1jatetxea.eus',
    'phone': '+34 943 12 34 56',
    'street': 'Goierri Kalea 10',
    'city': 'Ordizia',
    'zip': '20240',
    'website': 'http://localhost:8070',
})

# Website izena eguneratu
website = env['website'].browse(1)
website.write({
    'name': '1T Jatetxea · Osteria & Parrilla',
    'social_facebook': 'https://facebook.com',
    'social_twitter': 'https://twitter.com',
    'social_linkedin': 'https://linkedin.com',
})

# Admin pasahitza bermatu (admin)
admin_user = env['res.users'].browse(2)
admin_user.write({
    'name': 'Ander Urien (IT Lead)',
    'password': 'admin',
    'email': 'ander.urien@goierrieskola.eus',
})

# Homepage diseinua ezarri (Goierri Tech itxura bikaina jatetxera egokituta)
home_html = """<t name="Homepage" t-name="website.homepage">
    <t t-call="website.layout">
        <t t-set="pageName" t-value="'homepage'"/>
        <div id="wrap" class="oe_structure oe_empty">
            <section class="s_cover parallax s_parallax_is_fixed bg-900 pt120 pb120 text-center" style="background: linear-gradient(135deg, #0d1b2a 0%, #1b263b 50%, #415a77 100%); color: #ffffff;" data-snippet="s_cover">
                <div class="container">
                    <div class="row justify-content-center">
                        <div class="col-lg-10">
                            <span class="badge rounded-pill text-uppercase px-3 py-2 mb-4" style="background-color: #6c5b7b; color: #ffffff; letter-spacing: 1.5px; font-size: 13px;">
                                1T JATETXEA S.L. — OSTERIA &amp; PARRILLA ITALO-EUSKALDUNA
                            </span>
                            <h1 class="display-3 fw-bold mb-4" style="color: #ffffff; font-family: 'Cinzel', serif, system-ui; letter-spacing: -0.5px;">
                                Euskal Parrilla eta Italiar Gastronomia Goi-Mailako Kudeaketarekin
                            </h1>
                            <p class="lead mb-5 mx-auto" style="color: #cbd5e1; max-width: 820px; font-size: 1.25rem; line-height: 1.7;">
                                Gipuzkoako Goierri eskualdetik sortutako erronka integratua: lehengai bikainak, parrillako artisautza eta Odoo 17 ERP bidezko langile, fitxaketa, opor eta kontratuen kudeaketa integrala.
                            </p>
                            <div class="d-flex flex-wrap justify-content-center gap-3">
                                <a href="/web/login" class="btn btn-primary btn-lg px-4 py-3 shadow" style="background-color: #84596b; border-color: #84596b; font-weight: 600;">
                                    Sartu Odoo ERP Sisteman
                                </a>
                                <a href="http://localhost:8085" target="_blank" class="btn btn-outline-light btn-lg px-4 py-3" style="font-weight: 600;">
                                    Jatetxearen Karta eta Webgunea
                                </a>
                                <a href="/contactus" class="btn btn-secondary btn-lg px-4 py-3" style="background-color: #5c6b73; border-color: #5c6b73; font-weight: 600;">
                                    Kontaktua
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section class="py-5" style="background-color: #f8fafc; border-bottom: 1px solid #e2e8f0;">
                <div class="container">
                    <div class="row text-center g-4">
                        <div class="col-6 col-md-3">
                            <div class="display-4 fw-bold" style="color: #6c5b7b;">35</div>
                            <div class="text-muted text-uppercase fw-semibold" style="letter-spacing: 1px;">Karta Platerak</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="display-4 fw-bold" style="color: #6c5b7b;">15</div>
                            <div class="text-muted text-uppercase fw-semibold" style="letter-spacing: 1px;">Langile Plantilla</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="display-4 fw-bold" style="color: #6c5b7b;">2026</div>
                            <div class="text-muted text-uppercase fw-semibold" style="letter-spacing: 1px;">Goierri Erronka</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="display-4 fw-bold" style="color: #6c5b7b;">%100</div>
                            <div class="text-muted text-uppercase fw-semibold" style="letter-spacing: 1px;">Artisau Kudeaketa</div>
                        </div>
                    </div>
                </div>
            </section>

            <section class="py-5" style="background-color: #ffffff;">
                <div class="container py-4">
                    <div class="text-center mb-5">
                        <h2 class="fw-bold" style="color: #1e293b;">Jatetxearen Sail Integratuak Odoo 17 ERP-an</h2>
                        <p class="text-muted">Sail bakoitzeko langileak, kontratuak, fitxaketak eta bajak zentralizatuta daude</p>
                    </div>
                    <div class="row g-4">
                        <div class="col-md-4">
                            <div class="card h-100 shadow-sm border-0 p-4" style="background-color: #f1f5f9; border-radius: 12px;">
                                <h4 class="fw-bold text-dark mb-2">Sukaldea &amp; Parrilla</h4>
                                <p class="text-muted mb-0">Chef Burua (Ainhoa Goikoetxea) eta Bigarren Sukaldaria (Eneko Otegi). KDS Android TV pantailarekin denbora errealeko komunikazioa.</p>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="card h-100 shadow-sm border-0 p-4" style="background-color: #f1f5f9; border-radius: 12px;">
                                <h4 class="fw-bold text-dark mb-2">Jantokia &amp; Barra</h4>
                                <p class="text-muted mb-0">Zerbitzari Burua (Mikel Arregi), Zerbitzariak (Maite, Unai) eta Barrakoak (Gorka, Uxue). Eskaeren eta erreserben arreta.</p>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="card h-100 shadow-sm border-0 p-4" style="background-color: #f1f5f9; border-radius: 12px;">
                                <h4 class="fw-bold text-dark mb-2">Sistemak IT &amp; Zuzendaritza</h4>
                                <p class="text-muted mb-0">IT Taldea (Ander Urien, Julen, Iker, Aratz) eta Zuzendariak (Ander Madinabeitia, Joseba Ramos, Estebe Ormazabal).</p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </t>
</t>"""

views = env['ir.ui.view'].search([('key', '=', 'website.homepage')])
for v in views:
    v.write({'arch_db': {'en_US': home_html}})

env.cr.commit()
print("Odoo 17 Jatetxea Web Portala ONDO KONFIGURATUTA DAGO!")
