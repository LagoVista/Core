using LagoVista.Core.Models;
using System.Collections.Generic;
using System.Globalization;

namespace LagoVista.Core.Interfaces
{
    public interface ILocalizationService
    {
        List<EntityHeader> GetSupportedCultures();

        CultureInfo GetCulture(string key);
    }
}
