module eus.goierri.jatetxea.admin {
    requires javafx.controls;
    requires javafx.fxml;
    requires java.net.http;
    requires com.google.gson;
    requires atlantafx.base;

    exports eus.goierri.jatetxea.admin;
    opens eus.goierri.jatetxea.admin to javafx.fxml;
    opens eus.goierri.jatetxea.admin.controller to javafx.fxml;
    opens eus.goierri.jatetxea.admin.model to com.google.gson, javafx.base;
}
