package eus.goierri.jatetxea.admin.api;

import java.io.IOException;

public final class ApiSalbuespena extends IOException {
    private final int statusCode;

    public ApiSalbuespena(int statusCode, String message) {
        super("API errorea (" + statusCode + "): " + message);
        this.statusCode = statusCode;
    }

    public int getStatusCode() {
        return statusCode;
    }
}
