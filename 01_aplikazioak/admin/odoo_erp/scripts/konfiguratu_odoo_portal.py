# -*- coding: utf-8 -*-
"""
1T Jatetxea S.L. - Odoo 17 Web Portalaren Konfigurazioa
Script nagusi osoa 'konfiguratu_odoo_osoa.py' fitxategian dago.
Fitxategi hau atzeranzko bateragarritasunerako mantentzen da eta script nagusia exekutatzen du.
"""
import os

script_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "konfiguratu_odoo_osoa.py")
if os.path.exists(script_path):
    with open(script_path, "r", encoding="utf-8") as fh:
        exec(compile(fh.read(), script_path, "exec"))
else:
    print("Exekutatu konfiguratu_odoo_osoa.py Odoo shell barrutik.")
