# -*- coding: utf-8 -*-
{
    'name': '1T Jatetxea - Giza Baliabideak eta Langile Kudeaketa',
    'version': '17.0.1.0.0',
    'summary': 'Jatetxeko langileak, sailak, lanpostuak, fitxaketak, bajak eta kontratuak',
    'description': """
    1T Jatetxea Erronka - Odoo 17 HR Modulua
    =========================================
    Modulu honek jatetxearen giza baliabideen kudeaketa osoa automatizatzen du:
    * Sailak: Sukaldea, Jantokia, Barra, Biltegia, Sistemak IT, Zuzendaritza
    * IT Administrazio Taldea: Ander Urien, Julen Aguado, Iker Araujo, Aratz Huegun
    * Jatetxeko Langileak: Sukaldariak, Zerbitzariak, Barrakoak, Biltegikoak
    * Fitxaketak eta bertaratze kontrola (hr_attendance modulua erabiliz)
    * Oporrak eta lan-bajak (hr_holidays modulua erabiliz)
    * Lan-kontratuak eta soldatak (hr_contract modulua erabiliz)
    * MySQL jatetxea_db-rekin sinkronizazio gaitasuna
    """,
    'author': '1T Jatetxea IT Taldea (Ander Urien, Julen, Iker, Aratz)',
    'website': 'https://github.com/anderuriengoierri/1T_Jatetxea_erronka_agents',
    'category': 'Human Resources',
    'depends': [
        'base',
        'hr',
        'hr_attendance',
        'hr_holidays',
        'hr_contract',
    ],
    'data': [
        'data/hr_department_data.xml',
        'data/hr_job_data.xml',
        'data/hr_employee_data.xml',
        'data/hr_contract_data.xml',
    ],
    'demo': [],
    'installable': True,
    'application': True,
    'auto_install': False,
    'license': 'LGPL-3',
}
