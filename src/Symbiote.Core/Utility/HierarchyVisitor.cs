﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;

namespace Symbiote.Core.Utility
{
    public class HierarchyVisitor
        : IObservable<Tuple<object, string, object>>
    {
        protected ConcurrentBag<IObserver<Tuple<object, string, object>>> Watchers { get; set; }
        protected Predicate<object> MatchIdentifier { get; set; }

        public void Visit(object instance)
        {
            if(MatchIdentifier(instance))
                NotifyWatchers(null, null, instance);

            Crawl(instance);
        }

        protected void Crawl(object instance)
        {
            Reflector
                .GetProperties(instance.GetType())
                .ForEach(x => Visit(instance, x.Name, Reflector.ReadMember(instance, x.Name)));
        }

        protected void Visit(object parent, string member, object value)
        {
            if(value != null)
            {
                if(value.GetType().GetInterface("IEnumerable") != null)
                {
                    var enumerator = (value as IEnumerable).GetEnumerator();
                    do
                    {
                        if(enumerator.Current != null)
                            Visit(parent, member, enumerator.Current);
                    } while (enumerator.MoveNext());
                }
                else
                {
                    if (MatchIdentifier(value))
                        NotifyWatchers(parent, member, value);
                    Crawl(value);
                }
            }
        }

        protected void NotifyWatchers(object parent, string member, object instance)
        {
            Watchers.AsParallel().ForAll(x => x.OnNext(Tuple.Create(parent, member, instance)));
        }

        public IDisposable Subscribe(IObserver<Tuple<object, string, object>> observer)
        {
            var disposable = observer as IDisposable;
            Watchers.Add(observer);
            return disposable;
        }

        public HierarchyVisitor(Predicate<object> notifyOnMatches)
        {
            Watchers = new ConcurrentBag<IObserver<Tuple<object, string, object>>>();
        }
    }
}
