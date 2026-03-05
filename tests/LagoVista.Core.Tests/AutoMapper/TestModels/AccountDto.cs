using LagoVista.Core.Attributes;
using System;


namespace LagoVista.Core.Tests.Mapping
{
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        [ModernKeyId("account-{id}", IdPath=nameof(AccountDto.Id), CreateIfMissing = true)]
        [EncryptionKey("AccountKey-{id}", IdProperty = nameof(AccountDto.Id), CreateIfMissing = true)]
        private sealed class AccountDto
        {
            public Guid Id { get; set; }

            public string EncryptedBalance { get; set; }
        }
    }
}
