using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    public class Email
    {
        public List<EmailAddress> To { get; set; }
        public List<EmailAddress> Cc { get; set; }
        public EmailAddress From {get; set;}
        public EmailAddress ReplyTo { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }

        public string TrackingLink { get; set; }
     
        public EntityHeader Template { get; set; }
    }

    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
