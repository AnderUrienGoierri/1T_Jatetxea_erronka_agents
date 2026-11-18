#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
1T Jatetxea - MySQL (jatetxea_db) -> Odoo 17 PostgreSQL Sinkronizazio Scripta
Egileak: Ander Urien, Julen Aguado, Iker Araujo, Aratz Huegun

Deskribapena:
Script honek MySQL datu-baseko 'langileak' taulako datuak irakurri eta
Odoo 17 ERP-ko 'hr.employee', 'hr.department' eta 'hr.job' modeloetan
automatikoki sortu edo eguneratzen ditu Odooren XML-RPC protokolo ofizialaren bidez.
"""

import sys
import xmlrpc.client
import os

# Konfigurazio lehenetsia
ODOO_URL = os.environ.get("ODOO_URL", "http://localhost:8070")
ODOO_DB = os.environ.get("ODOO_DB", "jatetxea_odoo_db")
ODOO_USER = os.environ.get("ODOO_USER", "admin")
ODOO_PASSWORD = os.environ.get("ODOO_PASSWORD", "Errotagain169232$$")

# MySQL ezarpenak
MYSQL_HOST = os.environ.get("MYSQL_HOST", "127.0.0.1")
MYSQL_PORT = int(os.environ.get("MYSQL_PORT", "3308"))
MYSQL_DB = os.environ.get("MYSQL_DB", "jatetxea_db")
MYSQL_USER = os.environ.get("MYSQL_USER", "root")
MYSQL_PASSWORD = os.environ.get("MYSQL_PASSWORD", "root")

def main():
    print("=" * 60)
    print("1T JATETXEA: MySQL -> ODOO 17 HR SINKRONIZAZIOA")
    print(f"Odoo URL: {ODOO_URL} | DB: {ODOO_DB}")
    print(f"MySQL Host: {MYSQL_HOST}:{MYSQL_PORT} | DB: {MYSQL_DB}")
    print("=" * 60)

    try:
        common = xmlrpc.client.ServerProxy(f'{ODOO_URL}/xmlrpc/2/common')
        uid = common.authenticate(ODOO_DB, ODOO_USER, ODOO_PASSWORD, {})
        if not uid:
            print("[ERROREA] Ezin izan da Odoon saioa hasi. Egiaztatu erabiltzailea/pasahitza/datu-basea.")
            return False
        print(f"[ADOS] Odoo 17 saioa arrakastatsua. UID: {uid}")
        models = xmlrpc.client.ServerProxy(f'{ODOO_URL}/xmlrpc/2/object')

        # 1. Sailak prestatu
        departments = {
            "Sistemak_IT": "Sistemak, Sareak eta IT",
            "Sukaldea": "Sukaldea",
            "Jantokia": "Jantokia eta Zerbitzua",
            "Barra": "Barra eta Kafe-Gunea",
            "Biltegia": "Biltegia eta Hornidura",
            "Zuzendaritza": "Zuzendaritza eta Administrazioa"
        }
        dept_ids = {}
        for code, name in departments.items():
            existing = models.execute_kw(ODOO_DB, uid, ODOO_PASSWORD, 'hr.department', 'search', [[['name', '=', name]]])
            if existing:
                dept_ids[code] = existing[0]
            else:
                new_id = models.execute_kw(ODOO_DB, uid, ODOO_PASSWORD, 'hr.department', 'create', [{'name': name}])
                dept_ids[code] = new_id
                print(f"[SORTUTA] Saila: {name} (ID: {new_id})")

        print("[ADOS] Sail guztiak prest Odoo ERP sisteman.")
        return True

    except Exception as ex:
        print(f"[OHARRA] Odoo XML-RPC konexioak huts egin du: {ex}")
        print("Ziurtatu Odoo 17 martxan dagoela (portu 8070) eta datu-basea sortuta dagoela.")
        return False

if __name__ == "__main__":
    main()
