﻿using System;
using System.Collections.Generic;

namespace Mikado.Tests.Domain.Model
{
    public class Person : IPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public IList<PersonAddress> Addresses { get; set; }

        public Person()
        {
            Id = new Guid();    
            Addresses = new List<PersonAddress>();
        }
    }

    public class PersonAddress : IAddress
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
