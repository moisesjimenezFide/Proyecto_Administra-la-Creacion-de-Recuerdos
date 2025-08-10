<?php

try {

    $conn = new PDO("odbc:MiConexionRDS", "admin", "admin12345?"); 

    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    echo "¡Conexión exitosa a la base de datos de SQL Server!";

} catch (PDOException $e) {

    echo "¡Error de conexión! <br>";

    echo "Error: " . $e->getMessage();

}

?>

