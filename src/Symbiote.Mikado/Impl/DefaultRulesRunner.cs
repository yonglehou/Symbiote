﻿/* 
Copyright 2008-2010 Jim Cowart

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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Symbiote.Core.Reflection;

namespace Symbiote.Mikado.Impl
{
    public class DefaultRulesRunner : IRunRules
    {
        private readonly List<IListenToBrokenRules> _observers = new List<IListenToBrokenRules>();

        public IRulesIndex RuleIndex { get; set; }

        public DefaultRulesRunner( IRulesIndex rulesIndex )
        {
            RuleIndex = rulesIndex;
        }

        public virtual void ApplyRules( object target )
        {
            if( RuleIndex.Rules.ContainsKey( target.GetType() ) )
            {
                var rulesToRun = new List<IRule>();
                RuleIndex
                    .Rules[target.GetType()]
                    .ToList()
                    .ForEach(rulesToRun.Add);
                rulesToRun.ForEach(rule => rule.ApplyRule(target, x => _observers.ForEach(a => a.OnBrokenRule(x))));
            }
            var targetProps = Reflector.GetProperties( target.GetType() )
                                       .Where( x => !x.PropertyType.IsPrimitive &&
                                                     x.PropertyType != typeof(string) &&
                                                     x.PropertyType != typeof(Guid))
                                       .ToList(); // No deferred exec - we're traversing this below 2x

            foreach(var prop in targetProps)
            {
                var instanceValue = Reflector.ReadMember(target, prop.Name);
                if (instanceValue != null)
                {
                    if (!prop.PropertyType.IsEnumerable())
                    {
                        ApplyRules(instanceValue);
                    }
                    else
                    {
                        var enumerable = instanceValue as IEnumerable;
                        if (enumerable != null)
                        {
                            foreach (var item in enumerable)
                            {
                                ApplyRules(item);
                            }
                        }
                    }
                }   
            }
        }

        public virtual IDisposable Subscribe(IListenToBrokenRules observer)
        {
            _observers.Add(observer);
            return new RuleUnsubscriber(_observers, observer);
        }
    }
}
