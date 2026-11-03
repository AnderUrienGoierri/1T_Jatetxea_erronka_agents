# Datu-Base Arauak (MySQL 8.0 InnoDB - v2.0)

1. **Integritate Erreferentziala:**
   - Atzerriko gakoetan (Foreign Keys) **BETI `ON DELETE RESTRICT`** erabili.
2. **Lehengaien Stock eta Errezeta (Eskandallo) Egitura:**
   - Plater bakoitzaren osagaiak `plater_osagaiak` N:M taulan definitzen dira.
   - Kontsumitzean, lehengai bakoitzaren proportzioa deskontatzen da transakzio bakar baten barruan.
   - Auditoria bermatuta dago `stock_mugimenduak` taularen bidez.
3. **Ikuspegi Automatikoak (Views):**
   - `v_erosi_beharreko_lehengaiak`: Gutxieneko stocketik behera dauden lehengaiak bistaratzen ditu (Erosketa-zerrenda).
   - `v_plateren_stock_ahalmena`: Lehengaien uneko stockaren arabera zenbat plater egin daitezkeen kalkulatzen du.
4. **Blokeo Pesimistak (`SELECT ... FOR UPDATE`):**
   - Erreserbetan mahaiak blokeatu, eta platerak eskatzean errezetako lehengai guztiak blokeatu.
   - Blokeo-ordena uniformea: `mahaiak` -> `erreserbak / eskaerak` -> `lehengaiak / platerak`.
5. **Soft Delete printzipioa:**
   - Datu operatiboetan (`mahaiak`, `platerak`, `lehengaiak`, `langileak`), ezabaketak `ezabatua = 1` bidez egingo dira.
