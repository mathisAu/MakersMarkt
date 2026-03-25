using MakersMarkt.Data;
using MakersMarkt.Pages.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
using Windows.Storage;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Order
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistOrderPage : Page
    {
        public ArtistOrderPage()
        {
            InitializeComponent();
            LoadData();
        }
        public void LoadData()
        {
            var db = new AppDbContext();
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("UserId", out object userIdObj) && userIdObj is int userId)
            {
                OrderListView.ItemsSource = db.Orders.Include(o => o.Product).Where(o => o.Product != null).ToList();
            }
        }

        private async void OrderListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var order = (Data.Order)e.ClickedItem;

            // Maak de status selectie control
            var statusPanel = CreateStatusSelectionControl(order);

            var statusDialog = new ContentDialog
            {
                Title = "Status wijzigen",
                Content = statusPanel,
                PrimaryButtonText = "Opslaan",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot // Voeg XamlRoot toe voor consistente weergave
            };

            ContentDialogResult result = await statusDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Haal de geselecteerde status op uit de combobox
                var comboBox = (ComboBox)((StackPanel)statusDialog.Content).Children[1];
                string selectedStatus = ((ComboBoxItem)comboBox.SelectedItem)?.Content.ToString();

                if (!string.IsNullOrEmpty(selectedStatus))
                {
                    // Update de status in de database via EntityFramework
                    using (var context = new AppDbContext())
                    {
                        var orderToUpdate = await context.Orders.FindAsync(order.Id);
                        if (orderToUpdate != null)
                        {
                            orderToUpdate.Status = selectedStatus;
                            await context.SaveChangesAsync();

                            // Toon bevestiging - nu in dezelfde stijl als het voorbeeld
                            var successDialog = new ContentDialog
                            {
                                Title = "Succes",
                                Content = $"Status is gewijzigd naar: {selectedStatus}",
                                CloseButtonText = "OK",
                                XamlRoot = this.XamlRoot
                            };

                            await successDialog.ShowAsync();
                        }
                    }
                }
            }

            Frame.Navigate(typeof(ProductPage), order.Id);
        }

        private StackPanel CreateStatusSelectionControl(Data.Order order)
        {
            StackPanel panel = new StackPanel();

            // Voeg een label toe
            TextBlock label = new TextBlock
            {
                Text = "Selecteer nieuwe status:",
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 14
            };

            // Maak de combobox met status opties
            ComboBox statusComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                Width = 200,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Voeg de status opties toe
            statusComboBox.Items.Add(new ComboBoxItem { Content = "in productie" });
            statusComboBox.Items.Add(new ComboBoxItem { Content = "geaccepteerd" });
            statusComboBox.Items.Add(new ComboBoxItem { Content = "geweigerd" });

            // Selecteer de huidige status indien beschikbaar
            if (!string.IsNullOrEmpty(order.Status))
            {
                foreach (ComboBoxItem item in statusComboBox.Items)
                {
                    if (item.Content.ToString() == order.Status)
                    {
                        statusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            panel.Children.Add(label);
            panel.Children.Add(statusComboBox);

            return panel;
        }
    }
}
