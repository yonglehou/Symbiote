﻿using Machine.Specifications;

namespace SocketMQ.Tests
{
    public abstract class with_json_from_browser
    {
        protected static string json;
        private Establish context = () => json = @"{""To"":""Exchange"",""Body"":{""$type"":""SocketMQ.Tests.ElaborateMessageType, SocketMQ.Tests"",""Content"":""Test"",""Valid"":false,""Sender"":""test""}}";
    }
}