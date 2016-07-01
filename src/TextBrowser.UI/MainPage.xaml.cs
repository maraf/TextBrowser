using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
        private const string token = "43188a9c55599e6fac4c022559a857532dce3967";

        private readonly List<ReadabilityResponse> history = new List<ReadabilityResponse>();
        private int historyIndex = -1;

        public MainPage()
        {
            InitializeComponent();
            UpdateButtons();
            wevMain.Navigate(new Uri("http://www.codeproject.com"));
        }

        private IDisposable WithProgressBar()
        {
            grdLoading.Visibility = Visibility.Visible;
            prrMain.IsActive = true;
            return new DelegateDisposable(() =>
            {

                grdLoading.Visibility = Visibility.Collapsed;
                prrMain.IsActive = false;
            });
        }

        private void UpdateButtons()
        {
            btnReload.IsEnabled = history.Count > 0;
            btnBack.IsEnabled = historyIndex > 0;
            btnForward.IsEnabled = historyIndex < history.Count - 1;
        }

        private void ShowHistoryItem(int historyIndex)
        {
            ReadabilityResponse responseContent = history[historyIndex];
            wevMain.NavigateToString(responseContent.content);
            tbxUrl.Text = responseContent.url;
            this.historyIndex = historyIndex;
            UpdateButtons();
        }

        private void ShowResponse(ReadabilityResponse responseContent)
        {
            wevMain.NavigateToString(responseContent.content);
            tbxUrl.Text = responseContent.url;

            while (historyIndex + 1 < history.Count)
                history.RemoveAt(historyIndex + 1);

            history.Add(responseContent);
            historyIndex++;
            UpdateButtons();
        }

        private async Task NavigateTo(string url)
        {
            using (WithProgressBar())
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(String.Format(
                    "https://www.readability.com/api/content/v1/parser?url={0}&token={1}",
                    url,
                    token
                ));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ReadabilityResponse responseContent = JsonConvert.DeserializeObject<ReadabilityResponse>(await response.Content.ReadAsStringAsync());
                    if (responseContent != null)
                        ShowResponse(responseContent);
                }
            }
        }

        private async void wevMain_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs e)
        {
            if (e.Uri == null)
                return;

            e.Cancel = true;
            await NavigateTo(e.Uri.ToString());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count > 0)
            {
                int newIndex = historyIndex - 1;
                if (newIndex >= 0)
                    ShowHistoryItem(newIndex);
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count > 0)
            {
                int newIndex = historyIndex + 1;
                if (newIndex < history.Count)
                    ShowHistoryItem(newIndex);
            }
        }

        private async void btnReload_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count > 0)
                await NavigateTo(history[historyIndex].url);
        }

        private async void tbxUrl_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Uri uri;
            if (e.Key == VirtualKey.Enter && Uri.TryCreate(tbxUrl.Text, UriKind.Absolute, out uri))
            {
                grdNavigate.Visibility = Visibility.Collapsed;
                await NavigateTo(uri.ToString());
            }
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

    public enum NavigationDirection
    {
        Back,
        Forward
    }
}
