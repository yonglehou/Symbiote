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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Hash
{
    public class HGetManyCommand<T>
        : RedisCommand<IEnumerable<T>>
    {
        protected const string HGET_MANY = "*{0}\r\n$5\r\nHMGET\r\n${1}\r\n{2}\r\n";
        protected const string RECORD = "${0}\r\n{1}\r\n";

        protected string Key { get; set; }
        protected IEnumerable<string> Values { get; set; }

        public IEnumerable<T> HGet( IConnection connection )
        {
            var stream = new MemoryStream();
            Values.ForEach( v =>
                                {
                                    var header = Encoding.UTF8.GetBytes(
                                        RECORD.AsFormat(
                                            v.Length,
                                            v ) );
                                    stream.Write( header, 0, header.Length );
                                } );

            var response = connection.SendExpectDataList( stream.ToArray(),
                                                          HGET_MANY.AsFormat( Values.Count() + 2, Key.Length, Key ) );
            return response.Select( item => Deserialize<T>( item ) ); //.ToList();
        }

        public HGetManyCommand( string key, IEnumerable<string> values )
        {
            Key = key;
            Values = values;
            Command = HGet;
        }
    }
}