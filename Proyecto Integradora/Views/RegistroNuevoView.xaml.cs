using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Proyecto_Integradora.ViewModels;
using System.Windows.Controls;

namespace Proyecto_Integradora.Views
{
    public partial class RegistroNuevoView : Page
    {
        public RegistroNuevoView()
        {
            InitializeComponent();
        }

        private void txtPassword_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            // Accedemos al ViewModel y actualizamos la propiedad manualmente
            if (this.DataContext is RegistroViewModel viewModel)
            {
                viewModel.Contrasenia = ((PasswordBox)sender).Password;
            }
        }
    }
}