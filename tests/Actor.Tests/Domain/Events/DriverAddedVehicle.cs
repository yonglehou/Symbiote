﻿using Symbiote.Actor;
using Symbiote.Messaging.Impl.Monitor;

namespace Actor.Tests.Domain.Events
{
    public class DriverAddedVehicle
        : ActorEvent
    {
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        public DriverAddedVehicle() {}

        public DriverAddedVehicle( string vin, string make, string model, int year )
        {
            Vin = vin;
            Make = make;
            Model = model;
            Year = year;
        }
    }
}