# -*- coding: utf-8 -*-
"""
1T Jatetxea S.L. - Odoo 17 ERP Konfigurazio Integrala (ORM Script)
=================================================================
Exekuzioa Docker barrutik:
  docker exec -i jatetxea_odoo_erp odoo shell -d jatetxea_odoo --no-http < konfiguratu_odoo_osoa.py

Zer konfiguratzen du script idempotente honek?
  1. Enpresa (res.company) eta Webgunea (website): 1T Jatetxea S.L. logoa, favicona,
     Goierri Kalea 10 Ordizia helbidea, +34 943 12 34 56 telefonoa, IFZ eta UTF-8 izenak.
  2. Web Portala (http://localhost:8070): T1 Jatetxea v2.0 estilo diseinu gidarekin
     (Chianti #881337, Gold #D4A359, Warm Hospitality #FAF7F2, Evening Espresso #0F0D0E)
     goiburua, menuak, hasiera orria eta oina.
  3. Sailen hierarkia zuzena (6 sailak Zuzendaritzaren azpian, ez IT-ren azpian).
  4. Lan-kokalekuak, ordutegiak, etiketak, bankuak, gaitasunak eta CV lerroak.
  5. 15 langileen fitxa integralak: argazki korporatiboa, hierarkia, datu pribatuak,
     helbideak, IBAN, larrialdi kontaktua, ikasketak, NAN, Gizarte Segurantza,
     jaioteguna, PIN ofiziala (TPV/Kiosk sinkronizatua) eta Badge ID.
  6. 15 lan-kontratu aktibo, azken asteko fitxaketak eta 2026ko opor/baja eskaerak.
"""

import base64
import os
from typing import Any

# Odoo shell-ek 'env' objektua globalki injektatzen du exekuzioan
env: Any = globals().get("env")
if env is None:
    raise RuntimeError(
        "Script hau 'odoo shell' barrutik exekutatu behar da:\n"
        "  docker exec -i jatetxea_odoo_erp odoo shell -c /etc/odoo/odoo.conf -d jatetxea_odoo --no-http < konfiguratu_odoo_osoa.py"
    )

IMG_BASE = "/mnt/extra-addons/jatetxea_hr/static/src/img"


def read_b64(rel_path):
    full_path = os.path.join(IMG_BASE, rel_path)
    with open(full_path, "rb") as f:
        return base64.b64encode(f.read()).decode("ascii")


print("=== [1/7] Enpresa (res.company) eta Webgunea (website) konfiguratzen ===")
logo_official_b64 = read_b64("t1_logo_official.png")
logo_dark_b64 = read_b64("t1_logo_header_dark.png")
favicon_b64 = read_b64("t1_favicon.png")

spain = env["res.country"].search([("code", "=", "ES")], limit=1)
gipuzkoa = env["res.country.state"].search([("country_id", "=", spain.id), ("code", "=", "SS")], limit=1)

company = env["res.company"].browse(1)
company.write({
    "name": "1T Jatetxea S.L.",
    "email": "info@t1jatetxea.eus",
    "phone": "+34 943 12 34 56",
    "street": "Goierri Kalea 10",
    "city": "Ordizia",
    "zip": "20240",
    "state_id": gipuzkoa.id,
    "country_id": spain.id,
    "vat": "ESB20987654",
    "website": "http://localhost:8070",
    "logo": logo_official_b64,
    "report_header": "1T Jatetxea S.L. — Osteria & Parrilla Italo-Euskalduna · Ordizia",
})

company.partner_id.write({
    "name": "1T Jatetxea S.L.",
    "email": "info@t1jatetxea.eus",
    "phone": "+34 943 12 34 56",
    "street": "Goierri Kalea 10",
    "city": "Ordizia",
    "zip": "20240",
    "state_id": gipuzkoa.id,
    "country_id": spain.id,
    "vat": "ESB20987654",
    "website": "http://localhost:8070",
})

websites = env["website"].search([])
for ws in websites:
    ws.write({
        "name": "1T Jatetxea · Osteria & Parrilla",
        "logo": logo_dark_b64,
        "favicon": favicon_b64,
        "company_id": company.id,
        "social_facebook": "https://facebook.com",
        "social_twitter": "https://twitter.com",
        "social_linkedin": "https://linkedin.com",
        "social_instagram": "https://instagram.com",
    })

# Update website menus cleanly into Basque
for menu in env["website.menu"].search([]):
    if menu.url == "/":
        menu.write({"name": "Hasiera"})
    elif menu.url == "/contactus":
        menu.write({"name": "Kontaktua"})

# Add "Odoo ERP" menu item to Website 1 if not present
top_menu_1 = env["website.menu"].search([("website_id", "=", 1), ("url", "=", "/default-main-menu")], limit=1)
if top_menu_1:
    erp_menu = env["website.menu"].search([("website_id", "=", 1), ("url", "=", "/web")], limit=1)
    if not erp_menu:
        env["website.menu"].create({
            "name": "Langileak ERP",
            "url": "/web",
            "website_id": 1,
            "parent_id": top_menu_1.id,
            "sequence": 30,
        })

print("=== [2/7] Web Portala (Header, Footer, Homepage) T1 v2.0 diseinuarekin eguneratzen ===")

# Fix header phone placeholder (+1 555-555-5556 -> +34 943 12 34 56)
for view in env["ir.ui.view"].search([("key", "=", "website.header_text_element")]):
    arch = view.arch_db or ""
    if "+1 555-555-5556" in arch:
        new_arch = arch.replace("tel:+1 555-555-5556", "tel:+34943123456").replace("+1 555-555-5556", "+34 943 12 34 56")
        view.write({"arch_db": new_arch})

# Update header CTA button (Contact Us -> Sartu ERP-ra)
for view in env["ir.ui.view"].search([("key", "=", "website.header_call_to_action")]):
    arch = view.arch_db or ""
    new_arch = arch.replace('href="/contactus"', 'href="/web/login"').replace("Contact Us", "Sartu ERP-ra")
    if new_arch != arch:
        view.write({"arch_db": new_arch})

# Replace default footer with 1T Jatetxea S.L. Evening Espresso & Gold Footer
footer_arch = """<data inherit_id="website.layout" name="Default" active="True">
    <xpath expr="//div[@id='footer']" position="replace">
        <div id="footer" class="oe_structure oe_structure_solo" t-ignore="true" t-if="not no_footer">
            <section class="s_text_block pt48 pb24" data-snippet="s_text_block" data-name="1T Footer" style="background-color: #1a1618; color: #a8a29e;">
                <div class="container">
                    <div class="row g-4">
                        <div class="col-lg-5">
                            <div class="d-flex align-items-center gap-3 mb-3">
                                <img src="/jatetxea_hr/static/src/img/t1_favicon.png" alt="1T Jatetxea" style="width: 44px; height: 44px; border-radius: 10px;"/>
                                <div>
                                    <h5 class="mb-0" style="font-family: 'Cinzel', Georgia, serif; color: #d4a359; font-weight: 800; letter-spacing: 1.4px;">1T JATETXEA S.L.</h5>
                                    <small style="color: #e7e5e4; letter-spacing: 1.5px; text-transform: uppercase; font-size: 0.7rem;">Osteria &amp; Parrilla · Italo-Euskalduna</small>
                                </div>
                            </div>
                            <p style="font-size: 0.92rem; line-height: 1.65; color: #a8a29e;">
                                Bi tradizio, artisautza bakarra. Euskal parrillako txingarrak eta italiar sukaldaritza tradizionala, Odoo 17 ERP bidezko giza baliabideen kudeaketa profesionalarekin uztartuta.
                            </p>
                        </div>
                        <div class="col-lg-3">
                            <h6 class="text-uppercase fw-bold mb-3" style="color: #d4a359; letter-spacing: 1.2px; font-size: 0.8rem;">Atal Nagusiak</h6>
                            <ul class="list-unstyled" style="line-height: 2;">
                                <li><a href="/" style="text-decoration: none;">Hasiera Portala</a></li>
                                <li><a href="/web/login" style="text-decoration: none;">Odoo 17 ERP Sarbidea</a></li>
                                <li><a href="/web" style="text-decoration: none;">Langileak (HR Modulua)</a></li>
                                <li><a href="/contactus" style="text-decoration: none;">Kontaktua eta Kokapena</a></li>
                            </ul>
                        </div>
                        <div class="col-lg-4">
                            <h6 class="text-uppercase fw-bold mb-3" style="color: #d4a359; letter-spacing: 1.2px; font-size: 0.8rem;">Kontaktua eta Egoitza</h6>
                            <ul class="list-unstyled" style="line-height: 2; font-size: 0.92rem;">
                                <li><i class="fa fa-map-marker fa-fw me-2" style="color: #d4a359;"/>Goierri Kalea 10, 20240 Ordizia (Gipuzkoa)</li>
                                <li><i class="fa fa-phone fa-fw me-2" style="color: #d4a359;"/><a href="tel:+34943123456" style="text-decoration: none;">+34 943 12 34 56</a></li>
                                <li><i class="fa fa-envelope fa-fw me-2" style="color: #d4a359;"/><a href="mailto:info@t1jatetxea.eus" style="text-decoration: none;">info@t1jatetxea.eus</a></li>
                                <li><i class="fa fa-building fa-fw me-2" style="color: #d4a359;"/>IFZ / CIF: B20987654 · Goierri Eskola 2026</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </xpath>
</data>"""

for view in env["ir.ui.view"].search([("key", "=", "website.footer_custom")]):
    view.write({"arch_db": footer_arch})

# Replace homepage with T1 Jatetxea v2.0 Style Guide design
home_html = """<t name="Homepage" t-name="website.homepage">
    <t t-call="website.layout">
        <t t-set="pageName" t-value="'homepage'"/>
        <div id="wrap" class="oe_structure oe_empty">
            <!-- HERO: Evening Espresso & Chianti Editorial -->
            <section class="t1-hero text-center" data-snippet="s_cover">
                <div class="container">
                    <div class="row justify-content-center">
                        <div class="col-lg-10">
                            <div class="mb-4">
                                <span class="t1-hero-badge">
                                    1T JATETXEA S.L. · OSTERIA &amp; PARRILLA ITALO-EUSKALDUNA
                                </span>
                            </div>
                            <h1 class="display-4 t1-hero-title mb-4">
                                Euskal Parrilla eta Italiar Gastronomia<br/>Goi-Mailako ERP Kudeaketarekin
                            </h1>
                            <p class="lead mb-5 mx-auto" style="color: #e7e5e4; max-width: 780px; font-size: 1.15rem; line-height: 1.75;">
                                Gipuzkoako Goierri eskualdetik sortutako erronka integratua: bi tradizio eta artisautza bakarra, Odoo 17 Community ERP bidezko langile fitxa, organigrama, lan-kontratu, fitxaketa eta oporren kudeaketa zentralizatuarekin.
                            </p>
                            <div class="d-flex flex-wrap justify-content-center gap-3">
                                <a href="/web/login" class="btn t1-btn-gold btn-lg">
                                    <i class="fa fa-sign-in me-2"/>Sartu Odoo ERP Sisteman
                                </a>
                                <a href="http://localhost:8085" target="_blank" class="btn t1-btn-outline-gold btn-lg">
                                    <i class="fa fa-cutlery me-2"/>Jatetxearen Karta eta Webgunea
                                </a>
                                <a href="/contactus" class="btn t1-btn-outline-gold btn-lg">
                                    <i class="fa fa-envelope-o me-2"/>Kontaktua
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- KPI STRIP -->
            <section class="t1-kpi-strip py-5">
                <div class="container">
                    <div class="row text-center g-4">
                        <div class="col-6 col-md-3">
                            <div class="t1-kpi-value">15</div>
                            <div class="t1-kpi-label">Langile Plantilla</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="t1-kpi-value">6</div>
                            <div class="t1-kpi-label">Sail Hierarkizatuak</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="t1-kpi-value">15</div>
                            <div class="t1-kpi-label">Kontratu Aktiboak</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="t1-kpi-value">35</div>
                            <div class="t1-kpi-label">Karta Platerak</div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 6 DEPARTMENTS SHOWCASE -->
            <section class="t1-section-warm py-5">
                <div class="container py-4">
                    <div class="text-center mb-5">
                        <h2 class="t1-section-title h1">Jatetxearen Sail Integratuak Odoo 17 ERP-an</h2>
                        <div class="t1-title-underline"/>
                        <p class="mt-3 mx-auto" style="color: #57534e; max-width: 700px;">
                            Langileak (<code>hr</code>) moduluaren bidez 1T Jatetxea S.L.-ko 6 sailak, arduradunak, lanaldiak, gaitasunak eta TPV/Kiosk PIN kredentzialak modu bateratuan kudeatzen dira.
                        </p>
                    </div>
                    <div class="row g-4">
                        <div class="col-md-6 col-lg-4">
                            <div class="card t1-card h-100 p-4">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="fw-bold mb-0" style="color: #1c1917;">Zuzendaritza eta Administrazioa</h5>
                                    <span class="t1-mono-pill">3 langile</span>
                                </div>
                                <p class="mb-2" style="color: #57534e; font-size: 0.92rem;">
                                    Zuzendaritza estrategikoa, kontratuen onarpena eta giza baliabideen gainbegiratzea.
                                </p>
                                <small style="color: #78716c;"><strong>Arduraduna:</strong> Joseba Ramos (CEO) · Ander Madinabeitia · Estebe Ormazabal</small>
                            </div>
                        </div>
                        <div class="col-md-6 col-lg-4">
                            <div class="card t1-card h-100 p-4" style="border-top-color: #0369a1 !important;">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="fw-bold mb-0" style="color: #1c1917;">Sistemak, Sareak eta IT</h5>
                                    <span class="t1-mono-pill">4 langile</span>
                                </div>
                                <p class="mb-2" style="color: #57534e; font-size: 0.92rem;">
                                    Odoo 17 Docker azpiegitura, OPNsense suebakia, VLAN 10, REST API eta zibersegurtasuna.
                                </p>
                                <small style="color: #78716c;"><strong>Arduraduna:</strong> Ander Urien · Julen Aguado · Iker Araujo · Aratz Huegun</small>
                            </div>
                        </div>
                        <div class="col-md-6 col-lg-4">
                            <div class="card t1-card h-100 p-4" style="border-top-color: #c2410c !important;">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="fw-bold mb-0" style="color: #1c1917;">Sukaldea eta Parrilla</h5>
                                    <span class="t1-mono-pill">2 langile</span>
                                </div>
                                <p class="mb-2" style="color: #57534e; font-size: 0.92rem;">
                                    Euskal txingarra, italiar egur-labea, KDS Android TV komandak eta APPCC kalitate kontrola.
                                </p>
                                <small style="color: #78716c;"><strong>Arduraduna:</strong> Ainhoa Goikoetxea (Chef) · Eneko Otegi (Parrilleroa)</small>
                            </div>
                        </div>
                        <div class="col-md-6 col-lg-4">
                            <div class="card t1-card h-100 p-4" style="border-top-color: #15803d !important;">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="fw-bold mb-0" style="color: #1c1917;">Jantokia eta Zerbitzua</h5>
                                    <span class="t1-mono-pill">3 langile</span>
                                </div>
                                <p class="mb-2" style="color: #57534e; font-size: 0.92rem;">
                                    Mahaien arreta, sommelier zerbitzua, PDA mugikorreko komandak eta C# TPV kobrantzak.
                                </p>
                                <small style="color: #78716c;"><strong>Arduraduna:</strong> Mikel Arregi (Maître) · Maite Zabaleta · Unai Lasa</small>
                            </div>
                        </div>
                        <div class="col-md-6 col-lg-4">
                            <div class="card t1-card h-100 p-4" style="border-top-color: #b45309 !important;">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="fw-bold mb-0" style="color: #1c1917;">Barra eta Kafe-Gunea</h5>
                                    <span class="t1-mono-pill">2 langile</span>
                                </div>
                                <p class="mb-2" style="color: #57534e; font-size: 0.92rem;">
                                    Edarien karta, italiar espressoa, kokteleria eta Barrako BDS pantailaren kudeaketa.
                                </p>
                                <small style="color: #78716c;"><strong>Arduraduna:</strong> Gorka Mendizabal (Barman) · Uxue Etxarri</small>
                            </div>
                        </div>
                        <div class="col-md-6 col-lg-4">
                            <div class="card t1-card h-100 p-4" style="border-top-color: #57534e !important;">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="fw-bold mb-0" style="color: #1c1917;">Biltegia eta Hornidura</h5>
                                    <span class="t1-mono-pill">1 langile</span>
                                </div>
                                <p class="mb-2" style="color: #57534e; font-size: 0.92rem;">
                                    Lehengaien inbentarioa, hotz-katearen kontrola, hornitzaileen albaranak eta erosketa eskaerak.
                                </p>
                                <small style="color: #78716c;"><strong>Arduraduna:</strong> Kepa Aldasoro (Stock Arduraduna)</small>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- HR MODULE ECOSYSTEM SECTION -->
            <section class="py-5" style="background-color: #ffffff; border-top: 1px solid #e6dfd5;">
                <div class="container py-4">
                    <div class="text-center mb-5">
                        <h2 class="t1-section-title h2">Langileak (Employees) Moduluaren Gaitasunak</h2>
                        <div class="t1-title-underline"/>
                    </div>
                    <div class="row g-4">
                        <div class="col-md-3">
                            <div class="p-4 rounded-3 h-100" style="background-color: #faf7f2; border: 1px solid #e6dfd5;">
                                <div class="mb-2" style="color: #881337; font-size: 1.5rem;"><i class="fa fa-id-card-o"/></div>
                                <h6 class="fw-bold" style="color: #1c1917;">Fitxa Integralak eta CV</h6>
                                <p class="small mb-0" style="color: #57534e;">Datu pertsonalak, NAN, Gizarte Segurantza, IBAN, larrialdi kontaktua, ikasketak eta gaitasun mailak.</p>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="p-4 rounded-3 h-100" style="background-color: #faf7f2; border: 1px solid #e6dfd5;">
                                <div class="mb-2" style="color: #881337; font-size: 1.5rem;"><i class="fa fa-sitemap"/></div>
                                <h6 class="fw-bold" style="color: #1c1917;">Organigrama Hierarkikoa</h6>
                                <p class="small mb-0" style="color: #57534e;">Zuzendaritzatik sail bakoitzeko arduradun eta langileetarainoko aginte-katea argi egituratuta.</p>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="p-4 rounded-3 h-100" style="background-color: #faf7f2; border: 1px solid #e6dfd5;">
                                <div class="mb-2" style="color: #881337; font-size: 1.5rem;"><i class="fa fa-clock-o"/></div>
                                <h6 class="fw-bold" style="color: #1c1917;">Fitxaketak eta PIN Kioskoa</h6>
                                <p class="small mb-0" style="color: #57534e;">Sarrera-irteeren erregistroa eta 4 digituko PIN ofizialak TPV sistemarekin bateratuta.</p>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="p-4 rounded-3 h-100" style="background-color: #faf7f2; border: 1px solid #e6dfd5;">
                                <div class="mb-2" style="color: #881337; font-size: 1.5rem;"><i class="fa fa-file-text-o"/></div>
                                <h6 class="fw-bold" style="color: #1c1917;">Kontratuak eta Oporrak</h6>
                                <p class="small mb-0" style="color: #57534e;">Soldata gordinak, Gipuzkoako Ostalaritza Hitzarmena, urteko 23 opor egun eta mediku bajak.</p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </t>
</t>"""

for view in env["ir.ui.view"].search([("key", "=", "website.homepage")]):
    view.write({"arch_db": home_html})

print("=== [3/7] Sailen hierarkia, lan-kokalekuak, ordutegiak, etiketak eta bankuak ===")

# 6 independent top-level functional departments (reporting hierarchy is handled via employee parent_id / Org Chart)
dept_zuz = env["hr.department"].browse(8)
dept_it = env["hr.department"].browse(9)
dept_suk = env["hr.department"].browse(10)
dept_jan = env["hr.department"].browse(11)
dept_bar = env["hr.department"].browse(12)
dept_bil = env["hr.department"].browse(13)

dept_zuz.write({"name": "Zuzendaritza eta Administrazioa", "parent_id": False, "manager_id": 22})
dept_it.write({"name": "Sistemak, Sareak eta IT", "parent_id": False, "manager_id": 24})
dept_suk.write({"name": "Sukaldea", "parent_id": False, "manager_id": 28})
dept_jan.write({"name": "Jantokia eta Zerbitzua", "parent_id": False, "manager_id": 30})
dept_bar.write({"name": "Barra eta Kafe-Gunea", "parent_id": False, "manager_id": 33})
dept_bil.write({"name": "Biltegia eta Hornidura", "parent_id": False, "manager_id": 35})

# Work locations
def get_or_create_loc(name, loc_type="office"):
    loc = env["hr.work.location"].search([("name", "=", name)], limit=1)
    if not loc:
        loc = env["hr.work.location"].create({
            "name": name,
            "address_id": company.partner_id.id,
            "location_type": loc_type,
        })
    return loc

loc_bulegoa = get_or_create_loc("Bulegoa eta Zuzendaritza (1. Solairua)", "office")
loc_cpd = get_or_create_loc("IT Zerbitzari Gela (CPD - VLAN 10)", "office")
loc_sukaldea = get_or_create_loc("Sukalde Nagusia eta Euskal Parrilla", "other")
loc_jantokia = get_or_create_loc("Jantoki Nagusia (Osteria)", "other")
loc_barra = get_or_create_loc("Barra eta Kafe-Gunea", "other")
loc_biltegia = get_or_create_loc("Biltegia eta Hotz-Kamerak", "other")

# Resource calendars
cal_40_office = env["resource.calendar"].browse(1)
cal_40_office.write({"name": "Lanaldi Osoa 40h (Astelehena-Ostirala)"})

cal_40_hosp = env["resource.calendar"].search([("name", "=", "Ostalaritza 40h (Asteartea-Igandea)")], limit=1)
if not cal_40_hosp:
    cal_40_hosp = env["resource.calendar"].create({
        "name": "Ostalaritza 40h (Asteartea-Igandea)",
        "hours_per_day": 8.0,
        "tz": "Europe/Madrid",
    })

cal_20_part = env["resource.calendar"].search([("name", "=", "Lanaldi Partziala 20h (Asteburuak)")], limit=1)
if not cal_20_part:
    cal_20_part = env["resource.calendar"].create({
        "name": "Lanaldi Partziala 20h (Asteburuak)",
        "hours_per_day": 6.67,
        "tz": "Europe/Madrid",
    })

# Employee Tags (hr.employee.category)
tag_defs = [
    ("Zuzendaritza", 1),
    ("IT Admin", 4),
    ("Sukaldea", 2),
    ("Jantokia", 10),
    ("Barra", 3),
    ("Biltegia", 5),
    ("Elikagai-Manipulatzailea", 10),
    ("Lehen Sorospenak", 9),
    ("HR Kudeatzailea", 6),
    ("Goizeko Txanda", 7),
    ("Arratsaldeko Txanda", 8),
    ("Lanaldi Partziala", 11),
]
tags = {}
for tname, tcolor in tag_defs:
    t = env["hr.employee.category"].search([("name", "=", tname)], limit=1)
    if not t:
        t = env["hr.employee.category"].create({"name": tname, "color": tcolor})
    else:
        t.write({"color": tcolor})
    tags[tname] = t.id

# Basque Banks
def get_or_create_bank(name, bic):
    b = env["res.bank"].search([("name", "=", name)], limit=1)
    if not b:
        b = env["res.bank"].create({"name": name, "bic": bic, "country": spain.id})
    return b

bank_kutxa = get_or_create_bank("Kutxabank", "BASKES2B")
bank_laboral = get_or_create_bank("Laboral Kutxa", "CLPEES2M")
bank_sabadell = get_or_create_bank("Banco Sabadell", "BSABESBB")

print("=== [4/7] Gaitasunak (hr.skill) eta mailak prestatzen ===")
def ensure_skill_type(name, skills_list):
    st = env["hr.skill.type"].search([("name", "=", name)], limit=1)
    if not st:
        st = env["hr.skill.type"].create({"name": name})
    # Ensure 4 clean levels
    levels = {}
    level_specs = [
        ("Oinarrizkoa", 35, False),
        ("Ertaina", 65, False),
        ("Aurreratua", 85, True),
        ("Aditua", 100, False),
    ]
    for lname, prog, is_def in level_specs:
        lv = env["hr.skill.level"].search([("skill_type_id", "=", st.id), ("name", "=", lname)], limit=1)
        if not lv:
            lv = env["hr.skill.level"].create({
                "name": lname,
                "level_progress": prog,
                "skill_type_id": st.id,
                "default_level": is_def,
            })
        levels[lname] = lv.id
    skill_map = {}
    for sname in skills_list:
        sk = env["hr.skill"].search([("skill_type_id", "=", st.id), ("name", "=", sname)], limit=1)
        if not sk:
            sk = env["hr.skill"].create({"name": sname, "skill_type_id": st.id})
        skill_map[sname] = (st.id, sk.id)
    return st.id, skill_map, levels

_, sk_lang, lv_lang = ensure_skill_type("Hizkuntzak (1T)", ["Euskara", "Gaztelania", "Ingelesa", "Italiera", "Frantsesa"])
_, sk_gastro, lv_gastro = ensure_skill_type("Ostalaritza eta Sukaldaritza", [
    "Euskal Parrilla eta Txingarra",
    "Italiar Sukaldaritza eta Labea",
    "APPCC eta Alergenoen Kontrola",
    "Sommelier eta Ardo Karta",
    "Bezero Arreta eta Protokoloa",
    "Kokteleria eta Barista",
    "Stock eta Hornitzaile Kudeaketa",
    "TPV eta Komandera Digitala",
])
_, sk_it, lv_it = ensure_skill_type("Sistemak eta IT (1T)", [
    "Odoo 17 ERP eta PostgreSQL",
    "Docker eta Birtualizazioa",
    "OPNsense, Sareak eta VLAN",
    "Zibersegurtasuna eta Auditoria",
    "ASP.NET Core, JavaFX eta Flutter",
    "Windows Server eta Active Directory",
])
_, sk_mgmt, lv_mgmt = ensure_skill_type("Kudeaketa eta Lidergoa", [
    "Enpresa Zuzendaritza eta Estrategia",
    "Giza Baliabideak eta Lan-Kontratuak",
    "Finantza eta Kostuen Kontrola",
    "Talde Lidergoa eta Koord.",
])

all_skills = {}
all_skills.update(sk_lang)
all_skills.update(sk_gastro)
all_skills.update(sk_it)
all_skills.update(sk_mgmt)

all_levels = {
    sk_lang["Euskara"][0]: lv_lang,
    sk_gastro["Euskal Parrilla eta Txingarra"][0]: lv_gastro,
    sk_it["Odoo 17 ERP eta PostgreSQL"][0]: lv_it,
    sk_mgmt["Enpresa Zuzendaritza eta Estrategia"][0]: lv_mgmt,
}

# Resume line types
rt_exp = env["hr.resume.line.type"].search([("name", "=", "Experience")], limit=1)
rt_edu = env["hr.resume.line.type"].search([("name", "=", "Education")], limit=1)
rt_cert = env["hr.resume.line.type"].search([("name", "=", "Ziurtagiriak eta Proiektuak")], limit=1)
if not rt_cert:
    rt_cert = env["hr.resume.line.type"].create({"name": "Ziurtagiriak eta Proiektuak", "sequence": 3})

print("=== [5/7] 15 Langileen fitxa integralak betetzen (Datu Pribatuak, Lanekoak, HR, CV, Gaitasunak) ===")

# Complete realistic dataset for all 15 employees
EMP_DATA = {
    22: {  # Joseba Ramos — CEO
        "job_title": "Jatetxearen Zuzendari Nagusia (CEO)",
        "parent_id": False, "coach_id": False,
        "work_phone": "+34 943 12 34 50", "mobile_phone": "+34 633 11 22 34",
        "work_location_id": loc_bulegoa.id, "calendar_id": cal_40_office.id,
        "tags": ["Zuzendaritza", "HR Kudeatzailea", "Goizeko Txanda"],
        "private_street": "Nagusia Kalea 18, 3.B", "private_zip": "20240", "private_city": "Ordizia",
        "private_email": "joseba.ramos.g@gmail.com", "private_phone": "+34 633 11 22 34",
        "bank": bank_kutxa, "iban": "ES14 2095 5041 10 9102345601",
        "km": 1, "plate": "4821 KLM", "marital": "married", "children": 2,
        "emerg_name": "Nerea Ramos (Emaztea)", "emerg_phone": "+34 633 98 11 02",
        "cert": "master", "study_field": "Enpresen Administrazioa eta Zuzendaritza (MBA)", "study_school": "Deustuko Unibertsitatea",
        "ssnid": "20/10458921/34", "gender": "male", "birthday": "1979-04-12", "birth_place": "Ordizia",
        "pin": "5002", "barcode": "1TJ-EMP-001", "hourly_cost": 26.50,
        "contract_type": "Permanent", "contract_note": "Zuzendaritza Nagusiko kontratu mugagabea. 40h/astean. Giza Baliabideen arduradun nagusia.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"),
                   ("Enpresa Zuzendaritza eta Estrategia", "Aditua"), ("Giza Baliabideak eta Lan-Kontratuak", "Aditua")],
        "resume": [
            ("experience", "Zuzendari Nagusia (CEO) — 1T Jatetxea S.L.", "2024-10-01", False, "Jatetxearen estrategia orokorra, aliantzak eta HR zuzendaritza."),
            ("experience", "Ostalaritza Kudeatzailea — Goierri Gastronomia Taldea", "2015-03-01", "2024-09-30", "3 jatetxeren finantza eta langile kudeaketa."),
            ("education", "MBA — Enpresa Zuzendaritza", "2003-09-01", "2005-06-30", "Deustuko Unibertsitatea (Donostia)."),
        ],
    },
    21: {  # Ander Madinabeitia — Zuzendaria (Eragiketak)
        "job_title": "Jatetxearen Zuzendaria (Eragiketak eta HR)",
        "parent_id": 22, "coach_id": 22,
        "work_phone": "+34 943 12 34 51", "mobile_phone": "+34 633 11 22 33",
        "work_location_id": loc_bulegoa.id, "calendar_id": cal_40_office.id,
        "tags": ["Zuzendaritza", "HR Kudeatzailea", "Lehen Sorospenak"],
        "private_street": "Loinazko San Martin Plaza 5, 2.A", "private_zip": "20200", "private_city": "Beasain",
        "private_email": "ander.madinabeitia@gmail.com", "private_phone": "+34 633 11 22 33",
        "bank": bank_kutxa, "iban": "ES28 2095 5042 20 9102345602",
        "km": 4, "plate": "7194 LNP", "marital": "married", "children": 1,
        "emerg_name": "Leire Etxeberria (Bikotekidea)", "emerg_phone": "+34 633 77 45 19",
        "cert": "bachelor", "study_field": "Gastronomia eta Sukaldaritza Arteetako Gradua", "study_school": "Basque Culinary Center",
        "ssnid": "20/10682314/78", "gender": "male", "birthday": "1983-09-25", "birth_place": "Beasain",
        "pin": "5001", "barcode": "1TJ-EMP-002", "hourly_cost": 26.50,
        "contract_type": "Permanent", "contract_note": "Eragiketa Zuzendaritzako kontratu mugagabea. Sukaldea, Jantokia eta Barraren gainbegiratzea.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Italiera", "Ertaina"),
                   ("Talde Lidergoa eta Koord.", "Aditua"), ("Giza Baliabideak eta Lan-Kontratuak", "Aurreratua")],
        "resume": [
            ("experience", "Eragiketa Zuzendaria — 1T Jatetxea S.L.", "2024-10-01", False, "Eguneroko zerbitzuaren koordinazioa eta langile txanden plangintza."),
            ("experience", "Jantoki eta Operazio Arduraduna — Donostia Bistro", "2016-05-01", "2024-09-15", "25 langileko taldearen zuzendaritza."),
            ("education", "Gastronomia eta Sukaldaritza Zuzendaritza", "2008-09-01", "2012-06-30", "Basque Culinary Center, Mondragon Unibertsitatea."),
        ],
    },
    23: {  # Estebe Ormazabal — Zuzendaria (Finantzak & Hornidura)
        "job_title": "Jatetxearen Zuzendaria (Finantzak eta Hornidura)",
        "parent_id": 22, "coach_id": 22,
        "work_phone": "+34 943 12 34 52", "mobile_phone": "+34 633 11 22 35",
        "work_location_id": loc_bulegoa.id, "calendar_id": cal_40_office.id,
        "tags": ["Zuzendaritza", "HR Kudeatzailea", "Goizeko Txanda"],
        "private_street": "Elosegi Kalea 22, 1.Ezk", "private_zip": "20210", "private_city": "Lazkao",
        "private_email": "estebe.ormazabal@gmail.com", "private_phone": "+34 633 11 22 35",
        "bank": bank_laboral, "iban": "ES42 3035 0018 30 0180045603",
        "km": 5, "plate": "3056 JRT", "marital": "cohabitant", "children": 1,
        "emerg_name": "Amaia Ormazabal (Arreba)", "emerg_phone": "+34 633 61 09 44",
        "cert": "bachelor", "study_field": "Ekonomia eta Finantza Zuzendaritza", "study_school": "UPV/EHU (Donostia)",
        "ssnid": "20/10791245/12", "gender": "male", "birthday": "1985-11-08", "birth_place": "Lazkao",
        "pin": "5003", "barcode": "1TJ-EMP-003", "hourly_cost": 26.50,
        "contract_type": "Permanent", "contract_note": "Finantza eta Hornidura Zuzendaritzako kontratu mugagabea. Hornitzaile eta aurrekontu kontrola.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"),
                   ("Finantza eta Kostuen Kontrola", "Aditua"), ("Stock eta Hornitzaile Kudeaketa", "Aurreratua")],
        "resume": [
            ("experience", "Finantza Zuzendaria — 1T Jatetxea S.L.", "2024-10-01", False, "Kontabilitatea, hornitzaileen negoziazioa eta kostuen analisia."),
            ("education", "Ekonomia Gradua", "2004-09-01", "2008-06-30", "Euskal Herriko Unibertsitatea (UPV/EHU)."),
        ],
    },
    24: {  # Ander Urien Telleria — IT Lead
        "job_title": "IT Lead / FullStack Arkitektoa",
        "parent_id": 22, "coach_id": 22,
        "work_phone": "+34 943 12 34 60", "mobile_phone": "+34 688 11 22 33",
        "work_location_id": loc_cpd.id, "calendar_id": cal_40_office.id,
        "tags": ["IT Admin", "HR Kudeatzailea", "Goizeko Txanda"],
        "private_street": "Arranomendia Kalea 4, 2.C", "private_zip": "20240", "private_city": "Ordizia",
        "private_email": "ander.urien.dev@gmail.com", "private_phone": "+34 688 11 22 33",
        "bank": bank_kutxa, "iban": "ES56 2095 5041 40 9102345604",
        "km": 2, "plate": "8912 MCV", "marital": "single", "children": 0,
        "emerg_name": "Iñaki Urien (Aita)", "emerg_phone": "+34 688 44 10 21",
        "cert": "graduate", "study_field": "Plataforma Anitzeko Aplikazioen Garapena (DAM) & ASIR", "study_school": "Goierri Eskola (Ordizia)",
        "ssnid": "20/11845610/91", "gender": "male", "birthday": "2003-03-18", "birth_place": "Ordizia",
        "pin": "0001", "barcode": "1TJ-EMP-004", "hourly_cost": 23.80,
        "contract_type": "Permanent", "contract_note": "IT Saileko buruaren kontratu mugagabea. Odoo 17 ERP, .NET REST API eta sistema osoko arkitektura.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"),
                   ("Odoo 17 ERP eta PostgreSQL", "Aditua"), ("ASP.NET Core, JavaFX eta Flutter", "Aditua"), ("Docker eta Birtualizazioa", "Aurreratua")],
        "resume": [
            ("experience", "IT Lead / FullStack Arkitektoa — 1T Jatetxea S.L.", "2026-09-01", False, "Odoo 17 ERP inplantazioa, ASP.NET Core 10 REST API, C# TPV eta Admin IT Zentrala."),
            ("education", "Goi Mailako Zikloa — Goierri Eskola", "2024-09-01", "2026-06-30", "Aplikazioen garapena eta enpresa kudeaketa sistemak (ERP-CRM)."),
            ("cert", "Odoo 17 ERP Inplantazioa eta Docker Orkestrazioa", "2026-10-01", False, "1T Jatetxea Erronka Integratuko azpiegitura osoaren arkitektura."),
        ],
    },
    25: {  # Julen Aguado Fernández — Sareak & Suebakia
        "job_title": "Sare eta Suebaki Administratzailea",
        "parent_id": 24, "coach_id": 24,
        "work_phone": "+34 943 12 34 61", "mobile_phone": "+34 688 11 22 34",
        "work_location_id": loc_cpd.id, "calendar_id": cal_40_office.id,
        "tags": ["IT Admin", "Goizeko Txanda"],
        "private_street": "Nafarroa Etorbidea 14, 4.D", "private_zip": "20200", "private_city": "Beasain",
        "private_email": "julen.aguado.net@gmail.com", "private_phone": "+34 688 11 22 34",
        "bank": bank_laboral, "iban": "ES70 3035 0019 50 0190045605",
        "km": 4, "plate": "2341 LKG", "marital": "single", "children": 0,
        "emerg_name": "Elena Fernández (Ama)", "emerg_phone": "+34 688 52 33 18",
        "cert": "graduate", "study_field": "Sareko Informatika Sistemen Administrazioa (ASIR)", "study_school": "Goierri Eskola (Ordizia)",
        "ssnid": "20/11902344/55", "gender": "male", "birthday": "2003-07-22", "birth_place": "Beasain",
        "pin": "0002", "barcode": "1TJ-EMP-005", "hourly_cost": 22.50,
        "contract_type": "Permanent", "contract_note": "Sare eta Suebaki Administratzaile kontratu mugagabea. OPNsense, VLAN segmentazioa eta VPN.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Ertaina"),
                   ("OPNsense, Sareak eta VLAN", "Aditua"), ("Zibersegurtasuna eta Auditoria", "Aurreratua")],
        "resume": [
            ("experience", "Sare eta Suebaki Administratzailea — 1T Jatetxea S.L.", "2026-09-01", False, "OPNsense suebakia, VLAN 10/20/30 bideratzea eta DMZ zerbitzari segurtasuna."),
            ("education", "Goi Mailako Zikloa — Goierri Eskola", "2024-09-01", "2026-06-30", "Sare sistemen administrazioa eta suebakien kudeaketa."),
        ],
    },
    26: {  # Iker Araujo Dominguez — Sistemak eta Birtualizazioa
        "job_title": "Sistemen eta Birtualizazio Ingeniaria",
        "parent_id": 24, "coach_id": 24,
        "work_phone": "+34 943 12 34 62", "mobile_phone": "+34 688 11 22 35",
        "work_location_id": loc_cpd.id, "calendar_id": cal_40_office.id,
        "tags": ["IT Admin", "Goizeko Txanda"],
        "private_street": "Iparragirre Hiribidea 9, 3.A", "private_zip": "20700", "private_city": "Urretxu",
        "private_email": "iker.araujo.sys@gmail.com", "private_phone": "+34 688 11 22 35",
        "bank": bank_kutxa, "iban": "ES84 2095 5043 60 9102345606",
        "km": 14, "plate": "6109 MBF", "marital": "single", "children": 0,
        "emerg_name": "Carlos Araujo (Aita)", "emerg_phone": "+34 688 30 91 72",
        "cert": "graduate", "study_field": "Sistemen Administrazioa eta Birtualizazioa", "study_school": "Goierri Eskola (Ordizia)",
        "ssnid": "20/11918820/63", "gender": "male", "birthday": "2002-11-30", "birth_place": "Zumarraga",
        "pin": "0003", "barcode": "1TJ-EMP-006", "hourly_cost": 22.50,
        "contract_type": "Permanent", "contract_note": "Sistemen eta Birtualizazio Ingeniari kontratua. Windows Server 2025 AD, Docker eta backup politikak.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"),
                   ("Docker eta Birtualizazioa", "Aditua"), ("Windows Server eta Active Directory", "Aditua")],
        "resume": [
            ("experience", "Sistemen eta Birtualizazio Ingeniaria — 1T Jatetxea S.L.", "2026-09-01", False, "Windows Server 2025 (JATETXEA.LAN), Ubuntu DMZ eta Docker edukiontzien kudeaketa."),
            ("education", "Goi Mailako Zikloa — Goierri Eskola", "2024-09-01", "2026-06-30", "Sistema eragileak, birtualizazioa eta datu-base zerbitzariak."),
        ],
    },
    27: {  # Aratz Huegun Flores — Zibersegurtasuna
        "job_title": "Zibersegurtasun eta Ebaluazio Teknikaria",
        "parent_id": 24, "coach_id": 24,
        "work_phone": "+34 943 12 34 63", "mobile_phone": "+34 688 11 22 36",
        "work_location_id": loc_cpd.id, "calendar_id": cal_40_office.id,
        "tags": ["IT Admin", "Goizeko Txanda"],
        "private_street": "Rondilla Kalea 27, 2.Esk", "private_zip": "20400", "private_city": "Tolosa",
        "private_email": "aratz.huegun.sec@gmail.com", "private_phone": "+34 688 11 22 36",
        "bank": bank_sabadell, "iban": "ES19 0081 5120 70 0001456707",
        "km": 15, "plate": "9478 LZX", "marital": "single", "children": 0,
        "emerg_name": "Miren Flores (Ama)", "emerg_phone": "+34 688 71 20 85",
        "cert": "graduate", "study_field": "Zibersegurtasuna IT Inguruneetan", "study_school": "Goierri Eskola (Ordizia)",
        "ssnid": "20/11934501/29", "gender": "male", "birthday": "2003-01-14", "birth_place": "Tolosa",
        "pin": "0004", "barcode": "1TJ-EMP-007", "hourly_cost": 22.90,
        "contract_type": "Permanent", "contract_note": "Zibersegurtasun Teknikari kontratua. DB segurtasun-kopiak, JWT auditoretza eta GDPR betetzea.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"),
                   ("Zibersegurtasuna eta Auditoria", "Aditua"), ("Odoo 17 ERP eta PostgreSQL", "Aurreratua")],
        "resume": [
            ("experience", "Zibersegurtasun eta Ebaluazio Teknikaria — 1T Jatetxea S.L.", "2026-09-01", False, "Sare-auditoretza, sarbide-kontrolak (RBAC) eta DBLO/RGPD egokitzapena."),
            ("education", "Goi Mailako Zikloa — Goierri Eskola", "2024-09-01", "2026-06-30", "Zibersegurtasuna, sare-babesa eta sistemen monitorizazioa."),
        ],
    },
    28: {  # Ainhoa Goikoetxea — Chef Burua
        "job_title": "Chef Burua / Sukaldari Nagusia",
        "parent_id": 21, "coach_id": 21,
        "work_phone": "+34 943 12 34 70", "mobile_phone": "+34 677 11 22 33",
        "work_location_id": loc_sukaldea.id, "calendar_id": cal_40_hosp.id,
        "tags": ["Sukaldea", "Elikagai-Manipulatzailea", "Lehen Sorospenak"],
        "private_street": "Kale Nagusia 31, 2.B", "private_zip": "20213", "private_city": "Idiazabal",
        "private_email": "ainhoa.goikoetxea.chef@gmail.com", "private_phone": "+34 677 11 22 33",
        "bank": bank_kutxa, "iban": "ES33 2095 5044 80 9102345608",
        "km": 8, "plate": "5512 KTR", "marital": "married", "children": 1,
        "emerg_name": "Jon Goikoetxea (Anaia)", "emerg_phone": "+34 677 88 21 09",
        "cert": "bachelor", "study_field": "Sukaldaritza Zuzendaritza eta Gastronomia", "study_school": "Basque Culinary Center",
        "ssnid": "20/10891432/41", "gender": "female", "birthday": "1988-06-05", "birth_place": "Idiazabal",
        "pin": "3001", "barcode": "1TJ-EMP-008", "hourly_cost": 20.80,
        "contract_type": "Permanent", "contract_note": "Gipuzkoako Ostalaritza Hitzarmena — Chef Burua. Karta diseinua, APPCC eta KDS sukalde koordinazioa.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Italiera", "Aurreratua"),
                   ("Euskal Parrilla eta Txingarra", "Aditua"), ("Italiar Sukaldaritza eta Labea", "Aditua"), ("APPCC eta Alergenoen Kontrola", "Aditua")],
        "resume": [
            ("experience", "Chef Burua — 1T Jatetxea S.L.", "2025-01-15", False, "35 platerreko karta italo-euskaldunaren zuzendaritza, KDS Android TV kudeaketa eta hornitzaileen hautaketa."),
            ("experience", "Sous-Chef — Osteria del Borgo (Florentzia / Donostia)", "2018-04-01", "2024-12-20", "Pasta freskoa, egur-labea eta sukaldaritza taldearen koordinazioa."),
            ("education", "Gastronomia eta Sukaldaritza Zuzendaritza", "2012-09-01", "2016-06-30", "Basque Culinary Center."),
            ("cert", "Elikagai-Manipulatzaile Goi Mailako Ziurtagiria (APPCC)", "2025-01-10", False, "Eusko Jaurlaritza / Osasun Saila."),
        ],
    },
    29: {  # Eneko Otegi — Bigarren Sukaldaria / Parrilleroa
        "job_title": "Bigarren Sukaldaria / Parrilleroa",
        "parent_id": 28, "coach_id": 28,
        "work_phone": "+34 943 12 34 71", "mobile_phone": "+34 677 11 22 34",
        "work_location_id": loc_sukaldea.id, "calendar_id": cal_40_hosp.id,
        "tags": ["Sukaldea", "Elikagai-Manipulatzailea", "Arratsaldeko Txanda"],
        "private_street": "Zurbano Kalea 8, 1.A", "private_zip": "20214", "private_city": "Segura",
        "private_email": "eneko.otegi.parrilla@gmail.com", "private_phone": "+34 677 11 22 34",
        "bank": bank_laboral, "iban": "ES47 3035 0021 90 0210045609",
        "km": 10, "plate": "1823 JKP", "marital": "cohabitant", "children": 0,
        "emerg_name": "Olatz Mendia (Bikotekidea)", "emerg_phone": "+34 677 40 55 12",
        "cert": "graduate", "study_field": "Sukaldaritza eta Gastronomia Teknikaria", "study_school": "Cebanc Ostalaritza Eskola (Donostia)",
        "ssnid": "20/11230987/16", "gender": "male", "birthday": "1993-02-19", "birth_place": "Segura",
        "pin": "3002", "barcode": "1TJ-EMP-009", "hourly_cost": 16.25,
        "contract_type": "Permanent", "contract_note": "Gipuzkoako Ostalaritza Hitzarmena — Bigarren Sukaldaria eta Euskal Parrilla espezialista.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"),
                   ("Euskal Parrilla eta Txingarra", "Aditua"), ("Italiar Sukaldaritza eta Labea", "Aurreratua"), ("APPCC eta Alergenoen Kontrola", "Aurreratua")],
        "resume": [
            ("experience", "Bigarren Sukaldaria / Parrilleroa — 1T Jatetxea S.L.", "2025-03-01", False, "Haragi eta arrainen parrillako erreketak, egur-labea eta mis-en-place prestaketa."),
            ("experience", "Parrilla Laguntzailea — Erretegi Nagusia (Tolosa)", "2019-06-01", "2025-02-20", "Txuleta eta arrain errea haritz-ikatzean."),
            ("education", "Sukaldaritza eta Gastronomia Goi Mailako Teknikaria", "2015-09-01", "2017-06-30", "Cebanc (Donostia)."),
        ],
    },
    30: {  # Mikel Arregi — Maître / Zerbitzari Burua
        "job_title": "Maître / Zerbitzari Burua",
        "parent_id": 21, "coach_id": 21,
        "work_phone": "+34 943 12 34 75", "mobile_phone": "+34 666 11 22 33",
        "work_location_id": loc_jantokia.id, "calendar_id": cal_40_hosp.id,
        "tags": ["Jantokia", "Elikagai-Manipulatzailea", "Lehen Sorospenak"],
        "private_street": "Urdaneta Kalea 12, 3.Ezk", "private_zip": "20240", "private_city": "Ordizia",
        "private_email": "mikel.arregi.sala@gmail.com", "private_phone": "+34 666 11 22 33",
        "bank": bank_kutxa, "iban": "ES61 2095 5041 11 9102345610",
        "km": 1, "plate": "3904 KSD", "marital": "married", "children": 2,
        "emerg_name": "Idoia Arregi (Emaztea)", "emerg_phone": "+34 666 90 12 30",
        "cert": "graduate", "study_field": "Jatetxe Zerbitzuetako Zuzendaritza eta Sommelier", "study_school": "Donostiako Ostalaritza Eskola",
        "ssnid": "20/10945123/88", "gender": "male", "birthday": "1986-08-11", "birth_place": "Ordizia",
        "pin": "1001", "barcode": "1TJ-EMP-010", "hourly_cost": 17.50,
        "contract_type": "Permanent", "contract_note": "Jantokiko Maître eta Zerbitzari Buruaren kontratu mugagabea. Erreserbak, ardo-karta eta C# TPV.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"), ("Frantsesa", "Ertaina"),
                   ("Sommelier eta Ardo Karta", "Aditua"), ("Bezero Arreta eta Protokoloa", "Aditua"), ("TPV eta Komandera Digitala", "Aditua")],
        "resume": [
            ("experience", "Maître / Zerbitzari Burua — 1T Jatetxea S.L.", "2025-01-15", False, "Jantokiko 18 mahaien koordinazioa, ardo uztarketa eta zerbitzari taldearen gidaritza."),
            ("education", "Jatetxe Zerbitzuetako Zuzendaritza eta Sommelier Ikastaroa", "2010-09-01", "2012-06-30", "Donostiako Ostalaritza Eskola."),
        ],
    },
    31: {  # Maite Zabaleta — Jantokiko Zerbitzaria
        "job_title": "Jantokiko Zerbitzaria",
        "parent_id": 30, "coach_id": 30,
        "work_phone": "+34 943 12 34 76", "mobile_phone": "+34 666 11 22 34",
        "work_location_id": loc_jantokia.id, "calendar_id": cal_40_hosp.id,
        "tags": ["Jantokia", "Elikagai-Manipulatzailea", "Goizeko Txanda"],
        "private_street": "Santa Fe Kalea 15, 1.B", "private_zip": "20247", "private_city": "Zaldibia",
        "private_email": "maite.zabaleta@gmail.com", "private_phone": "+34 666 11 22 34",
        "bank": bank_laboral, "iban": "ES75 3035 0022 12 0220045611",
        "km": 6, "plate": "8120 LMN", "marital": "single", "children": 0,
        "emerg_name": "Begoña Zabaleta (Ama)", "emerg_phone": "+34 666 41 28 73",
        "cert": "other", "study_field": "Jatetxe eta Taberna Zerbitzuak (Erdi Maila)", "study_school": "Goierri Eskola / Cebanc",
        "ssnid": "20/11672340/04", "gender": "female", "birthday": "1998-05-27", "birth_place": "Zaldibia",
        "pin": "1002", "barcode": "1TJ-EMP-011", "hourly_cost": 12.10,
        "contract_type": "Full-Time", "contract_note": "Jantokiko zerbitzari kontratua (40h/astean). PDA mugikorreko komandera eta mahai zerbitzua.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Ertaina"),
                   ("Bezero Arreta eta Protokoloa", "Aurreratua"), ("TPV eta Komandera Digitala", "Aurreratua")],
        "resume": [
            ("experience", "Jantokiko Zerbitzaria — 1T Jatetxea S.L.", "2026-06-01", False, "Bezeroen harrera, alergenoen informazioa, PDA komandak eta TPV kobrantzak."),
            ("education", "Jatetxe Zerbitzuetako Teknikaria", "2018-09-01", "2020-06-30", "Cebanc (Donostia)."),
        ],
    },
    32: {  # Unai Lasa — Jantokiko Zerbitzaria (Lanaldi partziala)
        "job_title": "Jantokiko Zerbitzaria (Asteburuak)",
        "parent_id": 30, "coach_id": 30,
        "work_phone": "+34 943 12 34 77", "mobile_phone": "+34 666 11 22 35",
        "work_location_id": loc_jantokia.id, "calendar_id": cal_20_part.id,
        "tags": ["Jantokia", "Elikagai-Manipulatzailea", "Lanaldi Partziala"],
        "private_street": "San Martin Auzoa 19, Behea", "private_zip": "20211", "private_city": "Ataun",
        "private_email": "unai.lasa99@gmail.com", "private_phone": "+34 666 11 22 35",
        "bank": bank_kutxa, "iban": "ES89 2095 5045 13 9102345612",
        "km": 9, "plate": "4410 MGH", "marital": "single", "children": 0,
        "emerg_name": "Xabier Lasa (Aita)", "emerg_phone": "+34 666 33 78 91",
        "cert": "other", "study_field": "Ostalaritza eta Turismo Ikasketak", "study_school": "UPV/EHU",
        "ssnid": "20/11883412/52", "gender": "male", "birthday": "2001-10-03", "birth_place": "Ataun",
        "pin": "1003", "barcode": "1TJ-EMP-012", "hourly_cost": 11.80,
        "contract_type": "Part-Time", "contract_note": "Lanaldi partzialeko kontratua (20h/astean — Ostirala, Larunbata eta Igandea). Asteburuko indartzea.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Aurreratua"),
                   ("Bezero Arreta eta Protokoloa", "Aurreratua"), ("TPV eta Komandera Digitala", "Aurreratua")],
        "resume": [
            ("experience", "Jantokiko Zerbitzaria (Asteburuak) — 1T Jatetxea S.L.", "2026-06-01", False, "Asteburuko bazkari eta afari zerbitzuetako errefortzua."),
            ("education", "Turismo eta Ostalaritza Gradua (Bidean)", "2022-09-01", False, "Euskal Herriko Unibertsitatea."),
        ],
    },
    33: {  # Gorka Mendizabal — Barman / Barrako Arduraduna
        "job_title": "Barman / Barrako Arduraduna",
        "parent_id": 21, "coach_id": 21,
        "work_phone": "+34 943 12 34 80", "mobile_phone": "+34 655 11 22 33",
        "work_location_id": loc_barra.id, "calendar_id": cal_40_hosp.id,
        "tags": ["Barra", "Elikagai-Manipulatzailea", "Arratsaldeko Txanda"],
        "private_street": "Zumalakarregi Kalea 7, 2.D", "private_zip": "20216", "private_city": "Ormaiztegi",
        "private_email": "gorka.mendizabal.bar@gmail.com", "private_phone": "+34 655 11 22 33",
        "bank": bank_sabadell, "iban": "ES24 0081 5121 14 0001456713",
        "km": 7, "plate": "6653 KVR", "marital": "cohabitant", "children": 0,
        "emerg_name": "Ane Urkizu (Bikotekidea)", "emerg_phone": "+34 655 19 84 62",
        "cert": "graduate", "study_field": "Kokteleria, Barista eta Edarien Kudeaketa", "study_school": "Basque Barista Academy",
        "ssnid": "20/11345098/77", "gender": "male", "birthday": "1991-12-09", "birth_place": "Ormaiztegi",
        "pin": "2001", "barcode": "1TJ-EMP-013", "hourly_cost": 15.40,
        "contract_type": "Permanent", "contract_note": "Barrako Arduradunaren kontratu mugagabea. BDS pantaila, kafe-espezialitatea eta kokteleria.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Italiera", "Ertaina"),
                   ("Kokteleria eta Barista", "Aditua"), ("Sommelier eta Ardo Karta", "Aurreratua"), ("TPV eta Komandera Digitala", "Aditua")],
        "resume": [
            ("experience", "Barman / Barrako Arduraduna — 1T Jatetxea S.L.", "2025-02-01", False, "Barrako BDS kudeaketa, italiar espressoa, aperitiboak (Spritz/Negroni) eta sagardo/txakolin zerbitzua."),
            ("cert", "SCA Barista Professional & Mixologia", "2023-05-15", False, "Specialty Coffee Association."),
        ],
    },
    34: {  # Uxue Etxarri — Barrako Zerbitzaria
        "job_title": "Barrako Zerbitzaria",
        "parent_id": 33, "coach_id": 33,
        "work_phone": "+34 943 12 34 81", "mobile_phone": "+34 655 11 22 34",
        "work_location_id": loc_barra.id, "calendar_id": cal_40_hosp.id,
        "tags": ["Barra", "Elikagai-Manipulatzailea", "Goizeko Txanda"],
        "private_street": "Kale Nagusia 6, 1.A", "private_zip": "20249", "private_city": "Itsasondo",
        "private_email": "uxue.etxarri@gmail.com", "private_phone": "+34 655 11 22 34",
        "bank": bank_kutxa, "iban": "ES38 2095 5046 15 9102345614",
        "km": 5, "plate": "2987 LBC", "marital": "single", "children": 0,
        "emerg_name": "Koldo Etxarri (Aita)", "emerg_phone": "+34 655 82 31 04",
        "cert": "other", "study_field": "Kafetegi eta Barra Zerbitzuak", "study_school": "Cebanc (Donostia)",
        "ssnid": "20/11723981/30", "gender": "female", "birthday": "1999-07-16", "birth_place": "Ordizia",
        "pin": "2002", "barcode": "1TJ-EMP-014", "hourly_cost": 13.35,
        "contract_type": "Permanent", "contract_note": "Barrako zerbitzari kontratu mugagabea (40h/astean). Kafe-gunea, BDS edariak eta barra zerbitzua.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"), ("Ingelesa", "Ertaina"),
                   ("Kokteleria eta Barista", "Aurreratua"), ("Bezero Arreta eta Protokoloa", "Aurreratua")],
        "resume": [
            ("experience", "Barrako Zerbitzaria — 1T Jatetxea S.L.", "2025-05-10", False, "Goizeko kafe-zerbitzua, edari prestaketa eta BDS eskaeren kontrola."),
            ("education", "Ostalaritza eta Kafetegi Zerbitzuetako Prestakuntza", "2019-09-01", "2021-06-30", "Cebanc."),
        ],
    },
    35: {  # Kepa Aldasoro — Biltegia eta Stock Arduraduna
        "job_title": "Biltegi eta Stock Arduraduna",
        "parent_id": 23, "coach_id": 23,
        "work_phone": "+34 943 12 34 85", "mobile_phone": "+34 644 11 22 33",
        "work_location_id": loc_biltegia.id, "calendar_id": cal_40_office.id,
        "tags": ["Biltegia", "Elikagai-Manipulatzailea", "Goizeko Txanda"],
        "private_street": "Euskal Herria Plaza 3, 2.Ezk", "private_zip": "20230", "private_city": "Legazpi",
        "private_email": "kepa.aldasoro.log@gmail.com", "private_phone": "+34 644 11 22 33",
        "bank": bank_laboral, "iban": "ES52 3035 0023 16 0230045615",
        "km": 17, "plate": "7741 JNW", "marital": "married", "children": 2,
        "emerg_name": "Arantxa Mujika (Emaztea)", "emerg_phone": "+34 644 72 19 50",
        "cert": "graduate", "study_field": "Garraioa, Logistika eta Biltegi Kudeaketa", "study_school": "UGLE Eskola (Zumarraga)",
        "ssnid": "20/10754210/66", "gender": "male", "birthday": "1984-01-29", "birth_place": "Legazpi",
        "pin": "4001", "barcode": "1TJ-EMP-015", "hourly_cost": 15.00,
        "contract_type": "Permanent", "contract_note": "Biltegi eta Hornidura Arduradunaren kontratu mugagabea. Hotz-kamerak, JavaFX Stock app eta albaranak.",
        "skills": [("Euskara", "Aditua"), ("Gaztelania", "Aditua"),
                   ("Stock eta Hornitzaile Kudeaketa", "Aditua"), ("APPCC eta Alergenoen Kontrola", "Aurreratua")],
        "resume": [
            ("experience", "Biltegi eta Stock Arduraduna — 1T Jatetxea S.L.", "2025-01-15", False, "Lehengaien sarrera-irteerak, gutxieneko stock alertak eta hornitzaileen albaranen erregistroa."),
            ("experience", "Logistika Teknikaria — Goierri Elikagai Banaketa", "2014-02-01", "2024-12-31", "Hotz-kateko produktuen kudeaketa eta inbentarioa."),
            ("education", "Garraio eta Logistika Goi Mailako Teknikaria", "2006-09-01", "2008-06-30", "Urola Garaiko Lanbide Eskola (UGLE)."),
        ],
    },
}

# Managers user IDs for approvals
user_joseba = env["res.users"].browse(13)  # Joseba Ramos
user_ander_m = env["res.users"].browse(12)  # Ander Madinabeitia
user_ander_u = env["res.users"].browse(2)   # Ander Urien

# Update employee records in 2 passes (first fields without parent_id/coach_id to avoid loop issues, then hierarchy)
for emp_id, info in EMP_DATA.items():
    emp = env["hr.employee"].browse(emp_id)
    avatar_b64 = read_b64(f"avatars/emp_{emp_id}.png")

    # Ensure bank account on employee's work_contact_id (partner)
    partner = emp.work_contact_id or emp.user_id.partner_id
    bank_acc = env["res.partner.bank"].search([("acc_number", "=", info["iban"])], limit=1)
    if not bank_acc:
        bank_acc = env["res.partner.bank"].create({
            "acc_number": info["iban"],
            "bank_id": info["bank"].id,
            "partner_id": partner.id,
            "company_id": company.id,
        })
    elif bank_acc.partner_id.id != partner.id:
        bank_acc.write({"partner_id": partner.id})

    # Determine leave/attendance manager user
    if emp_id in (21, 22, 23):
        mgr_user = user_joseba.id
    elif emp_id in (24, 25, 26, 27):
        mgr_user = user_ander_u.id
    else:
        mgr_user = user_ander_m.id

    if emp.user_id:
        emp.user_id.write({"image_1920": avatar_b64})
        if emp.user_id.partner_id:
            emp.user_id.partner_id.write({"image_1920": avatar_b64})

    emp.write({
        "image_1920": avatar_b64,
        "job_title": info["job_title"],
        "work_phone": info["work_phone"],
        "mobile_phone": info["mobile_phone"],
        "address_id": company.partner_id.id,
        "work_location_id": info["work_location_id"],
        "resource_calendar_id": info["calendar_id"],
        "tz": "Europe/Madrid",
        "category_ids": [(6, 0, [tags[t] for t in info["tags"]])],
        "leave_manager_id": mgr_user,
        "attendance_manager_id": mgr_user,
        "private_street": info["private_street"],
        "private_zip": info["private_zip"],
        "private_city": info["private_city"],
        "private_state_id": gipuzkoa.id,
        "private_country_id": spain.id,
        "private_email": info["private_email"],
        "private_phone": info["private_phone"],
        "bank_account_id": bank_acc.id,
        "lang": "en_US",
        "km_home_work": info["km"],
        "private_car_plate": info["plate"],
        "marital": info["marital"],
        "children": info["children"],
        "emergency_contact": info["emerg_name"],
        "emergency_phone": info["emerg_phone"],
        "certificate": info["cert"],
        "study_field": info["study_field"],
        "study_school": info["study_school"],
        "country_id": spain.id,
        "ssnid": info["ssnid"],
        "gender": info["gender"],
        "birthday": info["birthday"],
        "place_of_birth": info["birth_place"],
        "country_of_birth": spain.id,
        "employee_type": "employee",
        "pin": info["pin"],
        "barcode": info["barcode"],
    })

    # Fill spouse info for married employees so Private Info tab has zero blank fields
    spouse_map = {
        22: ("Nerea Etxeberria", "1981-07-19"),
        21: ("Leire Etxeberria", "1985-03-11"),
        28: ("Iñigo Larrañaga", "1987-10-22"),
        30: ("Idoia Mendizabal", "1988-02-14"),
        35: ("Arantxa Mujika", "1985-09-03"),
    }
    if emp_id in spouse_map:
        emp.write({
            "spouse_complete_name": spouse_map[emp_id][0],
            "spouse_birthdate": spouse_map[emp_id][1],
        })

    # Sync Skills (idempotent)
    env["hr.employee.skill"].search([("employee_id", "=", emp_id)]).unlink()
    for sk_name, lv_name in info["skills"]:
        st_id, sk_id = all_skills[sk_name]
        lv_id = all_levels[st_id][lv_name]
        env["hr.employee.skill"].create({
            "employee_id": emp_id,
            "skill_type_id": st_id,
            "skill_id": sk_id,
            "skill_level_id": lv_id,
        })

    # Sync Resume Lines (idempotent)
    env["hr.resume.line"].search([("employee_id", "=", emp_id)]).unlink()
    for rtype, rtitle, dstart, dend, rdesc in info["resume"]:
        lt_id = rt_exp.id if rtype == "experience" else (rt_edu.id if rtype == "education" else rt_cert.id)
        vals = {
            "employee_id": emp_id,
            "name": rtitle,
            "line_type_id": lt_id,
            "date_start": dstart,
            "description": rdesc,
        }
        if dend:
            vals["date_end"] = dend
        env["hr.resume.line"].create(vals)

# Pass 2: flush pending computes first, then enforce exact reporting hierarchy (parent_id and coach_id)
env.flush_all()
for emp_id, info in EMP_DATA.items():
    env["hr.employee"].browse(emp_id).write({
        "parent_id": info["parent_id"],
        "coach_id": info["coach_id"],
    })
env.flush_all()
# Guarantee department managers never self-reference due to Odoo _compute_parent_id hook
for emp_id, info in EMP_DATA.items():
    pid = info["parent_id"] if info["parent_id"] else None
    cid = info["coach_id"] if info["coach_id"] else None
    env.cr.execute("UPDATE hr_employee SET parent_id = %s, coach_id = %s WHERE id = %s", (pid, cid, emp_id))

print("=== [6/7] Lan-kontratuak (hr.contract) aberasten ===")
contract_types = {ct.name: ct.id for ct in env["hr.contract.type"].search([])}
for emp_id, info in EMP_DATA.items():
    contract = env["hr.contract"].search([("employee_id", "=", emp_id)], limit=1)
    if contract:
        ct_id = contract_types.get(info["contract_type"]) or contract_types.get("Permanent")
        contract.write({
            "contract_type_id": ct_id,
            "resource_calendar_id": info["calendar_id"],
            "hr_responsible_id": user_joseba.id,
            "notes": info["contract_note"],
            "state": "open",
        })

print("=== [7/7] Fitxaketak (hr.attendance) eta Oporrak/Bajak (hr.leave) sortzen ===")

# Idempotent attendances for Nov 23-26, 2026 (realistic restaurant & IT shifts)
env["hr.attendance"].search([]).unlink()
attendance_rows = [
    # IT Team & Management (Mon-Thu: Nov 23, 24, 25, 26)
    (22, "2026-11-23 08:30:00", "2026-11-23 16:45:00"),
    (22, "2026-11-24 08:35:00", "2026-11-24 16:30:00"),
    (22, "2026-11-25 08:25:00", "2026-11-25 16:40:00"),
    (22, "2026-11-26 08:30:00", "2026-11-26 16:35:00"),
    (21, "2026-11-23 09:00:00", "2026-11-23 17:15:00"),
    (21, "2026-11-24 09:00:00", "2026-11-24 17:00:00"),
    (21, "2026-11-25 09:10:00", "2026-11-25 17:20:00"),
    (21, "2026-11-26 09:00:00", "2026-11-26 17:05:00"),
    (23, "2026-11-23 08:45:00", "2026-11-23 16:45:00"),
    (23, "2026-11-24 08:40:00", "2026-11-24 16:50:00"),
    (23, "2026-11-25 08:45:00", "2026-11-25 16:45:00"),
    (23, "2026-11-26 08:30:00", "2026-11-26 16:30:00"),
    (24, "2026-11-23 08:00:00", "2026-11-23 16:30:00"),
    (24, "2026-11-24 08:05:00", "2026-11-24 16:20:00"),
    (24, "2026-11-25 08:00:00", "2026-11-25 16:45:00"),
    (24, "2026-11-26 08:00:00", "2026-11-26 16:15:00"),
    (25, "2026-11-23 08:15:00", "2026-11-23 16:15:00"),
    (25, "2026-11-24 08:10:00", "2026-11-24 16:25:00"),
    (25, "2026-11-25 08:15:00", "2026-11-25 16:15:00"),
    (25, "2026-11-26 08:15:00", "2026-11-26 16:20:00"),
    (26, "2026-11-23 08:15:00", "2026-11-23 16:20:00"),
    (26, "2026-11-24 08:20:00", "2026-11-24 16:20:00"),
    (26, "2026-11-25 08:15:00", "2026-11-25 16:30:00"),
    (26, "2026-11-26 08:10:00", "2026-11-26 16:15:00"),
    (27, "2026-11-23 08:30:00", "2026-11-23 16:30:00"),
    (27, "2026-11-24 08:30:00", "2026-11-24 16:35:00"),
    (27, "2026-11-25 08:25:00", "2026-11-25 16:30:00"),
    (27, "2026-11-26 08:30:00", "2026-11-26 16:40:00"),
    # Kitchen (Tue-Thu: Nov 24, 25, 26)
    (28, "2026-11-24 09:30:00", "2026-11-24 17:45:00"),
    (28, "2026-11-25 09:30:00", "2026-11-25 17:30:00"),
    (28, "2026-11-26 09:20:00", "2026-11-26 17:40:00"),
    (29, "2026-11-24 10:00:00", "2026-11-24 18:00:00"),
    (29, "2026-11-25 10:00:00", "2026-11-25 18:10:00"),
    (29, "2026-11-26 10:00:00", "2026-11-26 18:00:00"),
    # Dining Room & Bar (Tue-Thu: Nov 24, 25, 26)
    (30, "2026-11-24 11:00:00", "2026-11-24 19:00:00"),
    (30, "2026-11-25 11:00:00", "2026-11-25 19:15:00"),
    (30, "2026-11-26 11:00:00", "2026-11-26 19:00:00"),
    (31, "2026-11-24 11:30:00", "2026-11-24 19:30:00"),
    (31, "2026-11-25 11:30:00", "2026-11-25 19:30:00"),
    (31, "2026-11-26 11:30:00", "2026-11-26 19:30:00"),
    # Unai Lasa (weekend part-time: Nov 20, 21, 22)
    (32, "2026-11-20 18:00:00", "2026-11-20 23:30:00"),
    (32, "2026-11-21 12:00:00", "2026-11-21 20:00:00"),
    (32, "2026-11-22 12:00:00", "2026-11-22 18:30:00"),
    # Bar
    (33, "2026-11-24 10:30:00", "2026-11-24 18:30:00"),
    (33, "2026-11-25 10:30:00", "2026-11-25 18:45:00"),
    (33, "2026-11-26 10:30:00", "2026-11-26 18:30:00"),
    (34, "2026-11-24 08:30:00", "2026-11-24 16:30:00"),
    (34, "2026-11-25 08:30:00", "2026-11-25 16:30:00"),
    (34, "2026-11-26 08:30:00", "2026-11-26 16:30:00"),
    # Warehouse
    (35, "2026-11-23 07:30:00", "2026-11-23 15:30:00"),
    (35, "2026-11-24 07:30:00", "2026-11-24 15:30:00"),
    (35, "2026-11-25 07:30:00", "2026-11-25 15:40:00"),
    (35, "2026-11-26 07:30:00", "2026-11-26 15:30:00"),
]
for eid, cin, cout in attendance_rows:
    env["hr.attendance"].create({
        "employee_id": eid,
        "check_in": cin,
        "check_out": cout,
    })

# Time Off Allocations (23 paid days / year for all 15 employees)
lt_paid = env["hr.leave.type"].browse(1)       # Paid Time Off
lt_sick = env["hr.leave.type"].browse(2)       # Sick Time Off
lt_training = env["hr.leave.type"].browse(6)   # Training Time Off

existing_alloc = env["hr.leave.allocation"].search([("name", "=", "2026ko Urteko Oporrak (23 egun)")])
if not existing_alloc:
    for emp_id in EMP_DATA.keys():
        alloc = env["hr.leave.allocation"].create({
            "name": "2026ko Urteko Oporrak (23 egun)",
            "holiday_status_id": lt_paid.id,
            "employee_id": emp_id,
            "number_of_days": 23,
            "date_from": "2026-01-01",
            "date_to": "2026-12-31",
        })
        if alloc.state != "validate":
            alloc.action_validate()

# Allocate 5 Training Days to IT Team (so Training Time Off requests pass validation)
existing_train_alloc = env["hr.leave.allocation"].search([("name", "=", "2026ko Prestakuntza Tekniko Egunak (5 egun)")])
if not existing_train_alloc:
    for it_id in (24, 25, 26, 27):
        t_alloc = env["hr.leave.allocation"].create({
            "name": "2026ko Prestakuntza Tekniko Egunak (5 egun)",
            "holiday_status_id": lt_training.id,
            "employee_id": it_id,
            "number_of_days": 5,
            "date_from": "2026-01-01",
            "date_to": "2026-12-31",
        })
        if t_alloc.state != "validate":
            t_alloc.action_validate()

# Realistic Leave Requests (idempotent check by name)
leaves_spec = [
    (31, lt_paid.id, "2026-11-10", "2026-11-13", "Urteko Oporrak — Maite Zabaleta", True),
    (29, lt_sick.id, "2026-11-02", "2026-11-05", "Gripe Arrunta (Mediku Baja) — Eneko Otegi", True),
    (26, lt_training.id, "2026-11-19", "2026-11-19", "Proxmox & Docker Segurtasun Prestakuntza — Iker Araujo", True),
    (35, lt_paid.id, "2026-12-07", "2026-12-11", "Abenduko Zubiko Opor Eskaera — Kepa Aldasoro", False),
]
for eid, ltid, dfrom, dto, lname, should_validate in leaves_spec:
    ex = env["hr.leave"].search([("employee_id", "=", eid), ("name", "=", lname)], limit=1)
    if not ex:
        lv = env["hr.leave"].create({
            "name": lname,
            "holiday_status_id": ltid,
            "employee_id": eid,
            "request_date_from": dfrom,
            "request_date_to": dto,
        })
        if should_validate and lv.state in ("confirm", "validate1"):
            try:
                lv.action_approve()
                if lv.state == "validate1":
                    lv.action_validate()
            except Exception as e:
                print(f"  [OHARRA] Leave validation note for {lname}: {e}")

env.flush_all()
# Final authoritative hierarchy enforcement right before commit so Odoo's dept-manager hook never self-links managers
for emp_id, info in EMP_DATA.items():
    pid = info["parent_id"] if info["parent_id"] else None
    cid = info["coach_id"] if info["coach_id"] else None
    env.cr.execute("UPDATE hr_employee SET parent_id = %s, coach_id = %s WHERE id = %s", (pid, cid, emp_id))
env.invalidate_all()

env.cr.commit()
print("=== AMAITUTA! 1T Jatetxea Odoo 17 ERP eta Langileak modulua %100 konfiguratuta! ===")
