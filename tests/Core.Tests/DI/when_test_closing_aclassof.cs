﻿using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI
{
    public class when_test_closing_aclassof
    {
        static bool closes;
        static bool open;

        private Because of = () => 
                                 { 
                                     open = typeof( AClassOf<> ).IsOpenGeneric();
                                     closes = typeof( ClosedClass ).Closes( typeof( AClassOf<> ) );
                                 };

        private It should_open_close = () => open.ShouldBeTrue();
        private It should_close = () => closes.ShouldBeTrue();
    }
}