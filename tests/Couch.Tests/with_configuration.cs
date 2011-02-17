﻿using Machine.Specifications;
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl.Http;
using Symbiote.Core;
using Symbiote.Couch;
using Symbiote.StructureMapAdapter;

namespace Couch.Tests
{
    public abstract class with_configuration
    {
        protected static ICouchConfiguration configuration;
        private Establish context = () =>
                                        {
                                            Assimilate.Core<StructureMapAdapter>().Couch();
                                            configuration = new CouchConfiguration();
                                        };

        protected static void WireUpCommandMock(IHttpAction commandMock)
        {
            Assimilate.Dependencies( x => x.For<IHttpAction>().Use( commandMock ) );
        }
    }
}