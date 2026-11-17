import os
from PIL import Image, ImageDraw, ImageFont
import docx
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

def sortu_diagrama_irudia(irteera_bidea):
    width = 1700
    height = 1250
    img = Image.new('RGB', (width, height), color=(255, 255, 255))
    draw = ImageDraw.Draw(img)

    try:
        font_header = ImageFont.truetype('arialbd.ttf', 24)
        font_sec = ImageFont.truetype('arialbd.ttf', 17)
        font_box_tit = ImageFont.truetype('arialbd.ttf', 15)
        font_box_sub = ImageFont.truetype('arial.ttf', 13)
        font_box_tech = ImageFont.truetype('ariali.ttf', 12)
        font_conn = ImageFont.truetype('arial.ttf', 12)
    except:
        font_header = ImageFont.load_default()
        font_sec = font_header
        font_box_tit = font_header
        font_box_sub = font_header
        font_tech = font_header
        font_conn = font_header

    # Kanpoko markoa
    draw.rectangle([(25, 25), (width - 25, height - 25)], outline=(0, 0, 0), width=2)

    # Titulua
    draw.text((width // 2, 55), "T1 JATETXEA — SISTEMAREN ARKITEKTURA ETA DATU-FLUXUAK", fill=(0, 0, 0), font=font_header, anchor="mm")
    draw.line([(50, 80), (width - 50, 80)], fill=(0, 0, 0), width=1)

    def draw_box(x1, y1, x2, y2, title, desc, tech, fill_c=(255, 255, 255)):
        draw.rectangle([(x1, y1), (x2, y2)], outline=(0, 0, 0), width=2, fill=fill_c)
        cx = (x1 + x2) // 2
        draw.text((cx, y1 + 18), title, fill=(0, 0, 0), font=font_box_tit, anchor="mm")
        if desc:
            draw.text((cx, y1 + 38), desc, fill=(0, 0, 0), font=font_box_sub, anchor="mm")
        if tech:
            draw.text((cx, y2 - 16), tech, fill=(60, 60, 60), font=font_box_tech, anchor="mm")

    # 1. TALDEA: BEZEROAK (Goi-Ezkerra)
    draw.rectangle([(60, 110), (460, 310)], outline=(0, 0, 0), width=1, fill=(250, 250, 250))
    draw.text((75, 128), "1. BEZEROAK", fill=(0, 0, 0), font=font_sec)
    draw_box(80, 150, 440, 285, "Web Publikoa & Erreserbak", "Karta, alergenoak, erreserbak", "[web_php | HTML5, JS, PHP]")

    # 2. TALDEA: ZERBITZARIAK ETA JANTOKIA (Goi-Eskuina)
    draw.rectangle([(1240, 110), (1640, 390)], outline=(0, 0, 0), width=1, fill=(250, 250, 250))
    draw.text((1255, 128), "2. ZERBITZARIAK ETA JANTOKIA", fill=(0, 0, 0), font=font_sec)
    draw_box(1260, 150, 1620, 255, "TPV Aretoa & Kutxa", "Mahai-mapa, erreserba egutegia, fitxatu", "[desktop_csharp | .NET 10 WinForms]")
    draw_box(1260, 275, 1620, 375, "Komandera Android", "Komandak, mahai-egoerak, fitxaketa", "[zerbitzaria_komandera_android | Compose]")

    # 3. TALDEA: SUKALDEA ETA BILTEGIA (Behe-Ezkerra)
    draw.rectangle([(60, 850), (460, 1180)], outline=(0, 0, 0), width=1, fill=(250, 250, 250))
    draw.text((75, 868), "3. SUKALDEA ETA BILTEGIA", fill=(0, 0, 0), font=font_sec)
    draw_box(80, 890, 440, 975, "Android TV Sukaldea (KDS)", "Pantaila handiko komandak", "[sukaldea_kds_android_tv | Compose TV]")
    draw_box(80, 990, 440, 1075, "JavaFX KDS & Biltegia", "Sukaldea, Barra (BDS), Stock & Fitxatu", "[desktop_javafx | Java 17/21]")
    draw_box(80, 1090, 440, 1165, "Android Mugikorra / Tableta", "Barrako komandak eta fitxaketa", "[zerbitzaria_komandera_android]")

    # 4. TALDEA: ADMIN IT, ZUZENDARITZA & ERP (Behe-Eskuina)
    draw.rectangle([(1240, 850), (1640, 1180)], outline=(0, 0, 0), width=1, fill=(250, 250, 250))
    draw.text((1255, 868), "4. KUDEAKETA, ADMIN IT & ERP", fill=(0, 0, 0), font=font_sec)
    draw_box(1260, 890, 1620, 975, "Admin IT Zentrala", "Docker, sareak, VMak, abiarazleak", "[admin_IT_zentrala | Web]")
    draw_box(1260, 990, 1620, 1075, "JavaFX Mahaigaina", "Admin/Zuzendaritza ikuspegi osoa", "[desktop_javafx | Java 17/21]")
    draw_box(1260, 1090, 1620, 1165, "Odoo 17 ERP", "Finantzak, erosketak, nominak", "[odoo_erp | Python / Web]")

    # ZENTROA: BACKEND REST API ZENTRALA
    draw.rectangle([(600, 360), (1100, 520)], outline=(0, 0, 0), width=3, fill=(240, 240, 240))
    draw.text((850, 395), "ASP.NET CORE REST API ZENTRALA", fill=(0, 0, 0), font=font_sec, anchor="mm")
    draw.text((850, 425), "Ekosistemaren Negozio Logika, Balidazioak & Transakzioak", fill=(0, 0, 0), font=font_box_sub, anchor="mm")
    draw.text((850, 452), "Segurtasuna: BCrypt PIN Hash, RBAC, HTTPS", fill=(0, 0, 0), font=font_conn, anchor="mm")
    draw.text((850, 485), "[Portua: 5050 | Docker jatetxea-api]", fill=(60, 60, 60), font=font_box_tech, anchor="mm")

    # ZENTROA: DATU-BASEAK (MYSQL ETA POSTGRESQL)
    # MySQL Operatiboa
    draw.rectangle([(550, 660), (870, 790)], outline=(0, 0, 0), width=2, fill=(255, 255, 255))
    draw.text((710, 685), "MySQL Datu-Basea", fill=(0, 0, 0), font=font_box_tit, anchor="mm")
    draw.text((710, 710), "Jatetxearen Datu Operatiboak", fill=(0, 0, 0), font=font_box_sub, anchor="mm")
    draw.text((710, 735), "Langileak, Fitxaketak, Mahaiak, Platerak,", fill=(0, 0, 0), font=font_conn, anchor="mm")
    draw.text((710, 755), "Eskaerak, Erreserbak, Fakturak, Stock", fill=(0, 0, 0), font=font_conn, anchor="mm")
    draw.text((710, 775), "[Portua: 3308 | jatetxea_db]", fill=(60, 60, 60), font=font_box_tech, anchor="mm")

    # PostgreSQL Odoo
    draw.rectangle([(930, 660), (1220, 790)], outline=(0, 0, 0), width=2, fill=(255, 255, 255))
    draw.text((1075, 685), "PostgreSQL Datu-Basea", fill=(0, 0, 0), font=font_box_tit, anchor="mm")
    draw.text((1075, 710), "Odoo ERP Egitura", fill=(0, 0, 0), font=font_box_sub, anchor="mm")
    draw.text((1075, 735), "Kontabilitatea, Hornitzaileak, Fakturazio", fill=(0, 0, 0), font=font_conn, anchor="mm")
    draw.text((1075, 755), "Ofiziala, Nominak eta Langile Fitxaketak", fill=(0, 0, 0), font=font_conn, anchor="mm")
    draw.text((1075, 775), "[Portua: 5432 | jatetxea_odoo]", fill=(60, 60, 60), font=font_box_tech, anchor="mm")

    # LERROAK ETA KONEXIOAK
    # API -> MySQL
    draw.line([(710, 520), (710, 660)], fill=(0, 0, 0), width=2)
    draw.polygon([(705, 650), (715, 650), (710, 660)], fill=(0, 0, 0))
    draw.polygon([(705, 530), (715, 530), (710, 520)], fill=(0, 0, 0))
    draw.text((735, 580), "SQL / CRUD", fill=(0, 0, 0), font=font_conn)

    # API -> Odoo (Sinkronizazioa)
    draw.line([(1000, 520), (1000, 660)], fill=(0, 0, 0), width=2)
    draw.polygon([(995, 650), (1005, 650), (1000, 660)], fill=(0, 0, 0))
    draw.polygon([(995, 530), (1005, 530), (1000, 520)], fill=(0, 0, 0))
    draw.text((1025, 580), "Sync / ERP", fill=(0, 0, 0), font=font_conn)

    # Bezeroak -> API
    draw.line([(440, 215), (600, 400)], fill=(0, 0, 0), width=2)
    draw.polygon([(590, 395), (600, 400), (595, 388)], fill=(0, 0, 0))
    draw.text((470, 290), "HTTPS / REST", fill=(0, 0, 0), font=font_conn)

    # Zerbitzariak -> API
    draw.line([(1260, 215), (1100, 400)], fill=(0, 0, 0), width=2)
    draw.polygon([(1110, 395), (1100, 400), (1105, 388)], fill=(0, 0, 0))
    draw.text((1150, 290), "HTTP REST (LAN/Wi-Fi)", fill=(0, 0, 0), font=font_conn)

    # Sukaldea & Biltegia -> API
    draw.line([(440, 990), (600, 490)], fill=(0, 0, 0), width=2)
    draw.polygon([(595, 495), (600, 490), (590, 502)], fill=(0, 0, 0))
    draw.text((470, 720), "HTTP REST & Socket", fill=(0, 0, 0), font=font_conn)

    # Admin & Odoo -> API
    draw.line([(1260, 990), (1100, 490)], fill=(0, 0, 0), width=2)
    draw.polygon([(1105, 495), (1100, 490), (1110, 502)], fill=(0, 0, 0))
    draw.text((1150, 720), "HTTP REST & Management", fill=(0, 0, 0), font=font_conn)

    # Odoo -> PostgreSQL zuzenean
    draw.line([(1260, 1125), (1180, 1125), (1180, 790)], fill=(0, 0, 0), width=2)
    draw.polygon([(1175, 800), (1185, 800), (1180, 790)], fill=(0, 0, 0))

    img.save(irteera_bidea, "PNG")
    print(f"Diagrama sortua: {irteera_bidea}")

if __name__ == "__main__":
    diagrama_path = r"C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\04_dokumentazioa\espezifikazioak\arkitektura_diagrama.png"
    sortu_diagrama_irudia(diagrama_path)
