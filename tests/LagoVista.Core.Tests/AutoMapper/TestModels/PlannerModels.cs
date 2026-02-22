using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class PlannerModels
    {
        public class SimpleEncrypted
        {
            public string Id { get; set; }

            [EncryptedField("EncryptedValue")]
            public decimal EncryptedValue { get; set; }
        }

        [EncryptionKey("Id")]
        public class SimpleEncryptedDTO
        {
            public string Id { get; set; }

            public string EncryptedValue { get; set; }
        }

    }
}
