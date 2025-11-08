// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f0fa22d35fae7f64587f1e58894fd6308c7fcae20c6a9a5694bf4cc1b78abd50
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Collections.Generic;
using System.Globalization;

namespace LagoVista.Core.Interfaces
{
    public interface ILocalizationService
    {
        List<EntityHeader> GetSupportedCultures();
        List<EnumDescription> GetCultureEnumOptions();
        CultureInfo GetCulture(string key);
    }
}
