// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1a41b65a80970b46a230b8442cf2ef72ca40896c642c8bb715550a4ead03f278
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Since we are using a document based structure we don't have foreign keys. we still need that functionality.
    /// 
    /// All foriegn keys are represented with an EntityHeader, this contains both the Id and Name field (in a Text property) of the 
    /// primary 
    /// 
    /// Service to provide function to simulate FKeys in a document based structure.  Will make sure
    /// we don't delete any dependent objects as well as rename as applicable.  The relationships are setup
    /// with FKeyProperty attribute on the property that would typically be an FKey.  The NuvIoT build tools will extract all the properties
    /// and the DependencyManager project in AppServices provides implementation.
    /// 
    /// If the objects are named properly this will work automatically w/o FKeyProperty attribute.  that is to say if your primary object is
    /// [Planner]
    /// And there is a dependent object that references this let's say the object name is "Module" and it has a property called [Planner] of 
    /// type EntityHeader, the system will be smart enough to not let the delete happen as well as perform rename.
    /// 
    /// 
    /// </summary>
    public interface IDependencyManager
    {
        Task<DependentObjectCheckResult> CheckForDependenciesAsync(IIDEntity instance);

        Task RenameObjectAsync(EntityHeader changedBy, string changedObjectId, string changedObjectType, string newName);
        Task RenameDependentObjectsAsync(EntityHeader changedBy, string changedObjectId, string changedObjectType, string dependentObjectId, string dependentObjectType, string newName);
    }
}
