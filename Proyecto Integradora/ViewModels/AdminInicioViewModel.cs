using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Proyecto_Integradora.ViewModels
{
    public class AdminInicioViewModel
    {
        public ICommand RegresarCommand { get; }

        public AdminInicioViewModel()
        {
            RegresarCommand = new RelayCommand(async () => EjecutarRegresar());
        }

        private void EjecutarRegresar()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Navegamos de vuelta a la vista de Login
                mainWindow.MainFrame.Navigate(new Uri("Views/Login.xaml", UriKind.Relative));
            }
        }
    }
}