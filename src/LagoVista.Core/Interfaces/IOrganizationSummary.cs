using LagoVista.Core.Models;

namespace LagoVista.Core.Interfaces
{
    public interface IOrganizationSummary
    {
        NormalizedId32 Id { get; set; }
        string Text { get; set; }
        string Name { get; set; }
        OrgNamespace? Namespace { get; set; }
        EntityHeader LightLogo { get; set; }
        EntityHeader DarkLogo { get; set; }
        LagoVistaIcon Icon { get; set; }
        string TagLine { get; set; }
        string DefaultTheme { get; set; }

        EntityHeader ToEntityHeader();
    }
}
