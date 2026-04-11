using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Proyecto_Integradora.Models;

namespace Proyecto_Integradora.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<SolicitudResponse> CrearSolicitudAsync(SolicitudRequest solicitud)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5185/api/Vales/Solicitar", solicitud);
                return await response.Content.ReadFromJsonAsync<SolicitudResponse>();
            }
            catch (Exception ex)
            {
                return new SolicitudResponse { status = false, message = ex.Message };
            }
        }
    }
}