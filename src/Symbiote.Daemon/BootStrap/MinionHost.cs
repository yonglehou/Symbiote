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

namespace Symbiote.Daemon.BootStrap
{
    public class MinionHost : MarshalByRefObject
    {
        public IMinion Minion { get; set; }

        public bool LoadMinion(Type type, IDictionary<string, object> startupData)
        {
            try
            {
                Minion = Activator.CreateInstance( type ) as IMinion;
                Minion.Start( startupData );
                return true;
            }
            catch ( Exception e )
            {
                return false;
            }
        }

        public MinionHost()
        {
        }
    }
}