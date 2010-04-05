﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.WebSocket;
using Symbiote.Log4Net;

namespace WebSocketService
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core()
                .Daemon<WebSocketService>(
                    x => x.Name("wss").DisplayName("Web Socket Service").Description("A web socket service").Arguments(args))
                .WebSocketServer(x => 
                    x.ServerUrl(@"http://localhost:80")
                     .SocketUrl(@"ws://localhost:8181/test")
                     .Port(8181))
                    //.OnClientConnect(FutureInstanceProxy<WebSocketService>.New<WebSocketService>().Instance.ClientConnected)
                    //.OnClientDisconnect(FutureInstanceProxy<WebSocketService>.New<WebSocketService>().Instance.ClientDisconnected)
                    //.OnMessage(FutureInstanceProxy<WebSocketService>.New<WebSocketService>().Instance.ClientMessage)
                    //.OnServerShutdown(FutureInstanceProxy<WebSocketService>.New<WebSocketService>().Instance.Shutdown))
                .AddConsoleLogger<WebSocketService>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddColorConsoleLogger<ISocketServer>(x => x.Info().MessageLayout(m => m.Message().Newline()).DefineColor().Text.IsYellow())
                .RunDaemon<WebSocketService>();
        }
    }

    public class WebSocketService
        : IDaemon
    {
        private ISocketServer _server;

        public void Start()
        {
            _server.Start();
        }

        public void ClientConnected(string clientName)
        {
            "Client {0} has connected."
                .ToInfo<WebSocketService>(clientName);
        }

        public void ClientDisconnected(string clientName)
        {
            "Client {0} has disconnected"
                .ToInfo<WebSocketService>(clientName);
        }

        public void ClientMessage(Tuple<string, string> message)
        {
            "Client {0} says: \r\n\t {1}"
                .ToInfo<WebSocketService>(message.Item1, message.Item2);
        }

        public void Shutdown()
        {
            "Web socket server is shutting down"
                .ToInfo<WebSocketService>();
        }

        public void Stop()
        {
            _server.Dispose();
            _server = null;
        }

        public WebSocketService(ISocketServer server)
        {
            _server = server;
        }
    }
}
