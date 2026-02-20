using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace LagoVista.Core.Tests.Mapping
{
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        public class FakeSecureStorage : ISecureStorage
        {
            private readonly Dictionary<string, string> _storage = new Dictionary<string, string>();

            public Task<InvokeResult<string>> AddSecretAsync(EntityHeader org, string value)
            {
                var id = Guid.NewGuid().ToString();
                _storage.Add(id, value);
                return Task.FromResult(InvokeResult<string>.Create(id));
            }

            public Task<InvokeResult<string>> AddSecretAsync(EntityHeader org, string id, string value)
            {
                _storage.Add(id, value);
                return Task.FromResult(InvokeResult<string>.Create(id));
            }

            public Task<InvokeResult<string>> AddUserSecretAsync(EntityHeader user, string value)
            {
                var id = Guid.NewGuid().ToString();
                _storage.Add(id, value);
                return Task.FromResult(InvokeResult<string>.Create(id));
            }

            public Task<InvokeResult<string>> GetSecretAsync(EntityHeader org, string id, EntityHeader user)
            {
                if (_storage.TryGetValue(id, out var value))
                {
                    return Task.FromResult(InvokeResult<string>.Create(value));
                }
                else
                {
                    return Task.FromResult(InvokeResult<string>.FromError($"Secret with id {id} not found."));
                }
            }

            public Task<InvokeResult<string>> GetUserSecretAsync(EntityHeader user, string id)
            {
                if (_storage.TryGetValue(id, out var value))
                {
                    return Task.FromResult(InvokeResult<string>.Create(value));
                }
                else
                {
                    return Task.FromResult(InvokeResult<string>.FromError($"Secret with id {id} not found."));
                }
            }

            public Task<InvokeResult> RemoveSecretAsync(EntityHeader org, string id)
            {
                throw new NotImplementedException();
            }

            public Task<InvokeResult> RemoveUserSecretAsync(EntityHeader user, string id)
            {
                throw new NotImplementedException();
            }
        }
    }
}
