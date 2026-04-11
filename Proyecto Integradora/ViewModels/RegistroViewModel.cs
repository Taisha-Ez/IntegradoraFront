using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.ViewModels
{
    // Solo una definición de la clase
    public class RegistroViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService = new AuthService();

        // 1. Propiedades para los Bindings
        public string Nombre { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public string Usuario { get; set; }
        public string Contrasenia { get; set; }

        public ICommand RegresarCommand { get; }
        public ICommand ConfirmarCommand { get; }

        // 2. Constructor único (Sin 'void' para evitar el error CS0542)
        public RegistroViewModel()
        {
            RegresarCommand = new RelayCommand(async () => EjecutarRegresar());
            ConfirmarCommand = new RelayCommand(async () => await EjecutarRegistro());
        }

        // 3. Lógica para registrar en la API
        private async Task EjecutarRegistro()
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(Contrasenia))
            {
                MessageBox.Show("Por favor, completa los campos obligatorios.");
                return;
            }

            var nuevoUsuario = new RegistroRequest
            {
                nombre = Nombre,
                apellido_paterno = ApellidoP,
                apellido_materno = ApellidoM,
                usuario = Usuario,
                contrasenia = Contrasenia
            };

            var result = await _authService.RegisterAsync(nuevoUsuario);

            if (result.status)
            {
                MessageBox.Show(result.message);
                EjecutarRegresar(); // Volver al login tras éxito
            }
            else
            {
                MessageBox.Show("Error: " + result.message);
            }
        }

        // 4. Lógica para navegar atrás
        private void EjecutarRegresar()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new Uri("Views/Login.xaml", UriKind.Relative));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}