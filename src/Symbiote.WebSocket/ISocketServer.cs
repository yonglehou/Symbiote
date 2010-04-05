﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Symbiote.WebSocket
{
    public interface ISocketServer :
        IDisposable,
        IObservable<Tuple<string, string>>
    {
        IList<IWebSocket> ClientSockets { get; }
        event Action<string> ClientConnected;
        event Action<string> ClientDisconnected;
        void Close();

        Socket Listener { get; }
        int Port { get; }
        string WebServerUrl { get; }
        string WebSocketUrl { get; }
        void SendToAll(string data, string from);
        void Send(string data, string from, string to);
        void Start();
    }
}