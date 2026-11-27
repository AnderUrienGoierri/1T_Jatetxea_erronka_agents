module eus.goierri.autoak {
    requires javafx.base;
    requires javafx.graphics;
    requires javafx.controls;
    requires javafx.fxml;

    exports eus.goierri.autoak;
    exports eus.goierri.autoak.model;
    exports eus.goierri.autoak.controller;

    opens eus.goierri.autoak to javafx.fxml, javafx.graphics;
    opens eus.goierri.autoak.controller to javafx.fxml;
    opens eus.goierri.autoak.model to javafx.base;
}
