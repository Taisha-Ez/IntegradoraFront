using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Proyecto_Integradora.Models
{
    // Lo que enviamos
    public class SolicitudRequest
    {
        [JsonPropertyName("monto_solicitar")]
        public decimal montoSolicitar { get; set; }

        [JsonPropertyName("plazo_pago_meses")]
        public int plazoPagoMeses { get; set; }
    }

    public class PagoValeRequest
    {
        [JsonPropertyName("monto_pago")]
        public decimal montoPago { get; set; }
    }

    // Lo que recibimos
    public class SolicitudResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; } // Opcional
    }

    public class PagoValeResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class CreditoDisponibleResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public decimal limiteCredito { get; set; }
    }
}