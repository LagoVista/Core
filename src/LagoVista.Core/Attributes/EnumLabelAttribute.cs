// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a894ca5f694e6139019e8734bdcb391de97e7f7b3eb18635d6c1d925ec6e9766
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Attributes
{

    public class EnumLabelAttribute : Attribute
    {
        private readonly Type _resourceType;
        private readonly String _labelResource;
        private readonly String _key;
        private readonly String _helpResource;
        private readonly bool _isActive;
        private readonly int _sortOrder;

        public EnumLabelAttribute(String Key, String LabelResource, Type ResourceType, String HelpResource = "", bool IsActive = true, int SortOrder = -1)
        {
            _labelResource = LabelResource;
            _key = Key;
            _resourceType = ResourceType;
            _helpResource = HelpResource;
            _isActive = IsActive;
            _sortOrder = SortOrder;
        }

        public String LabelResource { get { return _labelResource; } }
        public String Key { get { return _key; } }
        public String HelpResource { get { return _helpResource; } }
        public Type ResourceType { get { return _resourceType; } } 
        public bool IsActive { get => _isActive; }
        public int SortOrder { get => _sortOrder; }
    }
}
