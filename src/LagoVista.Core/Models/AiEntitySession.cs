using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class AiEntitySession
    {
        public EntityHeader Session { get; set; }
        public string SessionType { get; set; }
        public string SessionTypeKey { get; set; }

        public string CreationDate { get; set; }

        public EntityHeader CreatedBy { get; set; }
    }
}
