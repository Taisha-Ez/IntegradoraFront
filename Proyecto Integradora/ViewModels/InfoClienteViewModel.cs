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

        private ObservableCollection<Customer> _clientes;
        public ObservableCollection<Customer> Clientes
        {
            get => _clientes;
            set { _clientes = value; OnPropertyChanged(nameof(Clientes)); }
        }

        // Comandos separados para evitar el error de argumentos en el delegado
        public ICommand CargarTodosCommand { get; }
        public ICommand CargarCumplidosCommand { get; }
        public ICommand CargarMorososCommand { get; }

        public InfoClienteViewModel()
        {
            CargarTodosCommand = new RelayCommand(async () => await ObtenerDatos(""));
            CargarCumplidosCommand = new RelayCommand(async () => await ObtenerDatos("Cumplidos"));
            CargarMorososCommand = new RelayCommand(async () => await ObtenerDatos("Morosos"));

            // Carga inicial
            _ = ObtenerDatos("");
        }

        private async Task ObtenerDatos(string filtro)
        {
            var datos = await _adminService.GetCustomerListAsync(filtro);
            Clientes = new ObservableCollection<Customer>(datos);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}