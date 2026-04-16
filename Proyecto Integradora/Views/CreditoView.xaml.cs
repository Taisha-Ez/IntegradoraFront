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

namespace Proyecto_Integradora.Views
{
    /// <summary>
    /// Lógica de interacción para CreditoView.xaml
    /// </summary>
    public partial class CreditoView : Page
    {
        public CreditoView()
        {
            InitializeComponent();
            this.DataContext = new CreditoViewModel();
        }

        private void SolicitarCredito_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SolicitarCreditoView());
        }
    }
}
