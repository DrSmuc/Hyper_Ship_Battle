using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Hyper_Ship_Battle.LAN_Multiplayer
{
    internal class TcpClient
    {
        private Socket clientSocket;
        public event EventHandler<string> MessageReceived;

        public void Connect(string ipAddress, int port)
        {
            try
            {
                App.clientActive = true;
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(ipAddress, port);

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
            }
        }

        public void Send(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                clientSocket.Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    if (bytesRead > 0)
                    {
                        string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        OnMessageReceived(receivedMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving message: " + ex.Message);
            }
        }

        protected virtual void OnMessageReceived(string message)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MessageReceived?.Invoke(this, message); });
        }

        public void Disconnect()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                App.clientActive = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error disconnecting from server: " + ex.Message);
            }
        }
    }
}
