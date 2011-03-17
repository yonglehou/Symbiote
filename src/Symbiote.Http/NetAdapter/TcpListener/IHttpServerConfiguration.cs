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
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Symbiote.Http.NetAdapter.TcpListener
{
    public interface IHttpServerConfiguration
    {
        int AllowedPendingRequests { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string BaseUrl { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }
        string Host { get; set; }
        int Port { get; set; }
        List<Tuple<Type, Type>> RegisteredServices { get; set; }
        void UseDefaults();
        bool UseHttps { get; set; }
        string X509CertName { get; set; }
        StoreName X509StoreName { get; set; }
        StoreLocation X509StoreLocation { get; set; }
    }
}