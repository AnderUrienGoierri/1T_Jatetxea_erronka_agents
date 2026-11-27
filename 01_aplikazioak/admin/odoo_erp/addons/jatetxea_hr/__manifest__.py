# -*- coding: utf-8 -*-
{
    'name': '1T Jatetxea - Giza Baliabideak, Langile Kudeaketa eta Marka Identitatea',
    'version': '17.0.2.0.0',
    'summary': 'Jatetxeko langileak, sailak, lanpostuak, fitxaketak, bajak, kontratuak eta T1 v2.0 diseinua',
    'description': """
    1T Jatetxea Erronka - Odoo 17 HR eta Marka Modulua
    ===================================================
    Modulu honek jatetxearen giza baliabideen kudeaketa osoa eta 1T Jatetxea v2.0
    identitate korporatiboa (Chianti #881337, Gold #D4A359, Warm Hospitality) integratzen ditu:
    * Sailak eta hierarkia zuzena: Zuzendaritza, Sistemak IT, Sukaldea, Jantokia, Barra, Biltegia
    * 15 Langileen fitxa integralak (datu pertsonalak, larrialdiak, IBAN, CV, gaitasunak, PIN/Badge)
    * Fitxaketak eta bertaratze kontrola (hr_attendance)
    * Oporrak eta lan-bajak (hr_holidays)
    * Lan-kontratuak eta soldatak (hr_contract)
    * 1T Jatetxea logo korporatiboa eta Web Portala (http://localhost:8070)
    """,
    'author': '1T Jatetxea IT Taldea (Ander Urien, Julen Aguado, Iker Araujo, Aratz Huegun)',
    'website': 'https://github.com/anderuriengoierri/1T_Jatetxea_erronka_agents',
    'category': 'Human Resources',
    'depends': [
        'base',
        'hr',
        'hr_attendance',
        'hr_holidays',
        'hr_contract',
        'hr_skills',
        'hr_org_chart',
        'website',
    ],
    'data': [
        'data/hr_department_data.xml',
        'data/hr_job_data.xml',
        'data/hr_employee_data.xml',
        'data/hr_contract_data.xml',
        'views/website_templates.xml',
    ],
    'assets': {
        'web.assets_frontend': [
            'jatetxea_hr/static/src/scss/jatetxea_theme.scss',
        ],
        'web.assets_backend': [
            'jatetxea_hr/static/src/scss/jatetxea_backend.scss',
        ],
    },
    'demo': [],
    'installable': True,
    'application': True,
    'auto_install': False,
    'license': 'LGPL-3',
}
