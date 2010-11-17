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
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Config;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class RabbitChannel :
        IOpenChannel
        , IHaveChannelProxy
    {
        public string Name { get; set; }
        public IChannelProxy Proxy { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public ChannelDefinition Definition { get; set; }

        public void ExpectReply<TMessage, TReply>(TMessage message, Action<IEnvelope> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId,
                ReplyToKey = ConfigureResponseChannel<TReply>()
            };

            modifyEnvelope(envelope);
            dispatcher.ExpectResponse(envelope.MessageId.ToString(), onReply);
            envelope.ByteStream = Serializer.Serialize( envelope.Message );
            Proxy.Send(envelope);
        }

        public string ConfigureResponseChannel<TReply>()
        {
            var baseName = RabbitBroker.ResponseId;
            var messageType = typeof(TReply).Name;

            RabbitExtensions.AddRabbitChannel(null, x => x
                .AutoDelete()
                .Direct( baseName ));

            RabbitExtensions.AddRabbitQueue(null, x => x
                .AutoDelete()
                .ExchangeName( baseName )
                .QueueName("{0}.{1}.{2}".AsFormat(baseName, "response", messageType))
                .RoutingKeys(messageType)
                .NoAck()
                .StartSubscription());
            
            return messageType;
        }

        public void Send<TMessage>(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope(envelope);
            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            Proxy.Send(envelope);
        }

        public void Send<TMessage>(TMessage message)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            Proxy.Send(envelope);
        }

        public RabbitChannel(IChannelProxy proxy, IMessageSerializer serializer, ChannelDefinition definition)
        {
            Definition = definition;
            Proxy = proxy;
            Serializer = serializer;
        }
    }

    public class RabbitChannel<TMessage> :
        IChannel<TMessage>,
        IHaveChannelProxy
    {
        public string Name { get; set; }
        public IChannelProxy Proxy { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public ChannelDefinition<TMessage> Definition { get; set; }

        public void ExpectReply<TReply>(TMessage message, Action<IEnvelope> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId,
                ReplyToKey = ConfigureResponseChannel<TReply>()
            };

            modifyEnvelope(envelope);
            dispatcher.ExpectResponse(envelope.MessageId.ToString(), onReply);
            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            Proxy.Send(envelope);
        }

        public string ConfigureResponseChannel<TReply>()
        {
            var baseName = RabbitBroker.ResponseId;
            var messageType = typeof(TReply).Name;
            
            RabbitExtensions.AddRabbitChannel(null, x => x
                .AutoDelete()
                .Direct( baseName ));

            RabbitExtensions.AddRabbitQueue( null, x => x
                .AutoDelete()
                .QueueName("{0}.{1}.{2}".AsFormat(baseName, "response", messageType))
                .RoutingKeys(messageType)
                .NoAck()
                .StartSubscription());
            
            return messageType;
        }

        public void Send(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope(envelope);
            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            Proxy.Send(envelope);
        }

        public void Send(TMessage message)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            Proxy.Send(envelope);
        }

        public RabbitChannel(IChannelProxy proxy, IMessageSerializer serializer, ChannelDefinition<TMessage> definition)
        {
            Definition = definition;
            Proxy = proxy;
            Serializer = serializer;
        }
    }
}