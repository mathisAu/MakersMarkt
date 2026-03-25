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
        private async void Button_Click_Create(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = null!;

            var panel = new StackPanel { Spacing = 8 };

            // Input velden voor nieuw product
            var nameBox = new TextBox { Header = "Naam", PlaceholderText = "Voer productnaam in" };
            var descriptionBox = new TextBox
            {
                Header = "Beschrijving",
                PlaceholderText = "Voer beschrijving in",
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Height = 80
            };
            var typeBox = new TextBox { Header = "Type", PlaceholderText = "Voer type in" };
            var materialBox = new TextBox { Header = "Materiaal", PlaceholderText = "Voer materiaal in" };
            var productionTimeBox = new TextBox { Header = "Productietijd (dagen)", PlaceholderText = "Bijv. 14" };
            var complexityBox = new TextBox { Header = "Complexiteit", PlaceholderText = "Voer complexiteit in" };
            var sustainabilityBox = new TextBox { Header = "Duurzaamheid", PlaceholderText = "Voer duurzaamheid in" };
            var uniqueFeaturesBox = new TextBox { Header = "Unieke kenmerken", PlaceholderText = "Voer unieke kenmerken in" };
            var priceBox = new TextBox { Header = "Prijs", PlaceholderText = "Bijv. 99.99" };
            var imageBox = new TextBox { Header = "Afbeelding", PlaceholderText = "URL of bestandsnaam" };

            // Categories ophalen voor de ComboBox
            using var db = new AppDbContext();
            var categories = db.Categories.ToList();

            if (!categories.Any())
            {
                // Geen categorieën gevonden, toon waarschuwing
                var warningDialog = new ContentDialog
                {
                    Title = "Geen categorieën",
                    Content = "Er zijn nog geen categorieën aangemaakt. Maak eerst een categorie aan voordat je een product toevoegt.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await warningDialog.ShowAsync();
                return;
            }

            // ComboBox voor categorieën
            var categoryComboBox = new ComboBox
            {
                Header = "Categorie",
                ItemsSource = categories,
                DisplayMemberPath = "Name",     // Toont de naam in de lijst
                SelectedValuePath = "Id",       // Gebruikt Id als waarde
                PlaceholderText = "Selecteer een categorie",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 4)
            };

            // Optioneel: selecteer de eerste categorie als default
            if (categories.Any())
            {
                categoryComboBox.SelectedIndex = 0;
            }

            // Voeg alle velden toe aan het panel
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
            panel.Children.Add(categoryComboBox);

            dialog = new ContentDialog
            {
                Title = "Nieuw product toevoegen",
                PrimaryButtonText = "Toevoegen",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Primary,
                Content = new ScrollViewer
                {
                    Content = panel,
                    MaxHeight = 500,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                },
                XamlRoot = this.XamlRoot
            };

            dialog.PrimaryButtonClick += async (s, args) =>
            {
                // Validaties
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Naam is verplicht.", dialog);
                    return;
                }

                if (!int.TryParse(productionTimeBox.Text, out int productionTime))
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Productietijd moet een geheel getal zijn.", dialog);
                    return;
                }

                if (!decimal.TryParse(priceBox.Text, out decimal price))
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Prijs moet een geldig getal zijn.", dialog);
                    return;
                }

                // Controleer of er een categorie is geselecteerd
                var selectedCategoryId = categoryComboBox.SelectedValue;
                if (selectedCategoryId == null)
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Selecteer een geldige categorie.", dialog);
                    return;
                }

                int categoryId = (int)selectedCategoryId;

                try
                {
                    using var createDb = new AppDbContext();

                    // Maak nieuw product aan
                    var newProduct = new Data.Product
                    {
                        Name = nameBox.Text.Trim(),
                        Description = descriptionBox.Text.Trim(),
                        Type = typeBox.Text.Trim(),
                        Material = materialBox.Text.Trim(),
                        ProductionTime = productionTime,
                        Complexity = complexityBox.Text.Trim(),
                        Sustainability = sustainabilityBox.Text.Trim(),
                        UniqueFeatures = uniqueFeaturesBox.Text.Trim(),
                        Price = price,
                        Image = imageBox.Text.Trim(),
                        CategoryId = categoryId
                    };

                    createDb.Products.Add(newProduct);
                    await createDb.SaveChangesAsync();
                    Frame.Navigate(typeof(Product.OverviewPage));
                }
                catch (Exception ex)
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Product toevoegen mislukt: " + ex.Message, dialog);
                }
            };

            await dialog.ShowAsync();
        }
        private async void Button_Click_Edit(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedProduct = (Data.Product)button.DataContext;
            var db = new AppDbContext();

            ContentDialog dialog = null!;

            var panel = new StackPanel { Spacing = 8 };

            var nameBox = new TextBox { Header = "Naam", Text = selectedProduct.Name };
            var descriptionBox = new TextBox { Header = "Beschrijving", Text = selectedProduct.Description, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
            var typeBox = new TextBox { Header = "Type", Text = selectedProduct.Type };
            var materialBox = new TextBox { Header = "Materiaal", Text = selectedProduct.Material };
            var productionTimeBox = new TextBox { Header = "Productietijd (dagen)", Text = selectedProduct.ProductionTime.ToString() };
            var complexityBox = new TextBox { Header = "Complexiteit", Text = selectedProduct.Complexity };
            var sustainabilityBox = new TextBox { Header = "Duurzaamheid", Text = selectedProduct.Sustainability };
            var uniqueFeaturesBox = new TextBox { Header = "Unieke kenmerken", Text = selectedProduct.UniqueFeatures };
            var priceBox = new TextBox { Header = "Prijs", Text = selectedProduct.Price.ToString() };
            var imageBox = new TextBox { Header = "Afbeelding", Text = selectedProduct.Image };

            // Categories ophalen
            var categories = db.Categories.ToList();

            // ComboBox voor categorieën
            var categoryComboBox = new ComboBox
            {
                Header = "Categorie",
                ItemsSource = categories,
                DisplayMemberPath = "Name",     // Toont de naam in de lijst
                SelectedValuePath = "Id",       // Gebruikt Id als waarde
                SelectedValue = selectedProduct.CategoryId,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 4)
            };

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
            panel.Children.Add(categoryComboBox);  // Voeg ComboBox toe in plaats van TextBox

            dialog = new ContentDialog
            {
                Title = "Product bewerken",
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = new ScrollViewer { Content = panel },
                XamlRoot = this.XamlRoot
            };

            dialog.PrimaryButtonClick += async (s, args) =>
            {
                // Validaties
                if (!int.TryParse(productionTimeBox.Text, out int productionTime))
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Productietijd moet een geheel getal zijn.", dialog);
                    return;
                }

                if (!decimal.TryParse(priceBox.Text, out decimal price))
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Prijs moet een geldig getal zijn.", dialog);
                    return;
                }

                // Haal de geselecteerde categorie ID op uit de ComboBox
                var selectedCategoryId = categoryComboBox.SelectedValue;
                if (selectedCategoryId == null)
                {
                    args.Cancel = true;
                    await ShowErrorAsync("Selecteer een geldige categorie.", dialog);
                    return;
                }

                int categoryId = (int)selectedCategoryId;

                try
                {
                    using var updateDb = new AppDbContext();

                    var productToUpdate = updateDb.Products.FirstOrDefault(p => p.Id == selectedProduct.Id);

                    if (productToUpdate == null)
                    {
                        args.Cancel = true;
                        await ShowErrorAsync("Product niet gevonden.", dialog);
                        return;
                    }

                    productToUpdate.Name = nameBox.Text;
                    productToUpdate.Description = descriptionBox.Text;
                    productToUpdate.Type = typeBox.Text;
                    productToUpdate.Material = materialBox.Text;
                    productToUpdate.ProductionTime = productionTime;
                    productToUpdate.Complexity = complexityBox.Text;
                    productToUpdate.Sustainability = sustainabilityBox.Text;
                    productToUpdate.UniqueFeatures = uniqueFeaturesBox.Text;
                    productToUpdate.Price = price;
                    productToUpdate.Image = imageBox.Text;
                    productToUpdate.CategoryId = categoryId;

                    await updateDb.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    args.Cancel = true;
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
        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            var db = new AppDbContext();
            var button = (Button)sender;
            var selectedProduct = (Data.Product)button.DataContext;
            db.Remove(db.Products.FirstOrDefault(p => p.Id == selectedProduct.Id));
            db.SaveChanges();
            Frame.Navigate(typeof(Product.OverviewPage));
        }
        private async void Button_Click_Open(object sender, RoutedEventArgs e)
        {
            var db = new AppDbContext();
            var button = (Button)sender;
            var selectedProduct = (Data.Product)button.DataContext;
            db.Remove(db.Products.FirstOrDefault(p => p.Id == selectedProduct.Id));
            db.SaveChanges();
        }
    }
}
