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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Http.Owin;
using Symbiote.Core.Impl.Serialization;

namespace Symbiote.Http.Impl.Adapter.Web
{
    public class HttpResponseAdapter : IResponseAdapter
    {
        public HttpListenerResponse Response { get; set; }
        
        public void Respond( string status, IDictionary<string, IList<string>> headers, IEnumerable<object> body )
        {
            var httpStatus = HttpStatus.Lookup[status];
            Response.StatusCode = httpStatus.Code;
            Response.StatusDescription = httpStatus.Description;

            headers.ForEach( x => Response.AddHeader( x.Key, DelimitedBuilder.Construct( x.Value, ";" )) );
            var count = body
                .Select( Serialize )
                .Where( x => x.Length > 0)
                .Sum( x =>
                {
                    var length = x.Length;
                    Response.OutputStream.Write( x, 0, length );
                    return length;
                });

            Response.ProtocolVersion = HttpVersion.Version11;
            Response.ContentLength64 = count;
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/plain";

            Response.Close();
        }

        public byte[] Serialize(object o)
        {
            if (o == null)
                return new byte[] {};

            if (o is byte[])
                return o as byte[];

            var source = o is string
                         ? o.ToString()
                         : o.ToJson( false );
            var bytes = Encoding.UTF8.GetBytes( source );
            return bytes;
        }

        public HttpResponseAdapter( HttpListenerContext context )
        {
            Response = context.Response;
        }
    }
}