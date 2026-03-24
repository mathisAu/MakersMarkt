using MakersMarkt.Data;
using MakersMarkt.Pages.Login;
using MakersMarkt.Pages.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            int userId = e.Parameter is int id ? id : 2;
            await LoadUserAsync(userId);
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(ProductPage));
        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            if (User != null)
                Frame?.Navigate(typeof(MyProductsPage), User.Id);
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
    }
}