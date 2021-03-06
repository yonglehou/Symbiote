﻿// /* 
// Copyright 2008-2011 Jim Cowart & Alex Robson
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
using Symbiote.Core.Extensions;

namespace Symbiote.Core.UnitOfWork
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IList<IEventListener> _subscribers = new List<IEventListener>();
        private readonly IEventListenerManager _manager;

        public IDisposable Subscribe( IEventListener listener )
        {
            _subscribers.Add( listener );
            return new EventSubscriptionToken( listener, _subscribers );
        }

        public void PublishEvents( IEnumerable<IEvent> events )
        {
            events
                .ForEach( evnt =>
                              {
                                  _subscribers
                                      .ForEach( x => x.HandleEvent( evnt ) );
                                  if ( _manager != null )
                                  {
                                      _manager.PublishEvent( evnt );
                                  }
                              } );
        }

        public EventPublisher()
        {
        }

        public EventPublisher( IEventListenerManager manager )
        {
            _manager = manager;
        }
    }
}