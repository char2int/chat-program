using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    internal class encryption
    {
        private const string key = "char2int023948";

        public static string encrypt(string input)
        {
            return convert(XOR(input), true);
        }

        public static string decrypt(string input)
        {
            return XOR(convert(input, false));
        }

        private static string XOR(string input)
        {
            DateTime time = DateTime.UtcNow;
            char[] key = $"{encryption.key}{time.Minute}".ToCharArray();
            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (char)(input[i] ^ key[i % key.Length]);
            }
            return new string(output);
        }

        private static string convert(string input, bool enc)
        {
            if (enc)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                var base64EncodedBytes = System.Convert.FromBase64String(input);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
        }
    }
}
