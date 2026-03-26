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
        private List<MakersMarkt.Data.Product> _allMyProducts = new();

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
                _allMyProducts = context.Products
                    .Include(p => p.Category) // Include Category data so we can display the category name
                    .Where(p => p.MakerId == currentUser.Id) // Filter by the currently logged-in user's ID
                    .ToList();
            }

            var allCategories = context.Categories.ToList();

            // Insert "All Categories" as a dummy option directly to fit simple ItemSource databinding
            var categoryOptions = new List<Category> { new Category { Id = 0, Name = "All Categories" } };
            categoryOptions.AddRange(allCategories);
            
            FilterCategoryCombo.ItemsSource = categoryOptions;
            if (FilterCategoryCombo.SelectedIndex == -1) // Only select default if not already selected
            {
                FilterCategoryCombo.SelectedIndex = 0; 
            }

            // Load the top 5 categories from the database for the right sidebar
            var topCategories = allCategories.Take(5).ToList();
            
            // Bind the categories to the TopCategoriesListView in the XAML
            TopCategoriesListView.ItemsSource = topCategories;

            ApplyFiltersAndSort();
        }

        // Triggered when text in the search box changes
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        // Triggered when combobox selections change
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        // Applies the active search, filter, and sort definitions continuously across the local _allMyProducts buffer
        private void ApplyFiltersAndSort()
        {
            if (_allMyProducts == null) return;

            var filtered = _allMyProducts.AsEnumerable();

            // 1. Text Search Integration (Checks Name, Type, Specifications like Material/Complexity/UniqueFeatures)
            string searchText = SearchBox?.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(p => 
                    (p.Name != null && p.Name.ToLower().Contains(searchText)) || 
                    (p.Type != null && p.Type.ToLower().Contains(searchText)) ||
                    (p.Material != null && p.Material.ToLower().Contains(searchText)) ||
                    (p.Complexity != null && p.Complexity.ToLower().Contains(searchText)) ||
                    (p.UniqueFeatures != null && p.UniqueFeatures.ToLower().Contains(searchText))
                );
            }

            // 2. Category Dropdown Filter
            if (FilterCategoryCombo?.SelectedValue is int categoryId && categoryId != 0)
            {
                filtered = filtered.Where(p => p.CategoryId == categoryId);
            }

            // 3. Sorting Feature
            if (SortCombo?.SelectedItem is ComboBoxItem sortItem && sortItem.Tag is string sortTag)
            {
                switch (sortTag)
                {
                    case "NameAsc": filtered = filtered.OrderBy(p => p.Name); break;
                    case "NameDesc": filtered = filtered.OrderByDescending(p => p.Name); break;
                    case "PriceAsc": filtered = filtered.OrderBy(p => p.Price); break;
                    case "PriceDesc": filtered = filtered.OrderByDescending(p => p.Price); break;
                    case "TimeAsc": filtered = filtered.OrderBy(p => p.ProductionTime); break;
                }
            }

            // Execute bindings explicitly on valid filter execution
            if (ProductsListView != null)
            {
                ProductsListView.ItemsSource = filtered.ToList();
            }
        }

        // Navigates the user to the ProductPage (Home)
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage)); // Navigate to the main product overview
        }
private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Order.OrdersPage)); // Navigate to the main product overview
        }
        // Handles clicks on the "Items" button in the header
        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            // The user is already on the items page, so we don't need to navigate away
        }

        // Navigates the user back to their Profile page
        //private void SettingsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(ProfilePage)); 
        //}

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

