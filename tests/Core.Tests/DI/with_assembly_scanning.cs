﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;

namespace Core.Tests.DI
{
    public interface AnInterfaceOf<T> : AnInterfaceOf
    {
        
    }

    public abstract class with_assembly_scanning
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Dependencies(x => x.Scan(s =>
                                                                              {
                                                                                  s.AssemblyContainingType<IAmAnInterface>();
                                                                                  s.AddAllTypesOf<IAmAnInterface>();
                                                                                  s.AddSingleImplementations();
                                                                              }));
                                        };

    }
}
