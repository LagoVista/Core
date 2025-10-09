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
