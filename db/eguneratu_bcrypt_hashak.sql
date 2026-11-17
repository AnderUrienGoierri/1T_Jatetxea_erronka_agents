-- Langileen PINak BCrypt hash-etara bihurtzeko migrazioa
UPDATE langileak SET pasahitza_hash = '$2a$11$WjOP2N6biVYO1HrvAjYKyOmX5zpj5PZ2U16rjklM/tzSiUwNg6odG' WHERE erabiltzailea = 'anderu';
UPDATE langileak SET pasahitza_hash = '$2a$11$RqNGdk.gmKgvB5c4btG9leU8PMSBc782S1kS8PSXKsbt0D/HYVBAK' WHERE erabiltzailea = 'julen';
UPDATE langileak SET pasahitza_hash = '$2a$11$qJ8PWi0Cy4a4Cu.nNPwL0eDbGCPqI8M4C6c0JeqpnVmY9XmWttWUO' WHERE erabiltzailea = 'iker';
UPDATE langileak SET pasahitza_hash = '$2a$11$xikVH/hjNWd7KUpGt6w11Oyv4XAWbj3zzfYvUs4LXuVGkOCeUCsLC' WHERE erabiltzailea = 'aratz';
UPDATE langileak SET pasahitza_hash = '$2a$11$QznQKDnUuNCokupqxQ6RouhWUQkhtAcu6FiXjt0xxGPmNFTPoGjAG' WHERE erabiltzailea = 'ainhoa_chef';
UPDATE langileak SET pasahitza_hash = '$2a$11$TTG1qxhdXBNemtYawaY7OO.BKHvNx7QbzcOPHUl827LQ4QlySp0T2' WHERE erabiltzailea = 'eneko_suk';
UPDATE langileak SET pasahitza_hash = '$2a$11$qpinuX7AxybylayHyOrV9eA8WCDDBAkgwa5IxwG9bB/tFbtF8EiRS' WHERE erabiltzailea = 'mikel_zerb';
UPDATE langileak SET pasahitza_hash = '$2a$11$Wv/Yf.tCrQBwRSB5TGg82ez8.P/PRRQjdznEPcVao12YgaxqKjpoS' WHERE erabiltzailea = 'maite_zerb';
UPDATE langileak SET pasahitza_hash = '$2a$11$2t0Np6A5k7zkDlzh8Z6dreXnGyPU45VrelaqK0M44F2VrWqXIel.O' WHERE erabiltzailea = 'unai_zerb';
UPDATE langileak SET pasahitza_hash = '$2a$11$40DAjzi765gK1kjGKNbzZuuPEqpS4vg3TROpZ969ODsXytQ5GxgvO' WHERE erabiltzailea = 'gorka_barra';
UPDATE langileak SET pasahitza_hash = '$2a$11$ihyQpYEQBDmHDXNoAdy/Geuahgg6.WyCh4Yu.An2QvDSrXEwZrc16' WHERE erabiltzailea = 'uxue_barra';
UPDATE langileak SET pasahitza_hash = '$2a$11$xwax0r.nwsIPIwwEdSr.nO3hlwxCe5TIvj21OFFeRmCRd/vk3Z2WW' WHERE erabiltzailea = 'kepa_biltegi';
UPDATE langileak SET pasahitza_hash = '$2a$11$Fo4WievPUE35eElCleMJjusW.ENeraAMmOm9T6XDgPyGR2nKjB6zm' WHERE erabiltzailea = 'ander_m_zuz';
UPDATE langileak SET pasahitza_hash = '$2a$11$S7r5PkmmbwSvI2RySPVA1OKpEu/kmIxCdJhw9ZeXmCqY6tSSzhJYy' WHERE erabiltzailea = 'joseba_r_zuz';
UPDATE langileak SET pasahitza_hash = '$2a$11$maFEcb7ZQIzRSKjH0Rm.nu/2O7YLioEgLF0KPTyyXeqS47n6aJMjK' WHERE erabiltzailea = 'estebe_o_zuz';
