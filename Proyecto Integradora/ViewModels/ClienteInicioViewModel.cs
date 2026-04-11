using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Input;

namespace Proyecto_Integradora.ViewModels
{
    public class ClienteInicioViewModel
    {
        public ICommand RegresarCommand { get; }

        public ClienteInicioViewModel()
        {
            // Inicializamos el comando para volver al login
            RegresarCommand = new RelayCommand(async () => EjecutarRegresar());
        }

        private void EjecutarRegresar()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Redirige a la vista de Login
                mainWindow.MainFrame.Navigate(new Uri("Views/Login.xaml", UriKind.Relative));
            }
        }
    }
}
