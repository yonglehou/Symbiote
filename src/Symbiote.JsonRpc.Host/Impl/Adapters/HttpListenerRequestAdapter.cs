﻿using System;
using System.IO;
using System.Net;

namespace Symbiote.JsonRpc.Host.Impl.Adapters
{
    public class HttpListenerRequestAdapter : IHttpRequestAdapter
    {
        protected HttpListenerRequest Request { get; set; }

        public Uri Url
        {
            get { return Request.Url; }
        }

        public Stream InputStream
        {
            get { return Request.InputStream; }
        }

        public HttpListenerRequestAdapter(HttpListenerRequest request)
        {
            Request = request;
        }
    }
}