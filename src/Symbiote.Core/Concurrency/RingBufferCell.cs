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
using System.Threading;

namespace Symbiote.Core.Concurrency
{
    public class RingBufferCell
        : IDisposable
    {
        public volatile int LastWriter;
        public volatile object Value;
        public RingBuffer Ring { get; set; }
        public RingBufferCell Next { get; set; }
        public bool Tail { get; set; }
        public bool Disposed { get; set; }
        public object Lock { get; set; }

        public void Dispose()
        {
            Disposed = true;
            Next = null;
            Ring = null;
            Value = null;
        }

        public static RingBufferCell Build( RingBuffer ring, int count )
        {
            var head = new RingBufferCell( ring );
            var last = head;
            while ( --count > 0 )
            {
                var cell = new RingBufferCell( ring );
                last.Next = cell;
                last = cell;
            }
            last.Next = head;
            last.Tail = true;
            return head;
        }

        public bool Ready( int index )
        {
            return Ring.PreviousStepLookup[index] == LastWriter;
        }

        public RingBufferCell Transform( int index, Func<object, object> transform )
        {
            while ( !Ready( index ) )
                Thread.Sleep( 1 );
            lock( Lock )
            {
                var value = transform( Value );
                Value = value;
                LastWriter = index;
            }
            return Next;
        }

        public RingBufferCell( RingBuffer ring )
        {
            Ring = ring;
            LastWriter = 0;
            Lock = new object();
        }
    }
}