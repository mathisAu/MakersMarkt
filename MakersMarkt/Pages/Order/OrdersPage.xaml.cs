using MakersMarkt.Data;
using MakersMarkt.Pages.Product;
using Microsoft.EntityFrameworkCore;
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
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Order
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrdersPage : Page
    {
        public OrdersPage()
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
                OrderListView.ItemsSource = db.Orders.Where(o => o.BuyerId == userId).Include(o => o.Product).ToList();
            }
        }
        private async void OrderListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var db = new AppDbContext();
            var order = (Data.Order)e.ClickedItem;

            ContentDialog dialog = null; // wordt later toegewezen
            var panel = new StackPanel { Spacing = 8 };
            panel.Children.Add(new TextBlock { Text = $"Product: {order.Product.Name}" });
            panel.Children.Add(new TextBlock { Text = $"Status: {order.Status}" });
            panel.Children.Add(new TextBlock { Text = $"Prijs: {order.TotalPrice}" });
            panel.Children.Add(new TextBlock { Text = $"Geschiedenis: {order.History}" });
            // andere content toevoegen...

            // eigen sluitknop
            var closeButton = new Button { Content = "Sluiten", HorizontalAlignment = HorizontalAlignment.Right };
            closeButton.Click += (_, __) => dialog?.Hide(); // sluit de dialog

            panel.Children.Add(closeButton);

            dialog = new ContentDialog
            {
                Title = $"Order: {order.Id}",
                Content = panel,
            };
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();
        }
    }
}
