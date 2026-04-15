using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    public class CreditoViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _service = new CustomerService();

        private bool _isLoading = true;
        private string _saldoDisponible = "0.00";
        private string _usuario = "";
        private string _mensaje = "";
        private bool _tieneCreditoActivo = false;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public string SaldoDisponible
        {
            get => _saldoDisponible;
            set
            {
                _saldoDisponible = value;
                OnPropertyChanged(nameof(SaldoDisponible));
            }
        }

        public string Usuario
        {
            get => _usuario;
            set
            {
                _usuario = value;
                OnPropertyChanged(nameof(Usuario));
            }
        }

        public string Mensaje
        {
            get => _mensaje;
            set
            {
                _mensaje = value;
                OnPropertyChanged(nameof(Mensaje));
            }
        }

        public bool TieneCreditoActivo
        {
            get => _tieneCreditoActivo;
            set
            {
                _tieneCreditoActivo = value;
                OnPropertyChanged(nameof(TieneCreditoActivo));
            }
        }

        public CreditoViewModel()
        {
            CargarDatos();
        }

        private async void CargarDatos()
        {
            IsLoading = true;
            try
            {
                var response = await _service.ConsultarSaldoCreditoAsync();

                if (response != null && response.data != null)
                {
                    TieneCreditoActivo = true;
                    SaldoDisponible = response.data.saldoDisponible.ToString("C");
                    Usuario = response.data.userName ?? response.data.userName ?? "Usuario";
                    Mensaje = $"Tu saldo disponible es: {SaldoDisponible}";
                }
                else
                {
                    TieneCreditoActivo = false;
                    Mensaje = "No cuentas con un crédito autorizado activo.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar el saldo: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
