FROM odoo:17

USER root
COPY docker/odoo/config/odoo.conf /etc/odoo/odoo.conf
RUN chown odoo:odoo /etc/odoo/odoo.conf

USER odoo
EXPOSE 8069

CMD ["odoo"]
