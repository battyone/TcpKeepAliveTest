using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace TcpKeepAliveTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnHttpClientClick(object sender, RoutedEventArgs e)
        {
            await PerformAction(ExecuteWithHttpClient);
        }

        private async void OnHttpWebRequestClick(object sender, RoutedEventArgs e)
        {
            await PerformAction(ExecuteWithHttpWebRequest);
        }

        private async Task PerformAction(Func<string, int, int, Task<HttpStatusCode>> action)
        {
            try
            {
                ControlGrid.IsHitTestVisible = false;
                BusyIndicator.IsActive = true;
                BusyIndicator.Visibility = Visibility.Visible;

                var url = UrlTextBox.Text;
                var keepAliveTime = Convert.ToInt32(KeepAliveTimeSlider.Value);
                var keepAliveInterval = Convert.ToInt32(KeepAliveIntervalSlider.Value);

                var sw = Stopwatch.StartNew();
                var statusCode = await action(url, keepAliveTime, keepAliveInterval);
                sw.Stop();

                var messageDialog = new MessageDialog($"Request succeeded with status '{statusCode}' in {sw.Elapsed}");
                await messageDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var messageDialog = new MessageDialog($"Request failed with message '{ex.Message}'");
                await messageDialog.ShowAsync();
            }
            finally
            {
                ControlGrid.IsHitTestVisible = true;
                BusyIndicator.IsActive = false;
                BusyIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private async Task<HttpStatusCode> ExecuteWithHttpClient(string url, int keepAliveTime, int keepAliveInterval)
        {
            ServicePointManager.SetTcpKeepAlive(keepAliveTime > 0, keepAliveTime * 1000, keepAliveInterval * 1000);

            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(360)
            };
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            var response = await httpClient.SendAsync(request);
            return response.StatusCode;
        }

        private async Task<HttpStatusCode> ExecuteWithHttpWebRequest(string url, int keepAliveTime, int keepAliveInterval)
        {
            var request = WebRequest.CreateHttp(url);
            request.KeepAlive = true;
            request.Accept = "application/json";
            request.Timeout = 360 * 1000;
            request.ServicePoint.SetTcpKeepAlive(keepAliveTime > 0, keepAliveTime * 1000, keepAliveInterval * 1000);
            var response = await request.GetResponseAsync();
            var httpResponse = (HttpWebResponse) response;
            return httpResponse.StatusCode;
        }
    }
}
