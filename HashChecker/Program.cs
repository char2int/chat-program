using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace HashChecker
{
    internal class Program
    {
        private static WebClient client2 = new WebClient();
        private static string latest_hash = client2.DownloadString("https://raw.githubusercontent.com/char2int/chat-program/main/latest.HASH").Replace("\n", "");

        static void Main(string[] args)
        {
            Console.Title = "HashChecker | ConsoleMessenger";
            Console.WriteLine("Input Program: ");
            string program = Console.ReadLine().Replace("\"", "");
            Console.Clear();
            if (latest_hash == CalculateMD5(program))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Hashes match!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine("Latest Hash:");
            Console.WriteLine(latest_hash);
            Console.WriteLine("Current Hash: ");
            Console.WriteLine(CalculateMD5(program));
            Console.ReadKey();
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
