using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    public class PagarViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _service = new CustomerService();
        private ObservableCollection<Vale> _misVales;
        private Vale _valeSeleccionado;
        private string _montoPago = "0.00";
        private ObservableCollection<int> _mesesDisponibles = new ObservableCollection<int>();
        private int _mesesAPagar = 1;
        private decimal _pagoMensualEstimado;
        private decimal _montoPagoCalculado;

        public ObservableCollection<Vale> MisVales
        {
            get => _misVales;
            set
            {
                _misVales = value;
                OnPropertyChanged(nameof(MisVales));
            }
        }

        public Vale ValeSeleccionado
        {
            get => _valeSeleccionado;
            set
            {
                _valeSeleccionado = value;
                OnPropertyChanged(nameof(ValeSeleccionado));
                ConfigurarPagoParaVale();
            }
        }

        public ObservableCollection<int> MesesDisponibles
        {
            get => _mesesDisponibles;
            set
            {
                _mesesDisponibles = value;
                OnPropertyChanged(nameof(MesesDisponibles));
            }
        }

        public int MesesAPagar
        {
            get => _mesesAPagar;
            set
            {
                _mesesAPagar = value;
                OnPropertyChanged(nameof(MesesAPagar));
                RecalcularMontoPago();
            }
        }

        public decimal PagoMensualEstimado
        {
            get => _pagoMensualEstimado;
            set
            {
                _pagoMensualEstimado = value;
                OnPropertyChanged(nameof(PagoMensualEstimado));
            }
        }

        public string MontoPago
        {
            get => _montoPago;
            set
            {
                _montoPago = value;
                OnPropertyChanged(nameof(MontoPago));
            }
        }

        public ICommand VerTodosCommand { get; }
        public ICommand VerPendientesCommand { get; }
        public ICommand VerAceptadosCommand { get; }
        public ICommand VerRechazadosCommand { get; }
        public ICommand PagarValeCommand { get; }

        public PagarViewModel()
        {
            VerTodosCommand = new RelayCommand(async () => await CargarMisVales(""));
            VerPendientesCommand = new RelayCommand(async () => await CargarMisVales("Pendientes"));
            VerAceptadosCommand = new RelayCommand(async () => await CargarMisVales("Aceptados"));
            VerRechazadosCommand = new RelayCommand(async () => await CargarMisVales("Rechazados"));
            PagarValeCommand = new RelayCommand(async () => await PagarValeSeleccionado());

            _ = CargarMisVales("");
        }

        private async Task CargarMisVales(string status)
        {
            var lista = await _service.GetMisValesAsync(status);
            MisVales = new ObservableCollection<Vale>(lista);

            if (MisVales.Count == 0)
            {
                ValeSeleccionado = null;
                return;
            }

            ValeSeleccionado = MisVales[0];
        }

        private void ConfigurarPagoParaVale()
        {
            if (ValeSeleccionado == null)
            {
                MesesDisponibles = new ObservableCollection<int>();
                MesesAPagar = 1;
                PagoMensualEstimado = 0m;
                _montoPagoCalculado = 0m;
                MontoPago = "0.00";
                return;
            }

            PagoMensualEstimado = ValeSeleccionado.plazoPagoMeses > 0
                ? decimal.Round(ValeSeleccionado.montoSolicitado / ValeSeleccionado.plazoPagoMeses, 2)
                : 0m;

            var maxMeses = 1;
            if (PagoMensualEstimado > 0)
            {
                maxMeses = (int)System.Math.Ceiling((double)(ValeSeleccionado.montoRestante / PagoMensualEstimado));
                if (maxMeses < 1)
                {
                    maxMeses = 1;
                }
            }

            MesesDisponibles = new ObservableCollection<int>(Enumerable.Range(1, maxMeses));
            MesesAPagar = MesesDisponibles.FirstOrDefault();
        }

        private void RecalcularMontoPago()
        {
            if (ValeSeleccionado == null)
            {
                _montoPagoCalculado = 0m;
                MontoPago = "0.00";
                return;
            }

            if (PagoMensualEstimado <= 0)
            {
                _montoPagoCalculado = decimal.Round(ValeSeleccionado.montoRestante, 2);
                MontoPago = _montoPagoCalculado.ToString("0.00", CultureInfo.InvariantCulture);
                return;
            }

            var monto = decimal.Round(PagoMensualEstimado * MesesAPagar, 2);
            if (monto > ValeSeleccionado.montoRestante)
            {
                monto = decimal.Round(ValeSeleccionado.montoRestante, 2);
            }

            _montoPagoCalculado = monto;
            MontoPago = _montoPagoCalculado.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private async Task PagarValeSeleccionado()
        {
            if (ValeSeleccionado == null)
            {
                MessageBox.Show("Selecciona un vale para pagar.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_montoPagoCalculado <= 0)
            {
                MessageBox.Show("Selecciona una cantidad de meses valida para pagar.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = await _service.PagarValeAsync(ValeSeleccionado.id, _montoPagoCalculado);
            MessageBox.Show(res.message, res.status ? "Exito" : "Error", MessageBoxButton.OK,
                res.status ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (res.status)
            {
                await CargarMisVales("");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
