using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class server
    {
        public int status { get; set; }
        public List<string> messages { get; set; }
    }
    public class client
    {
        public string message { get; set; }
        public string token { get; set; }
    }
}
