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
    /// Lógica de interacción para RevisiónView.xaml
    /// </summary>
    public partial class RevisiónView : Page
    {
        public RevisiónView()
        {
            InitializeComponent();
        }

        private async void ResolverVale_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not RevisionViewModel vm)
            {
                return;
            }

            if (vm.ValeSeleccionado == null)
            {
                MessageBox.Show("Selecciona un vale en la tabla.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ResolverValeDialog(vm.ValeSeleccionado.id)
            {
                Owner = Application.Current.MainWindow
            };

            var confirmed = dialog.ShowDialog();
            if (confirmed != true)
            {
                return;
            }

            var response = await vm.ResolverValeSeleccionadoAsync(dialog.EstadoSeleccionado);

            MessageBox.Show(response.message,
                response.status ? "Exito" : "Error",
                MessageBoxButton.OK,
                response.status ? MessageBoxImage.Information : MessageBoxImage.Error);
        }
    }
}
