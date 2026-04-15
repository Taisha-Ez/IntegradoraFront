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

        // Si tieneCredito == true → status=false y data=null (SIN crédito, mostrar formulario de CRÉDITO)
        if (tieneCredito == true)
        {
            System.Diagnostics.Debug.WriteLine($"[ClienteInicioView] Navegando a SolicitarCreditoView (sin crédito autorizado)");
            ContentFrame.Navigate(new SolicitarCreditoView());
            return;
        }

        // Si tieneCredito == false o null → tiene crédito o error, mostrar vista de crédito
        System.Diagnostics.Debug.WriteLine($"[ClienteInicioView] Navegando a CreditoView (tiene crédito o verificación)");
        ContentFrame.Navigate(new CreditoView());
    }

    private void NavegarSolicitar_Click(object sender, RoutedEventArgs e)
    {
        // Ocultamos el panel de bienvenida
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a solicitar un VALE
        ContentFrame.Navigate(new SolicitarPagoView());
    }

    private void NavegarPagar_Click(object sender, RoutedEventArgs e)
    {
        WelcomePanel.Visibility = Visibility.Collapsed;
        // Navegamos a pagar vales
        ContentFrame.Navigate(new PagarView());
    }

    private async void NavegarCredito_Click(object sender, RoutedEventArgs e)
    {
        WelcomePanel.Visibility = Visibility.Collapsed;

        var tieneCredito = await _customerService.TieneCreditoRegistradoAsync();

        if (tieneCredito == true)
        {
            // No tiene crédito autorizado activo, por eso mostramos el formulario de solicitud
            ContentFrame.Navigate(new SolicitarCreditoView());
            return;
        }

        // Si sí tiene crédito, mostramos su saldo
        ContentFrame.Navigate(new CreditoView());
    }
}
}
