# Navigation Troubleshooting Guide

## Issue: Profile and My Items Buttons Not Working on ProductPage

### Root Causes
The navigation might not work if:
1. **UserId not saved in local settings** - Ensure user ID is stored when logging in
2. **Frame reference is null** - Navigation requires a valid Frame
3. **Exception being silently caught** - Check Debug Output for errors

### Debugging Steps

#### Step 1: Check if UserId is saved in LocalSettings
Add this to your LoginPage after successful login:
```csharp
var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
localSettings.Values["UserId"] = userId; // Make sure this is set
```

Verify in ProductPage by adding debug output:
```csharp
private void ProfileButton_Click(object sender, RoutedEventArgs e)
{
    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
    if (localSettings.Values.TryGetValue("UserId", out object userIdObj))
    {
        System.Diagnostics.Debug.WriteLine($"Found UserId: {userIdObj}");
    }
    else
    {
        System.Diagnostics.Debug.WriteLine("UserId NOT found in local settings!");
    }
}
```

#### Step 2: Check the Output Window
1. Run your app in Debug mode
2. Click on a button that doesn't work
3. Open **Output Window** (View ? Output)
4. Look for any error messages like "Navigation error: ..."

#### Step 3: Check Frame is Valid
The Frame should be set up in your shell/main window. Verify:
```csharp
// In your page, check that Frame is not null
if (Frame == null)
{
    System.Diagnostics.Debug.WriteLine("Frame is NULL!");
}
```

### Common Solutions

#### Solution 1: Ensure LoginPage Saves UserId
Make sure your LoginPage saves the user ID:
```csharp
private async void LoginButton_Click(object sender, RoutedEventArgs e)
{
    // ... login logic ...
    if (user != null)
    {
        // Save user ID to local settings
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        localSettings.Values["UserId"] = user.Id;
        
        // Set the logged-in user
        LoggedInUser.CurrentUser = user;
        
        // Navigate to home page
        Frame?.Navigate(typeof(ProductPage));
    }
}
```

#### Solution 2: Check RegisterPage
If users register, make sure RegisterPage also saves the UserId:
```csharp
// After successful registration
var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
localSettings.Values["UserId"] = newUser.Id;
LoggedInUser.CurrentUser = newUser;
Frame?.Navigate(typeof(ProductPage));
```

#### Solution 3: Add a Fallback Mechanism
If the ProfilePage/MyProductsPage navigation isn't working, add this fallback:
```csharp
private void ProfileButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue("UserId", out object userIdObj) && 
            int.TryParse(userIdObj?.ToString(), out int userId))
        {
            Frame?.Navigate(typeof(Pages.ProfilePage), userId);
        }
        else
        {
            // Fallback: use default user ID
            System.Diagnostics.Debug.WriteLine("UserId not found, using default ID 2");
            Frame?.Navigate(typeof(Pages.ProfilePage), 2);
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
    }
}
```

### Pages with Navigation Handlers
All these pages now have the unified header with working navigation handlers:
- ? ProductPage
- ? ProductDetailPage  
- ? AddProductPage
- ? EditProductPage
- ? MyProductsPage
- ? ProfilePage

### Navigation Flow
```
ProductPage
    ?? Profile Button ? ProfilePage (requires UserId)
    ?? My Items Button ? MyProductsPage (requires UserId)
    ?? Buy Products Button ? ProductPage (same page)
    ?? Order History Button ? ArtistOrderPage (requires UserId)
    ?? Home Button ? ProductPage (same page)
```

### If Still Not Working
1. Check the **Debug Output** window for errors
2. Ensure you're **logged in** before trying to navigate
3. Verify **MainWindow.xaml** has a Frame control
4. Make sure the namespace imports are correct in XAML files
5. Try **Clean Build** (Build ? Clean Solution, then Build)
