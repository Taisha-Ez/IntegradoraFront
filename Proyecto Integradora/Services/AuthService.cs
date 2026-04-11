using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Proyecto_Integradora.Models;

namespace Proyecto_Integradora.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5185/");
        }

        public async Task<AuthResponse> LoginAsync(string usuario, string contrasenia)
        {
            try
            {
                var loginData = new { usuario, contrasenia };
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponse>();
                }
                return new AuthResponse { status = false, message = "Error de conexión con el servidor." };
            }
            catch (Exception ex)
            {
                return new AuthResponse { status = false, message = ex.Message };
            }
        }

        public async Task<RegistroResponse> RegisterAsync(RegistroRequest datos)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync("http://localhost:5185/api/Auth/register", datos);
                    return await response.Content.ReadFromJsonAsync<RegistroResponse>();
                }
            }
            catch (Exception ex)
            {
                return new RegistroResponse { status = false, message = ex.Message };
            }
        }
    }

}