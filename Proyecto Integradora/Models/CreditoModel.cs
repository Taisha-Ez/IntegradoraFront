using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Proyecto_Integradora.Models
{
    public class ReferenciaCreditoRequest
    {
        [JsonPropertyName("parentesco")]
        public string parentesco { get; set; }

        [JsonPropertyName("nombre")]
        public string nombre { get; set; }

        [JsonPropertyName("numero_contacto")]
        public string numeroContacto { get; set; }
    }

    public class SolicitudCreditoRequest
    {
        [JsonPropertyName("nombre_completo")]
        public string nombreCompleto { get; set; }

        [JsonPropertyName("curp_rfc")]
        public string curpRfc { get; set; }

        [JsonPropertyName("direccion")]
        public string direccion { get; set; }

        [JsonPropertyName("telefono")]
        public string telefono { get; set; }

        [JsonPropertyName("ingresos_mensuales")]
        public decimal ingresosMensuales { get; set; }

        [JsonPropertyName("referencias")]
        public List<ReferenciaCreditoRequest> referencias { get; set; }
    }

    public class SaldoCreditoData
    {
        public int creditRequestId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public decimal saldoDisponible { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class SaldoCreditoResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public SaldoCreditoData data { get; set; }
    }

    public class SolicitudCreditoResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
