using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Integradora.Models
{
    public class RegistroRequest
    {
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string usuario { get; set; }
        public string contrasenia { get; set; }
        public string tipo_usuario { get; set; } = "cliente"; // Por defecto
    }

    public class RegistroResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}