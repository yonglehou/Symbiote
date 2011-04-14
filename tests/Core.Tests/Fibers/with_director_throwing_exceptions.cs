﻿using System;
using System.Collections.Generic;
using Machine.Specifications;
using Symbiote.Core.Fibers;

namespace Core.Tests.Fibers
{
    public abstract class with_director_throwing_exceptions
    {
        public static Director<int> director;

        public static Dictionary<string, List<int>> Values;

        private Establish context = () =>
            {
                Values = new Dictionary<string, List<int>>()
                    {
                        {"", new List<int>()},
                        {"one", new List<int>()},
                        {"two", new List<int>()},
                        {"three", new List<int>()},
                    };

                director = new Director<int>( ( id, m ) =>
                    {
                        if( m % 3 == 0 )
                            throw new Exception( "AHHH!" );
                        else
                            Values[id].Add( m );
                    } );
            };
    }
}