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
using WinRT.Interop;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddProductPage : Page
    {
        private string? _selectedImagePath;

        public AddProductPage()
        {
            InitializeComponent();
            Loaded += AddProductPage_Loaded;
        }

        private void AddProductPage_Loaded(object sender, RoutedEventArgs e)
        {
            using var context = new AppDbContext();
            
            var categories = context.Categories.ToList();
            CategoryCombo.ItemsSource = categories;
            if (categories.Any())
            {
                CategoryCombo.SelectedIndex = 0;
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

            // Get the window handle from the current window
            var window = (Application.Current as App)?.GetMainWindow();
            if (window != null)
            {
                var hwnd = WindowNative.GetWindowHandle(window);
                InitializeWithWindow.Initialize(picker, hwnd);
            }

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

            var user = LoggedInUser.CurrentUser;
            if (user == null)
            {
                ErrorText.Text = "Not logged in.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(NameInput.Text) || CategoryCombo.SelectedValue == null)
            {
                ErrorText.Text = "Please fill in all required fields (Name and Category).";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (!decimal.TryParse(PriceInput.Text, out decimal price)) price = 0;
            if (!int.TryParse(ProductionTimeInput.Text, out int prodTime)) prodTime = 0;

            // Use selected image path, or fall back to URL input, or use placeholder
            string imageUrl = _selectedImagePath ?? ImageUrlInput.Text ?? "ms-appx:///Assets/placeholder.png";

            var newProduct = new MakersMarkt.Data.Product
            {
                Name = NameInput.Text,
                CategoryId = (int)CategoryCombo.SelectedValue,
                ImageUrl = imageUrl,
                Price = price,
                Type = TypeInput.Text ?? "",
                Description = DescriptionInput.Text ?? "",
                Material = MaterialInput.Text ?? "",
                ProductionTime = prodTime,
                Complexity = ComplexityInput.Text ?? "",
                Sustainability = SustainabilityInput.Text ?? "",
                UniqueFeatures = UniqueFeaturesInput.Text ?? "",
                MakerId = user.Id
            };

            using var context = new AppDbContext();
            context.Products.Add(newProduct);
            context.SaveChanges();

            NavigateBack();
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
