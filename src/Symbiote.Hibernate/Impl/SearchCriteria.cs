// /* 
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
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Symbiote.Hibernate
{
    public class SearchCriteria<T> : ISearchCriteria<T>
    {
        private readonly IList<Expression<Func<T, bool>>> _list;
        private Queue<Tuple<string, SortOrder>> _orderClause;
        private int? _pageNumber;
        private int? _pageSize;

        public ISearchCriteria<T> Add( Expression<Func<T, bool>> expression )
        {
            _list.Add( expression );
            return this;
        }

        public int? PageNumber
        {
            get { return _pageNumber; }
        }

        public int? PageSize
        {
            get { return _pageSize; }
        }

        public IEnumerable<Expression<Func<T, bool>>> All
        {
            get { return _list; }
        }

        public IEnumerable<Tuple<string, SortOrder>> Order
        {
            get { return _orderClause.ToArray(); }
        }

        public ISearchCriteria<T> PageBy( int pageNumber, int pageSize )
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            return this;
        }

        public ISearchCriteria<T> OrderBy<TProperty>( Expression<Func<T, TProperty>> orderBy, SortOrder order )
        {
            _orderClause.Enqueue( Tuple.Create( orderBy.GetMemberName(), order ) );
            return this;
        }

        public SearchCriteria()
        {
            _list = new List<Expression<Func<T, bool>>>();
            _orderClause = new Queue<Tuple<string, SortOrder>>();
        }
    }
}