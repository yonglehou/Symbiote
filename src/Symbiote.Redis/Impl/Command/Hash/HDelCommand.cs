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
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Hash
{
    public class HDelCommand
        : RedisCommand<bool>
    {
        protected const string DEL = "HDEL {0} {1}\r\n";
        protected string Key { get; set; }
        protected string Field { get; set; }

        public bool Delete( IConnection connection )
        {
            return connection.SendExpectSuccess( null, DEL.AsFormat( Key, Field ) );
        }

        public HDelCommand( string key, string field )
        {
            Key = key;
            Field = field;
            Command = Delete;
        }
    }
}