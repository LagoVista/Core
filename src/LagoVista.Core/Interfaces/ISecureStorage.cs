using LagoVista.Core.Models;
using LagoVista.Core.Validation;
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
        Task<InvokeResult<string>> AddSecretAsync(EntityHeader org, string id, string value);

        /// <summary>
        /// Add a secret and generate and id
        /// </summary>
        /// <param name="value">Secret</param>
        /// <returns>Generated Id</returns>
        Task<InvokeResult<string>> AddSecretAsync(EntityHeader org, string value);

        /// <summary>
        /// Add a secret and generate and id
        /// </summary>
        /// <param name="value">Secret</param>
        /// <returns>Generated Id</returns>
        Task<InvokeResult<string>> AddUserSecretAsync(EntityHeader user, string value);


        /// <summary>
        /// Returns a Secret
        /// </summary>
        /// <param name="id">ID used for secret</param>
        /// <returns>plain text value for secret</returns>
        Task<InvokeResult<string>> GetSecretAsync(EntityHeader org, string id, EntityHeader user);


        /// <summary>
        /// Returns a Secret
        /// </summary>
        /// <param name="id">ID used for secret</param>
        /// <returns>plain text value for secret</returns>
        Task<InvokeResult<string>> GetUserSecretAsync(EntityHeader user, string id);

        /// <summary>
        /// Removes a secret
        /// </summary>
        /// <param name="id">ID of Secret to remove</param>
        /// <returns></returns>
        Task<InvokeResult> RemoveSecretAsync(EntityHeader org, string id);

        /// <summary>
        /// Removes a secret
        /// </summary>
        /// <param name="id">ID of Secret to remove</param>
        /// <returns></returns>
        Task<InvokeResult> RemoveUserSecretAsync(EntityHeader user, string id);

    }
}
