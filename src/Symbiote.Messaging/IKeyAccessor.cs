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

namespace Symbiote.Messaging
{
    public interface IKeyAccessor
    {
        string GetId<TActor>(TActor actor) where TActor : class;
        void SetId<TActor, TKey>(TActor actor, TKey id) where TActor : class;
    }

    public interface IKeyAccessor<in TActor>
        where TActor : class
    {
        string GetId(TActor actor);
        void SetId<TKey>(TActor actor, TKey id);
    }

    public class KeyAccessAdapter<T>
        : IKeyAccessor
        where T : class
    {
        public IKeyAccessor<T> Accessor { get; set; }

        public string GetId<TActor>(TActor actor) where TActor : class
        {
            if (typeof (T).IsAssignableFrom(typeof (TActor)))
            {
                return Accessor.GetId(actor as T);
            }
            throw new InvalidCastException("Key accessor cannot access an actor of {0} as type {1}".AsFormat(typeof(TActor), typeof(T)));
        }

        public void SetId<TActor, TKey>(TActor actor, TKey id) where TActor : class
        {
            if (typeof(T).IsAssignableFrom(typeof(TActor)))
            {
                Accessor.SetId(actor as T, id);
            }
            else
            {
                throw new InvalidCastException("Key accessor cannot access an actor of {0} as type {1}".AsFormat(typeof(TActor), typeof(T)));
            }
        }

        public KeyAccessAdapter(IKeyAccessor<T> accessor)
        {
            Accessor = accessor;
        }
    }
}