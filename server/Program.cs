using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace server
{
    internal class Program
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                client cli = new client();
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/post"))
                {
                    string text;
                    using (var reader = new StreamReader(req.InputStream,
                                                         req.ContentEncoding))
                    {
                        text = reader.ReadToEnd();
                    }
                    if (text == null || text == "")
                    {
                        // 403
                        resp.Close();
                    }
                    cli = JsonConvert.DeserializeObject<client>(encryption.decrypt(text));
                    Console.WriteLine($"Message saved from {cli.token}!");
                    helper.SaveMessage($"{cli.token} - {cli.message}");
                    byte[] data = Encoding.UTF8.GetBytes(encryption.encrypt("200"));
                    resp.ContentType = "text/json";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
                if ((req.HttpMethod == "GET") &&  (req.Url.AbsolutePath == "/get"))
                {
                    byte[] data = Encoding.UTF8.GetBytes(encryption.encrypt(helper.getMessages()));
                    resp.ContentType = "text/json";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
            }
        }


        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}
