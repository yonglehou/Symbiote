﻿// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit
{
    public static class RabbitExtensions
    {
        public static IBus AddRabbitQueue( this IBus bus, Action<EndpointConfigurator> configurate )
        {
            var endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            endpoints.ConfigureEndpoint( configurate );
            return bus;
        }

        public static IBus AddRabbitExchange( this IBus bus, Action<ChannelConfigurator> configurate )
        {
            var channels = Assimilate.GetInstanceOf<IChannelIndex>();
            var connectionManager = Assimilate.GetInstanceOf<IConnectionManager>();
            var configurator = new ChannelConfigurator();

            configurate( configurator );
            var channelDefinition = configurator.ChannelDefinition;

            channelDefinition.CreateOnBroker( connectionManager );
            channels.AddDefinition( channelDefinition );
            return bus;
        }

        public static IBus BindExchangeToExchange( this IBus bus, string source, string destination,
                                                params string[] routingKeys )
        {
            var connectionManager = Assimilate.GetInstanceOf<IConnectionManager>();
            var connection = connectionManager.GetConnection();
            using( var channel = connection.CreateModel() )
            {
                if ( routingKeys.Length == 0 )
                    channel.ExchangeBind( destination, source, "", null );
                else
                    routingKeys
                        .ForEach( x => channel.ExchangeBind( destination, source, x, null ) );
            }

            return bus;
        }

        public static IBus BindExchangeToQueue( this IBus bus, string exchange, string queue,
                                                params string[] routingKeys )
        {
            var connectionManager = Assimilate.GetInstanceOf<IConnectionManager>();
            var connection = connectionManager.GetConnection();
            using( var channel = connection.CreateModel() )
            {
                if ( routingKeys.Length == 0 )
                    channel.QueueBind( queue, exchange, "", null );
                else
                    routingKeys
                        .ForEach( x => channel.QueueBind( queue, exchange, x, null ) );
            }

            return bus;
        }

        public static void CommitExchange( this IBus bus, string channelName )
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor( channelName ) as IHaveChannelProxy;
            channel.Proxy.Channel.TxCommit();
        }

        public static void RollbackExchange( this IBus bus, string channelName )
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor( channelName ) as IHaveChannelProxy;
            channel.Proxy.Channel.TxRollback();
        }
    }
}