using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;


namespace LagoVista.Core.Tests.Mapping
{
    public sealed class Account
    {
        public string Id { get; set; }

        [EncryptedField(nameof(AccountDto.EncryptedBalance), SaltProperty = nameof(AccountDto.Id), SkipIfEmpty = true)]
        public double Balance { get; set; }
    }

    [ModernKeyId("account-{id}", IdPath = nameof(AccountDto.Id), CreateIfMissing = true)]
    [EncryptionKey("AccountKey-{id}", IdProperty = nameof(AccountDto.Id), CreateIfMissing = true)]
    public sealed class AccountDto
    {
        public Guid Id { get; set; }

        public string EncryptedBalance { get; set; }
    }



    public sealed class PlainSource
    {
        public string name { get; set; }
        public string EXTERNALPROVIDERID { get; set; }
        public string ShouldNotCopy { get; set; }
    }

    public sealed class PlainEntityHeaderSource
    {
        [MapTo("Id")]
        [IgnoreOnMapTo]
        public EntityHeader<PlainSource> Source { get; set; }
    }

    public sealed class PlainEntityHeaderDestination
    {
        public string Id { get; set; }
    }

}
