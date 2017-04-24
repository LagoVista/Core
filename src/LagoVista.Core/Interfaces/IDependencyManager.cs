using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Note we are not qualifying these with an interface or generic.   This object really
    /// doesn't know anything about who dependes on it, that's what this class is for.
    /// 
    /// The Implementation should implement a switch statement based on type that 
    /// will eventually resolve the dependencies.
    /// 
    /// Since we can't count on a storage implementation we have to manage our 
    /// own Foriegn Key concept, the implementation is really the "spec"
    /// 
    /// There is absolutely a "code-smell" here and we may want to refactor
    /// but right now this should work for V1.
    /// 
    /// KDW 4/24/2017
    /// 
    /// </summary>
    public interface IDependencyManager
    {
        Task<DependentObjectCheckResult> CheckForDependenciesAsync(object instance);

        Task RenameDependentObjectsAsync(object instance, string newName);
    }
}
