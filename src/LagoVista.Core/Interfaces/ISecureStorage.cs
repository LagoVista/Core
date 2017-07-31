using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ISecureStorage
    {
        /// <summary>
        /// Add a secret with a previously generated id
        /// </summary>
        /// <param name="id">Id to store secret</param>
        /// <param name="value">Secret</param>
        /// <returns>Id as passed in</returns>
        Task<InvokeResult<string>> AddSecretAsync(string id, string value);

        /// <summary>
        /// Add a secret and generate and id
        /// </summary>
        /// <param name="value">Secret</param>
        /// <returns>Generated Id</returns>
        Task<InvokeResult<string>> AddSecretAsync(string value);

        /// <summary>
        /// Returns a Secret
        /// </summary>
        /// <param name="id">ID used for secret</param>
        /// <returns>plain text value for secret</returns>
        Task<InvokeResult<string>> GetSecretAsync(string id, EntityHeader user, EntityHeader org);

        /// <summary>
        /// Removes a secret
        /// </summary>
        /// <param name="id">ID of Secret to remove</param>
        /// <returns></returns>
        Task<InvokeResult> RemoveSecretAsync(string id);
    }
}
