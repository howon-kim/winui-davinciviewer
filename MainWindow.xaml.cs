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
using Windows.UI.ApplicationSettings;
using Windows.Web.UI.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DavinciViewer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            NavView.SelectedItem = NavView.MenuItems[0];
            NavigateToWebsite("https://www.google.com"); // Navigate to the initial website
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                // Handle settings selection if needed
            }
            else
            {
                if (args.SelectedItem.GetType() == typeof(NavigationViewItem))
                {
                    var item = args.SelectedItem as NavigationViewItem;
                    string websiteUrl = item.Tag.ToString();
                    NavigateToWebsite(websiteUrl);
                }
            }
        }

        private void NavigateToWebsite(string url)
        {
            try
            {
                WebViewControl.Source = new Uri(url);
            }
            catch (Exception ex)
            {
                // Handle exception, such as invalid URL
                Console.WriteLine($"Error navigating to website: {ex.Message}");
            }
        }
    }
}
