﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Npgsql;

namespace Hyper_Ship_Battle.LAN_Multiplayer
{
    public class TcpServer
    {
        private Socket serverSocket;
        private Socket clientSocket;
        private bool isRunning = false;
        public event EventHandler<string> MessageReceived;

        public TcpServer()
        {
            //serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartServer(string name, int port)
        {
            IPAddress getIPAddress()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip;
                    }
                }
                return null;
            }
            try
            {
                
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                serverSocket.Bind(new IPEndPoint(getIPAddress(), port));
                serverSocket.Listen(10);
                isRunning = true;
                Thread acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();

                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
                builder.Host = "narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud";
                builder.Port = 26257;
                builder.Username = "dr";
                builder.Password = "dFvz2ADZeOpME1TfRcAI1A";
                builder.Database = "presets";
                builder.SslMode = SslMode.Require;
                string connectionString = builder.ConnectionString;

                var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                string sql = "INSERT INTO room_list(name, ip) VALUES('" + name + "', '" + getIPAddress().ToString() + "');";
                var cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();

                App.serverActive = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting server: " + ex.Message);
            }
        }

        private void AcceptClients()
        {
            while (isRunning)
            {
                try
                {
                    clientSocket = serverSocket.Accept();
                    Thread receiveThread = new Thread(ReceiveMessages);
                    receiveThread.IsBackground = true;
                    receiveThread.Start(clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error accepting client: " + ex.Message);
                }
            }
        }

        private void ReceiveMessages(object client)
        {
            Socket clientSocket = (Socket)client;
            try
            {
                while (isRunning)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        OnMessageReceived(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving message: " + ex.Message);
                //client disconnected
            }
        }

        protected virtual void OnMessageReceived(string message)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MessageReceived?.Invoke(this, message); });
        }

        public void Send(string message)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                clientSocket.Send(data);
            }
        }

        public void StopServer()
        {
            try
            {
                isRunning = false;
                serverSocket.Close();
                clientSocket?.Close();
                App.serverActive = false;
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping server: " + ex.Message);
            }
        }
    }
}
