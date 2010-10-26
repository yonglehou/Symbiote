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

using System.Collections.Generic;

namespace Symbiote.Messaging.Impl.Channels
{
    public interface IChannelManager
    {
        void AddDefinition(IChannelDefinition definition);
        IChannel<TMessage> GetChannelFor<TMessage>();
        IEnumerable<IChannel<TMessage>> GetChannelsFor<TMessage>();
        IChannel<TMessage> GetChannelFor<TMessage>(string channelName);

        bool HasChannelFor<TMessage>();
        bool HasChannelFor<TMessage>(string channelName);
    }
}