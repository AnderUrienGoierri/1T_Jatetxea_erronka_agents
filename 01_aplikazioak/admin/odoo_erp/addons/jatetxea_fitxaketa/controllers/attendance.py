from odoo import http
from odoo.addons.hr_attendance.controllers.main import HrAttendance
from odoo.exceptions import AccessError
from odoo.http import request


class JatetxeaAttendance(HrAttendance):
    @http.route()
    def systray_attendance(self, latitude=False, longitude=False):
        employee = request.env.user.with_company(self._get_active_company(request)).employee_id
        if not employee:
            raise AccessError("No employee is linked to this user.")
        employee = employee.sudo()
        geo = self._get_geoip_response("systray", latitude=latitude, longitude=longitude)
        employee._attendance_action_change(geo)
        return self._get_employee_info_response(employee)

    @http.route()
    def scan_barcode(self, token, barcode):
        company = self._get_company(token)
        if company and company.attendance_kiosk_mode == "manual":
            return {}
        return super().scan_barcode(token, barcode)
