using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crescent
{
    internal class CrescentConfig
    {
        OSCConfig OSC = new();
        public class OSCConfig
        {
            public string IP = "127.0.0.1";
            public short IncomingPort = 9001;
            public short OutgoingPort = 9000;
        }
        WebhookServerConfig WebhookServer = new();
        public class WebhookServerConfig
        {
            public bool Enabled = false;
            public string AuthKey = "ea7337e5-4eb2-4495-a565-d039c2f224f9";
        };

    }

 
}
