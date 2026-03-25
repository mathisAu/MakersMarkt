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
using Microsoft.UI.Xaml.Media.Animation;
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
            LoadDate();
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
            var localSettings = ApplicationData.Current.LocalSettings;
            var ratings = db.Reviews.Where(r => r.ProductId == product.Id).ToList();

            int som = 0;

            foreach (var rating in ratings)
            {
                som += rating.Rating;
            }
            int average = som / ratings.Count;
            score.Text = average.ToString();



            ReviewListView.ItemsSource = db.Reviews.Where(r => r.ProductId == product.Id).ToList();
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
                    var user = db2.Users.FirstOrDefault(u => u.Id == userId);
                    user.Credit = user.Credit - product.Price;

                    if (user.Credit >= 0)
                    {
                        db2.Add(new Data.Order
                        {
                            BuyerId = userId,
                            ProductId = product.Id,
                            TotalPrice = product.Price,
                            Status = "Niet bekend",
                        });
                        db.Update(user);
                        await db2.SaveChangesAsync();
                    }
                    else
                    {
                        errors.Text = "te weinig credits";
                        return;
                    }
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

        private void Commando_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            var product = db.Products.FirstOrDefault(p => p.Id == _productId);
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.TryGetValue("UserId", out object userIdObj) &&
                int.TryParse(userIdObj?.ToString(), out int userId))
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                db.Add(new Review
                {
                    Comment = review.Text,
                    Rating = (int)rating.Value,
                    UserId = userId,
                    ProductId = product.Id 
                });
                db.SaveChanges();
            }
            Frame.Navigate(typeof(Product.DetailPage), product.Id);
        }
    }
}

