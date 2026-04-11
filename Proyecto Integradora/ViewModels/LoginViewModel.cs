using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows;
using Proyecto_Integradora.Services;
using Proyecto_Integradora.Core;
using System;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace Proyecto_Integradora.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService = new AuthService();

        public string Usuario { get; set; }
        public string Contrasenia { get; set; } // Nota: En producción, PasswordBox requiere manejo especial por seguridad

        public ICommand LoginCommand { get; }

        public ICommand IrARegistroCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async () => await EjecutarLogin());

            IrARegistroCommand = new RelayCommand(async () => EjecutarIrARegistro());
        }

        private void EjecutarIrARegistro()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new Uri("Views/RegistroNuevoView.xaml", UriKind.Relative));
            }
        }

        private async Task EjecutarLogin()
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(Contrasenia))
            {
                MessageBox.Show("Por favor llena todos los campos.");
                return;
            }

            var result = await _authService.LoginAsync(Usuario, Contrasenia);

            if (result.status && !string.IsNullOrEmpty(result.data))
            {
                string rol = ObtenerRolDesdeToken(result.data);
                // Guardamos en la memoria global
                UserSession.Token = result.data;
                UserSession.Role = rol;
                UserSession.Usuario = Usuario;
                // ... lógica de navegación de antes
                var mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null)
                {
                    if (rol == "admin")
                    {
                        mainWindow.MainFrame.Navigate(new Uri("Views/AdminInicioView.xaml", UriKind.Relative));
                    }
                    else if (rol == "cliente")
                    {
                        mainWindow.MainFrame.Navigate(new Uri("Views/FormularioSolicitarValeView.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Rol no reconocido: " + rol);
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: " + result.message);
            }
        }

        // Método para extraer el rol del JWT sin librerías externas
        private string ObtenerRolDesdeToken(string token)
        {
            try
            {
                // El JWT tiene formato: Header.Payload.Signature
                var partes = token.Split('.');
                if (partes.Length < 2) return string.Empty;

                string payloadBase64 = partes[1];

                // Ajustar el padding de Base64 si es necesario
                payloadBase64 = payloadBase64.PadRight(payloadBase64.Length + (4 - payloadBase64.Length % 4) % 4, '=')
                                             .Replace('-', '+')
                                             .Replace('_', '/');

                byte[] datosBytes = Convert.FromBase64String(payloadBase64);
                string jsonPayload = Encoding.UTF8.GetString(datosBytes);

                // Usamos JsonDocument para leer el campo "role"
                using (JsonDocument doc = JsonDocument.Parse(jsonPayload))
                {
                    if (doc.RootElement.TryGetProperty("role", out JsonElement rolElement))
                    {
                        return rolElement.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error decodificando token: " + ex.Message);
            }
            return string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    // Clase auxiliar simple para los comandos
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        public RelayCommand(Func<Task> execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public async void Execute(object parameter) => await _execute();
        public event EventHandler CanExecuteChanged;
    }
}