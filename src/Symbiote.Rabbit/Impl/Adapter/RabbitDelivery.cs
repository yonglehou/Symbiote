﻿/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using RabbitMQ.Client;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class RabbitDelivery
    {
        public string ConsumerTag { get; set; }
        public ulong DeliveryTag { get; set; }
        public bool Redelivered { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public IBasicProperties Properties { get; set; }
        public byte[] Body { get; set; }

        public RabbitDelivery(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            Redelivered = redelivered;
            Exchange = exchange;
            RoutingKey = routingKey;
            Properties = properties;
            Body = body;
        }
    }
}