﻿/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/


using System;
using System.Collections.Generic;
using Symbiote.Messaging.Impl.Transform;

namespace Symbiote.Messaging.Impl.Channels
{
    public abstract class BaseChannelDefinition<TMessage> :
        IChannelDefinition<TMessage>
    {
        public string Name { get; set; }
        public Type MessageType { get; private set; }
        public Func<TMessage, string> RoutingMethod { get; set; }
        public Func<TMessage, string> CorrelationMethod { get; set; }
        public abstract Type ChannelType { get; }
        public abstract Type FactoryType { get; }

        public IConfigureChannel<TMessage> Named( string channelName )
        {
            Name = channelName;
            return this;
        }

        public IConfigureChannel<TMessage> CorrelateBy( string correlationId )
        {
            CorrelationMethod = x => correlationId;
            return this;
        }

        public IConfigureChannel<TMessage> CorrelateBy( Func<TMessage, string> messageProperty )
        {
            CorrelationMethod = messageProperty;
            return this;
        }

        public IConfigureChannel<TMessage> RouteBy( string routingKey )
        {
            RoutingMethod = x => routingKey;
            return this;
        }

        public IConfigureChannel<TMessage> RouteBy( Func<TMessage, string> messageProperty )
        {
            RoutingMethod = messageProperty;
            return this;
        }
        
        protected BaseChannelDefinition()
        {
            Name = "default";
            MessageType = typeof(TMessage);
            RoutingMethod = x => "";
            CorrelationMethod = x => "";
        }
    }

    public abstract class BaseChannelDefinition :
        IOpenChannelDefinition
    {
        public string Name { get; set; }
        public virtual Type ChannelType { get { return typeof(LocalChannel); } }
        public Type MessageType { get { return typeof(object); } }
        public virtual Type FactoryType { get { return typeof(LocalChannelFactory); } }
        public Dictionary<Type, Func<object, string>> RoutingMethods { get; set; }
        public Dictionary<Type, Func<object, string>> CorrelationMethods { get; set; }

        public IConfigureChannel Named(string channelName)
        {
            Name = channelName;
            return this;
        }

        public IConfigureChannel CorrelateBy<TMessage>(string correlationId)
        {
            CorrelationMethods.Add( typeof(TMessage), o => correlationId );
            return this;
        }

        public IConfigureChannel CorrelateBy<TMessage>(Func<TMessage, string> messageProperty)
        {
            CorrelationMethods.Add(typeof(TMessage), o => messageProperty((TMessage)o));
            return this;
        }

        public IConfigureChannel RouteBy<TMessage>(string routingKey)
        {
            RoutingMethods.Add( typeof(TMessage), o => routingKey );
            return this;
        }

        public IConfigureChannel RouteBy<TMessage>(Func<TMessage, string> messageProperty)
        {
            RoutingMethods.Add( typeof(TMessage), o => messageProperty( (TMessage) o ) );
            return this;
        }

        public string GetCorrelationId<TMessage>(TMessage message)
        {
            var type = typeof(TMessage);
            var id = "";
            if(CorrelationMethods.ContainsKey(type))
            {
                id = CorrelationMethods[type]( message );
            }
            return id;
        }

        public string GetRoutingKey<TMessage>(TMessage message)
        {
            var type = typeof(TMessage);
            var key = "";
            if (RoutingMethods.ContainsKey(type))
            {
                key = RoutingMethods[type](message);
            }
            return key;
        }

        protected BaseChannelDefinition()
        {
            Name = "default";
            RoutingMethods = new Dictionary<Type, Func<object, string>>();
            CorrelationMethods = new Dictionary<Type, Func<object, string>>();
        }
    }
}
