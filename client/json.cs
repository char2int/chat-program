using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class client
    {
        public string message { get; set; }
        public string token { get; set; }
    }

    public class admin
    {
        public string action { get; set; }
        public string data { get; set; }
        public string password { get; set; }
        public string ip { get; set; }
    }
}