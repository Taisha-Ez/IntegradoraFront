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
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private bool _isSyncingPassword;
        private bool _isPasswordVisible;

        public Login()
        {
            InitializeComponent();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncingPassword)
            {
                return;
            }

            _isSyncingPassword = true;
            txtPasswordVisible.Text = txtPassword.Password;

            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.Contrasenia = txtPassword.Password;
            }

            _isSyncingPassword = false;
        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingPassword || !_isPasswordVisible)
            {
                return;
            }

            _isSyncingPassword = true;
            txtPassword.Password = txtPasswordVisible.Text;

            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.Contrasenia = txtPasswordVisible.Text;
            }

            _isSyncingPassword = false;
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Visibility = Visibility.Visible;
                btnTogglePassword.Content = "Ocultar";
                txtPasswordVisible.Focus();
                txtPasswordVisible.CaretIndex = txtPasswordVisible.Text.Length;
                return;
            }

            txtPassword.Password = txtPasswordVisible.Text;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
            btnTogglePassword.Content = "Mostrar";
            txtPassword.Focus();
        }
    }
}
