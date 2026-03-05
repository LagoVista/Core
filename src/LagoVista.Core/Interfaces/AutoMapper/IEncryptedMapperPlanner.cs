using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces.AutoMapper
{
    public interface IEncryptedMapperPlanner
    {
        IEncryptedMapPlan<TDomain, TDto> GetOrBuildPlan<TDomain, TDto>()
            where TDomain : class
            where TDto : class;
    }

    public interface IEncryptedMapPlan<TDomain, TDto>
        where TDomain : class
        where TDto : class
    {
        bool CreateIfMissing { get; }

        /// <summary>
        /// Builds the legacy secret id (v1) using the plan's EncryptionKeyAttribute configuration.
        /// </summary>
        string BuildSecretId(TDto dto, EntityHeader org);

        IReadOnlyList<IEncryptedFieldPlan<TDomain, TDto>> Fields { get; }
    }

    public interface IEncryptedFieldPlan<TDomain, TDto>
        where TDomain : class
        where TDto : class
    {
        string CiphertextPropertyName { get; }
        string SaltPropertyName { get; }
        bool SkipIfEmpty { get; }

        Func<TDto, string> GetCiphertext { get; }
        Action<TDto, string> SetCiphertext { get; }

        Func<TDto, string> GetSalt { get; }

        Func<TDomain, string> GetPlaintext { get; }
        Action<TDomain, string> SetPlaintext { get; }
    }
}