using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.MessageQueue.TestSupport
{
    public class PublisherBaseMessage
    {
        public string Id { get; set; }
    }

    public class PublisherDerivedMessage : PublisherBaseMessage
    {
        public string Name { get; set; }
    }

    public class PublisherOtherMessage
    {
        public string Id { get; set; }
    }
}
