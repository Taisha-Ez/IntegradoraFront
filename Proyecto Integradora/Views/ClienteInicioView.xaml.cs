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
using System.Threading.Tasks;
using Proyecto_Integradora.Views;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.Views
{
    public partial class ClienteInicioView : Page
{
    private readonly CustomerService _customerService = new CustomerService();

    public ClienteInicioView()
    {
        InitializeComponent();
        Loaded += ClienteInicioView_Loaded;
    }

    private async void ClienteInicioView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ClienteInicioView_Loaded;
        await CargarVistaInicialClienteAsync();
    }

    private async Task CargarVistaInicialClienteAsync()
    {
        WelcomePanel.Visibility = Visibility.Collapsed;

        var saldoCredito = await _customerService.ConsultarSaldoCreditoAsync();
        var tieneCreditoRegistrado = saldoCredito.data != null
            && (
                saldoCredito.data.creditRequestId > 0
                || !string.IsNullOrWhiteSpace(saldoCredito.data.status)
            );

        if (tieneCreditoRegistrado)
        {
            ContentFrame.Navigate(new SolicitarPagoView());
            return;
        }

        ContentFrame.Navigate(new FormularioSolicitarValeView());
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
