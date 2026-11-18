package eus.goierri.jatetxea.admin.controller;

import javafx.scene.control.Alert;
import javafx.scene.control.ButtonType;

import java.util.Optional;

final class PantailaLaguntza {
    private PantailaLaguntza() {
    }

    static double parseNonNegative(String value, String fieldName) {
        try {
            double number = Double.parseDouble(value.trim().replace(',', '.'));
            if (!Double.isFinite(number) || number < 0) {
                throw new NumberFormatException();
            }
            return number;
        } catch (NumberFormatException ex) {
            throw new IllegalArgumentException(fieldName + ": sartu zero edo handiagoa den zenbaki bat.");
        }
    }

    static double parsePositive(String value, String fieldName) {
        double number = parseNonNegative(value, fieldName);
        if (number == 0) {
            throw new IllegalArgumentException(fieldName + ": zero baino handiagoa izan behar du.");
        }
        return number;
    }

    static boolean confirm(String title, String message) {
        Alert alert = new Alert(Alert.AlertType.CONFIRMATION, message, ButtonType.CANCEL, ButtonType.OK);
        alert.setTitle(title);
        alert.setHeaderText(null);
        Optional<ButtonType> answer = alert.showAndWait();
        return answer.orElse(ButtonType.CANCEL) == ButtonType.OK;
    }
}
