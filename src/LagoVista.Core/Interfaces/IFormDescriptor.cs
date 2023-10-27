using System;
using System.Collections.Generic;
using System.Text;

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
}
