using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace client
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string token { get; set; }
        private static string old_response { get; set; }
        static async Task Main(string[] args)
        {
            Console.Title = "Log In | ConsoleMessenger";
            Console.WriteLine("Username: ");
            token = Console.ReadLine();
            Console.Clear();
            Console.Title = $"Chatting as {token} | ConsoleMessenger";
            Thread a = new Thread(Program.Thread);
            a.Start();
            while (true)
            {
                string message = Console.ReadLine();
                await SendMessage(message);
            }
        }

        static async void Thread()
        {
            while (true)
            {
                while (!Console.KeyAvailable)
                {
                    await GetMessages();
                }
            }
        }

        static async Task SendMessage(string message)
        {
            admin adm = new admin();
            client cli = new client();
            cli.message = message;
            cli.token = token;
            if (cli.message == "")
            {
                old_response = "";
                return;
            }
            if (cli.message.StartsWith("!") && cli.token == "admin")
            {
                try
                {
                    WebClient webby = new WebClient();
                    adm.action = cli.message.Replace("!", "").Split()[0];
                    adm.ip = webby.DownloadString("https://icanhazip.com/").Replace("\n", "");
                    if (cli.message.Contains("!ban"))
                    {
                        adm.data = cli.message.Replace("!ban ", "");
                    }
                    else
                    {
                        adm.data = "null";
                    }
                    Console.WriteLine("Password: ");
                    adm.password = Console.ReadLine();
                    var content = new StringContent(encryption.encrypt(JsonConvert.SerializeObject(adm)));
                    var response = await client.PostAsync("http://localhost/admin", content);
                    if (encryption.decrypt(response.Content.ReadAsStringAsync().Result) != "200")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] " + encryption.decrypt(response.Content.ReadAsStringAsync().Result));
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (encryption.decrypt(response.Content.ReadAsStringAsync().Result) == "200")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[Admin] Action completed successfully!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText("error.log", ex.Message + "\n");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                old_response = "";
            }
            else
            {
                try
                {
                    var content = new StringContent(encryption.encrypt(JsonConvert.SerializeObject(cli)));
                    var response = await client.PostAsync("http://localhost/post", content);
                    if (encryption.decrypt(response.Content.ReadAsStringAsync().Result) != "200")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] " + encryption.decrypt(response.Content.ReadAsStringAsync().Result));
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText("error.log", ex.Message + "\n");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        static async Task GetMessages()
        {
            try
            {
                var response = await client.GetAsync("http://localhost/get");
                var messages = encryption.decrypt(response.Content.ReadAsStringAsync().Result);
                if (messages != old_response)
                {
                    old_response = messages;
                    Console.Clear();
                    Console.WriteLine(messages);
                    Console.WriteLine("\nMessage: ");
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", ex.Message + "\n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] " + ex.Message);
                Console.WriteLine("Please look at error.log!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }
    }
}
