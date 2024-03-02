using Hyper_Ship_Battle.LAN_Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper_Ship_Battle
{
    internal class ClientManager
    {
        private static ClientManager instance;
        private TcpClient client;

        private ClientManager()
        {
            client = new TcpClient();
        }

        public static ClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientManager();
                }
                return instance;
            }
        }

        public TcpClient GetClient()
        {
            return client;
        }
    }
}
