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
using Windows.Storage;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MakersMarkt.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        public int _productId { get; set; }
        public DetailPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int ProductId)
            {
                _productId = ProductId;
            }
        }
        public void LoadDate()
        {
            var db = new Data.AppDbContext();
            var product = db.Products.FirstOrDefault(p => p.Id == _productId);
        }

        private async void Buy_Click(object sender, RoutedEventArgs e)
        {
            var db = new Data.AppDbContext();
            var product = db.Products.FirstOrDefault(p => p.Id == _productId);
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.TryGetValue("UserId", out object userIdObj) &&
                int.TryParse(userIdObj?.ToString(), out int userId))
            {
                ContentDialog? dialog = null;

                var saveButton = new Button
                {
                    Content = "Save",
                    Width = 100
                };

                var panel = new StackPanel
                {
                    Spacing = 8
                };

                saveButton.Click += async (s, args) =>
                {
                    using var db2 = new AppDbContext();

                    db2.Add(new Data.Order
                    {
                        BuyerId = userId,
                        ProductId = product.Id,
                        TotalPrice = product.Price,
                        Status = "Niet bekend",
                    });

                    await db2.SaveChangesAsync();

                    dialog?.Hide();
                };

                panel.Children.Add(saveButton);

                dialog = new ContentDialog
                {
                    Title = $"Koop {product.Name}",
                    Content = panel,
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }
        }
    }

