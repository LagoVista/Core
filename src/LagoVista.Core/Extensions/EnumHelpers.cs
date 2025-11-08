// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6ecf8b507d5bd84a31920c74c0e7d1827a51beeb35593a1a48f0c6302128cfdf
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Attributes;

namespace LagoVista.Core
{
    public static class EnumHelpers
    {
        public static string GetEnumLabel<TEnum>(this string value)
        {
            if (typeof(TEnum).GetTypeInfo().IsEnum)
            {
                var valueMember = typeof(TEnum).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == value.ToString()).FirstOrDefault();

                var enumValues = Enum.GetValues(typeof(TEnum));
                var len = enumValues.GetLength(0);
                for (var idx = 0; idx <len; idx++)
                {
                    var enumValue = enumValues.GetValue(idx);
                    var enumMember = typeof(TEnum).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == enumValue.ToString()).FirstOrDefault();
                    var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();
                    if (enumAttr != null)
                    {
                        if (enumAttr.Key == value)
                        {
                            var labelProperty = enumAttr.ResourceType.GetTypeInfo().GetDeclaredProperty(enumAttr.LabelResource);
                            return (string)labelProperty.GetValue(labelProperty.DeclaringType, null);
                        }
                    }
                }

                return value;
            }

            throw new Exception("GetEnumLabel only works with Enums.");
        }

        public static bool TryGetFromValue<TEnum>(this string value, out TEnum outValue) 
        {
            outValue = default(TEnum);
            if(String.IsNullOrEmpty(value))
            {
                return false;
            }
            
            if (typeof(TEnum).GetTypeInfo().IsEnum)
            {
                var valueMember = typeof(TEnum).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == value.ToString()).FirstOrDefault();

                var enumValues = Enum.GetValues(typeof(TEnum));
                var len = enumValues.GetLength(0);
                for (var idx = 0; idx < len; idx++)
                {
                    var enumValue = enumValues.GetValue(idx);
                    var enumMember = typeof(TEnum).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == enumValue.ToString()).FirstOrDefault();
                    var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();
                    if (enumAttr != null)
                    {
                        if (enumAttr.Key == value)
                        {
                            outValue = (TEnum)enumValue;
                            return true;
                        }
                    }
                }

                return false;
            }

            throw new Exception("GetEnumLabel only works with Enums.");
        }
    }
}
