using MakersMarkt.Data;
using MakersMarkt.Pages.Login;
using MakersMarkt.Pages.Product;
using MakersMarkt.Pages.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace MakersMarkt.Pages
{
    public sealed partial class ProfilePage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<MakersMarkt.Data.Product> Products { get; } = new();
        private User? _user;
        public User? User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public ProfilePage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            LoadNotifications();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            int userId = e.Parameter is int id ? id : 2;
            await LoadUserAsync(userId);
        }

        private async void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (User == null) return;

            Frame?.Navigate(typeof(EditProfile), User.Id);
        }

        private async Task LoadUserAsync(int userId)
        {
            using var db = new AppDbContext();

            var user = await db.Users
                               .Include(u => u.Products)
                               .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                User = null;
                Products.Clear();
                return;
            }

            User = user;

            Products.Clear();

            var products = (user.Products ?? new List<MakersMarkt.Data.Product>())
                .OrderByDescending(p => p.Id)
                .Take(6);

            foreach (var p in products)
            {
                var imageUrl = p.ImageUrl;

                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    imageUrl = "ms-appx:///Assets/placeholder.png";
                }
                else if (!imageUrl.StartsWith("ms-appx://", StringComparison.OrdinalIgnoreCase)
                      && !imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    imageUrl = $"ms-appx:///{imageUrl.TrimStart('/')}";
                }

                p.ImageUrl = imageUrl;

                Products.Add(p);
            }
        }

        private void ProductImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image img)
            {
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/placeholder.png"));
            }
        }
        

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(Product.ProductPage));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(ProductPage));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (User != null)
                Frame?.Navigate(typeof(ProfilePage), User.Id);
        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            if (User != null)
                Frame?.Navigate(typeof(MyProductsPage), User.Id);
        }
        

private void ArtistsButton_Click(object sender, RoutedEventArgs e)
        {
            if (User != null)
                Frame?.Navigate(typeof(Order.ArtistOrderPage), User.Id);
        }



        private void SeeMore_Click(object sender, RoutedEventArgs e)
        {
            if (User != null)
                Frame?.Navigate(typeof(MyProductsPage), User.Id);
        }

        private void Product_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MakersMarkt.Data.Product p)
            {
                Frame?.Navigate(typeof(ProductDetailPage), p.Id);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void LoadNotifications()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("UserId", out object userIdObj))
            {
                int userId = Convert.ToInt32(userIdObj);

                using (var db = new AppDbContext())
                {
                    var orders = db.Orders
                        .Where(o => o.BuyerId == userId)
                        .Include(o => o.Product)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Ingelogde userId: {userId}");
                    System.Diagnostics.Debug.WriteLine($"Aantal orders na filter: {orders.Count}");

                    foreach (var item in orders)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"OrderId: {item.Id}, BuyerId: {item.BuyerId}, Status: {item.Status}"
                        );

                        if (!string.IsNullOrWhiteSpace(item.Status))
                        {
                            ShowOrderNotification(item.Id, item.Status);
                        }
                    }
                    foreach (var review in db.Reviews.Include(p => p.Product).Where(r => r.Product.MakerId == userId))
                    {
                        ShowReviewNotification(review.ProductId, review.Comment);
                    }
                    foreach (var order in db.Orders.Include(o => o.Product).Where(o => o.Product.MakerId == userId))
                    {
                        ShowProductNotification(order.Product.Name);
                    }
                        
                    }


            }

        }
        private void ShowOrderNotification(int OrderNumber, string status)
        {
            var notification = new AppNotificationBuilder()
                .AddText("Order update")
                .AddText($"De order van {OrderNumber} status is gewijzigd naar: {status}")
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
        private void ShowReviewNotification(int ProductId, string Review)
        {
            var notification = new AppNotificationBuilder()
                .AddText($"Review update {ProductId}")
                .AddText($"{Review}")
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
        private void ShowProductNotification(string ProductName)
        {
            var notification = new AppNotificationBuilder()
                .AddText($"{ProductName} besteld")
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
    }


}