<?php
// Adminer laguntzailea Odoo PostgreSQL eta 1T Jatetxea MySQL datu-baseetara automatikoki konektatzeko

if (empty($_GET) && empty($_POST)) {
    header('Location: ./?pgsql=db-odoo&username=odoo&db=jatetxea_odoo');
    exit;
}

if (isset($_GET['server']) && $_GET['server'] === 'db-odoo') {
    $params = $_GET;
    unset($params['server']);
    $params['pgsql'] = 'db-odoo';
    if (empty($params['username'])) $params['username'] = 'odoo';
    if (empty($params['db'])) $params['db'] = 'jatetxea_odoo';
    header('Location: ./?' . http_build_query($params));
    exit;
}

if (!empty($_POST['auth']['server']) && $_POST['auth']['server'] === 'db-odoo' && (!isset($_POST['auth']['driver']) || $_POST['auth']['driver'] === 'server')) {
    $_POST['auth']['driver'] = 'pgsql';
}

return new class extends \Adminer\Plugin {
    public function credentials() {
        if (\Adminer\DRIVER === 'pgsql' && (\Adminer\SERVER === 'db-odoo' || \Adminer\SERVER === 'localhost')) {
            $password = !empty($_POST['auth']['password']) ? $_POST['auth']['password'] : 'odoo';
            return array(\Adminer\SERVER, $_GET['username'] ?: 'odoo', $password);
        }
        if ((\Adminer\DRIVER === 'server' || \Adminer\DRIVER === 'mysql') && (\Adminer\SERVER === 'jatetxea-db' || \Adminer\SERVER === 'localhost')) {
            $password = !empty($_POST['auth']['password']) ? $_POST['auth']['password'] : 'root';
            return array(\Adminer\SERVER, $_GET['username'] ?: 'root', $password);
        }
        return null;
    }
    public function login($login, $password) {
        return true;
    }
};
