using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using Proyecto_CreandoRecuerdos.base_de_datos;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Proyecto_CreandoRecuerdos.Helpers
{
    public static class JwtManager
    {
        // Clave secreta tomada del Web.config
        private static string Secret = ConfigurationManager.AppSettings["JwtSecretKey"];

        // 🔹 Generar el token
        public static string GenerateToken(string username, string role, string userId, int expireMinutes = 60)
        {
            var key = Encoding.UTF8.GetBytes(Secret);
            var handler = new JwtSecurityTokenHandler();
            var jti = Guid.NewGuid().ToString(); // Identificador único del token

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "JWT"),
                    new Claim("jti", jti)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);

            // Guarda el jti en la base de datos para el usuario
            GuardarJtiEnBD(userId, jti);

            return handler.WriteToken(token);
        }

        // 🔹 Validar el token
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(Secret);
            var handler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true, // 👈 verifica expiración
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            SecurityToken validatedToken;
            return handler.ValidateToken(token, parameters, out validatedToken);
        }

        // Método para guardar el jti en la base de datos (implementa según tu ORM)
        private static void GuardarJtiEnBD(string userId, string jti)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuario = context.tabla_usuarios.Find(int.Parse(userId));
                if (usuario != null)
                {
                    usuario.jti = jti;
                    context.SaveChanges();
                }
            }
        }

        public static bool JtiValido(string userId, string jti)
        {
            using (var context = new BD_CREANDO_RECUERDOSEntities())
            {
                var usuario = context.tabla_usuarios.Find(int.Parse(userId));
                return usuario != null && usuario.jti == jti;
            }
        }
    }
}
