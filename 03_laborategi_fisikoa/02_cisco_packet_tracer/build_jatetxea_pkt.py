import xml.etree.ElementTree as ET
import copy, os, subprocess

def build_jatetxea_pkt():
    src_xml = r'C:\Users\anurt\.gemini\antigravity-ide\brain\b8678417-c3d3-405b-984a-1a8822153580\scratch\1_ariketa.xml'
    out_dir = r'C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\03_laborategi_fisikoa\02_cisco_packet_tracer'
    xml_path = os.path.join(out_dir, 'topologia_orokorra.xml')
    pkt_path = os.path.join(out_dir, 'topologia_orokorra.pkt')

    t = ET.parse(src_xml)
    root = t.getroot()
    net = root.find('NETWORK')
    devs_el = net.find('DEVICES')
    links_el = net.find('LINKS')
    pw_el = root.find('PHYSICALWORKSPACE')

    dev_map = {}
    for d in devs_el:
        name = d.find('.//NAME').text
        dev_map[name] = d

    # Grab copper module template from Router1 slot 1 (PT-ROUTER-NM-1CFE)
    r1_copper_mod = copy.deepcopy(dev_map['Router1'].find('.//MODULE').findall('SLOT')[1].find('MODULE'))

    def set_pos(d, x, y):
        log = d.find('.//LOGICAL')
        if log is not None:
            gx, gy = log.find('X'), log.find('Y')
            if gx is not None: gx.text = str(x)
            if gy is not None: gy.text = str(y)

    def rename_pw_node(old_name, new_name):
        for n in pw_el.findall('.//NODE'):
            ne = n.find('NAME')
            if ne is not None and ne.text == old_name:
                ne.text = new_name
                break

    def remove_pw_node(name):
        for parent in pw_el.findall('.//CHILDREN'):
            for c in list(parent.findall('NODE')):
                ne = c.find('NAME')
                if ne is not None and ne.text == name:
                    parent.remove(c)

    def set_ios_config(d, hostname, lines):
        eng = d.find('ENGINE')
        if eng is not None:
            sn = eng.find('SYS_NAME')
            if sn is not None: sn.text = hostname
            nm = eng.find('NAME')
            if nm is not None: nm.text = hostname
        for tag in ['RUNNINGCONFIG', 'STARTUPCONFIG']:
            cfg = d.find('.//' + tag)
            if cfg is not None:
                cfg.clear()
                for line in lines:
                    el = ET.SubElement(cfg, 'LINE')
                    el.text = line

    def set_router_slots(router_dev, slot_specs):
        slots = router_dev.find('.//MODULE').findall('SLOT')
        for i in range(len(slots)):
            old_m = slots[i].find('MODULE')
            if old_m is not None:
                slots[i].remove(old_m)
            if i < len(slot_specs):
                ip, mask, desc = slot_specs[i]
                mod = copy.deepcopy(r1_copper_mod)
                p = mod.find('.//PORT')
                p.find('POWER').text = 'true'
                p.find('IP').text = ip
                p.find('SUBNET').text = mask
                p.find('DESCRIPTION').text = desc
                slots[i].append(mod)

    def set_switch_vlans(sw_dev, custom_vlans):
        # 1. Update <VLANS> engine tag
        vlans_el = sw_dev.find('.//VLANS')
        all_vlans = [('1', 'default')] + custom_vlans + [
            ('1002', 'fddi-default'),
            ('1003', 'token-ring-default'),
            ('1004', 'fddinet-default'),
            ('1005', 'trnet-default'),
        ]
        if vlans_el is not None:
            vlans_el.clear()
            for num, vname in all_vlans:
                v_el = ET.SubElement(vlans_el, 'VLAN')
                v_el.set('name', vname)
                v_el.set('number', num)
                v_el.set('rspan', '0')

        # 2. Update/add vlan.dat in FILE_MANAGER for flash persistence
        eng = sw_dev.find('ENGINE')
        if eng is not None:
            fm = eng.find('FILE_MANAGER')
            if fm is None:
                fm = ET.SubElement(eng, 'FILE_MANAGER')
            vdat = None
            for f in fm.findall('.//FILE'):
                if f.findtext('NAME') == 'vlan.dat':
                    vdat = f
                    break
            if vdat is None:
                dir_node = fm.find('.//DIRECTORY')
                if dir_node is None:
                    dir_node = fm
                vdat = ET.SubElement(dir_node, 'FILE', {'class': 'CFile'})
                ET.SubElement(vdat, 'FILE_NUMBER').text = '2'
                ET.SubElement(vdat, 'NAME').text = 'vlan.dat'
                ET.SubElement(vdat, 'DATE_TIME').text = '0'
                ET.SubElement(vdat, 'PERMISSION').text = '6'
            fc = vdat.find('FILE_CONTENT')
            if fc is None:
                fc = ET.SubElement(vdat, 'FILE_CONTENT', {'class': 'CVlanDatFileContent'})
            fc.clear()
            ET.SubElement(fc, 'VLAN_COUNT').text = str(len(all_vlans))
            v_node = ET.SubElement(fc, 'VLANS')
            for num, vname in all_vlans:
                ET.SubElement(v_node, 'VLAN', {'name': vname, 'number': num, 'rspan': '0'})
            vtp = ET.SubElement(fc, 'VTP')
            ET.SubElement(vtp, 'DOMAIN_NAME')
            ET.SubElement(vtp, 'MODE').text = '0'
            ET.SubElement(vtp, 'VERSION').text = '1'
            ET.SubElement(vtp, 'PASSWORD')
            ET.SubElement(vtp, 'CONFIG_REVISION').text = '4'
            ET.SubElement(vtp, 'UPDATER_IP').text = '0.0.0.0'
            ET.SubElement(vtp, 'UPDATE_TIMESTAMP').text = '3-1-93 00:02:25'

    def build_switch_running_config(hostname, port_configs):
        lines = [
            '!',
            'version 12.1',
            'no service timestamps log datetime msec',
            'no service timestamps debug datetime msec',
            'no service password-encryption',
            '!',
            f'hostname {hostname}',
            '!',
            'spanning-tree mode pvst',
            'spanning-tree extend system-id',
            '!',
        ]
        for port_num in range(1, 25):
            lines.append(f'interface FastEthernet0/{port_num}')
            if port_num in port_configs:
                lines.extend(port_configs[port_num])
            lines.append('!')
        lines.extend([
            'interface Vlan1',
            ' no ip address',
            ' shutdown',
            '!',
            'line con 0',
            '!',
            'line vty 0 4',
            ' login',
            'line vty 5 15',
            ' login',
            '!',
            'end',
        ])
        return lines

    def configure_server(orig_name, new_name, x, y, ip, mask, gw, dns, http_on=False, dns_records=None):
        srv = dev_map[orig_name]
        srv.find('.//NAME').text = new_name
        rename_pw_node(orig_name, new_name)
        set_pos(srv, x, y)

        port = srv.find('.//MODULE').find('.//PORT')
        port.find('IP').text = ip
        port.find('SUBNET').text = mask
        port.find('PORT_GATEWAY').text = gw
        port.find('PORT_DNS').text = dns

        eng = srv.find('ENGINE')
        if eng is not None:
            gw_el = eng.find('GATEWAY')
            if gw_el is not None: gw_el.text = gw
            dns_c = eng.find('DNS_CLIENT')
            if dns_c is not None:
                sip = dns_c.find('SERVER_IP')
                if sip is not None: sip.text = dns
            if http_on:
                http_srv = eng.find('HTTP_SERVER')
                if http_srv is not None:
                    he = http_srv.find('ENABLED')
                    if he is not None: he.text = '1'
            if dns_records:
                dns_srv = eng.find('DNS_SERVER')
                if dns_srv is not None:
                    de = dns_srv.find('ENABLED')
                    if de is not None: de.text = '1'
                    db = dns_srv.find('NAMESERVER-DATABASE')
                    if db is not None:
                        db.clear()
                        for domain, d_ip in dns_records:
                            rec = ET.SubElement(db, 'RESOURCE-RECORD')
                            ET.SubElement(rec, 'TYPE').text = 'A-REC'
                            ET.SubElement(rec, 'NAME').text = domain
                            ET.SubElement(rec, 'TTL').text = '86400'
                            ET.SubElement(rec, 'IPADDRESS').text = d_ip
        return srv

    def configure_pc(orig_name, new_name, x, y, ip, mask, gw, dns):
        pc = dev_map[orig_name]
        pc.find('.//NAME').text = new_name
        rename_pw_node(orig_name, new_name)
        set_pos(pc, x, y)

        eng = pc.find('ENGINE')
        if eng is not None:
            gw_el = eng.find('GATEWAY')
            if gw_el is not None: gw_el.text = gw
            dns_c = eng.find('DNS_CLIENT')
            if dns_c is not None:
                sip = dns_c.find('SERVER_IP')
                if sip is not None: sip.text = dns

        port = pc.find('.//MODULE').find('.//PORT')
        if port.find('PORT_DHCP_ENABLE') is not None:
            port.find('PORT_DHCP_ENABLE').text = 'false'
        port.find('IP').text = ip
        port.find('SUBNET').text = mask
        port.find('PORT_GATEWAY').text = gw
        port.find('PORT_DNS').text = dns
        return pc

    # --- 1. ROUTERS ---
    # GOIERRI-ESKOLA-GW (reuse Router1)
    r_goierri = dev_map['Router1']
    r_goierri.find('.//NAME').text = 'GOIERRI-ESKOLA-GW'
    rename_pw_node('Router1', 'GOIERRI-ESKOLA-GW')
    set_pos(r_goierri, 420, 80)
    set_router_slots(r_goierri, [
        ('192.168.115.254', '255.255.255.0', 'UPLINK-GELAKO-RACK-SW'),
        ('8.8.8.1', '255.255.255.0', 'INTERNET-SIMULAZIOA'),
    ])
    set_ios_config(r_goierri, 'GOIERRI-ESKOLA-GW', [
        '!', 'version 12.2', 'hostname GOIERRI-ESKOLA-GW', '!',
        'ip cef', 'no ipv6 cef', '!',
        'ip dhcp excluded-address 192.168.115.1 192.168.115.150',
        'ip dhcp excluded-address 192.168.115.159',
        'ip dhcp excluded-address 192.168.115.254', '!',
        'ip dhcp pool GOIERRI_WAN',
        ' network 192.168.115.0 255.255.255.0',
        ' default-router 192.168.115.254',
        ' dns-server 8.8.8.8', '!',
        'interface FastEthernet0/0',
        ' description UPLINK-GELAKO-RACK-SW',
        ' ip address 192.168.115.254 255.255.255.0',
        ' duplex auto', ' speed auto', '!',
        'interface FastEthernet1/0',
        ' description INTERNET-SIMULAZIOA',
        ' ip address 8.8.8.1 255.255.255.0',
        ' duplex auto', ' speed auto', '!',
        'ip classless', '!',
        'line con 0', 'line vty 0 4', ' login', '!', 'end'
    ])

    # OPNSENSE-FW (reuse Router3) -> 3 physical NICs (WAN, LAN, OPT1 DMZ)
    r_opnsense = dev_map['Router3']
    r_opnsense.find('.//NAME').text = 'OPNSENSE-FW'
    rename_pw_node('Router3', 'OPNSENSE-FW')
    set_pos(r_opnsense, 420, 260)
    set_router_slots(r_opnsense, [
        ('192.168.115.159', '255.255.255.0', 'WAN-GOIERRI-ESKOLA'),
        ('192.168.10.1', '255.255.255.0', 'LAN-JATETXEA'),
        ('192.168.20.1', '255.255.255.0', 'OPT1-DMZ-UBUNTU'),
    ])
    set_ios_config(r_opnsense, 'OPNSENSE-FW', [
        '!', 'version 12.2', 'hostname OPNSENSE-FW', '!',
        'ip cef', 'no ipv6 cef', '!',
        'interface FastEthernet0/0',
        ' description WAN-GOIERRI-ESKOLA',
        ' ip address 192.168.115.159 255.255.255.0',
        ' ip nat outside',
        ' duplex auto', ' speed auto', '!',
        'interface FastEthernet1/0',
        ' description LAN-JATETXEA',
        ' ip address 192.168.10.1 255.255.255.0',
        ' ip nat inside',
        ' duplex auto', ' speed auto', '!',
        'interface FastEthernet2/0',
        ' description OPT1-DMZ-UBUNTU',
        ' ip address 192.168.20.1 255.255.255.0',
        ' ip access-group DMZ_OPT1_IN in',
        ' ip nat inside',
        ' duplex auto', ' speed auto', '!',
        'ip nat inside source list 10 interface FastEthernet0/0 overload',
        'ip nat inside source static tcp 192.168.20.10 80 192.168.115.159 80',
        'ip nat inside source static tcp 192.168.20.10 5050 192.168.115.159 5050',
        'ip classless',
        'ip route 0.0.0.0 0.0.0.0 192.168.115.254', '!',
        'ip access-list extended DMZ_OPT1_IN',
        ' permit tcp host 192.168.20.10 host 192.168.10.19 eq 3306',
        ' deny ip 192.168.20.0 0.0.0.255 192.168.10.0 0.0.0.255',
        ' permit ip any any', '!',
        'access-list 10 permit 192.168.10.0 0.0.0.255',
        'access-list 10 permit 192.168.20.0 0.0.0.255', '!',
        'line con 0', 'line vty 0 4', ' login', '!', 'end'
    ])

    # --- 2. SWITCHES ---
    custom_vlans = [
        ('10', 'LAN'),
        ('20', 'DMZ'),
        ('30', 'WAN_GOIERRI'),
    ]

    sw_rack_ports = {}
    for p in range(1, 7):
        sw_rack_ports[p] = [' switchport access vlan 10', ' switchport mode access']
    for p in range(7, 13):
        sw_rack_ports[p] = [' switchport access vlan 20', ' switchport mode access']
    for p in range(13, 21):
        sw_rack_ports[p] = [' switchport access vlan 30', ' switchport mode access']
    # Fa0/24 -> Trunk to SW-MAHAIA
    sw_rack_ports[24] = [' switchport trunk allowed vlan 10,20,30', ' switchport mode trunk']

    sw_rack = dev_map['Switch0']
    sw_rack.find('.//NAME').text = 'SW-RACK'
    rename_pw_node('Switch0', 'SW-RACK')
    set_pos(sw_rack, 200, 260)
    set_switch_vlans(sw_rack, custom_vlans)
    set_ios_config(sw_rack, 'SW-RACK', build_switch_running_config('SW-RACK', sw_rack_ports))

    sw_mahaia_ports = {}
    for p in range(1, 7):
        sw_mahaia_ports[p] = [' switchport access vlan 10', ' switchport mode access']
    for p in range(7, 13):
        sw_mahaia_ports[p] = [' switchport access vlan 20', ' switchport mode access']
    for p in range(13, 21):
        sw_mahaia_ports[p] = [' switchport access vlan 30', ' switchport mode access']
    sw_mahaia_ports[24] = [' switchport trunk allowed vlan 10,20,30', ' switchport mode trunk']

    sw_mahaia = dev_map['Switch1']
    sw_mahaia.find('.//NAME').text = 'SW-MAHAIA'
    rename_pw_node('Switch1', 'SW-MAHAIA')
    set_pos(sw_mahaia, 680, 260)
    set_switch_vlans(sw_mahaia, custom_vlans)
    set_ios_config(sw_mahaia, 'SW-MAHAIA', build_switch_running_config('SW-MAHAIA', sw_mahaia_ports))

    # --- 3. SERVERS ---
    srv_win = configure_server('STailerra', 'WinServer2025-LAN', 120, 460, '192.168.10.19', '255.255.255.0', '192.168.10.1', '8.8.8.8')
    srv_ubuntu = configure_server('S-DMZ', 'UbuntuServer-DMZ', 290, 460, '192.168.20.10', '255.255.255.0', '192.168.20.1', '8.8.8.8', http_on=True)
    srv_inet = configure_server('SAdministrazioa', 'Internet-DNS-Server', 680, 80, '8.8.8.8', '255.255.255.0', '8.8.8.1', '8.8.8.8', http_on=True, dns_records=[
        ('jatetxea.eus', '192.168.115.159'),
        ('api.jatetxea.eus', '192.168.115.159'),
    ])

    # --- 4. PCs ---
    pc_tpv = configure_pc('Ta-1', 'Laptop-Ander-TPV', 580, 460, '192.168.10.50', '255.255.255.0', '192.168.10.1', '8.8.8.8')
    pc_kds = configure_pc('Ta-2', 'PC-Sukaldea-KDS', 740, 460, '192.168.10.51', '255.255.255.0', '192.168.10.1', '8.8.8.8')
    pc_wan = configure_pc('Ad-1', 'PC-Kanpokoa-WAN', 880, 260, '192.168.115.50', '255.255.255.0', '192.168.115.254', '8.8.8.8')

    # --- 5. REMOVE UNUSED DEVICES (keep Power Distribution Devices intact!) ---
    unused = ['Switch2', 'Switch3', 'Ad-2', 'PC4', 'Printer0', 'Server3']
    for u_name in unused:
        if u_name in dev_map:
            devs_el.remove(dev_map[u_name])
            remove_pw_node(u_name)

    # --- 6. LINKS ---
    links_el.clear()

    def add_link(from_dev, from_port, to_dev, to_port, cable_type='eStraightThrough', color='#6ba72e'):
        lnk = ET.SubElement(links_el, 'LINK')
        ET.SubElement(lnk, 'TYPE').text = 'eCopper'
        c = ET.SubElement(lnk, 'CABLE')
        ET.SubElement(c, 'LENGTH').text = '1'
        ET.SubElement(c, 'FUNCTIONAL').text = 'true'
        ET.SubElement(c, 'FROM').text = from_dev.find('.//SAVE_REF_ID').text
        ET.SubElement(c, 'PORT').text = from_port
        ET.SubElement(c, 'TO').text = to_dev.find('.//SAVE_REF_ID').text
        ET.SubElement(c, 'PORT').text = to_port
        ET.SubElement(c, 'GEO_VIEW_COLOR').text = color
        ET.SubElement(c, 'IS_MANAGED_IN_RACK_VIEW').text = 'true'
        ET.SubElement(c, 'TYPE').text = cable_type

    # Goierri Gateway Links
    add_link(r_goierri, 'FastEthernet0/0', sw_rack, 'FastEthernet0/20', 'eStraightThrough')
    add_link(r_goierri, 'FastEthernet1/0', srv_inet, 'FastEthernet0', 'eCrossOver')

    # OPNsense Firewall Links (3 physical NICs -> SW-RACK)
    add_link(r_opnsense, 'FastEthernet0/0', sw_rack, 'FastEthernet0/13', 'eStraightThrough')
    add_link(r_opnsense, 'FastEthernet1/0', sw_rack, 'FastEthernet0/1', 'eStraightThrough')
    add_link(r_opnsense, 'FastEthernet2/0', sw_rack, 'FastEthernet0/7', 'eStraightThrough')

    # SW-RACK to Servers
    add_link(sw_rack, 'FastEthernet0/2', srv_win, 'FastEthernet0', 'eStraightThrough')
    add_link(sw_rack, 'FastEthernet0/8', srv_ubuntu, 'FastEthernet0', 'eStraightThrough')

    # SW-RACK to SW-MAHAIA (Trunk on Fa0/24)
    add_link(sw_rack, 'FastEthernet0/24', sw_mahaia, 'FastEthernet0/24', 'eCrossOver')

    # SW-MAHAIA to Clients
    add_link(sw_mahaia, 'FastEthernet0/1', pc_tpv, 'FastEthernet0', 'eStraightThrough')
    add_link(sw_mahaia, 'FastEthernet0/2', pc_kds, 'FastEthernet0', 'eStraightThrough')
    add_link(sw_mahaia, 'FastEthernet0/13', pc_wan, 'FastEthernet0', 'eStraightThrough')

    # --- 7. NOTES (Logical Workspace Labels) ---
    notes_el = pw_el.find('NOTES')
    if notes_el is not None:
        notes_el.clear()
        note = ET.SubElement(notes_el, 'NOTE')
        ET.SubElement(note, 'X').text = '355'
        ET.SubElement(note, 'Y').text = '335'
        ET.SubElement(note, 'Z').text = '40000'
        ET.SubElement(note, 'TEXT').text = (
            'OPNsense 25.7 - PC 3 NICs (FreeBSD)\n'
            'Hardwarea: PC fisikoa (3 Ethernet txartel)\n'
            'Softwarea: Router / Suebakia / NAT\n'
            'WAN (Urdina): 192.168.115.159/24\n'
            'LAN (Horia): 192.168.10.1/24\n'
            'DMZ (Berdea): 192.168.20.1/24'
        )
        ET.SubElement(note, 'NOTECLUSTERID').text = '1-1'
        ET.SubElement(note, 'MEM_ADDR').text = '2081385480001'

    t.write(xml_path, encoding='utf-8', xml_declaration=True)
    print(f'Wrote XML: {xml_path}')

    repack_script = os.path.join(out_dir, 'tools', 'unpacket', 'repacket.py')
    res = subprocess.run(['python', repack_script, xml_path, '-o', pkt_path], check=True, capture_output=True, text=True)
    print(f'Repacketed to PKT: {pkt_path}')
    print(res.stdout)

if __name__ == '__main__':
    build_jatetxea_pkt()
