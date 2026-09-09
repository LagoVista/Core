using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models.UIMetaData
{
    public class FormSection
    {
        public string Key { get; set; }

        public Type ResourceType { get; set; }

        public string TitleResource { get; set; }

        public List<string> Fields { get; set; }
    }

    public class FormSectionResponse
    {
        public string Key { get; set; }

        public string Title { get; set; }

        public List<string> Fields { get; set; }
    }
}
