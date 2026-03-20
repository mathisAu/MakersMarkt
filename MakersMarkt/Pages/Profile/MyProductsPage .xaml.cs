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
using MakersMarkt.Pages.Product;



namespace MakersMarkt.Pages
{
   
    public sealed partial class MyProductsPage : Page
    {
        public MyProductsPage()
        {
            InitializeComponent();
            // Subscribe to the Loaded event to fetch data when the page is displayed
            Loaded += MyProductsPage_Loaded;
        }

        // Event handler called when the page finishes loading
        private void MyProductsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData(); // Fetch products and categories from the database
        }

        // Retrieves necessary data from the database and binds it to the UI
        private void LoadData()
        {
            using var context = new AppDbContext();
            
            // Get the currently logged-in user
            var currentUser = LoggedInUser.CurrentUser;

            // If a user is logged in, fetch only their products
            if(currentUser != null)
            {
                var myProducts = context.Products
                    .Include(p => p.Category) // Include Category data so we can display the category name
                    .Where(p => p.MakerId == currentUser.Id) // Filter by the currently logged-in user's ID
                    .ToList();
                    
                // Bind the filtered products to the ListView in the XAML
                ProductsListView.ItemsSource = myProducts;
            }

            // Load the top 5 categories from the database for the right sidebar
            var topCategories = context.Categories.Take(5).ToList();
            
            // Bind the categories to the TopCategoriesListView in the XAML
            TopCategoriesListView.ItemsSource = topCategories;
        }

        // Navigates the user to the ProductPage (Home)
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage)); // Navigate to the main product overview
        }

        // Handles clicks on the "Items" button in the header
        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            // The user is already on the items page, so we don't need to navigate away
        }

        // Navigates the user back to their Profile page
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage)); 
        }

        // Navigates the user to the page where they can add a new product
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddProductPage));
        }

        // Handles editing an existing product
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // Extract the product ID from the clicked button's Tag property
            if (sender is Button btn && btn.Tag is int productId)
            {
                // Navigate to the edit page, passing the specific product ID
                Frame.Navigate(typeof(EditProductPage), productId);
            }
        }

        // Handles deleting a product with a confirmation dialog
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // Extract the product ID from the clicked button's Tag property
            if (sender is Button btn && btn.Tag is int productId)
            {
                // Create a confirmation dialog to prevent accidental deletions
                var dialog = new ContentDialog
                {
                    Title = "Delete Product",
                    Content = "Are you sure you want to delete this product?",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    XamlRoot = this.XamlRoot
                };

                // Show the dialog and wait for the user's response
                var result = await dialog.ShowAsync();

                // If the user clicked "Delete"
                if (result == ContentDialogResult.Primary)
                {
                    using var context = new AppDbContext();
                    
                    // Find the product in the database by its ID
                    var product = context.Products.Find(productId);
                    
                    // If the product exists, remove it
                    if (product != null)
                    {
                        context.Products.Remove(product);
                        context.SaveChanges(); // Apply the deletion to the database
                        
                        LoadData(); // Refresh the list on the UI to reflect the deleted item
                    }
                }
            }
        }
    }
}
