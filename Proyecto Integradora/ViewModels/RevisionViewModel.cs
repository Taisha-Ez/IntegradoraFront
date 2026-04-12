using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    public class RevisionViewModel : INotifyPropertyChanged
    {
        private readonly AdminService _adminService = new AdminService();
        private ObservableCollection<Vale> _valesPendientes;
        private Vale _valeSeleccionado;

        public ObservableCollection<Vale> ValesPendientes
        {
            get => _valesPendientes;
            set
            {
                _valesPendientes = value;
                OnPropertyChanged(nameof(ValesPendientes));
            }
        }

        public Vale ValeSeleccionado
        {
            get => _valeSeleccionado;
            set
            {
                _valeSeleccionado = value;
                OnPropertyChanged(nameof(ValeSeleccionado));
            }
        }

        public ICommand RecargarCommand { get; }

        public RevisionViewModel()
        {
            RecargarCommand = new RelayCommand(async () => await CargarPendientesAsync());
            _ = CargarPendientesAsync();
        }

        private async Task CargarPendientesAsync()
        {
            var todos = await _adminService.GetAdminValesAsync();

            // Refuerzo: detectamos cualquier variante de pendiente (Pendiente, Pendientes, PendienteRevision, etc.).
            var pendientes = todos
                .Where(v => !string.IsNullOrWhiteSpace(v.status)
                    && v.status.Contains("Pend", System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Si no hay pendientes, mostramos todo para evitar una tabla vacia.
            ValesPendientes = new ObservableCollection<Vale>(pendientes.Count > 0 ? pendientes : todos);

            if (ValesPendientes.Count > 0)
            {
                ValeSeleccionado = ValesPendientes[0];
            }
            else
            {
                ValeSeleccionado = null;
            }
        }

        public async Task<ResolverValeResponse> ResolverValeSeleccionadoAsync(string nuevoStatus)
        {
            if (ValeSeleccionado == null)
            {
                return new ResolverValeResponse
                {
                    status = false,
                    message = "Selecciona un vale antes de resolverlo."
                };
            }

            if (string.IsNullOrWhiteSpace(ValeSeleccionado.status)
                || !ValeSeleccionado.status.Contains("Pend", System.StringComparison.OrdinalIgnoreCase))
            {
                return new ResolverValeResponse
                {
                    status = false,
                    message = "Solo se pueden resolver vales en estado Pendiente."
                };
            }

            if (nuevoStatus != "Aceptado" && nuevoStatus != "Rechazado")
            {
                return new ResolverValeResponse
                {
                    status = false,
                    message = "El estado solo puede ser Aceptado o Rechazado."
                };
            }

            var resultado = await _adminService.ResolverValeAsync(ValeSeleccionado.id, nuevoStatus);
            if (resultado.status)
            {
                await CargarPendientesAsync();
            }

            return resultado;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
