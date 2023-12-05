using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface IFormDescriptor
    {
        List<string> GetFormFields();
    }

    public interface IFormDescriptorCol2
    {
        List<string> GetFormFieldsCol2();
    }

    public interface IFormDescriptorAdvanced
    {
        List<string> GetAdvancedFields();
    }

    public interface IFormDescriptorSimple
    {
        List<string> GetSimpleFields();
    }

    public interface IFormAdditionalActions
    {
        List<FormAdditionalAction> GetAdditionalActions();
    }
}
