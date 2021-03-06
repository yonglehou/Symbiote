﻿    // /* 
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
using Symbiote.Core.UnitOfWork;

namespace Symbiote.Core.Actor
{
    public class DefaultActorFactory<TActor>
        : IActorFactory<TActor>
        where TActor : class
    {
        protected IKeyAccessor KeyAccessor { get; set; }

        public TActor CreateInstance<TKey>( TKey id )
        {
            var actor = Assimilate.GetInstanceOf<TActor>();
            KeyAccessor.SetId( actor, id );
            return actor;
        }

        public TActor CreateInstanceFrom<TKey>( TKey id, object command )
        {
            var actor = Assimilate.GetInstanceOf<TActor>();
            KeyAccessor.SetId( actor, id );
            return actor;
        }

        public DefaultActorFactory( IKeyAccessor keyAccessor )
        {
            KeyAccessor = keyAccessor;
        }
    }
}