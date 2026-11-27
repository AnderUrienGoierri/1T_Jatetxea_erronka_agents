package eus.goierri.jatetxea.admin.api;

import eus.goierri.jatetxea.admin.model.Faktura;
import eus.goierri.jatetxea.admin.model.FinantzaLaburpena;
import eus.goierri.jatetxea.admin.model.KudeatzaileErabiltzailea;
import eus.goierri.jatetxea.admin.model.TxatEgoera;
import eus.goierri.jatetxea.admin.model.TxatMezua;

import java.io.IOException;
import java.util.List;

/**
 * REST API bezeroaren kontratu-interfazea (OOP abstrakzioa eta testagarritasuna).
 */
public interface ApiKontratua {
    String getBaseUrl();

    ApiBezeroa.ApiHealth checkHealth() throws IOException, InterruptedException;

    KudeatzaileErabiltzailea login(String username, String password) throws IOException, InterruptedException;

    <T> List<T> getList(String path, Class<T> itemType) throws IOException, InterruptedException;

    <T> T getData(String path, Class<T> dataType) throws IOException, InterruptedException;

    void post(String path, Object body) throws IOException, InterruptedException;

    void put(String path, Object body) throws IOException, InterruptedException;

    void delete(String path) throws IOException, InterruptedException;

    List<Faktura> getFakturak(String mota) throws IOException, InterruptedException;

    FinantzaLaburpena getFinantzaLaburpena() throws IOException, InterruptedException;

    void sortuFaktura(Object body) throws IOException, InterruptedException;

    void aldatuFakturaEgoera(int id, String egoera) throws IOException, InterruptedException;

    ApiBezeroa.ServiceStatus getServiceStatus() throws IOException, InterruptedException;

    void eguneratuLangileTxata(int id, boolean txataAktibo) throws IOException, InterruptedException;

    void eguneratuLangileAktibo(int id, boolean aktibo) throws IOException, InterruptedException;

    TxatEgoera getTxatEgoera(int langileaId) throws IOException, InterruptedException;

    List<TxatMezua> getTxatMezuak(String gela, Integer langileaId) throws IOException, InterruptedException;

    void bidaliTxatMezua(int langileaId, String gela, String testua) throws IOException, InterruptedException;
}
