module eus.goierri.jatetxea.core {
    requires transitive javafx.controls;
    requires transitive javafx.fxml;
    requires transitive java.net.http;
    requires transitive com.google.gson;

    opens eus.goierri.jatetxea.core.model to javafx.base, com.google.gson;
    opens eus.goierri.jatetxea.core.api to javafx.base, com.google.gson;
    opens eus.goierri.jatetxea.core.controller to javafx.fxml;
    opens eus.goierri.jatetxea.core.session to com.google.gson;

    exports eus.goierri.jatetxea.core.model;
    exports eus.goierri.jatetxea.core.api;
    exports eus.goierri.jatetxea.core.session;
    exports eus.goierri.jatetxea.core.util;
    exports eus.goierri.jatetxea.core.controller;
}
