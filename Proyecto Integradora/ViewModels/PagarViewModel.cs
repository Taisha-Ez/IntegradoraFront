using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        private string _montoPago = "0.01";

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

                if (_valeSeleccionado != null && _valeSeleccionado.montoRestante > 0)
                {
                    MontoPago = _valeSeleccionado.montoRestante.ToString("0.00", CultureInfo.InvariantCulture);
                }
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

        private async Task PagarValeSeleccionado()
        {
            if (ValeSeleccionado == null)
            {
                MessageBox.Show("Selecciona un vale para pagar.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(MontoPago, NumberStyles.Number, CultureInfo.InvariantCulture, out var monto) || monto <= 0)
            {
                MessageBox.Show("Ingresa un monto de pago valido mayor a 0.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = await _service.PagarValeAsync(ValeSeleccionado.id, monto);
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
