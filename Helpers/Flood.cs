using System.Diagnostics;

namespace Stresser.Helpers.Flood
{
    public class Flood
    {
        private static readonly HttpClient _client = new HttpClient();
        
        public static async Task SendGetRequests(string url, int threads, int requestsPerSecond, int durationSeconds)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(() => FireRequests(url, requestsPerSecond, durationSeconds)));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private static async Task FireRequests(string url, int requestsPerSecond, int durationSeconds)
        {
            var delay = (int)(1000.0 / requestsPerSecond); // Convert to milliseconds

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
            {
                try
                {
                    await _client.GetAsync(url);
                }
                catch (Exception)
                {
                    // Handle exceptions if needed
                }

                await Task.Delay(delay);
            }
        }

        public static async Task Start(string targetUrl, int threads, int requestsPerSecond, int durationSeconds)
        {
            await SendGetRequests(targetUrl, threads, requestsPerSecond, durationSeconds);
        }
    }
}
