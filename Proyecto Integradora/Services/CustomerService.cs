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
            if (saldo.data != null)
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
                var httpResponse = await _httpClient.GetAsync($"{BaseUrl}/Creditos/saldo");
                var raw = await httpResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    try
                    {
                        var typed = JsonSerializer.Deserialize<SaldoCreditoResponse>(raw, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (typed != null)
                        {
                            return typed;
                        }
                    }
                    catch
                    {
                        // Intentamos parseo manual abajo para estructuras ligeramente distintas.
                    }

                    try
                    {
                        using var doc = JsonDocument.Parse(raw);
                        var root = doc.RootElement;

                        var parsed = new SaldoCreditoResponse
                        {
                            status = root.TryGetProperty("status", out var s) && s.ValueKind == JsonValueKind.True,
                            message = root.TryGetProperty("message", out var m) ? m.GetString() ?? string.Empty : string.Empty,
                            data = null
                        };

                        if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                        {
                            var dataParsed = new SaldoCreditoData
                            {
                                creditRequestId = TryGetInt(data, "creditRequestId") ?? 0,
                                userId = TryGetInt(data, "userId") ?? 0,
                                userName = TryGetString(data, "userName") ?? TryGetString(data, "usuario") ?? string.Empty,
                                saldoDisponible = TryGetDecimal(data, "saldoDisponible") ?? 0m,
                                status = TryGetString(data, "status") ?? string.Empty,
                                createdAt = TryGetDateTime(data, "createdAt") ?? default
                            };

                            parsed.data = dataParsed;
                        }

                        return parsed;
                    }
                    catch
                    {
                        // Si no puede parsearse, seguimos con fallback de error.
                    }
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

        private static string TryGetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var p))
            {
                return null;
            }

            return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
        }

        private static int? TryGetInt(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var p))
            {
                return null;
            }

            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var i))
            {
                return i;
            }

            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var parsed))
            {
                return parsed;
            }

            return null;
        }

        private static decimal? TryGetDecimal(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var p))
            {
                return null;
            }

            if (p.ValueKind == JsonValueKind.Number)
            {
                return p.GetDecimal();
            }

            if (p.ValueKind == JsonValueKind.String && decimal.TryParse(p.GetString(), out var parsed))
            {
                return parsed;
            }

            return null;
        }

        private static DateTime? TryGetDateTime(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var p))
            {
                return null;
            }

            if (p.ValueKind == JsonValueKind.String && DateTime.TryParse(p.GetString(), out var dt))
            {
                return dt;
            }

            return null;
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

        public async Task<bool?> TieneCreditoRegistradoAsync()
        {
            try
            {
                SetJwtHeader();
                var response = await _httpClient.GetAsync($"{BaseUrl}/Creditos/saldo");
                var raw = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] Raw Response: {raw}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] HTTP Error: {response.StatusCode}");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(raw))
                {
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] Empty response");
                    return null;
                }

                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;

                // Obtener status
                bool statusIsFalse = false;
                if (root.TryGetProperty("status", out var statusProp))
                {
                    statusIsFalse = statusProp.ValueKind == JsonValueKind.False;
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] status property found, is False: {statusIsFalse}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] status property NOT found");
                }

                // Obtener data
                bool dataIsNull = false;
                if (root.TryGetProperty("data", out var dataElement))
                {
                    dataIsNull = dataElement.ValueKind == JsonValueKind.Null;
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] data property found, is Null: {dataIsNull}, ValueKind: {dataElement.ValueKind}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] data property NOT found");
                }

                // Si status=false Y data=null → retorna true (mostrar formulario de solicitar crédito)
                if (statusIsFalse && dataIsNull)
                {
                    System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] RESULTADO: true (Mostrar formulario de solicitar)");
                    return true;
                }

                // En cualquier otro caso → retorna false (ir a pagar)
                System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] RESULTADO: false (Ir a pagar)");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TieneCreditoRegistradoAsync] Exception: {ex.Message}");
                return null;
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