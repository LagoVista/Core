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

    public interface IFormDescriptorBottom
    {
        List<string> GetFormFieldsBottom();
    }

    public interface IFormDescriptorTabs
    {
        List<string> GetFormFieldsTabs();
    }

    public interface IFormDescriptorAdvanced
    {
        List<string> GetAdvancedFields();
    }

    public interface IFormDescriptorAdvancedCol2
    {
        List<string> GetAdvancedFieldsCol2();
    }

    public interface IFormDescriptorInlineFields
    {
        List<string> GetInlineFields();
    }

    public interface IFormMobileFields
    {
        List<string> GetMobileFields();
    }

    public interface IFormDescriptorSimple
    {
        List<string> GetSimpleFields();
    }

    public interface IFormDescriptorQuickCreate
    {
        List<string> GetQuickCreateFields();
    }

    public interface IFormAdditionalActions
    {
        List<FormAdditionalAction> GetAdditionalActions();
    }
}
