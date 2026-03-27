using MakersMarkt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

namespace MakersMarkt.Pages.Profile
{
    public sealed partial class EditProfile : Page
    {
        private User? _user;
        private int _userId;

        public EditProfile()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // verwacht: parameter is userId (int). Pas aan indien je anders navigeert.
            _userId = e.Parameter is int id ? id : 0;
            if (_userId == 0)
            {
                // fallback of navigeren terug
                return;
            }

            await LoadUserAsync(_userId);
        }

        private async Task LoadUserAsync(int userId)
        {
            using var db = new AppDbContext();
            _user = await db.Users.FindAsync(userId);
            if (_user == null)
            {
                // gebruiker niet gevonden - handel naar wens af
                return;
            }

            // vul UI velden
            UsernameBox.Text = _user.Username ?? string.Empty;
            // wachtwoord niet tonen
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;
            var newUsername = UsernameBox.Text?.Trim();
            var newPassword = PasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newUsername))
            {
                ShowError("Gebruikersnaam mag niet leeg zijn.");
                return;
            }

            if (!string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(confirm))
            {
                if (newPassword != confirm)
                {
                    ShowError("Wachtwoorden komen niet overeen.");
                    return;
                }

                if (newPassword.Length < 6)
                {
                    ShowError("Wachtwoord moet minimaal 6 tekens zijn.");
                    return;
                }
            }

            using var db = new AppDbContext();

            // controleer of username uniek is (indien gewijzigd)
            var exists = await db.Users.AnyAsync(u => u.Username == newUsername && u.Id != _userId);
            if (exists)
            {
                ShowError("Deze gebruikersnaam is al in gebruik.");
                return;
            }

            var userInDb = await db.Users.FindAsync(_userId);
            if (userInDb == null)
            {
                ShowError("Gebruiker niet gevonden.");
                return;
            }

            userInDb.Username = newUsername;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                // OPMERKING: sla nooit een wachtwoord in cleartext op.
                // Gebruik hashing (bv BCrypt). Voorbeeld hieronder als BCrypt geïnstalleerd is:
                // userInDb.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                // Als jouw model 'Password' heet en je voorlopig plaintext wil (niet aanbevolen):
                userInDb.PasswordHash = newPassword;
            }

            await db.SaveChangesAsync();

            // Optioneel: toon bevestiging en navigeer terug naar ProfilePage of ga back
            var successDialog = new ContentDialog
            {
                Title = "Succes",
                Content = "Je profiel is bijgewerkt.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();

            // Navigeren terug: als ProfilePage verwacht userId als parameter:
            Frame?.Navigate(typeof(MakersMarkt.Pages.ProfilePage), _userId);
            // of Frame?.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame?.CanGoBack == true)
                Frame.GoBack();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}