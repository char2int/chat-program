using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using System.Linq;
using System.Xml.Linq;

namespace server
{
    internal class Program
    {
        public static HttpListener listener;
        public static string url = "http://*:80/";
        private const string admin_password = @"1&P7F9NxZ7@ZiqH77By!Jh#K";

        public static async Task HandleIncomingConnections()
        {
            helper.shutdown = false;
            int requests = 0;
            while (!helper.shutdown)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();
                requests++;
                bool save = true;
                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                client cli = new client();
                banned b_users = JsonConvert.DeserializeObject<banned>(File.ReadAllText("banned.json"));
                Console.WriteLine("Request from: " + req.RemoteEndPoint.Address + ":" + req.RemoteEndPoint.Port);
                Console.WriteLine("Request url: " + req.Url.AbsolutePath);
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
                    if (cli.message == "")
                    {
                        save = false;
                    }
                    Console.WriteLine($"Message saved from {cli.token}!");
                    string data_response = "200";

                    if (req.RemoteEndPoint.Address != null && b_users != null)
                    {
                        foreach (var user in b_users.users)
                        {
                            if (cli.token == user.username)
                            {
                                data_response = "user banned";
                                save = false;
                            }
                            if (req.RemoteEndPoint.Address.ToString() == user.ip)
                            {
                                data_response = "ip banned";
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        helper.SaveMessage($"{cli.token} - {cli.message}");
                    }
                    byte[] data = Encoding.UTF8.GetBytes(encryption.encrypt(data_response));
                    resp.ContentType = "text/json";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/admin"))
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
                    admin adminrequest = JsonConvert.DeserializeObject<admin>(encryption.decrypt(text));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("[ADMIN] Admin Request from " + adminrequest.ip);
                    Console.WriteLine("[ADMIN] Admin action: " + adminrequest.action);
                    Console.WriteLine("[ADMIN] Admin data: " + adminrequest.data);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string response_data;
                    if (adminrequest.password == admin_password)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[ADMIN] Admin authenticated!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        response_data = helper.AdminAction(adminrequest.action, adminrequest.data);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ADMIN] Admin used incorrect password!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        File.AppendAllText("failed-admin.log", JsonConvert.SerializeObject(adminrequest));
                        response_data = "403";
                    }
                    byte[] data = Encoding.UTF8.GetBytes(encryption.encrypt(response_data));
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
                resp.Close();
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
