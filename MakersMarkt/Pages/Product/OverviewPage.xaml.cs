using MakersMarkt.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OverviewPage : Page
    {
        public OverviewPage()
        {
            InitializeComponent();
            LoadDate();
        }
        public void LoadDate()
        {
            var db = new AppDbContext();
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("UserId", out object userIdObj) && userIdObj is int userId)
            {
                ProductListView.ItemsSource = db.Products.Where(p => p.MakerId == userId).ToList();
            }
        }

        private async void Button_Click_Edit(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = null;

            var panel = new StackPanel { Spacing = 8 };

            // Velden
            var nameBox = new TextBox { Header = "Naam" };
            var descriptionBox = new TextBox { Header = "Beschrijving", AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
            var typeBox = new TextBox { Header = "Type" };
            var materialBox = new TextBox { Header = "Materiaal" };
            var productionTimeBox = new TextBox { Header = "Productietijd (dagen)", InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };
            var complexityBox = new TextBox { Header = "Complexiteit" };
            var sustainabilityBox = new TextBox { Header = "Duurzaamheid" };
            var uniqueFeaturesBox = new TextBox { Header = "Unieke kenmerken" };
            var priceBox = new TextBox { Header = "Prijs", PlaceholderText = "bijv. 59.99" };
            var imageBox = new TextBox { Header = "Afbeelding (bestand in Assets)", PlaceholderText = "bijv. ring.png" };
            var makerIdBox = new TextBox { Header = "Maker Id", InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };
            var categoryIdBox = new TextBox { Header = "Categorie Id", InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };

            panel.Children.Add(nameBox);
            panel.Children.Add(descriptionBox);
            panel.Children.Add(typeBox);
            panel.Children.Add(materialBox);
            panel.Children.Add(productionTimeBox);
            panel.Children.Add(complexityBox);
            panel.Children.Add(sustainabilityBox);
            panel.Children.Add(uniqueFeaturesBox);
            panel.Children.Add(priceBox);
            panel.Children.Add(imageBox);
            panel.Children.Add(makerIdBox);
            panel.Children.Add(categoryIdBox);

            dialog = new ContentDialog
            {
                Title = "Nieuw product",
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = new ScrollViewer { Content = panel }
            };


            dialog.PrimaryButtonClick += async (s, e) =>
            {
                // Validatie/parsing
                if (!int.TryParse(productionTimeBox.Text, out int productionTime))
                {
                    e.Cancel = true;
                    await ShowErrorAsync("Productietijd moet een geheel getal zijn.", dialog);
                    return;
                }

                if (!decimal.TryParse(priceBox.Text, out decimal price))
                {
                    e.Cancel = true;
                    await ShowErrorAsync("Prijs moet een geldig getal zijn (bijv. 59.99).", dialog);
                    return;
                }

                if (!int.TryParse(makerIdBox.Text, out int makerId))
                {
                    e.Cancel = true;
                    await ShowErrorAsync("Maker Id moet een geheel getal zijn.", dialog);
                    return;
                }

                if (!int.TryParse(categoryIdBox.Text, out int categoryId))
                {
                    e.Cancel = true;
                    await ShowErrorAsync("Categorie Id moet een geheel getal zijn.", dialog);
                    return;
                }

                var product = new Data.Product
                {
                    Name = nameBox.Text,
                    Description = descriptionBox.Text,
                    Type = typeBox.Text,
                    Material = materialBox.Text,
                    ProductionTime = productionTime,
                    Complexity = complexityBox.Text,
                    Sustainability = sustainabilityBox.Text,
                    UniqueFeatures = uniqueFeaturesBox.Text,
                    Price = price,
                    Image = imageBox.Text,
                    MakerId = makerId,
                    CategoryId = categoryId
                };

                try
                {
                    using var db = new AppDbContext();
                    db.Products.Add(product);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    e.Cancel = true; // dialoog open laten
                    await ShowErrorAsync("Opslaan mislukt: " + ex.Message, dialog);
                }
            };

            await dialog.ShowAsync();
        }

        private async Task ShowErrorAsync(string message, ContentDialog owner)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Fout",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = owner.XamlRoot
            };

            await errorDialog.ShowAsync();
        }

        private async System.Threading.Tasks.Task ShowErrorAsync(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Fout",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
        private async void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            var db = new AppDbContext();
            var button = (Button)sender;
            var selectedProduct = (Data.Product)button.DataContext;
            db.Remove(db.Products.FirstOrDefault(p => p.Id == selectedProduct.Id));
            db.SaveChanges();
        }
    }
}
