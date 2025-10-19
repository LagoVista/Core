// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1519e09b67039157106461f1ff8c73fde9769ef5f2f7b9a1b88056e9149fdb59
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IOrganizationSummary
    {
        string Id { get; set; }
        string Text { get; set; }
        string Name { get; set; }
        string Namespace { get; set; }
        EntityHeader LightLogo { get; set; }
        EntityHeader DarkLogo { get; set; }
        string Icon { get; set; }
        string TagLine { get; set; }
        string DefaultTheme { get; set; }

        EntityHeader ToEntityHeader();
    }
}
