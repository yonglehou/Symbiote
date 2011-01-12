﻿using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbGetBucketReq" )]
    public class GetBucketProperties : RiakCommand<GetBucketProperties, BucketProperties>
    {
        [DataMember( Order = 1, Name = "bucket", IsRequired = true )]
        public byte[] Bucket { get; set; }

        public GetBucketProperties() {}

        public GetBucketProperties( string bucket )
        {
            Bucket = bucket.ToBytes();
        }
    }
}