using LagoVista.Core.Attributes;
using System;


namespace LagoVista.Core.Tests.Mapping
{
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        [EncryptionKey("AccountKey-{id}", IdProperty = nameof(AccountDto.Id), CreateIfMissing = true)]
        private sealed class AccountDto
        {
            public Guid Id { get; set; }

            public string EncryptedBalance { get; set; }
        }
    }
}
