using System.Collections.Generic;
using System.Net.Http.Headers;

namespace LagoVista.Core.Models
{
    public class Email
    {
        public List<EmailAddress> To { get; set; }
        public List<EmailAddress> Cc { get; set; }
        public EmailAddress From { get; set; }
        public EmailAddress ReplyTo { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }

        public string TrackingLink { get; set; }

        public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();

        public EntityHeader Template { get; set; }
    }

    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class EmailAttachment 
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Base64Content { get; set; }

    }

}
