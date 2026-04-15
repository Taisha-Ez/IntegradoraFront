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

        // Verificamos si el usuario tiene crédito registrado
        var tieneCredito = await _customerService.TieneCreditoRegistradoAsync();
        
        System.Diagnostics.Debug.WriteLine($"[ClienteInicioView] tieneCredito result: {tieneCredito}");

        // Si status=false y data=null → mostrar formulario para SOLICITAR crédito
        if (tieneCredito == true)
        {
            System.Diagnostics.Debug.WriteLine($"[ClienteInicioView] Navegando a FormularioSolicitarValeView (sin crédito)");
            ContentFrame.Navigate(new FormularioSolicitarValeView());
            return;
        }

        // Si tiene crédito → mostrar vista de CRÉDITO (es la vista inicial)
        System.Diagnostics.Debug.WriteLine($"[ClienteInicioView] Navegando a CreditoView (tiene crédito)");
        ContentFrame.Navigate(new CreditoView());
    }

    private void NavegarSolicitar_Click(object sender, RoutedEventArgs e)
    {
        // Ocultamos el panel de bienvenida
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a solicitar crédito
        ContentFrame.Navigate(new FormularioSolicitarValeView());
    }

    private void NavegarPagar_Click(object sender, RoutedEventArgs e)
    {
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a pagar vales
        ContentFrame.Navigate(new SolicitarPagoView());
    }

    private void NavegarCredito_Click(object sender, RoutedEventArgs e)
    {
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a ver saldo de crédito
        ContentFrame.Navigate(new CreditoView());
    }
}
}
