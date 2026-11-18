module eus.goierri.jatetxea.sukaldea {
    requires eus.goierri.jatetxea.core;
    requires javafx.controls;
    requires javafx.fxml;
    requires com.google.gson;

    opens eus.goierri.jatetxea.sukaldea to javafx.fxml;
    opens eus.goierri.jatetxea.sukaldea.controller to javafx.fxml;

    exports eus.goierri.jatetxea.sukaldea;
    exports eus.goierri.jatetxea.sukaldea.controller;
}
