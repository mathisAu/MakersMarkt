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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddProductPage : Page
    {
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
            
            ImageUrlInput.Text = "ms-appx:///Assets/placeholder.png";
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

            var newProduct = new MakersMarkt.Data.Product
            {
                Name = NameInput.Text,
                CategoryId = (int)CategoryCombo.SelectedValue,
                ImageUrl = ImageUrlInput.Text,
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
    }
}
