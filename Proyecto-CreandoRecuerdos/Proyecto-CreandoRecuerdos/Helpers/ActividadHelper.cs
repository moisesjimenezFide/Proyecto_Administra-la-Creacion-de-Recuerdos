using Newtonsoft.Json;
using Proyecto_CreandoRecuerdos.base_de_datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_CreandoRecuerdos.Helpers
{
    public static class ActividadHelper
    {
        public static void RegistrarAccion(
            int idUsuario,
            string tipoAccion,
            string tablaAfectada,
            int? idRegistroAfectado = null,
            Dictionary<string, object> valoresAnteriores = null,
            Dictionary<string, object> valoresNuevos = null,
            string descripcionAdicional = "")
        {
            try
            {
                var ip = HttpContext.Current?.Request.UserHostAddress;
                var descripcion = GenerarDescripcion(tipoAccion, tablaAfectada, descripcionAdicional);

                using (var context = new BD_CREANDO_RECUERDOSEntities())
                {
                    context.sp_registrar_auditoria(
                        idUsuario,
                        tipoAccion,
                        tablaAfectada,
                        idRegistroAfectado,
                        valoresAnteriores != null ? SerializarDiccionario(valoresAnteriores) : null,
                        valoresNuevos != null ? SerializarDiccionario(valoresNuevos) : null,
                        descripcion
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar auditoría: {ex.Message}");
            }
        }



        private static string GenerarDescripcion(string tipoAccion, string tablaAfectada, string adicional)
        {
            return $"{tipoAccion} en {tablaAfectada}. {adicional}";
        }

        public static string SerializarDiccionario(Dictionary<string, object> diccionario)
        {
            if (diccionario == null) return "{}";

            try
            {
                return JsonConvert.SerializeObject(diccionario, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                });
            }
            catch
            {
                var diccionarioSimple = diccionario.ToDictionary(
                    k => k.Key,
                    v => v.Value?.ToString() ?? "null");
                return JsonConvert.SerializeObject(diccionarioSimple);
            }
        }
    }
}