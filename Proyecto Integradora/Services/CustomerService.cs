using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Core;

namespace Proyecto_Integradora.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "http://localhost:5185/api";

        private void SetJwtHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(UserSession.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);
            }
        }

        public async Task<SolicitudResponse> CrearSolicitudAsync(SolicitudRequest solicitud)
        {
            try
            {
                SetJwtHeader();
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Vales/Solicitar", solicitud);

                var body = await response.Content.ReadFromJsonAsync<SolicitudResponse>();
                if (body != null)
                {
                    return body;
                }

                return new SolicitudResponse
                {
                    status = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Solicitud enviada." : "No se pudo procesar la solicitud."
                };
            }
            catch (Exception ex)
            {
                return new SolicitudResponse { status = false, message = ex.Message };
            }
        }

        public async Task<List<Vale>> GetMisValesAsync(string status = "")
        {
            try
            {
                SetJwtHeader();

                var url = string.IsNullOrWhiteSpace(status)
                    ? $"{BaseUrl}/Vales/mis-vales"
                    : $"{BaseUrl}/Vales/mis-vales?status={Uri.EscapeDataString(status)}";

                var response = await _httpClient.GetFromJsonAsync<ValesResponse>(url);
                return response?.status == true ? response.data ?? new List<Vale>() : new List<Vale>();
            }
            catch
            {
                return new List<Vale>();
            }
        }

        public async Task<PagoValeResponse> PagarValeAsync(string valeId, decimal montoPago)
        {
            try
            {
                SetJwtHeader();
                var payload = new PagoValeRequest { montoPago = montoPago };

                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Vales/{valeId}/Pagar", payload);
                var body = await response.Content.ReadFromJsonAsync<PagoValeResponse>();

                if (body != null)
                {
                    return body;
                }

                return new PagoValeResponse
                {
                    status = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Pago aplicado correctamente." : "No se pudo aplicar el pago."
                };
            }
            catch (Exception ex)
            {
                return new PagoValeResponse { status = false, message = ex.Message };
            }
        }

        public async Task<CreditoDisponibleResponse> ConsultarCreditoDisponibleAsync()
        {
            var saldo = await ConsultarSaldoCreditoAsync();
            if (saldo.status && saldo.data != null)
            {
                return new CreditoDisponibleResponse
                {
                    status = true,
                    message = saldo.message,
                    limiteCredito = saldo.data.saldoDisponible
                };
            }

            var endpoints = new[]
            {
                $"{BaseUrl}/Customers/consultar-credito",
                $"{BaseUrl}/Customers/credito",
                $"{BaseUrl}/Customers/mi-credito"
            };

            foreach (var endpoint in endpoints)
            {
                try
                {
                    SetJwtHeader();
                    var response = await _httpClient.GetAsync(endpoint);
                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var raw = await response.Content.ReadAsStringAsync();
                    var parsed = ParseCreditoResponse(raw);
                    if (parsed.status)
                    {
                        return parsed;
                    }
                }
                catch
                {
                    // Probamos con el siguiente endpoint conocido.
                }
            }

            return new CreditoDisponibleResponse
            {
                status = false,
                message = "No fue posible consultar el credito disponible para validar el monto."
            };
        }

        public async Task<SaldoCreditoResponse> ConsultarSaldoCreditoAsync()
        {
            try
            {
                SetJwtHeader();
                var response = await _httpClient.GetFromJsonAsync<SaldoCreditoResponse>($"{BaseUrl}/Creditos/saldo");

                if (response != null)
                {
                    return response;
                }

                return new SaldoCreditoResponse
                {
                    status = false,
                    message = "No se recibio respuesta al consultar saldo de credito."
                };
            }
            catch (Exception ex)
            {
                return new SaldoCreditoResponse
                {
                    status = false,
                    message = ex.Message
                };
            }
        }

        public async Task<SolicitudCreditoResponse> SolicitarCreditoAsync(SolicitudCreditoRequest solicitud)
        {
            try
            {
                SetJwtHeader();
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Creditos/solicitar", solicitud);
                var body = await response.Content.ReadFromJsonAsync<SolicitudCreditoResponse>();

                if (body != null)
                {
                    return body;
                }

                return new SolicitudCreditoResponse
                {
                    status = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Solicitud de credito enviada." : "No se pudo enviar la solicitud de credito."
                };
            }
            catch (Exception ex)
            {
                return new SolicitudCreditoResponse
                {
                    status = false,
                    message = ex.Message
                };
            }
        }

        private static CreditoDisponibleResponse ParseCreditoResponse(string rawJson)
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;

            var result = new CreditoDisponibleResponse
            {
                status = root.TryGetProperty("status", out var statusProp) && statusProp.GetBoolean(),
                message = root.TryGetProperty("message", out var msgProp) ? msgProp.GetString() ?? string.Empty : string.Empty,
                limiteCredito = 0m
            };

            if (!root.TryGetProperty("data", out var dataProp))
            {
                return result;
            }

            if (dataProp.ValueKind == JsonValueKind.Number)
            {
                result.limiteCredito = dataProp.GetDecimal();
                return result;
            }

            if (dataProp.ValueKind == JsonValueKind.Object)
            {
                if (TryGetDecimalProperty(dataProp, "limite_credito", out var limite)
                    || TryGetDecimalProperty(dataProp, "limiteCredito", out limite)
                    || TryGetDecimalProperty(dataProp, "credito_disponible", out limite)
                    || TryGetDecimalProperty(dataProp, "creditoDisponible", out limite)
                    || TryGetDecimalProperty(dataProp, "montoDisponible", out limite)
                    || TryGetDecimalProperty(dataProp, "monto_disponible", out limite))
                {
                    result.limiteCredito = limite;
                }
            }

            return result;
        }

        private static bool TryGetDecimalProperty(JsonElement element, string propertyName, out decimal value)
        {
            value = 0m;
            if (!element.TryGetProperty(propertyName, out var prop))
            {
                return false;
            }

            if (prop.ValueKind == JsonValueKind.Number)
            {
                value = prop.GetDecimal();
                return true;
            }

            if (prop.ValueKind == JsonValueKind.String && decimal.TryParse(prop.GetString(), out var parsed))
            {
                value = parsed;
                return true;
            }

            return false;
        }
    }
}