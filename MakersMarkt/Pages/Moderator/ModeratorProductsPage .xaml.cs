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
    public sealed partial class ModeratorProductsPage : Page
    {
        private int? _userId = null;

        public ModeratorProductsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int userId)
            {
                _userId = userId;
            }

            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                if (_userId.HasValue)
                {
                    // Haal alleen producten op van deze specifieke gebruiker
                    // (_userId.Value = de echte waarde van de userId)
                    ProductsListView.ItemsSource = db.Products.Where(p => p.MakerId == _userId.Value).ToList();
                }
                else
                {
                    // Als er geen userId is, haal ALLE producten op
                    ProductsListView.ItemsSource = db.Products.ToList();
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            //Als de knop is ingedrukt én die knop een geldig productId bevat, ga dan verder”
            if (sender is Button button && button.Tag is int productId)
            {
                using (var db = new AppDbContext())
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == productId);
                    if (product != null)
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                        LoadProducts();
                    }
                }
            }
        }
    }
}
