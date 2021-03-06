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

namespace Symbiote.Core.UnitOfWork
{
    public class EventSubscriptionToken
        : IDisposable
    {
        private readonly IEventListener _listener;
        private readonly IList<IEventListener> _listeners;

        public void Dispose()
        {
            // TODO : determine if "Contains" is really necessary here.  Probably just extra overhead...
            if ( _listener != null && _listeners.Contains( _listener ) )
                _listeners.Remove( _listener );
        }

        public EventSubscriptionToken( IEventListener listener, IList<IEventListener> listeners )
        {
            _listener = listener;
            _listeners = listeners;
        }
    }
}