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
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Futures;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Channels
{
    public class NamedPipeChannel
        : IChannel
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public NamedPipeClientStream ClientStream { get; set; }
        public MessageOptimizedSerializer SerializationProvider { get; set; }

        public string Name { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new Envelope<TMessage>( message )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message )
                               };
            modifyEnvelope( envelope );
            var envelopeBuffer = SerializeEnvelope( envelope );
            ClientStream.Write( envelopeBuffer, 0, envelopeBuffer.Length );
            ClientStream.Flush();

            return Future.Of( GetReply<TReply> );
        }

        public void Send<TMessage>( TMessage message )
        {
            Send( message, x => { } );
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new Envelope<TMessage>( message )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message )
                               };
            modifyEnvelope( envelope );
            var envelopeBuffer = SerializeEnvelope( envelope );
            ClientStream.Write( envelopeBuffer, 0, envelopeBuffer.Length );
            ClientStream.Flush();
        }

        public TReply GetReply<TReply>()
        {
            var readBuffer = ClientStream.ReadToEnd( 1000 );
            var typeHeaderLength = BitConverter.ToInt32( readBuffer, 0 );
            var typeHeader = BitConverter.ToString( readBuffer, 4, typeHeaderLength );
            var type = Type.GetType( typeHeader );
            var envelopeType = typeof( Envelope<> ).MakeGenericType( type );
            var envelope = SerializationProvider.Deserialize( envelopeType,
                                                              readBuffer.Skip( 4 + typeHeaderLength ).ToArray() );
            return (envelope as IEnvelope<TReply>).Message;
        }

        public byte[] SerializeEnvelope<TMessage>( Envelope<TMessage> envelope )
        {
            var messageType = typeof( TMessage );
            var typeName = messageType.AssemblyQualifiedName;
            var typeBuffer = Encoding.UTF8.GetBytes( typeName );
            var headerLength = typeBuffer.Length;
            var messageBuffer = SerializationProvider.Serialize( envelope );
            int messageLength = messageBuffer.Length;
            using( var stream = new MemoryStream( 4 + headerLength + messageLength ) )
            {
                stream.Write( BitConverter.GetBytes( headerLength ), 0, 4 );
                stream.Write( typeBuffer, 0, headerLength );
                stream.Write( messageBuffer, 0, messageLength );
                return stream.ToArray();
            }
        }

        public void ConfigurePipe()
        {
            ClientStream = new NamedPipeClientStream(
                Definition.Name,
                Definition.Server,
                Definition.Direction,
                Definition.Options );
            ClientStream.ReadMode = Definition.Mode;
            ClientStream.Connect( Definition.ConnectionTimeout );
        }

        public NamedPipeChannel( NamedPipeChannelDefinition definition )
        {
            Definition = definition;
            Name = definition.Name;
            ConfigurePipe();
            SerializationProvider = new MessageOptimizedSerializer();
        }
    }
}