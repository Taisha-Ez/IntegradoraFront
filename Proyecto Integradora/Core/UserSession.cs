using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Integradora.Core
{
    class UserSession
    {
        // Aquí guardamos el token JWT
        public static string Token { get; set; }

        // Puedes guardar también el rol o el nombre si los necesitas a la mano
        public static string Role { get; set; }
        public static string Usuario { get; set; }

        // Método para "Cerrar Sesión"
        public static void Clear()
        {
            Token = null;
            Role = null;
            Usuario = null;
        }
    }
}
