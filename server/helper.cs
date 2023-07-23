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
                string old_messages = getMessages();
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

        }

        private static void ClearChat()
        {
            File.WriteAllText(file, "");
            SaveMessage("Chat Cleared!");
        }
    }
}
