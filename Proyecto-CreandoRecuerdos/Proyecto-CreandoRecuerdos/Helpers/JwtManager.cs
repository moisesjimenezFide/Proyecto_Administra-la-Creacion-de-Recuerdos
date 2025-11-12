using Microsoft.IdentityModel.Tokens;
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
        public static string GenerateToken(string username, string role, int expireMinutes = 60)
        {
            var key = Encoding.UTF8.GetBytes(Secret);
            var handler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "JWT")
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
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
    }
}
