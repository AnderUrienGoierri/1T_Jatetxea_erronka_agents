# Erronka 1 - Makina Birtualen Kredentzialak eta Konexioak (EGIAZTATUTA)

Dokumentu honek VirtualBox-en sortutako hiru makinen konexio datuak, sareko portuak eta erabiltzaile/pasahitzak jasotzen ditu. Makina guztiak instalatuta eta zerbitzuak martxan daude.

---

## 1. Kredentzial Nagusiak (Guztietan berdinak)

* **Erabiltzailea:** `ander`
* **Pasahitza:** `Errotagain169232$$`
* **Root / Administratzailea pasahitza:** `Errotagain169232$$`

---

## 2. Makinak eta Portuen Bideratzea (NAT Port Forwarding)

| Makina | Sistema Eragilea | Host Portua (Windows) | Gonbidatu Portua (VM) | Zerbitzua | Konexio Bidea / Komandoa |
| :--- | :--- | :---: | :---: | :--- | :--- |
| **Erronka-Ubuntu** | Ubuntu Desktop 26.04 | `2230` | `22` | SSH | `ssh -p 2230 ander@127.0.0.1` |
| **Erronka-OPNsense** | OPNsense 26.7 (FreeBSD) | `2231` | `22` | SSH | `ssh -p 2231 ander@127.0.0.1` edo `root` |
| | | `8443` / `8080` | `80` | WebGUI | Nabigatzailean: `http://127.0.0.1:8443` (Erabiltzailea: `root`) |
| **Erronka-WinServer2025** | Windows Server 2025 | `3391` | `3389` | RDP | `mstsc /v:127.0.0.1:3391` (Erabiltzailea: `.\ander`) |
| | | `2232` | `22` | SSH | `ssh -p 2232 ander@127.0.0.1` |

> **OHARRA OPNsense WebGUI-rako:**
> Nabigatzailean sartzean, idatzi derrigorrez **`http://`** eta EZ `https://` (adibidez: `http://127.0.0.1:8443` edo `http://127.0.0.1:8080`).
> WebGUI-n sartzeko erabiltzailea **`root`** da eta pasahitza **`Errotagain169232$$`**.

---

## 3. Windows Terminal Konfigurazioa (Profilak)

Zure Windows Terminaleko ezarpenetan (`Settings` -> `Add new profile`), hiru makinetarako sarbide azkarreko profilak sortu ditzakezu:

### Profila 1: Ubuntu Desktop (SSH)
* **Nombre:** `Erronka - Ubuntu (SSH)`
* **Línea de comandos:** `ssh -p 2230 ander@127.0.0.1`
* **Directorio de inicio:** `Usar directorio de proceso primario`

### Profila 2: OPNsense (SSH)
* **Nombre:** `Erronka - OPNsense (SSH)`
* **Línea de comandos:** `ssh -p 2231 root@127.0.0.1`
* **Directorio de inicio:** `Usar directorio de proceso primario`

### Profila 3: Windows Server 2025 (SSH)
* **Nombre:** `Erronka - WinServer 2025 (SSH)`
* **Línea de comandos:** `ssh -p 2232 ander@127.0.0.1`
* **Directorio de inicio:** `Usar directorio de proceso primario`

---

## 4. Makinak Pizteko eta Kudeatzeko Komandoak

### Atzeko planoan piztu (Headless moduan - Gomendatua, RAM/CPU gutxiago erabiltzeko):
```powershell
& 'C:\Program Files\Oracle\VirtualBox\VBoxManage.exe' startvm "Erronka-Ubuntu" --type headless
& 'C:\Program Files\Oracle\VirtualBox\VBoxManage.exe' startvm "Erronka-OPNsense" --type headless
& 'C:\Program Files\Oracle\VirtualBox\VBoxManage.exe' startvm "Erronka-WinServer2025" --type headless
```

### Modu seguruan itzali:
```powershell
& 'C:\Program Files\Oracle\VirtualBox\VBoxManage.exe' controlvm "Erronka-Ubuntu" acpipowerbutton
& 'C:\Program Files\Oracle\VirtualBox\VBoxManage.exe' controlvm "Erronka-OPNsense" acpipowerbutton
& 'C:\Program Files\Oracle\VirtualBox\VBoxManage.exe' controlvm "Erronka-WinServer2025" acpipowerbutton
```
