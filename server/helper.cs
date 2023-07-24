using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class helper
    {
        private static string file = "logs.txt";
        public static bool shutdown { get; set; }
        public static void SaveMessage(string message) 
        {
            try
            {
                string old_messages = getMessages().Replace("Chat Cleared!\n", "");
                string message2save = old_messages + "\n" + message;
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.WriteLine(serverencryption.encrypt(message2save));
                }
            }catch (Exception ex)
            {
                Console.WriteLine("[Error] Could not save message! Check error.log!");
                File.AppendAllText("error.log", ex.Message + "\n");
            }
        }
        public static string getMessages()
        {
            try
            {
                string text = File.ReadAllText(file);
                return serverencryption.decrypt(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] Could not get messages! Check error.log!");
                File.AppendAllText("error.log", ex.Message + "\n");
                return "";
            }

        }

        public static string AdminAction(string action, string data)
        {
            switch (action)
            {
                case "ban":
                    BanUser(data); return "200";
                case "clear":
                    ClearChat(); return "200";
                case "shutdown":
                    shutdown = true; return "200";
                default:
                    return "200";
            }
        }

        private static void BanUser(string username)
        {
            banned b_users = JsonConvert.DeserializeObject<banned>(File.ReadAllText("banned.json"));
            User new_banned = new User();
            new_banned.ip = "";
            new_banned.username = username;
            b_users.users.Add(new_banned);
            File.WriteAllText("banned.json", JsonConvert.SerializeObject(b_users));
        }

        public static void addIP(string username, string ip)
        {
            banned b_users = JsonConvert.DeserializeObject<banned>(File.ReadAllText("banned.json"));
            foreach (var user in b_users.users)
            {
                if (user.username == username)
                {
                    user.ip = ip;
                }
            }
            File.WriteAllText("banned.json", JsonConvert.SerializeObject(b_users));
        }

        private static void ClearChat()
        {
            File.WriteAllText(file, "");
            SaveMessage("Chat Cleared!");
        }
    }
}
