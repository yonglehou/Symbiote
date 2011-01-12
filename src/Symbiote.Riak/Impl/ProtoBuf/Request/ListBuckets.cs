﻿using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [DataContract]
    public class ListBuckets : RiakCommand<ListBuckets, BucketList>
    {
    }
}