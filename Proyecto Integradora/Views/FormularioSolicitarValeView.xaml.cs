using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Threading.Tasks;
using Proyecto_Integradora.Models;
using Proyecto_Integradora.Services;

namespace Proyecto_Integradora.Views
{
    // Cambiado de Window a Page
    public partial class FormularioSolicitarValeView : Page
    {
        private int currentStep = 1;
        private readonly CustomerService _customerService = new CustomerService();

        public FormularioSolicitarValeView()
        {
            InitializeComponent();
        }

        private async void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep < 4)
            {
                currentStep++;
                ActualizarInterfaz();
            }
            else
            {
                await EnviarSolicitudCreditoAsync();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep > 1)
            {
                currentStep--;
                ActualizarInterfaz();
            }
        }

        private void ActualizarInterfaz()
        {
            // Resetear visibilidades
            Step1.Visibility = Visibility.Collapsed;
            Step2.Visibility = Visibility.Collapsed;
            Step3.Visibility = Visibility.Collapsed;
            Step4.Visibility = Visibility.Collapsed;

            // Mostrar paso actual
            switch (currentStep)
            {
                case 1: Step1.Visibility = Visibility.Visible; break;
                case 2: Step2.Visibility = Visibility.Visible; break;
                case 3: Step3.Visibility = Visibility.Visible; break;
                case 4: Step4.Visibility = Visibility.Visible; break;
            }

            // Actualizar textos y progress bar
            lblPaso.Text = $"PASO {currentStep} DE 4";
            pgBar.Value = currentStep;
            btnNext.Content = (currentStep == 4) ? "Finalizar" : "Siguiente";
            btnBack.Visibility = (currentStep == 1) ? Visibility.Hidden : Visibility.Visible;
        }

        private async Task EnviarSolicitudCreditoAsync()
        {
            if (!decimal.TryParse(txtIngresos.Text?.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var ingresos)
                && !decimal.TryParse(txtIngresos.Text?.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out ingresos))
            {
                MessageBox.Show("Ingresa un valor valido para ingresos mensuales.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text)
                || string.IsNullOrWhiteSpace(txtRFC.Text)
                || string.IsNullOrWhiteSpace(txtDireccion.Text)
                || string.IsNullOrWhiteSpace(txtTel.Text)
                || string.IsNullOrWhiteSpace(txtRefNombre1.Text)
                || string.IsNullOrWhiteSpace(txtRefParentesco1.Text)
                || string.IsNullOrWhiteSpace(txtRefTel1.Text)
                || string.IsNullOrWhiteSpace(txtRefNombre2.Text)
                || string.IsNullOrWhiteSpace(txtRefParentesco2.Text)
                || string.IsNullOrWhiteSpace(txtRefTel2.Text))
            {
                MessageBox.Show("Completa todos los campos del formulario antes de enviar.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnNext.IsEnabled = false;

            var request = new SolicitudCreditoRequest
            {
                nombreCompleto = txtNombre.Text.Trim(),
                curpRfc = txtRFC.Text.Trim(),
                direccion = txtDireccion.Text.Trim(),
                telefono = txtTel.Text.Trim(),
                ingresosMensuales = ingresos,
                referencias = new System.Collections.Generic.List<ReferenciaCreditoRequest>
                {
                    new ReferenciaCreditoRequest
                    {
                        parentesco = txtRefParentesco1.Text.Trim(),
                        nombre = txtRefNombre1.Text.Trim(),
                        numeroContacto = txtRefTel1.Text.Trim()
                    },
                    new ReferenciaCreditoRequest
                    {
                        parentesco = txtRefParentesco2.Text.Trim(),
                        nombre = txtRefNombre2.Text.Trim(),
                        numeroContacto = txtRefTel2.Text.Trim()
                    }
                }
            };

            var response = await _customerService.SolicitarCreditoAsync(request);
            MessageBox.Show(response.message, response.status ? "Exito" : "Error",
                MessageBoxButton.OK,
                response.status ? MessageBoxImage.Information : MessageBoxImage.Error);

            btnNext.IsEnabled = true;
        }
    }
}