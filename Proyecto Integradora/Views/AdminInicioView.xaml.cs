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
using Proyecto_Integradora.Views;

namespace Proyecto_Integradora.Views
{
    public partial class AdminInicioView : Page
    {
        public AdminInicioView()
        {
            InitializeComponent();
        }

        private void NavegarClientes_Click(object sender, RoutedEventArgs e)
        {
            DefaultContent.Visibility = Visibility.Collapsed; // Oculta el logo central
            AdminContentFrame.Navigate(new ClientePantallaPrinicipalView());
        }

        private void NavegarVales_Click(object sender, RoutedEventArgs e)
        {
            DefaultContent.Visibility = Visibility.Collapsed;
            AdminContentFrame.Navigate(new ValesExpedidosView());
        }

        private void NavegarIngresos_Click(object sender, RoutedEventArgs e)
        {
            DefaultContent.Visibility = Visibility.Collapsed;
            // Asegúrate de que el nombre de la clase sea correcto
            AdminContentFrame.Navigate(new IngresosEgresosView());
        }

        private void NavegarRevision_Click(object sender, RoutedEventArgs e)
        {
            DefaultContent.Visibility = Visibility.Collapsed;
            // Asegúrate de que el nombre de la clase sea correcto
            AdminContentFrame.Navigate(new RevisiónView());
        }
    }
}
