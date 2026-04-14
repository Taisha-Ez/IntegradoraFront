using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Integradora.Models
{
    public class Customer
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string usuario { get; set; }
        public string tipo_usuario { get; set; }
        // Agrega aquí más campos si tu API devuelve saldos, estados, etc.
    }

    public class CustomerResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Customer> data { get; set; }
    }

    public class AdminCreditoCliente
    {
        public int userId { get; set; }
        public string usuario { get; set; }
        public string nombreCompleto { get; set; }
        public string tipoUsuario { get; set; }
        public int creditRequestId { get; set; }
        public string curpRfc { get; set; }
        public decimal monthlyIncome { get; set; }
        public decimal estimatedCredit { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class AdminCreditoClientesResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<AdminCreditoCliente> data { get; set; }
    }
}