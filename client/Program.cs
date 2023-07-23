using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace client
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string token { get; set; }

        static async Task Main(string[] args)
        {
            Console.Title = "ConsoleMessenger | Log In";
            Console.WriteLine("Login Token: ");
            token = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Getting Messages....");
            await GetMessages();
        }

        static async Task SendMessage(string message)
        {
            client cli = new client();
            cli.message = message;
            cli.token = "1234";
            var content = new StringContent(encryption.encrypt(JsonConvert.SerializeObject(cli)));

            var response = await client.PostAsync("http://localhost:8000/post", content);
        }

        static async Task GetMessages()
        {
            var response = await client.GetAsync("http://localhost:8000/get");
            //var messages = encryption.decrypt(response.Content.ToString());
            var messages = encryption.decrypt(response.Content.ReadAsStringAsync().Result);
            Console.WriteLine(messages);
        }
    }
}
