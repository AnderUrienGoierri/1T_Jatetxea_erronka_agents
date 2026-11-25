# Helbideratze, Segmentazio, Suebaki eta Kredentzialen Plana (Egoera Erreala)

Goierri Eskolako laborategi fisikoan aplikatutako IP helbide, VLAN, bideratze, suebaki-arau eta sarbide-kredentzialen dokumentazio osoa.

---

## 1. Azpisareen eta VLANen Taula Erreala

| VLAN ID | Izena | Sarea / CIDR | Gateway | DHCP / Esleipena | Xedea / Funtzioa |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **VLAN 10** | `LAN` | `192.168.10.0/24` | `192.168.10.1` (`OPNsense re1`) | Estatikoak / OPNsense DHCP | Barne zerbitzariak (Windows Server 2025), Admin eta taldekideen portatilak |
| **VLAN 20** | `DMZ` (`OPT1`) | `192.168.20.0/24` | `192.168.20.1` (`OPNsense re0`) | Estatikoa (`192.168.20.10`) | Ubuntu Web zerbitzaria (`1taldea-dmz`), LANetik isolatuta |
| **VLAN 30** | `WAN` (Goierri) | `192.168.115.0/24` | `192.168.115.254` (Fortinet) | Eskolako DHCP (`192.168.115.254`) | Kanpoko sarea eta Interneterako irteera |
| **VLAN 1** | `MGMT` (Switch) | `192.168.1.0/24` | — | Estatikoa (`192.168.1.254`) | Cisco SG200-50 mahai gaineko switch-aren WebGUI kudeaketa |

---

## 2. IP Helbideen Inbentario Osoa (MAC eta Ostalariak)

### 2.1. VLAN 30 — WAN (`192.168.115.0/24`)
- `192.168.115.254` — Goierri Eskolako Fortinet Gateway & DHCP (`00-09-0f-09-00-0d`)
- `192.168.115.159` — OPNsense `WAN (re2)` (`00-18-e7-16-65-0c`, DHCP bidez jasoa)

### 2.2. VLAN 10 — LAN (`192.168.10.0/24`)
- `192.168.10.1` — OPNsense `LAN (re1)` Gateway & WebGUI (`bc-ee-7b-77-22-ec`)
- `192.168.10.19` — Windows Server 2025 `WIN-U1CN1H8S33S` (`00-e0-4c-43-24-63`, estatikoa, GW `192.168.10.1`, DNS `192.168.10.1`)
- `192.168.10.99` — Admin Portatila `ANDER-SGB5PRO` (`34-99-71-E4-43-ED`, estatikoa)
- `192.168.10.101 - .109` — Taldekideen portatilak (Cisco SG200-50 `LAN` ataketan)
- *(Hurrengo fasea: `.30` TPV, `.40` KDS, `.50` Inprimagailua)*

### 2.3. VLAN 20 — DMZ / OPT1 (`192.168.20.0/24`)
- `192.168.20.1` — OPNsense `OPT1 (re0)` Gateway (`00-e0-ff-68-0f-ff`)
- `192.168.20.10` — Ubuntu Server 26.04.1 LTS `1taldea-dmz` (`00-e0-4c-22-a0-9c`, Netplan estatikoa, GW `192.168.20.1`, DNS `1.1.1.1`, `192.168.20.1`)

---

## 3. Kredentzialen eta Sarbideen Taula (Erabiltzaileak eta Pasahitzak)

| Sistema | IP / URL | Portua | Erabiltzailea | Pasahitza | Konexio Komandoa / Metodoa |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **OPNsense 25.7** | `https://192.168.10.1` | TCP `443` | `root` | `opnsense` | Nabigatzailea (LANetik) |
| **Windows Server 2025** | `192.168.10.19` | TCP `3389` / `22` | `WIN-U1CN1H8S33S\Administrador` | `2MG31taldea` | `mstsc` (RDP) / `ssh Administrador@192.168.10.19` |
| **MySQL 8.4 (`jatetxea_db`)** | `192.168.10.19` | TCP `3306` | `root` / `jatetxea_user` | `2MG32026` / `Jatetxea2026!` | `jatetxea_user` soilik `192.168.20.10`-etik baimendua |
| **Ubuntu Server DMZ** | `192.168.20.10` | TCP `22` | `taldea1` | `1Taldea1` | `ssh taldea1@192.168.20.10` |
| **Web Publikoa (Apache/PHP 8.4)** | `http://192.168.20.10` / `http://192.168.115.159` | TCP `80` | — | — | Docker `jatetxea_web_dmz` (LAN eta WAN NAT) |
| **REST API (.NET 10)** | `http://192.168.20.10:5050` / `http://192.168.115.159:5050` | TCP `5050` | `anderu` (PIN `0001`) | `Admin1234!` | Docker `jatetxea_api_dmz` (LAN eta WAN NAT) |
| **Cisco SG200-50** | `http://192.168.1.254` | TCP `80/443` | `cisco` | `cisco` | Nabigatzailea (VLAN 1 atakatik) |

---

## 4. OPNsense Suebaki-Arauak eta NAT Publikoa (Aplikatuta)

| Interfazea | Ordena | Ekintza | Protokoloa | Jatorria (Source) | Helburua (Destination) | Deskribapena |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **OPT1 (DMZ)** | 1 | **PASS** | `IPv4 TCP` | `192.168.20.10/32` | `192.168.10.19/32 : 3306` | `Baimendu DMZ API -> LAN MySQL (3306)` |
| **OPT1 (DMZ)** | 2 | **BLOCK** | `IPv4 *` | `OPT1 net` | `LAN net` | `Blokeatu DMZ -> LAN` (gainerako LAN osoa blokeatuta) |
| **OPT1 (DMZ)** | 3 | **PASS** | `IPv4 *` | `OPT1 net` | `* (any)` | `Baimendu DMZ -> Internet` (eguneraketak/Docker) |
| **LAN** | 1 | **PASS** | `IPv4 *` | `LAN net` | `* (any)` | `Default allow LAN to any` (LAN -> DMZ eta Internet) |
| **WAN (NAT)** | 1 | **PORT FWD** | `IPv4 TCP` | `*` | `WAN address : 80` -> `192.168.20.10:80` | `Web Publikoa DMZ (80)` |
| **WAN (NAT)** | 2 | **PORT FWD** | `IPv4 TCP` | `*` | `WAN address : 5050` -> `192.168.20.10:5050` | `REST API DMZ (5050)` |

---

## 5. Erabilitako Konfigurazio Komandoak (Erreferentzia)

### 5.1. Windows Server 2025 (PowerShell)
```powershell
netsh interface ip set address name="Ethernet" static 192.168.10.19 255.255.255.0 192.168.10.1
netsh interface ip set dns name="Ethernet" static 192.168.10.1
net user Administrador 2MG31taldea
```

### 5.2. Ubuntu Server DMZ (`/etc/netplan/00-installer-config.yaml`)
```bash
sudo tee /etc/netplan/00-installer-config.yaml << 'EOF'
network:
  version: 2
  ethernets:
    enp1s0:
      dhcp4: false
      addresses:
        - 192.168.20.10/24
      routes:
        - to: default
          via: 192.168.20.1
      nameservers:
        addresses: [1.1.1.1, 192.168.20.1]
EOF
sudo chmod 600 /etc/netplan/00-installer-config.yaml
sudo netplan apply
sudo systemctl enable --now ssh
```

### 5.3. Admin / Taldekideen Portatila (Windows PowerShell Administratzailea)
```powershell
# LAN IPa gehitu eta DMZ (192.168.20.0/24) OPNsense-ra bideratu
netsh interface ip add address name="Ethernet 5" 192.168.10.99 255.255.255.0
route add 192.168.20.0 mask 255.255.255.0 192.168.10.1

# Amaitzean DHCP automatikora itzultzeko:
netsh interface ip set address name="Ethernet 5" dhcp
netsh interface ip set dns name="Ethernet 5" dhcp
```
