module eus.goierri.jatetxea.barra {
    requires eus.goierri.jatetxea.core;
    requires javafx.controls;
    requires javafx.fxml;
    requires com.google.gson;

    opens eus.goierri.jatetxea.barra to javafx.fxml;
    opens eus.goierri.jatetxea.barra.controller to javafx.fxml;

    exports eus.goierri.jatetxea.barra;
    exports eus.goierri.jatetxea.barra.controller;
}
