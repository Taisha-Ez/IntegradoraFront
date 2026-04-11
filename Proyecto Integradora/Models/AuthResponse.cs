namespace Proyecto_Integradora.Models
{
    public class AuthResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string data { get; set; } // Aquí llega tu Token JWT
    }
}