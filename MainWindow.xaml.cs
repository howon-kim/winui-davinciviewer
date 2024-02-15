using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.WebUI;
using Windows.Web.UI.Interop;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Contacts;
using System.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DavinciViewer
{

    public sealed partial class MainWindow : Window
    {
        private Dictionary<String, Uri> uris = new Dictionary<String, Uri>();
        private Dictionary<NavigationViewItem, WebView2> webviews = new Dictionary<NavigationViewItem, WebView2>();
        string url = "https://raw.githubusercontent.com/howon-kim/winui-davinciviewer/master/data/path.json";

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeWebView();
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        private async Task LoadJsonDataAsync()
        {
            try
            {
                // Create an HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Make a GET request to fetch the JSON data
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string jsonText = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON data into objects
                        JsonDocument doc = JsonDocument.Parse(jsonText);
                        JsonElement root = doc.RootElement;

                        // Get the array of workspaces
                        JsonElement workspaces = root.GetProperty("Workspace");

                        // Loop through each workspace
                        for (int index = 0; index < workspaces.GetArrayLength(); index++)
                        {
                            JsonElement workspace = workspaces[index];

                            // Extract name and URL
                            string name = workspace.GetProperty("Name").GetString();
                            string urlValue = workspace.GetProperty("URL").GetString();
                            uris[name] = new Uri(urlValue);

                            NavigationViewItem navItem = new NavigationViewItem();
                            navItem.Content = name;
                            navItem.Tag = urlValue;
                            navItem.Icon = CreateIconForIndex(index);

                            webviews[navItem] = new WebView2();
                            webviews[navItem].Source = new Uri(urlValue);
                            await webviews[navItem].EnsureCoreWebView2Async();
                            webviews[navItem].CoreWebView2.NewWindowRequested += WebViewControl_New_NewWindowRequested;
                            NavView.MenuItems.Add(navItem);
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        // For example, display an error message
                        // Console.WriteLine("Failed to fetch JSON data: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                // For example, display an error message
                // Console.WriteLine("Error: " + ex.Message);
            }
        }

        private IconElement CreateIconForIndex(int index)
        {
            int start = 326 + index;
            string iconName = char.ConvertFromUtf32(0xF000 + start);
            Debug.WriteLine(iconName);
            FontIcon icon = new FontIcon();
            icon.Glyph = iconName;
            return icon;
        }

        private async void InitializeWebView()
        {
            var selectedItem = NavView.MenuItems[0] as NavigationViewItem;
            webviews[selectedItem] = new WebView2();
            var uri = new Uri(selectedItem.Tag.ToString());
            webviews[selectedItem].Source = uri;
            await webviews[selectedItem].EnsureCoreWebView2Async();

            await Task.Delay(TimeSpan.FromSeconds(6));
            LoadJsonDataAsync();

            /**
            for (int i = 1; i < NavView.MenuItems.Count; i++)
            {

                if (NavView.MenuItems[i] is NavigationViewItem)
                {
                    selectedItem = NavView.MenuItems[i] as NavigationViewItem;
                    Debug.WriteLine(i);
                    webviews[selectedItem] = new WebView2();
                    webviews[selectedItem].Source = uris[selectedItem.Content.ToString()];
                    await webviews[selectedItem].EnsureCoreWebView2Async();
                    webviews[selectedItem].CoreWebView2.NewWindowRequested += WebViewControl_New_NewWindowRequested;
                }
            }
            **/
            // Ensure CoreWebView2 is ready
            //await WebViewControl.EnsureCoreWebView2Async();

            // Navigate to the initial website
            //NavigateToWebsite("https://www.google.com");
        }

        private async void NavigateToWebsite(string url)
        {
            try
            {
                // Navigate to the URL
                //WebViewControl.Source = new Uri(url);
                //await WebViewControl.EnsureCoreWebView2Async();
                //WebViewControl.CoreWebView2.NewWindowRequested += WebViewControl_New_NewWindowRequested;
            }
            catch (Exception ex)
            {
                // Handle exception, such as invalid URL
                Console.WriteLine($"Error navigating to website: {ex.Message}");
            }
        }


        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            ContentGrid.Children.Clear();
            // NEW CODE
            var selectedItem = args.SelectedItem as NavigationViewItem;
            if (!webviews.ContainsKey(selectedItem) )
            {
                Debug.WriteLine("Create new Webview2");
                webviews[selectedItem] = new WebView2();
                var uri = new Uri(selectedItem.Tag.ToString());
                webviews[selectedItem].Source = uri;
            } 

            Debug.WriteLine("Loading the Website");
            ContentGrid.Children.Add(webviews[selectedItem]);
        }

        private void WebViewControl_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            // Cancel the navigation to prevent it from opening in a new window
            args.Cancel = true;

            // Navigate to the URL using the same WebView control
            NavigateToWebsite(args.Uri.ToString());
        }

        private void WebViewControl_New_NewWindowRequested(object sender,CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.NewWindow = (CoreWebView2)sender;
            //e.Handled = true;
        }


    }
}