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
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbMapRedReq" )]
    public class RunMapReduce : RiakCommand<RunMapReduce, MapReduceResult>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "request" )]
        public byte[] Request { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "content_type" )]
        public byte[] ContentType { get; set; }

        public RunMapReduce() {}

        public RunMapReduce( string request, string contentType )
        {
            Request = request.ToBytes();
            ContentType = contentType.ToBytes();
        }
    }
}