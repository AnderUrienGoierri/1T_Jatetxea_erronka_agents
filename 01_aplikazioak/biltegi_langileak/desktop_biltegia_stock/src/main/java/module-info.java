module eus.goierri.jatetxea.biltegia {
    requires eus.goierri.jatetxea.core;
    requires javafx.controls;
    requires javafx.fxml;
    requires com.google.gson;

    opens eus.goierri.jatetxea.biltegia to javafx.fxml;
    opens eus.goierri.jatetxea.biltegia.controller to javafx.fxml;

    exports eus.goierri.jatetxea.biltegia;
    exports eus.goierri.jatetxea.biltegia.controller;
}
