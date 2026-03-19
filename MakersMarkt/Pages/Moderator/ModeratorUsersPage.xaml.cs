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
                UsersListView.ItemsSource = db.Users.ToList();
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
    }
}
