﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Futures;

namespace Symbiote.Net 
{
	public class SocketServer 
		: ISocketServer
    {
        public ConcurrentDictionary<string, ISocketAdapter> Adapters { get; set; }
        public SocketConfiguration Configuration { get; set; }
        public Socket Listener { get; set; }
        public bool Running { get; protected set; }
        public Action<ISocketAdapter, Action<ISocketAdapter>> OnConnection { get; set; }
        public IPEndPoint ServerEndpoint { get; set; }

        public void OnDisconnection( ISocketAdapter adapter )
        {
			//Console.WriteLine( "closing...");
            adapter.Close();
        }

        public void WaitForConnection()
        {
            Listener.BeginAccept( OnClient, null );
        }

        private void OnClient( IAsyncResult result )
        {
            try
            {
                var socket = Listener.EndAccept( result );
                WaitForConnection();
				var adapter = new SocketAdapter( socket, Configuration );
                OnConnection.BeginInvoke( adapter, OnDisconnection, Completed, null );
            }
            catch( SocketException sockEx )
            {
                Console.WriteLine( "WinSock sharted: {0}".AsFormat( sockEx ) );
            }
            finally
            {
                
            }
        }
		
		private void Completed( IAsyncResult result )
		{
			result.AsyncWaitHandle.Dispose();
		}

        public void Start( Action<ISocketAdapter, Action<ISocketAdapter>> onConnection )
        {
            Running = true;

            OnConnection = onConnection;
            Listener = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            Listener.Bind( ServerEndpoint );
            Listener.Listen( 10000 );
            WaitForConnection();
        }

        public void Stop()
        {
            Running = false;
            Adapters.ForEach( x => x.Value.Close() );
        }

        public SocketServer( SocketConfiguration configuration )
        {
            Adapters = new ConcurrentDictionary<string, ISocketAdapter>();
            Configuration = configuration;
            ServerEndpoint = new IPEndPoint( IPAddress.Any, configuration.Port );
        }
    }
}
