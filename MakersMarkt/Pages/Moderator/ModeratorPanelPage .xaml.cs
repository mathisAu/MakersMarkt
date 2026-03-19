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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Moderator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModeratorPanelPage : Page
    {
        public ModeratorPanelPage()
        {
            InitializeComponent();
            AdminContentFrame.Navigate(typeof(ModeratorStatsPage));
        }

        private void AdminNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Controle voor NavigationItem
            if (args.SelectedItem is NavigationViewItem item)
            {
                // Kijk naar de Tag van het aangeklikte item om te bepalen waar we naartoe navigeren
                switch (item.Tag)
                {
                    case "Dashboard":
                        // Navigeer naar de ModeratorStatsPage 
                        AdminContentFrame.Navigate(typeof(ModeratorStatsPage));
                        break;

                    case "Users":
                        // Navigeer naar de ModeratorUsersPage
                        // Hier geef je ook AdminContentFrame mee als parameter (optioneel, afhankelijk van gebruik)
                        AdminContentFrame.Navigate(typeof(ModeratorUsersPage), AdminContentFrame);
                        break;

                    case "Products":
                        // Navigeer naar de ModeratorProductsPage
                        AdminContentFrame.Navigate(typeof(ModeratorProductsPage));
                        break;
                }
            }
        }
    }
}
