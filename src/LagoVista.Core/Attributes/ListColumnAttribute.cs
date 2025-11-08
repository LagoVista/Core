// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1d6b6cdc305cf0a36b9ddce1ffbb175dd765cc36f49bac2bdf31cd8c1c33c08a
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ListColumnAttribute : Attribute
    {
        private string _headerResource;
        private string _helpResource;
        private int? _ordinal;
        private bool _visible;
        private bool _sortable;
        private TextAlignment _alignment;
        private string _formatString;
        private Type _resourceType;


        public enum TextAlignment
        {
            Left,
            Center,
            Right
        }

        public ListColumnAttribute(String HeaderResource = "", String HelpResources = "", int Ordinal = -1, bool Visible = true, bool Sortable = false, String FormatString = "", TextAlignment Alignment = TextAlignment.Left, Type ResourceType = null)
        {
            _headerResource = HeaderResource;
            _helpResource = HelpResources;
            _ordinal = Ordinal >= 0 ? Ordinal : (int?)null;
            _visible = Visible;
            _sortable = Sortable;
            _alignment = Alignment;
            _formatString = FormatString;
            _resourceType = ResourceType;
        }

        public String HelpResource { get { return _helpResource; } }
        public String HeaderResource { get { return _headerResource; } }
        public int? Ordinal { get { return _ordinal; } }
        public bool Visible { get { return _visible; } }
        public bool Sortable { get { return _sortable; } }
        public TextAlignment Alignment { get { return _alignment; } }
        public String FormatString { get { return _formatString; } }

        public Type ResourceType { get { return _resourceType; } }
    }
}
