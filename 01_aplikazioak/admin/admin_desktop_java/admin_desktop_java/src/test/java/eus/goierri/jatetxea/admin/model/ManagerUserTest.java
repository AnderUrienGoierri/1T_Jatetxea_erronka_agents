package eus.goierri.jatetxea.admin.model;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ManagerUserTest {
    @Test
    void managerRolesCanEnterBackOffice() {
        KudeatzaileErabiltzailea user = new KudeatzaileErabiltzailea();
        user.rola = "Kudeatzailea";

        assertTrue(user.isManager());
    }

    @Test
    void adminItRoleCanEnterBackOffice() {
        KudeatzaileErabiltzailea user = new KudeatzaileErabiltzailea();
        user.rola = "AdminIT";

        assertTrue(user.isManager());
    }

    @Test
    void employeeRolesCannotEnterBackOffice() {
        KudeatzaileErabiltzailea user = new KudeatzaileErabiltzailea();
        user.rola = "Zerbitzaria";

        assertFalse(user.isManager());
    }
}
