using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Stresser.Helpers.ConfigHelper;
using Stresser.Helpers.Flood;
using Stresser.Helpers.HLogger;

namespace Stresser.Services.WebServerService
{
    class WebServerService
    {
        public void Start(string d_port, string d_host)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    int port = int.Parse(d_port);
                    IPAddress hostIp;
                    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (IPAddress.TryParse(d_host, out hostIp))
                    {
                        options.Listen(hostIp, port);
                    }
                    else
                    {
                        options.ListenAnyIP(port);
                    }
                    #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                })
                .Configure(ConfigureApp)
                .Build();

            host.Run();
        }

        private static async Task ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.Method == HttpMethods.Post && request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();
                StringValues action;
                if (form.TryGetValue("action", out action))
                {
                    switch (action)
                    {
                        case "status":
                            Program.hLogger.Log(LogType.Info, $"Got am request for status");
                            await ExecuteStatusAction(response);
                            break;
                        case "attack":
                            Program.hLogger.Log(LogType.Info, $"Got a request for an attack!");
                            if (form.TryGetValue("domain", out var domain) &&
                           form.TryGetValue("threads", out var threads) &&
                           form.TryGetValue("rspt", out var rspt) &&
                           form.TryGetValue("token", out var token) &&
                           form.TryGetValue("time", out var time) &&
                           form.TryGetValue("type", out var type))
                            {
                                if (token == ConfigHelper.GetSetting("webserver", "token"))
                                {
                                    Program.hLogger.Log(LogType.Info, $"Attack started on {domain} for {time}s !");
                                    if (type == "L7")
                                    {
                                        #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                        //ExecuteHTTPAttackAction(response, domain, threads, rspt, time);
                                        #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    }
                                    if (type == "L4") {
                                        #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                        //ExecutePINGAttackAction(response, domain, threads, rspt, time);
                                        #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    }
                                    await WriteErrorResponse(response, HttpStatusCode.OK, "OK");
                                }
                                else
                                {
                                    Program.hLogger.Log(LogType.Warning, $"Some one tried to start an attack for {domain} but he provided a wrong token!");
                                    await WriteErrorResponse(response, HttpStatusCode.Forbidden, "Token is wrong bozo!.");
                                    break;
                                }
                            }
                            else
                            {
                                Program.hLogger.Log(LogType.Warning, $"Missing parameters for an attack!");
                                await WriteErrorResponse(response, HttpStatusCode.BadRequest, "Missing parameters for 'attack' action.");
                            }
                            break;
                        case "connect":
                            if (form.TryGetValue("token", out var authtoken))
                            {
                                if (authtoken == ConfigHelper.GetSetting("webserver", "token"))
                                {
                                    await WriteErrorResponse(response, HttpStatusCode.OK, "OK");
                                }
                                else
                                {
                                    await WriteErrorResponse(response, HttpStatusCode.Forbidden, "Token is wrong bozo!.");
                                }
                            }
                            else
                            {
                                await WriteErrorResponse(response, HttpStatusCode.BadRequest, "Missing parameters for 'connect' action.");
                            }
                            break;
                        default:
                            await WriteErrorResponse(response, HttpStatusCode.BadRequest, "Invalid action specified.");
                            break;
                    }
                }
                else
                {
                    await WriteErrorResponse(response, HttpStatusCode.BadRequest, "Action not specified.");
                }
            }
            else
            {
                await WriteErrorResponse(response, HttpStatusCode.MethodNotAllowed, "Unsupported method or content type.");
            }
        }

        private static async Task ExecuteStatusAction(HttpResponse response)
        {
            var statusResponse = new
            {
                status = "OK"
            };
            await WriteJsonResponse(response, HttpStatusCode.OK, statusResponse);
        }

        private static async Task WriteJsonResponse(HttpResponse response, HttpStatusCode statusCode, object responseObject)
        {
            var jsonResponse = JsonConvert.SerializeObject(responseObject);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.StatusCode = (int)statusCode;
            response.ContentType = "application/json";
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task WriteErrorResponse(HttpResponse response, HttpStatusCode statusCode, string errorMessage)
        {
            var errorResponse = new
            {
                message = errorMessage,
                error = statusCode.ToString()
            };
            await WriteJsonResponse(response, statusCode, errorResponse);
        }

        private static void ConfigureApp(IApplicationBuilder app)
        {
            app.Run(ProcessRequest);
        }
    }
}
