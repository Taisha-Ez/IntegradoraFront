using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Integradora.Models
{
    // Lo que enviamos
    public class SolicitudRequest
    {
        public int userId { get; set; }
        public decimal montoSolicitado { get; set; }
        public int plazoPagoMeses { get; set; }
    }

    // Lo que recibimos
    public class SolicitudResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; } // Opcional
    }
}