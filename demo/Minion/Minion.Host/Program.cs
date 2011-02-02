﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Minion.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Daemon.BootStrap;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Log4Net;
using Symbiote.Rabbit;

namespace Minion.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging()
                .AddConsoleLogger<IDaemon>( l => l.Info().MessageLayout( m => m.Message().Newline() ) )
                .Daemon( x => x.Arguments( args ).WithBootStraps( b => b.HostApplicationsFrom( @"..\..\..\Test" ) ) )
                .Rabbit( x => x.AddBroker( r => r.Defaults() ) )
                .RunDaemon();
        }

        public class Host : IDaemon
        {
            public IBus Bus { get; set; }

            public void Start()
            {
                Bus.AddLocalChannel();
                Bus.AddRabbitChannel(x => x.Direct("Host").AutoDelete());
                Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Host").ExchangeName("Host").StartSubscription().NoAck());

                Bus.AddRabbitChannel(x => x.Direct("Hosted").AutoDelete());
                Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Hosted").ExchangeName("Hosted").NoAck());
            }

            public void Stop()
            {
                
            }

            public Host( IBus bus )
            {
                Bus = bus;
            }
        }

        public class NotificationHandler : IHandle<MinionUp>
        {
            public IBus Bus { get; set; }

            public void Handle( IEnvelope<MinionUp> envelope )
            {
                "Minion says: {0}"
                    .ToInfo<IDaemon>( envelope.Message.Text );

                Enumerable
                    .Range(0, 1000)
                    .ForEach( x => Bus.Publish( "Hosted", new MinionDoThis() { Text = "Command {0}".AsFormat( x )} ) );
            }

            public NotificationHandler( IBus bus )
            {
                Bus = bus;
            }
        }
    }
}