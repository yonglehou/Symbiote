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
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Core.Reflection;
using Symbiote.Messaging.Extensions;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class ActorMessageDispatcher<TActor, TMessage>
        : IDispatchMessage<TActor, TMessage>
        where TActor : class
    {
        protected IEnumerable<Type> HandlesMessagesOf { get; set; }
        protected IAgency Agency { get; set; }
        protected IAgent<TActor> Agent { get; set; }
        protected IHandle<TActor, TMessage> Handler { get; set; }

        public Type ActorType
        {
            get { return typeof( TActor ); }
        }

        public bool CanHandle( object payload )
        {
            return payload.IsOfType<TMessage>();
        }

        public bool DispatchForActor
        {
            get { return true; }
        }

        public IEnumerable<Type> Handles
        {
            get
            {
                HandlesMessagesOf = HandlesMessagesOf ?? GetMessageChain().ToList();
                return HandlesMessagesOf;
            }
        }

        public void Dispatch( IEnvelope envelope )
        {
            var actor = Agent.GetActor( envelope.CorrelationId );
            var message = (TMessage)envelope.Message;
            Handler = Handler ?? Assimilate.GetInstanceOf<IHandle<TActor, TMessage>>();
            Handler.Handle( actor, message )( envelope );
            Agent.Memoize( actor );
        }

        private IEnumerable<Type> GetMessageChain()
        {
            yield return typeof( TMessage );
            var chain = Reflector.GetInheritanceChain( typeof( TMessage ) );
            if ( chain != null )
            {
                foreach( var type in chain )
                {
                    yield return type;
                }
            }
            yield break;
        }

        public ActorMessageDispatcher( IAgency agency )
        {
            Agency = agency;
            Agent = Agency.GetAgentFor<TActor>();
        }
    }
}