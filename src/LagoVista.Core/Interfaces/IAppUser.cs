using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IAppUser : IIDEntity, IAuditableEntity
    {
        string UserName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        bool EmailConfirmed { get; set; }        
        string PhoneNumber { get; set; }
        bool PhoneNumberConfirmed { get; set; }
        IEntityHeader CurrentAccount { get; set; }
        List<IEntityHeader> Accounts { get; set; }
        IEntityHeader CurrentAccountRoles { get; }
    }
}