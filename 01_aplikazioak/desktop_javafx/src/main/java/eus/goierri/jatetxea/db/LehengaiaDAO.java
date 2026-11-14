package eus.goierri.jatetxea.db;

import eus.goierri.jatetxea.model.Lehengaia;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

/**
 * Lehengaiak datu-basetik zuzenean irakurtzeko DAO klasea (JDBC).
 */
public class LehengaiaDAO {

    public List<Lehengaia> lortuGuztiak() throws SQLException {
        List<Lehengaia> zerrenda = new ArrayList<>();
        String sql = "SELECT id, izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea FROM lehengaiak WHERE ezabatua = 0 ORDER BY izena ASC";

        try (Connection kon = DBKonexioa.getKonexioa();
             PreparedStatement stmt = kon.prepareStatement(sql);
             ResultSet rs = stmt.executeQuery()) {

            while (rs.next()) {
                zerrenda.add(new Lehengaia(
                        rs.getInt("id"),
                        rs.getString("izena"),
                        rs.getString("unitatea"),
                        rs.getDouble("uneko_stocka"),
                        rs.getDouble("gutxieneko_stocka"),
                        rs.getDouble("kostu_unitarioa"),
                        rs.getString("hornitzailea")
                ));
            }
        }
        return zerrenda;
    }
}
