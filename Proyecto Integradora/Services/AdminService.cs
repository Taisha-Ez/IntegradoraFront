using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Core;

namespace Proyecto_Integradora.Services
{
    // Servicio exclusivo para funciones administrativas
    public class AdminService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        private void SetJwtHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(UserSession.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);
            }
        }

        public async Task<List<Customer>> GetCustomerListAsync(string type = "")
        {
            try
            {
                SetJwtHeader();
                // Solo el Admin consume esta ruta
                string url = string.IsNullOrEmpty(type)
                    ? "http://localhost:5185/api/Customers"
                    : $"http://localhost:5185/api/Customers/{type}";

                var response = await _httpClient.GetFromJsonAsync<CustomerResponse>(url);
                return response?.status == true ? response.data : new List<Customer>();
            }
            catch (Exception)
            {
                return new List<Customer>();
            }
        }
        public async Task<List<Vale>> GetValesListAsync(string status = "")
        {
            try
            {
                SetJwtHeader();
                // Ruta: /api/Vales/{status} o /api/Vales para todos
                string url = string.IsNullOrEmpty(status)
                    ? "http://localhost:5185/api/Vales"
                    : $"http://localhost:5185/api/Vales/{status}";

                var response = await _httpClient.GetFromJsonAsync<ValesResponse>(url);
                return response?.status == true ? response.data : new List<Vale>();
            }
            catch (Exception)
            {
                return new List<Vale>();
            }
        }

        public async Task<List<Vale>> GetAdminValesAsync()
        {
            try
            {
                SetJwtHeader();
                var response = await _httpClient.GetFromJsonAsync<ValesResponse>("http://localhost:5185/api/admin/vales");
                return response?.status == true ? response.data : new List<Vale>();
            }
            catch (Exception)
            {
                return new List<Vale>();
            }
        }

        public async Task<ResolverValeResponse> ResolverValeAsync(string valeId, string status)
        {
            try
            {
                SetJwtHeader();
                var payload = new ResolverValeRequest { status = status };
                var response = await _httpClient.PostAsJsonAsync($"http://localhost:5185/api/admin/vales/{valeId}/resolver", payload);
                var rawContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(rawContent))
                {
                    return new ResolverValeResponse
                    {
                        status = response.IsSuccessStatusCode,
                        message = response.IsSuccessStatusCode
                            ? "Vale resuelto correctamente."
                            : $"No se pudo resolver el vale. HTTP {(int)response.StatusCode}."
                    };
                }

                ResolverValeResponse body = null;
                try
                {
                    body = JsonSerializer.Deserialize<ResolverValeResponse>(rawContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch
                {
                    // El backend respondio texto plano o HTML; usamos fallback sin romper la app.
                }

                if (body != null)
                {
                    return body;
                }

                return new ResolverValeResponse
                {
                    status = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? rawContent : $"No se pudo resolver el vale: {rawContent}"
                };
            }
            catch (Exception ex)
            {
                return new ResolverValeResponse
                {
                    status = false,
                    message = ex.Message
                };
            }
        }
    }
}