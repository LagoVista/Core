using LagoVista.Core.Attributes;
using System;


namespace LagoVista.Core.Tests.Mapping
{
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        private sealed class Account
        {
            public string Id { get; set; }

            [EncryptedField(nameof(AccountDto.EncryptedBalance), SaltProperty = nameof(AccountDto.Id), SkipIfEmpty = true)]
            public double Balance { get; set; }
        }
    }
}
