using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    public class InfoClienteViewModel : INotifyPropertyChanged
    {
        private readonly AdminService _adminService = new AdminService();

        private ObservableCollection<AdminCreditoCliente> _clientes;
        public ObservableCollection<AdminCreditoCliente> Clientes
        {
            get => _clientes;
            set { _clientes = value; OnPropertyChanged(nameof(Clientes)); }
        }

        public ICommand RecargarCommand { get; }

        public InfoClienteViewModel()
        {
            RecargarCommand = new RelayCommand(async () => await ObtenerDatos());
            _ = ObtenerDatos();
        }

        private async Task ObtenerDatos()
        {
            var datos = await _adminService.GetAdminCreditoClientesAsync();
            Clientes = new ObservableCollection<AdminCreditoCliente>(datos);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}