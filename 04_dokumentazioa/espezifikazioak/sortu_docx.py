import os
import docx
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_ALIGN_VERTICAL
from docx.oxml import OxmlElement, parse_xml
from docx.oxml.ns import qn, nsdecls

def set_cell_borders(cell, top=None, bottom=None, left=None, right=None):
    tcPr = cell._tc.get_or_add_tcPr()
    tcBorders = OxmlElement('w:tcBorders')
    
    borders = {'top': top, 'bottom': bottom, 'left': left, 'right': right}
    for border_name, border_style in borders.items():
        if border_style:
            b = OxmlElement(f'w:{border_name}')
            b.set(qn('w:val'), border_style.get('val', 'single'))
            b.set(qn('w:sz'), str(border_style.get('sz', 4)))
            b.set(qn('w:space'), '0')
            b.set(qn('w:color'), border_style.get('color', '000000'))
            tcBorders.append(b)
        else:
            b = OxmlElement(f'w:{border_name}')
            b.set(qn('w:val'), 'none')
            tcBorders.append(b)
    tcPr.append(tcBorders)

def set_cell_shading(cell, color_hex):
    shading_elm = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{color_hex}"/>')
    cell._tc.get_or_add_tcPr().append(shading_elm)

def sortu_docx_dokumentua():
    doc = docx.Document()

    # Marjinak (orrialde estandarra: 2.5 cm / ~1 hazbete)
    sections = doc.sections
    for section in sections:
        section.top_margin = Inches(1.0)
        section.bottom_margin = Inches(1.0)
        section.left_margin = Inches(1.0)
        section.right_margin = Inches(1.0)
        
        # Orri-oin txukuna
        footer = section.footer
        f_p = footer.paragraphs[0]
        f_p.alignment = WD_ALIGN_PARAGRAPH.RIGHT
        f_run = f_p.add_run("1T Jatetxea — Proiektuaren Proposamena eta Eragileak")
        f_run.font.name = "Calibri"
        f_run.font.size = Pt(9)
        f_run.font.color.rgb = RGBColor(120, 120, 120)

    # Estilo lehenetsiak ezarri (Calibri, beltz hutsa)
    normal_style = doc.styles['Normal']
    normal_style.font.name = 'Calibri'
    normal_style.font.size = Pt(11)
    normal_style.font.color.rgb = RGBColor(0, 0, 0)

    # 1. DOKUMENTUAREN GOIBURUA ETA IZENBURUA
    p_meta = doc.add_paragraph()
    p_meta.paragraph_format.space_before = Pt(0)
    p_meta.paragraph_format.space_after = Pt(4)
    run_meta = p_meta.add_run("GOIERRI ESKOLA — INFORMATIKAKO DEPARTAMENTUA\n1T ERRONTZA INTEGRALA — SISTEMAK ETA SAREAK")
    run_meta.font.name = "Calibri"
    run_meta.font.size = Pt(9.5)
    run_meta.font.bold = True
    run_meta.font.color.rgb = RGBColor(80, 80, 80)

    p_title = doc.add_paragraph()
    p_title.paragraph_format.space_before = Pt(8)
    p_title.paragraph_format.space_after = Pt(6)
    run_title = p_title.add_run("Jatetxearen Kudeaketa Integrala:\nProiektuaren Proposamena eta Eragileak")
    run_title.font.name = "Calibri"
    run_title.font.size = Pt(20)
    run_title.font.bold = True
    run_title.font.color.rgb = RGBColor(0, 0, 0)

    # Egileak eta Data
    p_info = doc.add_paragraph()
    p_info.paragraph_format.space_before = Pt(0)
    p_info.paragraph_format.space_after = Pt(14)
    run_info = p_info.add_run("Egileak: ")
    run_info.bold = True
    p_info.add_run("Ander Urien, Aratz Huegun, Iker Araujo, Julen Aguado\n")
    run_data = p_info.add_run("Data: ")
    run_data.bold = True
    p_info.add_run("2026ko Azaroa | ")
    run_bertsioa = p_info.add_run("Bertsioa: ")
    run_bertsioa.bold = True
    p_info.add_run("2.1 (Eguneratua)")
    for r in p_info.runs:
        r.font.name = "Calibri"
        r.font.size = Pt(10.5)

    # Bereizle lerroa (beltza)
    p_line = doc.add_paragraph()
    p_line.paragraph_format.space_before = Pt(0)
    p_line.paragraph_format.space_after = Pt(14)
    r_line = p_line.add_run("―" * 58)
    r_line.font.color.rgb = RGBColor(0, 0, 0)
    r_line.font.bold = True

    # 1. ATALA: HELBURUA ETA IKUSPEGI OROKORRA
    h1 = doc.add_paragraph()
    h1.paragraph_format.space_before = Pt(14)
    h1.paragraph_format.space_after = Pt(6)
    r_h1 = h1.add_run("1. Proiektuaren Helburua eta Ikuspegi Orokorra")
    r_h1.font.name = "Calibri"
    r_h1.font.size = Pt(14)
    r_h1.font.bold = True
    r_h1.font.color.rgb = RGBColor(0, 0, 0)

    p1 = doc.add_paragraph(
        "Dokumentu honek Jatetxearen Erronka Integraleko proposamen tekniko, arkitektoniko eta funtzionala zehazten du. "
        "Proiektu honen helburu nagusia jatetxe baten eguneroko jardun osoa digitalizatzea, arintzea eta zentralizatzea da, "
        "bezeroak mahaia erreserbatzen duen unetik hasi eta hornitzaileen stocka kudeatu, langileen lanaldiaren fitxaketa erregistratu "
        "edota kaxa-itxiera eta fakturazioa burutu arte."
    )
    p1.paragraph_format.space_after = Pt(6)
    p1.paragraph_format.line_spacing = 1.15

    p2 = doc.add_paragraph(
        "Informazio operatibo guztia backend API zentral baten bidez komunikatzen da (ASP.NET Core REST API). Datu-baseen antolaketan "
        "bi motor nagusi bereizten dira: alde batetik, MySQL datu-basea (jatetxea_db), jatetxearen jardun operatibo osoa denbora errealean "
        "kudeatzen duena; eta bestetik, PostgreSQL datu-basea (jatetxea_odoo), Odoo 17 ERP sistemak kontabilitate, albaranak, hornitzaileak "
        "eta giza-baliabideak modu ofizialean kudeatzeko erabiltzen duena."
    )
    p2.paragraph_format.space_after = Pt(6)
    p2.paragraph_format.line_spacing = 1.15

    p3 = doc.add_paragraph(
        "Gailu mugikor eta tabletei dagokienez, proiektuak Android natiboa hautatu du (Jetpack Compose), gailu bakoitzaren hardware eta "
        "pantaila neurrietara era aproposenean egokitzeko (Flutter bezalako bitartekariak alboratuz). Gainera, indarrean dagoen lan-araudia "
        "betetzeko eta txanden jarraipena bermatzeko, langileek aplikazio nagusietatik (C# TPV, JavaFX eta Android Komandera) "
        "lanaldiaren fitxaketa erregistratzeko funtzionaltasun integratua dute, 4 digituko segurtasun-PIN bidez babestua."
    )
    p3.paragraph_format.space_after = Pt(14)
    p3.paragraph_format.line_spacing = 1.15

    # 2. ATALA: ERAGILEAK ETA BARKOTZAREN APLIKAZIOAK
    h2 = doc.add_paragraph()
    h2.paragraph_format.space_before = Pt(14)
    h2.paragraph_format.space_after = Pt(6)
    r_h2 = h2.add_run("2. Eragileak (Rolak) eta Bakoitzaren Aplikazioak")
    r_h2.font.name = "Calibri"
    r_h2.font.size = Pt(14)
    r_h2.font.bold = True
    r_h2.font.color.rgb = RGBColor(0, 0, 0)

    p_intro_table = doc.add_paragraph(
        "Jatetxearen ekosisteman 6 erabiltzaile-rol nagusi definitu dira, bakoitzari bere zereginetarako interfaze espezifikoa esleituz:"
    )
    p_intro_table.paragraph_format.space_after = Pt(8)

    # TAULA
    table_data = [
        ("Rola / Erabiltzailea", "Erabilitako Aplikazioak", "Plataforma / Teknologia", "Funtzio Nagusiak"),
        ("Bezeroak", "• Web-App publikoa (web_php_bezeroak)\n• Mugikorreko App-a (mobile_bezeroa_flutter)", "• Web orria (HTML5, JS, PHP)\n• Android/iOS (Flutter / Dart)", "Karta kontsultatu, alergenoen iragazketa, mahai-erreserbak egin, auto-eskaera eta iritziak utzi."),
        ("Zerbitzariak\n(Jantokia)", "• TPV Salmenta Puntua (desktop_tpv_csharp)\n• Android Komandera (zerbitzaria_komandera_android)", "• Windows Desktop (.NET 10)\n• Android (Mugikorra + Tableta)", "Mahaien mapa eta okupazioa kudeatu, erreserba egutegia ikusi, komandak mahai ondoan jaso, sukaldera igorri, kobraketak bideratu eta lanaldia fitxatu."),
        ("Sukaldariak", "• Sukaldeko KDS Telebista (sukaldea_kds_android_tv)\n• JavaFX Aplikazioa (desktop_javafx)", "• Android TV (Compose TV)\n• Desktop (Java 17/21, JavaFX)", "Sukaldeko pantaila handian komandak denbora errealean jaso, plateren egoerak eguneratu (Zai, Martxan, Eginda), lehengaiak kontrolatu eta lanaldia fitxatu."),
        ("Barrakoa\n(Kutxazaina)", "• TPV Salmenta Puntua (desktop_tpv_csharp)\n• JavaFX Aplikazioa (BDS modulu)\n• Android Komandera (Mugikorra/Tableta)", "• Windows Desktop (.NET 10)\n• Desktop (JavaFX BDS)\n• Android (Mugikorra/Tableta)", "Barran zuzeneko salmentak erregistratu, edarien komandak prestatu (BDS), tiketak eta fakturak inprimatu, kontuak kobratu eta lanaldia fitxatu."),
        ("Biltegikoak", "• JavaFX Biltegia eta Stocka (desktop_javafx)\n• Odoo ERP Kudeaketa (odoo_erp)", "• Desktop (JavaFX MVC)\n• Web ERP (Odoo 17 / PostgreSQL)", "Lehengaien stock errealaren kontrola, gutxieneko stock alertak aztertu, hornitzaileen albaranak jaso, erosketa-zerrendak kudeatu eta lanaldia fitxatu."),
        ("Admin IT /\nZuzendaritza", "• TI Aginte Panela (admin_IT_zentrala)\n• JavaFX Aplikazio Integratua\n• Odoo ERP Finantzak (odoo_erp)\n• Backend REST API (backend_rest_api_csharp)", "• Web Dashboard\n• Desktop (JavaFX)\n• Odoo ERP (PostgreSQL)\n• ASP.NET Core REST API", "Azpiegitura eta zerbitzuen monitorizazioa, langileen login eta baimenak (RBAC/PINak), asistentzia eta fitxaketen ikuskaritza, finantza-txostenak eta segurtasun-kopiak.")
    ]

    table = doc.add_table(rows=len(table_data), cols=4)
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = False

    col_widths = [Inches(1.4), Inches(2.0), Inches(1.8), Inches(2.2)]

    for row_idx, row in enumerate(table.rows):
        # Tr altuera
        trPr = row._tr.get_or_add_trPr()
        trHeight = OxmlElement('w:trHeight')
        trHeight.set(qn('w:val'), '360')
        trPr.append(trHeight)

        is_header = (row_idx == 0)
        for col_idx, cell in enumerate(row.cells):
            cell.width = col_widths[col_idx]
            cell.vertical_alignment = WD_ALIGN_VERTICAL.CENTER
            
            # Ertzak: goiburuan beheko lerro sendoa, besteetan lerro mehea
            border_style = {'val': 'single', 'sz': 4, 'color': '000000'}
            border_header_bottom = {'val': 'single', 'sz': 12, 'color': '000000'}
            
            if is_header:
                set_cell_borders(cell, top=border_style, bottom=border_header_bottom, left=border_style, right=border_style)
                set_cell_shading(cell, "E0E0E0")
            else:
                set_cell_borders(cell, top=border_style, bottom=border_style, left=border_style, right=border_style)
                if row_idx % 2 == 1:
                    set_cell_shading(cell, "FFFFFF")
                else:
                    set_cell_shading(cell, "F7F7F7")

            # Testua jarri
            text_val = table_data[row_idx][col_idx]
            cell.text = ""
            p_cell = cell.paragraphs[0]
            p_cell.paragraph_format.space_before = Pt(3)
            p_cell.paragraph_format.space_after = Pt(3)
            p_cell.paragraph_format.line_spacing = 1.05
            
            lines = text_val.split("\n")
            for l_idx, line in enumerate(lines):
                if l_idx > 0:
                    p_cell = cell.add_paragraph()
                    p_cell.paragraph_format.space_before = Pt(1)
                    p_cell.paragraph_format.space_after = Pt(2)
                    p_cell.paragraph_format.line_spacing = 1.05
                run = p_cell.add_run(line)
                run.font.name = "Calibri"
                run.font.size = Pt(9.5 if not is_header else 10)
                run.font.bold = is_header or (col_idx == 0)
                run.font.color.rgb = RGBColor(0, 0, 0)

    doc.add_paragraph().paragraph_format.space_after = Pt(8)

    # 3. ATALA: ARKITEKTURA DIAGRAMA ETA APLIKAZIOEN DESKRIBAPENA
    h3 = doc.add_paragraph()
    h3.paragraph_format.space_before = Pt(14)
    h3.paragraph_format.space_after = Pt(6)
    r_h3 = h3.add_run("3. Sistemaren Arkitektura eta Aplikazioen Deskribapena")
    r_h3.font.name = "Calibri"
    r_h3.font.size = Pt(14)
    r_h3.font.bold = True
    r_h3.font.color.rgb = RGBColor(0, 0, 0)

    p_diag_intro = doc.add_paragraph(
        "Jarraian ikus daiteke jatetxearen ekosistema osatzen duten aplikazio, datu-base eta azpiegitura-zerbitzuen arteko "
        "harreman orokorra eta komunikazio-fluxu teknikoa:"
    )
    p_diag_intro.paragraph_format.space_after = Pt(8)

    # Diagrama Irudia txertatu
    diagrama_path = r"C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\04_dokumentazioa\espezifikazioak\arkitektura_diagrama.png"
    if os.path.exists(diagrama_path):
        p_img = doc.add_paragraph()
        p_img.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p_img.paragraph_format.space_before = Pt(4)
        p_img.paragraph_format.space_after = Pt(4)
        run_img = p_img.add_run()
        run_img.add_picture(diagrama_path, width=Inches(6.3))

        p_caption = doc.add_paragraph()
        p_caption.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p_caption.paragraph_format.space_before = Pt(2)
        p_caption.paragraph_format.space_after = Pt(14)
        run_cap = p_caption.add_run("1. Irudia — Jatetxearen aplikazioen arkitektura integrala eta datu-fluxuak.")
        run_cap.font.name = "Calibri"
        run_cap.font.size = Pt(9.5)
        run_cap.font.italic = True
        run_cap.font.color.rgb = RGBColor(70, 70, 70)

    # 3.1 Bezeroen Interfazea
    def add_subsection(title, desc_paragraphs):
        h_sub = doc.add_paragraph()
        h_sub.paragraph_format.space_before = Pt(10)
        h_sub.paragraph_format.space_after = Pt(4)
        r_sub = h_sub.add_run(title)
        r_sub.font.name = "Calibri"
        r_sub.font.size = Pt(12)
        r_sub.font.bold = True
        r_sub.font.color.rgb = RGBColor(0, 0, 0)

        for p_text in desc_paragraphs:
            p = doc.add_paragraph(p_text)
            p.paragraph_format.space_after = Pt(5)
            p.paragraph_format.line_spacing = 1.15

    add_subsection("3.1. Bezeroen Interfaze Publikoak", [
        "• Web-App Publikoa (01_aplikazioak/bezeroak/web_php_bezeroak): Jatetxearen atari korporatiboa eta erreserba-gunea. "
        "Bezeroek online bidez mahai-erreserbak egin ditzakete, eguneko menua zein karta kontsultatu eta alergenoen araberako "
        "iragazketa azkarrak aplikatu. Sukaldean edo biltegian agortutako platerak automatikoki agortu gisa bistaratzen dira.",
        "• Bezeroen Mugikorreko App-a (01_aplikazioak/bezeroak/mobile_bezeroa_flutter): Flutter bidez garatutako Android aplikazioa. "
        "Mahaian QR kodea eskaneatuz eskaerak auto-kudeatzeko, plateren xehetasunak ikusteko eta ordainketa digital zibersegurua "
        "burutzeko tresna intuitiboa."
    ])

    add_subsection("3.2. Zerbitzarien eta Jantokiaren Interfazeak", [
        "• TPV Salmenta Puntua Aretoa & Kutxazaina (01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp): C# eta .NET 10 bidez garatutako mahaigaineko "
        "soluzio sendoa. Mahaien banaketa interaktiboa (Jangela Nagusia, Terraza, Barra, VIP), erreserben 5 hilabeteko egutegi integrala, "
        "kobraketa azkarrak eta txanda hasiera/amaierako fitxaketa erregistratzea ahalbidetzen ditu.",
        "• Komandera Android Natiboa (01_aplikazioak/zerbitzaria_komandera_android): Jetpack Compose teknologiarekin sortutako "
        "aplikazioa, mugikor zein tablet formatuetarako prestatua. Zerbitzariek mahai ondoan bertan komandak jaso, plateren ohar bereziak "
        "zehaztu (sukaldaritza puntua, alergiak), sukaldera edo barrara aginduak bidali eta lanaldi-fitxaketa gauzatu dezakete."
    ])

    add_subsection("3.3. Sukaldeko Interfazeak (KDS - Kitchen Display System)", [
        "• Android TV Sukaldeko Pantaila (01_aplikazioak/sukaldea_kds_android_tv): Sukaldeko horman kokatutako telebista nagusirako "
        "garatutako interfazea (Compose for TV). Sartzen diren platerak denbora errealean erakusten ditu txartel formatuan, "
        "lehentasunaren eta itxaronaldiaren arabera antolatuta.",
        "• JavaFX Sukaldea KDS Modulua (01_aplikazioak/desktop_javafx): Sukaldari buruak eta taldeak plateren egoerak klik bakarrean "
        "kudeatzeko aukera dute (Zai, Martxan, Eginda) eta txandaren fitxaketa erraz burutu dezakete."
    ])

    add_subsection("3.4. Barrako eta Salmenta Puntuko Interfazeak", [
        "• C# TPV Salmenta Puntua (01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp): Barran kokatutako kobraketa-terminala. Tiket fiskalak inprimatzea, "
        "ordainketa konbinatuak (eskudirua, txartela) eta eguneko kaxa-itxiera (Z txostena) kudeatzen ditu.",
        "• JavaFX Barra BDS Modulua (01_aplikazioak/desktop_javafx): Barran prestatzen diren edari, aperitibo eta kafetegiko aginduak "
        "bereizita kudeatzeko pantaila dedikatua.",
        "• Android Komandera (01_aplikazioak/zerbitzaria_komandera_android): Barrako langileek ere erabil dezakete mugikorrean edo tabletan "
        "eskaerak erregistratzeko, egoerak gainbegiratzeko eta fitxatzeko."
    ])

    add_subsection("3.5. Biltegiko Interfazea eta Hornitzaileen Kudeaketa", [
        "• JavaFX Biltegia eta Stock Modulua (01_aplikazioak/desktop_javafx): Despentsa, ganbara eta hozkailuetako stock errealaren "
        "jarraipena. Gutxieneko stocketik behera dauden produktuak automatikoki seinalatzen ditu eta biltegiko langileen fitxaketa erregistratzen du.",
        "• Odoo 17 ERP Integrazioa (01_aplikazioak/odoo_erp): Hornitzaileen erosketa-eskaerak, albaranak eta hornitzaileen fakturak "
        "ofizialki erregistratzeko sistema integrala (PostgreSQL gainean)."
    ])

    add_subsection("3.6. Administrazioa, TI eta Zuzendaritza", [
        "• Admin IT Zentrala (01_aplikazioak/admin/admin_IT_zentrala): Administratzaileentzako aginte-zentroa. Docker edukiontzien egoera, "
        "portuen erabilgarritasuna, sare-konexioak, aplikazioen abiarazleak eta VirtualBox makina birtualen kontrol osoa biltzen ditu.",
        "• JavaFX Aplikazio Integratua (01_aplikazioak/desktop_javafx): Admin IT eta Zuzendaritzak Sukaldea (KDS), Barra (BDS) eta "
        "Biltegia (Stock) ataletarako sarbide osoa dute administratzaile baimenekin.",
        "• Odoo ERP Kudeaketa Orokorra: Zuzendaritzarentzat finantza-egoerak, nominak eta asistentzia/fitxaketa erregistroak ikuskatzeko tresna.",
        "• Backend REST API Zentrala (01_aplikazioak/admin/backend_rest_api_csharp): ASP.NET Core arkitekturako zerbitzari nagusia. Segurtasun transakzionala, "
        "BCrypt pasahitzen balidazioa eta langileen fitxaketen erregistro iraunkorra bermatzen ditu."
    ])

    # 4. ATALA: AZPIEGITURA BIRTUALA ETA DATU-BASEAK
    h4 = doc.add_paragraph()
    h4.paragraph_format.space_before = Pt(14)
    h4.paragraph_format.space_after = Pt(6)
    r_h4 = h4.add_run("4. Azpiegitura Birtuala, Segurtasuna eta Datu-Baseak")
    r_h4.font.name = "Calibri"
    r_h4.font.size = Pt(14)
    r_h4.font.bold = True
    r_h4.font.color.rgb = RGBColor(0, 0, 0)

    p_azpi1 = doc.add_paragraph(
        "Jatetxearen azpiegiturak datu-baseen banaketa argia eta sarearen segmentazio zorrotza aplikatzen ditu:"
    )
    p_azpi1.paragraph_format.space_after = Pt(5)

    doc.add_paragraph(
        "• MySQL Datu-Basea (jatetxea_db, 3308 ataka): Jatetxeko eguneroko operazio guztiak kudeatzen ditu: langileen profilak, "
        "fitxaketak (ordutegi kontrola), mahaien egoerak, karta, platerak, eskaerak, fakturak eta stock mugimenduak."
    ).paragraph_format.space_after = Pt(4)

    doc.add_paragraph(
        "• PostgreSQL Datu-Basea (jatetxea_odoo, 5432 ataka): Odoo 17 ERP sistemari eskainitako datu-base esklusiboa. Enpresaren "
        "kontabilitate ofiziala, hornitzaileen albaranak eta nominak modu isolatuan gordetzen ditu."
    ).paragraph_format.space_after = Pt(4)

    doc.add_paragraph(
        "• OPNsense Suebakia (Erronka-OPNsense): Router-on-a-stick bidez sareak segmentatzen ditu VLAN desberdinetan (Bezeroen Wi-Fi publikoa, "
        "Langileen sarea, Zerbitzarien DMZ eta Kudeaketa sarea), kanpoko sarbide bidegabeak blokeatuz."
    ).paragraph_format.space_after = Pt(4)

    doc.add_paragraph(
        "• Docker Ingurunea: Backend APIa, datu-baseak eta web aplikazioak edukiontzietan hedatuta daude (docker-compose bidez), "
        "hedapen azkarra, isolamendua eta zerbitzuen berrabiarazte fidagarria bermatuz."
    ).paragraph_format.space_after = Pt(4)

    doc.add_paragraph(
        "• Segurtasuna eta Sarbide Kontrola (RBAC): Erabiltzaile bakoitzak bere rolari dagozkion datuak bakarrik eskura ditzake. "
        "Langileen PIN zenbakiak BCrypt hash algoritmo sendoarekin babestuta daude datu-basean."
    ).paragraph_format.space_after = Pt(14)

    # Dokumentua gorde
    irteera_docx = r"C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\04_dokumentazioa\espezifikazioak\proiektuaren_proposamena_eta_erabiltzaileak.docx"
    doc.save(irteera_docx)
    print(f"DOCX dokumentua sortua: {irteera_docx}")

if __name__ == "__main__":
    sortu_docx_dokumentua()
