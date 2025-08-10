using System;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class HorarioModel
    {
        public int id_horario { get; set; }
        public int id_usuario { get; set; }
        public string dia_semana { get; set; }
        public TimeSpan hora_entrada { get; set; }
        public TimeSpan hora_salida { get; set; }
        public string nombre_usuario { get; set; }
    }
}
