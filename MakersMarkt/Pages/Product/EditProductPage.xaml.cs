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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditProductPage : Page
    {
        private int _productId;
        private string? _selectedImagePath;

        public EditProductPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.Parameter is int productId)
            {
                _productId = productId;
                LoadProductData();
            }
            else
            {
                ErrorText.Text = "Invalid product ID.";
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void LoadProductData()
        {
            using var context = new AppDbContext();
            
            // Load Categories
            var categories = context.Categories.ToList();
            CategoryCombo.ItemsSource = categories;

            var product = context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == _productId);

            if (product != null)
            {
                NameInput.Text = product.Name ?? "";
                ImageUrlInput.Text = product.ImageUrl ?? "";
                if (ImageStatusText != null && !string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    ImageStatusText.Text = $"Current: {Path.GetFileName(product.ImageUrl)}";
                }
                PriceInput.Text = product.Price.ToString("0.00");
                TypeInput.Text = product.Type ?? "";
                DescriptionInput.Text = product.Description ?? "";
                MaterialInput.Text = product.Material ?? "";
                ProductionTimeInput.Text = product.ProductionTime.ToString();
                ComplexityInput.Text = product.Complexity ?? "";
                SustainabilityInput.Text = product.Sustainability ?? "";
                UniqueFeaturesInput.Text = product.UniqueFeatures ?? "";

                if (categories.Any(c => c.Id == product.CategoryId))
                {
                    CategoryCombo.SelectedValue = product.CategoryId;
                }
            }
            else
            {
                ErrorText.Text = "Product not found.";
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private async void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".webp");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                _selectedImagePath = file.Path;
                ImageUrlInput.Text = file.Name;
                if (ImageStatusText != null)
                {
                    ImageStatusText.Text = $"Selected: {file.Name}";
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(NameInput.Text) || CategoryCombo.SelectedValue == null)
            {
                ErrorText.Text = "Please fill in all required fields (Name and Category).";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (!decimal.TryParse(PriceInput.Text, out decimal price)) price = 0;
            if (!int.TryParse(ProductionTimeInput.Text, out int prodTime)) prodTime = 0;

            using var context = new AppDbContext();
            var product = context.Products.Find(_productId);

            if (product != null)
            {
                product.Name = NameInput.Text;
                product.CategoryId = (int)CategoryCombo.SelectedValue;
                // Use selected image path if available, otherwise keep existing or use input
                product.ImageUrl = _selectedImagePath ?? ImageUrlInput.Text ?? product.ImageUrl;
                product.Price = price;
                product.Type = TypeInput.Text ?? "";
                product.Description = DescriptionInput.Text ?? "";
                product.Material = MaterialInput.Text ?? "";
                product.ProductionTime = prodTime;
                product.Complexity = ComplexityInput.Text ?? "";
                product.Sustainability = SustainabilityInput.Text ?? "";
                product.UniqueFeatures = UniqueFeaturesInput.Text ?? "";

                context.SaveChanges();
                NavigateBack();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        private void NavigateBack()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(ProductPage));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue("UserId", out object userIdObj) && 
                    int.TryParse(userIdObj?.ToString(), out int userId))
                {
                    Frame?.Navigate(typeof(Pages.ProfilePage), userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue("UserId", out object userIdObj) && 
                    int.TryParse(userIdObj?.ToString(), out int userId))
                {
                    Frame?.Navigate(typeof(Pages.MyProductsPage), userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
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
