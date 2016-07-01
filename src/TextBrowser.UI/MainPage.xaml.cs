using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextBrowser.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            wevMain.Navigate(new Uri("http://www.codeproject.com"));
        }

        private async void wevMain_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs e)
        {
            if (e.Uri == null)
                return;

            e.Cancel = true;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(String.Format(
                    "https://www.readability.com/api/content/v1/parser?url={0}&token=43188a9c55599e6fac4c022559a857532dce3967",
                    e.Uri.ToString()
                ));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ReadabilityResponse responseContent = JsonConvert.DeserializeObject<ReadabilityResponse>(await response.Content.ReadAsStringAsync());
                    if (responseContent != null)
                        wevMain.NavigateToString(responseContent.content);

                    btnBack.IsEnabled = wevMain.CanGoBack;
                    btnForward.IsEnabled = wevMain.CanGoForward;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (wevMain.CanGoBack)
                wevMain.GoBack();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (wevMain.CanGoForward)
                wevMain.GoForward();
        }
    }

    public class ReadabilityResponse
    {
        public string domain { get; set; }
        public object next_page_id { get; set; }
        public string url { get; set; }
        public string short_url { get; set; }
        public object author { get; set; }
        public string excerpt { get; set; }
        public string direction { get; set; }
        public int word_count { get; set; }
        public int total_pages { get; set; }
        public string content { get; set; }
        public object date_published { get; set; }
        public object dek { get; set; }
        public object lead_image_url { get; set; }
        public string title { get; set; }
        public int rendered_pages { get; set; }
    }
}
