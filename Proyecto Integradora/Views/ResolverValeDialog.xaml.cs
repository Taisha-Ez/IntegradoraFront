using System.Windows;
using System.Windows.Controls;

namespace Proyecto_Integradora.Views
{
    public partial class ResolverValeDialog : Window
    {
        public string EstadoSeleccionado { get; private set; }

        public ResolverValeDialog(string valeId)
        {
            InitializeComponent();
            TxtValeId.Text = $"Vale ID: {valeId}";
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbEstado.SelectedItem is ComboBoxItem item && item.Content is string estado)
            {
                EstadoSeleccionado = estado;
                DialogResult = true;
                Close();
                return;
            }

            MessageBox.Show("Selecciona un estado valido.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
