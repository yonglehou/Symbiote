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
using System.Text;

namespace Symbiote.Core.Concurrency
{
    public interface IEventLoop
    {
        /// <summary>
        /// Enqueue an action to take place on one of the event loop's threads
        /// </summary>
        void Enqueue( Action action );

        /// <summary>
        /// Indicates whether or not the loop is currently dequeueing and executing actions
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Starts the event loop with a specific number of threads to
        /// dequeue and execute queued actions
        /// </summary>
        void Start( int workers );

        /// <summary>
        /// Stops processing of enqueued actions
        /// </summary>
        void Stop();
    }
}
