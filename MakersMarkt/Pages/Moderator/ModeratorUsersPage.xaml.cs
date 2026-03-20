using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MakersMarkt.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Moderator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModeratorUsersPage : Page
    {
        private Frame _adminContentFrame;

        public ModeratorUsersPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Frame frame)
            {
                _adminContentFrame = frame;
            }

            LoadUsers();
        }

        private void LoadUsers()
        {
            //naar de db
            using (var db = new AppDbContext())
            {
                //Laad en maak een lijst van users
                UsersListView.ItemsSource = db.Users
                    .Where(u => !u.IsModerator && u.Role == "gebruiker")  // alleen normale gebruikers
                    .OrderBy(u => u.Username) // optioneel sorteren
                    .ToList();
            }
        }

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {  //Als bijvoorbeeld "gebruiker1" aan word geklikt, word je doorgestuurd naar de pagina van "gebruiker1"
           //waar je zijn producten kan zien en eventueel verwijderen

            if (e.ClickedItem is User user)
            {
                _adminContentFrame?.Navigate(typeof(ModeratorProductsPage), user.Id);
            }
        }

        // Hulp: pak de User bij de aangeklikte knop
        private User GetUserFromSender(object sender)
        {
            var button = sender as Button;
            return button?.DataContext as User;
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
           var user = GetUserFromSender(sender);
if (user == null) return;

// Moderators zelf mogen niet geverifieerd worden
if (user.IsModerator)
{
    ShowMessage("Je kunt moderators niet verifiëren.");
    return;
}

// Alleen moderators mogen verifiëren -> controleer huidige gebruiker
if (!CurrentUserIsModerator())
{
    ShowMessage("Je hebt geen rechten om gebruikers te verifiëren.");
    return;
}



            using (var db = new AppDbContext())
            {
                var u = db.Users.FirstOrDefault(x => x.Id == user.Id);
                if (u == null) return;

                u.IsVerified = true;
                u.VerifiedAt = DateTime.UtcNow;
                // Stel in op Id van ingelogde moderator
                u.VerifiedById = GetCurrentUserId();

                db.SaveChanges();
            }

            if (sender is Button btn)
            {
                btn.Content = "Geverifieerd";
                btn.IsEnabled = false; // optioneel: knop uitschakelen
            }
            // In-memory object bijwerken zodat de UI ververst
            user.IsVerified = true;
            ShowMessage($"Gebruiker '{user.Username}' is geverifieerd.");
        }

        private bool CurrentUserIsModerator()
        {
            // TODO: vervang dit door echte controle op ingelogde gebruiker
            // Voor nu: altijd waar als je deze pagina alleen voor moderators toont
            return true;
        }

        private int GetCurrentUserId()
        {
            // TODO: haal Id van ingelogde moderator op
            // Voor nu: dummy waarde 1
            return 1;
        }
        private async void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Melding",
                Content = message,
                CloseButtonText = "OK",
                //Koppel de dialog aan deze pagina
                XamlRoot = this.Content.XamlRoot   // belangrijk voor WinUI 3

            };
            await dialog.ShowAsync();

        }
    }
}
