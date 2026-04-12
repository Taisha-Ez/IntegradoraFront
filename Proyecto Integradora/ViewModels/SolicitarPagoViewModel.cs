using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    public class SolicitarPagoViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _service = new CustomerService();

        private double _monto = 1000;
        private int _plazo = 3;
        private double _pagoMensual;
        private double _totalDevolver;
        private double _limiteCredito = 50000;

        public double LimiteCredito
        {
            get => _limiteCredito;
            set
            {
                _limiteCredito = value;
                OnPropertyChanged(nameof(LimiteCredito));

                if (Monto > _limiteCredito)
                {
                    Monto = _limiteCredito;
                }
            }
        }

        public double Monto
        {
            get => _monto;
            set
            {
                var boundedValue = value;
                if (boundedValue > LimiteCredito)
                {
                    boundedValue = LimiteCredito;
                }

                if (boundedValue < 1)
                {
                    boundedValue = 1;
                }

                _monto = boundedValue;
                OnPropertyChanged(nameof(Monto));
                CalcularResumen();
            }
        }

        public int Plazo
        {
            get => _plazo;
            set { _plazo = value; OnPropertyChanged(nameof(Plazo)); CalcularResumen(); }
        }

        public double PagoMensual
        {
            get => _pagoMensual;
            set { _pagoMensual = value; OnPropertyChanged(nameof(PagoMensual)); }
        }

        public double TotalDevolver
        {
            get => _totalDevolver;
            set { _totalDevolver = value; OnPropertyChanged(nameof(TotalDevolver)); }
        }

        public ICommand EnviarCommand { get; }

        public SolicitarPagoViewModel()
        {
            EnviarCommand = new RelayCommand(async () => await EnviarSolicitud());
            CalcularResumen(); // Cálculo inicial
            _ = CargarLimiteCredito();
        }

        private async Task CargarLimiteCredito()
        {
            var credito = await _service.ConsultarCreditoDisponibleAsync();
            if (credito.status && credito.limiteCredito > 0)
            {
                LimiteCredito = (double)credito.limiteCredito;
                return;
            }

            MessageBox.Show(string.IsNullOrWhiteSpace(credito.message)
                ? "No se pudo consultar el limite de credito. Se usara un limite temporal de $50,000."
                : credito.message,
                "Aviso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void CalcularResumen()
        {
            // Tasa del 5% anual según tu UI
            double tasaMensual = 0.05 / 12;
            TotalDevolver = Monto * (1 + (tasaMensual * Plazo));
            PagoMensual = TotalDevolver / Plazo;
        }

        private async Task EnviarSolicitud()
        {
            if (Monto > LimiteCredito)
            {
                MessageBox.Show($"El monto solicitado no puede superar tu limite disponible (${LimiteCredito:N2}).", "Validacion");
                return;
            }

            var req = new SolicitudRequest
            {
                montoSolicitar = (decimal)Monto,
                plazoPagoMeses = Plazo
            };

            var res = await _service.CrearSolicitudAsync(req);
            MessageBox.Show(res.message, res.status ? "Éxito" : "Error");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}