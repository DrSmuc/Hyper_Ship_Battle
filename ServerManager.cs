using Hyper_Ship_Battle.LAN_Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper_Ship_Battle
{
    public class ServerManager
    {
        private static ServerManager instance;
        private TcpServer host;
        private static int port_number = 1234;

        private ServerManager()
        {
            host = new TcpServer();
        }

        public static ServerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerManager();
                }
                return instance;
            }
        }

        public TcpServer GetHost()
        {
            return host;
        }

        public static int PORT_NUMBER()
        {
            return port_number;
        }
    }
}
