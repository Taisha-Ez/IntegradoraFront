using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Proyecto_Integradora.Models
{
    public class Vale
    {
        public string id { get; set; }
        public int userId { get; set; }
        public string usuario { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public decimal montoSolicitado { get; set; }
        public decimal montoRestante { get; set; }
        public int plazoPagoMeses { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class ValesResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Vale> data { get; set; }
    }
}