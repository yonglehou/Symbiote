﻿using Machine.Specifications;
using Moq;
using Symbiote.Core;
using Symbiote.Couch.Impl.Http;
using It = Machine.Specifications.It;

namespace Couch.Tests.Commands
{
    public abstract class with_mock_http_action : with_configuration
    {
        protected static Mock<IHttpAction> mockAction;
        protected static IHttpAction action { get { return mockAction.Object; }}

        private Establish context = () =>
                                        {
                                            mockAction = new Mock<IHttpAction>();
                                            Assimilate.Dependencies( x => x.For<IHttpAction>().CreateWith( () => action ) );
                                        };
    }

    public class when_testing_blank_action : with_mock_http_action
    {
        It should_not_be_null = () => { mockAction.ShouldNotBeNull(); };
    }
}