
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stresser.Helpers.ConfigHelper;


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
                    options.Listen(IPAddress.Parse(d_host), port);
                })
                .Configure(ConfigureApp)
                .Build();

            host.Run();
        }
        private static (bool isValid, string message) IsAllowed(HttpRequest request)
        {
            string key = request.Headers["Authorization"];
            if (string.IsNullOrEmpty(key))
            {
                return (false, "The token is invalid!");
            }

            if (key == ConfigHelper.GetSetting("webserver", "token"))
            {
                return (true, "Authorized.");
            }
            return (false, "Key is invalid!");
        }

        private static void ConfigureApp(IApplicationBuilder app)
        {
            app.Run(ProcessRequest);
        }
        private static async Task ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            var absolutePath = request.Path.Value.TrimStart('/');
            var (isValidKey, KeyMessage) = IsAllowed(request);
            if (isValidKey)
            {
                switch (absolutePath)
                {
                    case "":
                        {
                            var errorResponse = new
                            {
                                message = "Bad Request",
                                error = "Please provide a valid API endpoint."
                            };
                            var errorJson = JsonConvert.SerializeObject(errorResponse);
                            var errorBuffer = Encoding.UTF8.GetBytes(errorJson);
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            response.ContentType = "application/json";
                            response.ContentLength = errorBuffer.Length;
                            await response.Body.WriteAsync(errorBuffer, 0, errorBuffer.Length);
                            break;
                        }
                    case "test":
                        {
                            var presponse = new
                            {
                                message = "Example Request",
                                error = "This is an example request"
                            };
                            var pjson = JsonConvert.SerializeObject(presponse);
                            var pBuffer = Encoding.UTF8.GetBytes(pjson);
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.ContentType = "application/json";
                            response.ContentLength = pBuffer.Length;
                            await response.Body.WriteAsync(pBuffer, 0, pBuffer.Length);
                            break;
                        }
                    default:
                        {
                            var errorResponse = new
                            {
                                message = "Page not found",
                                error = "The requested page does not exist."
                            };
                            var errorJson = JsonConvert.SerializeObject(errorResponse);
                            var errorBuffer = Encoding.UTF8.GetBytes(errorJson);
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            response.ContentType = "application/json";
                            response.ContentLength = errorBuffer.Length;
                            await response.Body.WriteAsync(errorBuffer, 0, errorBuffer.Length);
                            break;
                        }
                }
            } else {
                var errorResponse = new
                {
                    message = KeyMessage,
                    error = "Invalid API key."
                };
                var errorJson = JsonConvert.SerializeObject(errorResponse);
                var errorBuffer = Encoding.UTF8.GetBytes(errorJson);
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.ContentType = "application/json";
                response.ContentLength = errorBuffer.Length;
                await response.Body.WriteAsync(errorBuffer, 0, errorBuffer.Length);
            }
        }
    }

}