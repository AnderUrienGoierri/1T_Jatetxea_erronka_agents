package eus.goierri.jatetxea.db;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

/**
 * MySQL datu-basearekin (3308 portua / jatetxea_db) JDBC bidez konektatzeko klasea.
 */
public class DBKonexioa {
    private static final String URL = "jdbc:mysql://localhost:3308/jatetxea_db?useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=UTC";
    private static final String ERABILTZAILEA = "root";
    private static final String PASAHITZA = "root";

    public static Connection getKonexioa() throws SQLException {
        return DriverManager.getConnection(URL, ERABILTZAILEA, PASAHITZA);
    }
}
