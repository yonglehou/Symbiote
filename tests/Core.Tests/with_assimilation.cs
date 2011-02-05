﻿using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;

namespace Core.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>();
                                        };
    }
}