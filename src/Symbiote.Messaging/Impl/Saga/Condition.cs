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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Symbiote.Messaging.Impl.Saga
{
    [DebuggerDisplay("Transitions with guard: {GuardExpression}")]
    public class Condition<TActor>
        : ICondition<TActor>
    {
        public Expression<Predicate<TActor>> GuardExpression { get; set; }
        public Predicate<TActor> Guard { get; protected set; }
        public Dictionary<Type, IConditionalTransition<TActor>> Transitions { get; set; }
        public bool Exclusive { get; set; }
        
        public List<Type> Handles
        {
            get { return Transitions.Keys.ToList(); }
        }

        public bool IsValid(TActor instance)
        {
            return Guard( instance );
        }

        public ICondition<TActor> On<TMessage>( Func<TActor, TMessage, Action<IEnvelope>> processMessage )
        {
            var onMessage = new ConditionalTransition<TActor, TMessage>
                                {
                                    Process = processMessage,
                                    Guard = Guard,
                                    GuardExpression = GuardExpression
                                };
            Transitions.Add( typeof( TMessage ), onMessage );
            return this;
        }

        public ICondition<TActor> On<TMessage>( Func<TActor, Action<IEnvelope>> transition)
        {
            var onMessage = new ConditionalTransition<TActor, TMessage>
                                {
                                    Transition = transition,
                                    Guard = Guard,
                                    GuardExpression = GuardExpression
                                };
            Transitions.Add( typeof( TMessage ), onMessage );
            return this;
        }

        public ICondition<TActor> On<TMessage>( Func<TActor, TMessage, Action<IEnvelope>> processMessage, Func<TActor, Action<IEnvelope>> transition )
        {
            var onMessage = new ConditionalTransition<TActor, TMessage>
                                {
                                    Transition = transition,
                                    Process = processMessage,
                                    Guard = Guard,
                                    GuardExpression = GuardExpression
                                };
            Transitions.Add( typeof( TMessage ), onMessage );
            return this;
        }

        public Condition()
            : this( x => true )
        {
        }

        public Condition( Expression<Predicate<TActor>> guard )
        {
            GuardExpression = guard;
            Guard = guard.Compile();
            Transitions = new Dictionary<Type, IConditionalTransition<TActor>>();
        }
    }
}