"""Run with Odoo shell; set ODOO_DEMO_DRY_RUN=1 to roll back the changes."""

import os


translations = [
    ("hr.home_work_location", "name", "Home", "Etxea"),
    ("hr.home_work_office", "name", "Office", "Bulegoa"),
    ("hr.home_work_other", "name", "Other", "Beste bat"),
    ("hr.departure_fired", "name", "Fired", "Kaleratzea"),
    ("hr.departure_resigned", "name", "Resigned", "Borondatezko baja"),
    ("hr.departure_retired", "name", "Retired", "Erretiroa"),
    ("hr_skills.hr_skill_type_lang", "name", "Languages", "Hizkuntzak"),
    ("hr_skills.resume_type_experience", "name", "Experience", "Esperientzia"),
    ("hr_skills.resume_type_education", "name", "Education", "Ikasketak"),
    ("hr_holidays.holiday_status_cl", "name", "Paid Time Off", "Opor ordainduak"),
    ("hr_holidays.holiday_status_sl", "name", "Sick Time Off", "Gaixotasun baja"),
    ("hr_holidays.holiday_status_unpaid", "name", "Unpaid", "Ordaindu gabeko baimena"),
    ("hr_holidays.holiday_status_comp", "name", "Compensatory Days", "Konpentsazio egunak"),
    ("hr.onboarding_plan", "name", "Onboarding", "Langile berrien harrera"),
    ("hr.offboarding_plan", "name", "Offboarding", "Langilearen irteera"),
    ("mail.mail_activity_data_email", "name", "Email", "Mezu elektronikoa"),
    ("mail.mail_activity_data_call", "name", "Call", "Deia"),
    ("mail.mail_activity_data_meeting", "name", "Meeting", "Bilera"),
    ("mail.mail_activity_data_todo", "name", "To-Do", "Egitekoa"),
    ("mail.mail_activity_data_upload_document", "name", "Upload Document", "Dokumentua igo"),
    ("hr_holidays.mail_act_leave_approval", "name", "Time Off Approval", "Baimena onartu"),
    ("hr_holidays.mail_act_leave_second_approval", "name", "Time Off Second Approve", "Baimenaren bigarren onarpena"),
    ("hr_holidays.mail_act_leave_allocation_approval", "name", "Allocation Approval", "Esleipena onartu"),
]

plan_steps = [
    ("hr.onboarding_setup_it_materials", "Setup IT Materials", "IT ekipamendua prestatu",
     "DEMO: Prestatu kontua eta lanerako behar duen ekipamendua."),
    ("hr.onboarding_plan_training", "Plan Training", "Prestakuntza planifikatu",
     "DEMO: Antolatu lanpostuari eta segurtasunari buruzko harrera."),
    ("hr.onboarding_training", "Training", "Prestakuntza egin",
     "DEMO: Berrikusi laneko prozedurak eta fitxatzeko modua."),
    ("hr.offboarding_setup_compute_out_delais", "Organize knowledge transfer inside the team",
     "Ezagutza taldeari transferitu",
     "DEMO: Adostu lanen eta dokumentazioaren eskualdaketa."),
    ("hr.offboarding_take_back_hr_materials", "Take Back HR Materials", "Materiala itzuli",
     "DEMO: Egiaztatu emandako materialaren itzulera eta sarbideen itxiera."),
]

for xmlid, field, original, translated in translations:
    record = env.ref(xmlid, raise_if_not_found=False)
    if not record:
        print("FALTA:", xmlid)
        continue
    english = record.with_context(lang="en_US")
    current = english[field]
    if current not in (original, translated):
        print("ALDATU GABE (pertsonalizatua):", xmlid)
        continue
    if current != translated:
        english.write({field: translated})
    if record._fields[field].translate:
        for language in env["res.lang"].search([("active", "=", True)]).mapped("code"):
            record.with_context(lang=language).write({field: translated})
    print("ITZULITA:", xmlid)

for xmlid, original, translated, note in plan_steps:
    record = env.ref(xmlid, raise_if_not_found=False)
    if not record:
        print("FALTA:", xmlid)
        continue
    if record.summary not in (original, translated):
        print("ALDATU GABE (pertsonalizatua):", xmlid)
        continue
    values = {"summary": translated}
    if not record.note:
        values["note"] = "<p>%s</p>" % note
    record.write(values)
    print("PRESTATUTA:", xmlid)

company = env.company
company.write({
    "attendance_from_systray": True,
    "attendance_kiosk_mode": "manual",
    "attendance_kiosk_use_pin": True,
})
print("FITXAKETA: kontua eta eskuzko kioskoa PINarekin")

if os.environ.get("ODOO_DEMO_DRY_RUN") == "1":
    env.cr.rollback()
    print("PROBA: aldaketak atzera bota dira")
else:
    env.cr.commit()
    print("ALDAKETAK GORDETA")
