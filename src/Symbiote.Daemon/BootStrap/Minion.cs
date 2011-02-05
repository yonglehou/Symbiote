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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.BootStrap
{
    public class Minion
    {
        public string Name { get; set; }
        public AppDomain DomainHandle { get; set; }
        public AppDomainSetup Setup { get; set; }
        public Evidence MinionEvidence { get; set; }
        public IMinion Instance { get; set; }
        public string DaemonDisplayName { get; set; }
        public string MinionPath { get; set; }
        public bool Running { get; set; }
        public bool Starting { get; set; }
        public bool Stopping { get; set; }
        public object MinionLock { get; set; }

        public void StartUp()
        {
            lock(MinionLock)
            if(!Starting && !Running)
            {
                try
                {
                    Starting = true;
                    DomainHandle = AppDomain.CreateDomain(Setup.ApplicationName, AppDomain.CurrentDomain.Evidence, Setup);
                    var locator =
                        (MinionLocator)
                        DomainHandle.CreateInstanceFromAndUnwrap( "Symbiote.Daemon.dll", typeof( MinionLocator ).FullName );
                    var host = (IMinion) locator.GetMinionHost(MinionPath);
                    host.Start( null );
                    Running = true;
                    Starting = false;
                }
                catch ( Exception e )
                {
                    "An error occurred attempting to start the minion at {0}. \r\n\t {1}"
                        .ToError<IDaemon>(MinionPath, e);
                    Running = false;
                    Starting = false;
                }
            }
        }

        public void ShutItDown()
        {
            AppDomain.Unload( DomainHandle );
        }

        public Minion( string path )
        {
            MinionPath = Path.GetFullPath( path );
            Setup = new AppDomainSetup();
            Setup.LoaderOptimization = LoaderOptimization.SingleDomain;
            Setup.ShadowCopyFiles = "true";
            Setup.ShadowCopyDirectories = MinionPath;
            Setup.CachePath = @"c:\shadows";
            Setup.ApplicationName = MinionPath.Split( Path.DirectorySeparatorChar ).Last();
            Setup.ApplicationBase = MinionPath;
            Setup.PrivateBinPath = MinionPath;
            MinionEvidence = AppDomain.CurrentDomain.Evidence;
            MinionLock = new object();
        }
    }
}