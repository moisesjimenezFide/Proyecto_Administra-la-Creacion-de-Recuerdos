<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Prueba de Conexión</title>
</head>
<body>
    <h1>Estado de la Conexión a la Base de Datos</h1>
    <%
        string connectionString = ConfigurationManager.ConnectionStrings["BD_CREANDO_RECUERDOSEntities"].ConnectionString;

        // Extraer la cadena de conexión de SQL Server de la cadena de Entity Framework
        string sqlConnectionString = "";
        Match match = Regex.Match(connectionString, "provider connection string=\"(.+)\"");
        if (match.Success)
        {
            sqlConnectionString = match.Groups[1].Value.Replace("&quot;", "\"");
        }

        // Crear y probar la conexión
        SqlConnection connection = new SqlConnection(sqlConnectionString);
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