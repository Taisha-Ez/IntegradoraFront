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
using System.Xml.Linq;
using Proyecto_Integradora.Views;

namespace Proyecto_Integradora.Views
{
    public partial class ClienteInicioView : Page
{
    public ClienteInicioView()
    {
        InitializeComponent();
    }

    private void NavegarSolicitar_Click(object sender, RoutedEventArgs e)
    {
        // Ocultamos el panel de bienvenida
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a la vista correspondiente
        ContentFrame.Navigate(new SolicitarPagoView());
    }

    private void NavegarPagar_Click(object sender, RoutedEventArgs e)
    {
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a la vista correspondiente
        ContentFrame.Navigate(new PagarView());
    }
}
}
