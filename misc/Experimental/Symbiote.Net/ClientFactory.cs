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

using System.Net.Sockets;

namespace Symbiote.Net
{
    public class ClientFactory
    {
        protected IHttpServerConfiguration Configuration { get; set; }

        public IHttpClient GetClient(TcpClient client)
        {
            if(Configuration.UseHttps)
            {
                return CreateSecureClient(client);
            }
            else
            {
                return CreateSimpleClient(client);
            }
        }

        protected IHttpClient CreateSecureClient(TcpClient client)
        {
            return new SecureHttpClient(
                client, 
                Configuration.X509StoreLocation, 
                Configuration.X509StoreName,
                Configuration.X509CertName);
        }

        protected IHttpClient CreateSimpleClient(TcpClient client)
        {
            return new SimpleHttpClient(client);
        }

        public ClientFactory(IHttpServerConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}