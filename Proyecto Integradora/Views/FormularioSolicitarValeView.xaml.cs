using System.Windows;
using System.Windows.Controls;

namespace Proyecto_Integradora.Views
{
    // Cambiado de Window a Page
    public partial class FormularioSolicitarValeView : Page
    {
        private int currentStep = 1;

        public FormularioSolicitarValeView()
        {
            InitializeComponent();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep < 4)
            {
                currentStep++;
                ActualizarInterfaz();
            }
            else
            {
                MessageBox.Show("¡Solicitud enviada con éxito!");
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
    }
}