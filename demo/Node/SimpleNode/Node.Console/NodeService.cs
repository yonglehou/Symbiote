﻿using System;
using System.Timers;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Node.Console
{
    public class NodeService
        : IDaemon
    {
        public INode Node { get; set; }
        public INodeIdentityProvider IdentityProvider { get; set; }
        public Timer Timer { get; set; }

        public void Start()
        {
            "Starting node {0}"
                .ToDebug<NodeService>(IdentityProvider.Identity);
        }

        public void Stop()
        {
            "Stopping node {0}"
                .ToDebug<NodeService>(IdentityProvider.Identity);
        }

        public NodeService(INode node, INodeIdentityProvider identityProvider)
        {
            Node = node;
            IdentityProvider = identityProvider;
            this.Timer = new Timer(100);
            this.Timer.Start();
            this.Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var message = new Message("from '{0}' at {1}".AsFormat(IdentityProvider.Identity, e.SignalTime));
            Action<IEnvelope> modifyEnvelope = x => x.CorrelationId = DateTime.UtcNow.Ticks.ToString();
            try
            {
                Node.Publish(message, modifyEnvelope);
            }
            catch (Exception exception)
            {
                Node.Publish(message, modifyEnvelope);
            }
        }
    }
}