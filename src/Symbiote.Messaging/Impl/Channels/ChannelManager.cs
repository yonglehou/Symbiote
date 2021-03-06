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
using System.Collections.Concurrent;
using Symbiote.Core;

namespace Symbiote.Messaging.Impl.Channels
{
    public class ChannelManager
        : IChannelManager
    {
        public ConcurrentDictionary<string, IChannel> Channels { get; set; }
        public ConcurrentDictionary<Type, IChannelFactory> ChannelFactories { get; set; }
        public IChannelIndex Index { get; set; }

        public IChannel GetChannelFor( string channelName )
        {
            IChannel channel;
            if ( !Channels.TryGetValue( channelName, out channel ) )
            {
                var definition = Index.GetDefinition( channelName );
                channel = CreateChannelInstance( channelName, definition );
            }
            return channel;
        }

        public IChannel CreateChannelInstance( string channelName, IChannelDefinition definition )
        {
            var factory = GetChannelFactory( definition );
            IChannel channel = factory.CreateChannel( definition );
            Channels.TryAdd( channelName, channel );
            return channel;
        }

        public IChannelFactory GetChannelFactory( IChannelDefinition definition )
        {
            IChannelFactory factory;
            if ( !ChannelFactories.TryGetValue( definition.ChannelType, out factory ) )
            {
                factory = Assimilate.GetInstanceOf( definition.FactoryType ) as IChannelFactory;
                ChannelFactories.TryAdd( definition.FactoryType,
                                         factory );
            }
            return factory;
        }

        public ChannelManager( IChannelIndex channelIndex )
        {
            Index = channelIndex;
            Channels = new ConcurrentDictionary<string, IChannel>();
            ChannelFactories = new ConcurrentDictionary<Type, IChannelFactory>();
        }
    }
}