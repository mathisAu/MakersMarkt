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
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
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
    public sealed partial class ArtistOrderPage : Page
    {
        public ArtistOrderPage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            using var db = new AppDbContext();
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.TryGetValue("UserId", out object userIdObj))
            {
                int userId = System.Convert.ToInt32(userIdObj);

                var currentOrders = db.Orders
                    .Include(o => o.Product)
                    .Include(o => o.Buyer)
                    .Where(o => o.Product.MakerId == userId && o.Status != null)
                    .ToList();

                var oldOrders = db.Orders
                    .Include(o => o.Product)
                    .Include(o => o.Buyer)
                    .Where(o => o.Product.MakerId == userId && o.Status != null)
                    .ToList();

                OrderListView.ItemsSource = currentOrders;
                OldOrderListView.ItemsSource = oldOrders;
            }
        }

        private async void OrderListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var order = (Data.Order)e.ClickedItem;
            var statusPanel = CreateStatusSelectionControl(order);

            var statusDialog = new ContentDialog
            {
                Title = "Status wijzigen",
                Content = statusPanel,
                PrimaryButtonText = "Opslaan",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await statusDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var panel = (StackPanel)statusDialog.Content;
                var comboBox = (ComboBox)panel.Children[1];
                var rejectBox = (TextBox)panel.Children[2];

                string selectedStatus = ((ComboBoxItem)comboBox.SelectedItem)?.Content?.ToString();
                string rejectReason = rejectBox.Text;

                if (!string.IsNullOrEmpty(selectedStatus))
                {
                    using var context = new AppDbContext();
                    var orderToUpdate = await context.Orders.FindAsync(order.Id);

                    if (orderToUpdate != null)
                    {
                        orderToUpdate.Status = selectedStatus;

                        if (selectedStatus == "geweigerd")
                        {
                            orderToUpdate.RejectDescription = rejectReason;
                        }

                        if (selectedStatus != "geweigerd")
                        {
                            using var db = new AppDbContext();
                            var customer = db.Users.Find(orderToUpdate.BuyerId);
                            var product = db.Products.FirstOrDefault(p => p.Id == orderToUpdate.ProductId);

                            if (customer != null && product != null)
                            {
                                customer.Credit -= product.Price;
                                db.Update(customer);
                                db.SaveChanges();
                            }
                        }

                        await context.SaveChangesAsync();

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

            Frame.Navigate(typeof(ArtistOrderPage), order.Id);
        }

        private StackPanel CreateStatusSelectionControl(Data.Order order)
        {
            StackPanel panel = new StackPanel();

            TextBlock label = new TextBlock
            {
                Text = "Selecteer nieuwe status:",
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 14
            };

            ComboBox statusComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                Width = 200,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            statusComboBox.Items.Add(new ComboBoxItem { Content = "in productie" });
            statusComboBox.Items.Add(new ComboBoxItem { Content = "geaccepteerd" });
            statusComboBox.Items.Add(new ComboBoxItem { Content = "geweigerd" });

            TextBox rejectBox = new TextBox
            {
                Header = "Reden van weigering",
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Height = 80,
                Visibility = Visibility.Collapsed
            };

            statusComboBox.SelectionChanged += (s, e) =>
            {
                var selected = ((ComboBoxItem)statusComboBox.SelectedItem)?.Content?.ToString();
                rejectBox.Visibility = selected == "geweigerd"
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };

            if (!string.IsNullOrEmpty(order.Status))
            {
                foreach (ComboBoxItem item in statusComboBox.Items)
                {
                    if (item.Content?.ToString() == order.Status)
                    {
                        statusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            panel.Children.Add(label);
            panel.Children.Add(statusComboBox);
            panel.Children.Add(rejectBox);

            return panel;
        }
    }
}