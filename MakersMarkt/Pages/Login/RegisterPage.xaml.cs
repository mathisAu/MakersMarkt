using MakersMarkt.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;

namespace MakersMarkt.Pages.Login
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AttemptRegister();
        }

        private void AttemptRegister()
        {
            ErrorMessage.Visibility = Visibility.Collapsed;

            string username = usernameTextBox.Text.Trim();
            string password = passwordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Vul alle velden in.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Wachtwoorden komen niet overeen.");
                return;
            }

            RegisterButton.IsEnabled = false;

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username.ToLower() == username.ToLower()))
                {
                    ShowError("Gebruikersnaam is al in gebruik.");
                    RegisterButton.IsEnabled = true;
                    return;
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var newUser = new User
                {
                    Username = username,
                    PasswordHash = hashedPassword,
                    Role = "koper", // Always with making a new account, the role is set to "koper"
                    CreatedAt = DateTime.Now
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            Frame.Navigate(typeof(LoginPage));
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }
    }
}