using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ICoreEmailServices
    {
        Task<InvokeResult> SendToAppUserAsync(string appuUserId, string subject, string body);
        Task<InvokeResult> SendAsync(string email, string subject, string body, bool hasFullEmail = false, string appName = "", string appLogo = "");
        Task<InvokeResult<string>> SendAsync(Email email, EntityHeader org, EntityHeader user);
    }
}
