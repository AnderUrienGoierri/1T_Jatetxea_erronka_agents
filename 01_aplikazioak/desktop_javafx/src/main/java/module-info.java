module eus.goierri.jatetxea {
    // JavaFX
    requires javafx.controls;
    requires javafx.fxml;

    // Datu-basea (JDBC + MySQL driver)
    requires java.sql;
    requires mysql.connector.j;

    // APIa (HTTP bezeroa + JSON)
    requires java.net.http;
    requires com.google.gson;

    // FXML kontroladoreak irakurri ahal izateko
    opens eus.goierri.jatetxea.controller to javafx.fxml;

    // TableView-k ereduak irakurri ahal izateko (PropertyValueFactory)
    opens eus.goierri.jatetxea.model to javafx.base, com.google.gson;

    // Gson-ek eta TableView-k API klaseak irakurtzeko
    opens eus.goierri.jatetxea.api to com.google.gson, javafx.base;

    exports eus.goierri.jatetxea;
}
