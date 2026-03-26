using MakersMarkt.Data;
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
using Windows.System;
using Microsoft.EntityFrameworkCore;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductPage : Page
    {
        // I made this to make a filter for the product, so it is easy to search a specific product
        private string? _filter;
        public ProductPage()
        {
            InitializeComponent();
            LoadData();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string filterName && !string.IsNullOrWhiteSpace(filterName))
            {
                _filter = filterName;
            }
            else
            {
                _filter = null;
            }

            LoadData();
        }
        private void productNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = productNameSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));
            ProductListView.ItemsSource = query
                .ToList();
        }


        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                CategoryListView.ItemsSource = db.Categories.ToList();

                var productsQuery = db.Products
                                      .Include(p => p.Category)
                                      .AsQueryable();


                if (!string.IsNullOrWhiteSpace(_filter))
                {
                    productsQuery = productsQuery
                        .Where(p => p.Category != null && p.Category.Name == _filter);
                }

                ProductListView.ItemsSource = productsQuery.ToList();
            }
        }

        // clicks of listview
        private void ProductListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var product = (Data.Product)e.ClickedItem;

            var productId = product.Id;
            Frame.Navigate(typeof(Product.ProductDetailPage), productId);
        }

        private void CategoryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // this will reload the page to see only products with the specific category to wish
            var category = (Data.Category)e.ClickedItem;
            Frame.Navigate(typeof(ProductPage), category.Name);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(ProductPage));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(ProfilePage));

        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(MyProductsPage));

        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(ProductPage));
        }

        private void ArtistsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue("UserId", out object userIdObj) && 
                    int.TryParse(userIdObj?.ToString(), out int userId))
                {
                    Frame?.Navigate(typeof(Pages.Order.ArtistOrderPage), userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }
    }
}