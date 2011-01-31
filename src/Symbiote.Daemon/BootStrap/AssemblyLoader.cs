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
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Core.Saga;
using Symbiote.Messaging;

namespace Symbiote.Daemon.BootStrap
{
    public class AssemblyLoader
    {

    }

    public class DaemonApplication
    {
        public string Name { get; set; }
        public AppDomain DomainHandle { get; set; }
        public bool Running { get; set; }
        public bool Starting { get; set; }
        public bool Stopping { get; set; }

        public void StartUp()
        {
            
        }

        public void ShutItDown()
        {
            
        }
    }

    public class DaemonApplicationKeyAccessor
        : IKeyAccessor<DaemonApplication>
    {
        public string GetId( DaemonApplication actor )
        {
            return actor.Name;
        }

        public void SetId<TKey>( DaemonApplication actor, TKey id )
        {
            actor.Name = id.ToString();
        }
    }

    public class ApplicationSaga
        : Saga<DaemonApplication>
    {
        public override Action<StateMachine<DaemonApplication>> Setup()
        {
            return x =>
                       {
                           x.When( a => a.Running )
                               .On<ApplicationDeleted>( ( a, h ) => a.ShutItDown() )
                               .On<ApplicationChanged>( ( a, h ) =>
                                                            {
                                                                a.ShutItDown();
                                                                a.StartUp();
                                                            } );
                           x.When( a => !a.Running )
                               .On<ApplicationChanged>( ( a, h ) => a.StartUp() )
                               .On<NewApplication>( ( a, h ) => a.StartUp() );
                       };
        }

        public ApplicationSaga( StateMachine<DaemonApplication> stateMachine ) : base( stateMachine )
        {
        }
    }

    public class ApplicationBootStapper
    {
        
    }
}