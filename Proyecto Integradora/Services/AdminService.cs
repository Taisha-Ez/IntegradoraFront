using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Proyecto_Integradora.Models;

namespace Proyecto_Integradora.Services
{
    // Servicio exclusivo para funciones administrativas
    public class AdminService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<Customer>> GetCustomerListAsync(string type = "")
        {
            try
            {
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
    }
}