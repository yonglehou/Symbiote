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
using Symbiote.Messaging.Impl.Channels;

namespace Symbiote.Messaging
{
    public static class LocalChannelExtension
    {
        private static IChannelIndex Index
        {
            get { return Assimilate.GetInstanceOf<IChannelIndex>(); }
        }

        public static IBus AddLocalChannel( this IBus bus )
        {
            bool hasChannelFor = Index.HasChannelFor( "local" );

            LocalChannelDefinition definition =
                hasChannelFor
                    ? Index.GetDefinition("local") as LocalChannelDefinition
                    : new LocalChannelDefinition {Name = "local"};
            
            if(!hasChannelFor)
                Index.AddDefinition( definition );
            
            return bus;
        }

        public static IBus AddLocalChannel( this IBus bus, Action<IConfigureChannel> configure )
        {
            bool hasChannelFor = Index.HasChannelFor("local");

            LocalChannelDefinition definition =
                hasChannelFor
                    ? Index.GetDefinition("local") as LocalChannelDefinition
                    : new LocalChannelDefinition { Name = "local" };

            configure(definition);

            if (!hasChannelFor)
                Index.AddDefinition(definition);
            return bus;
        }
    }
}