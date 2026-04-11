using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    public class ValesExpedidosViewModel : INotifyPropertyChanged
    {
        private readonly AdminService _adminService = new AdminService();
        private ObservableCollection<Vale> _vales;

        public ObservableCollection<Vale> Vales
        {
            get => _vales;
            set { _vales = value; OnPropertyChanged(nameof(Vales)); }
        }

        public ICommand VerTodosCommand { get; }
        public ICommand VerAceptadosCommand { get; }
        public ICommand VerRechazadosCommand { get; }

        public ValesExpedidosViewModel()
        {
            VerTodosCommand = new RelayCommand(async () => await CargarVales(""));
            VerAceptadosCommand = new RelayCommand(async () => await CargarVales("aceptados"));
            VerRechazadosCommand = new RelayCommand(async () => await CargarVales("rechazados"));

            // Carga inicial al abrir la ventana
            _ = CargarVales("");
        }

        private async Task CargarVales(string filtro)
        {
            var lista = await _adminService.GetValesListAsync(filtro);
            Vales = new ObservableCollection<Vale>(lista);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}