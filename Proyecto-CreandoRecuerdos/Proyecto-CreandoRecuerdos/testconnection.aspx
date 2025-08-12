<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Prueba de Conexión</title>
</head>
<body>
    <h1>Estado de la Conexión a la Base de Datos</h1>
    <%
        string connectionString = ConfigurationManager.ConnectionStrings["BD_CREANDO_RECUERDOSEntities"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        string message = "";

        try
        {
            connection.Open();
            message = "¡Conexión exitosa a la base de datos!";
            connection.Close();
        }
        catch (Exception ex)
        {
            message = "Error de conexión: " + ex.Message;
        }
    %>
    <p>
        <%= message %>
    </p>
</body>
</html>