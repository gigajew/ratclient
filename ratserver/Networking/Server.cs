﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ratserver.Networking
{
    public sealed class Server : IDisposable
    {
        public event EventHandler<bool> ServerStateChanged;
        public event EventHandler<bool> ClientStateChanged;
        public event EventHandler<string> ClientMessageReceived;

        private Socket socket;

        public bool Listen(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(endPoint);
                socket.Listen(100);
                OnServerStateChanged(true);
                return true;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        
        private void BeginAccept()
        {
            socket.BeginAccept(EndAccept, this);
        }

        private void EndAccept(IAsyncResult ar )
        {
            Server server = (Server)ar.AsyncState;
            Socket client = server.socket.EndAccept(ar);

            ClientNode node = new ClientNode();
            node.StateChanged += OnClientStateChanged;
            node.MessageReceived += OnClientMessageReceived;
            node.InitializeFromSocket(client);

            server.BeginAccept();
        }

        private void OnClientMessageReceived(object sender, string message)
        {
            EventHandler<string> handler = ClientMessageReceived;
            if (handler != null)
            {
                handler(sender, message);
            }
        }

        private void OnClientStateChanged(object sender, bool connected)
        {
            if (connected)
            {
                 // add client to list
            }
            else
            {
                // remove client from list
            }

            EventHandler<bool> handler = ClientStateChanged;
            if (handler != null)
            {
                handler(sender, connected);
            }
        }

        private void OnServerStateChanged(bool listening)
        {
            if (listening)
            {
                BeginAccept();
            }

            EventHandler<bool> handler = ServerStateChanged;
            if (handler != null)
            {
                handler(this, listening);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}