using MakersMarkt.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Linq;
using MakersMarkt.Pages.Product;
using MakersMarkt.Pages.Moderator;
using Windows.Storage;

namespace MakersMarkt.Pages.Login
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            ErrorMessage.Visibility = Visibility.Collapsed;

            string enteredUsername = usernameTextBox.Text.Trim();
            string enteredPassword = passwordBox.Password;

            if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
            {
                ShowError("Vul zowel gebruikersnaam als wachtwoord in.");
                return;
            }

            LoginButton.IsEnabled = false;

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u =>
                    u.Username.ToLower() == enteredUsername.ToLower());

                if (user == null || !BCrypt.Net.BCrypt.Verify(enteredPassword, user.PasswordHash))
                {
                    ShowError("Onjuiste gebruikersnaam of wachtwoord.");
                    LoginButton.IsEnabled = true;
                    return;
                }

                LoggedInUser.CurrentUser = user;
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["UserId"] = user.Id;

                if (user.Role == "moderator")
                    Frame.Navigate(typeof(ModeratorPanelPage));
                else
                    Frame.Navigate(typeof(Product.OverviewPage));
            }

            LoginButton.IsEnabled = true;
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}