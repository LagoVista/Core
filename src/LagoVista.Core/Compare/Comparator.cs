using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;

namespace LagoVista.Core.Compare
{
    public static class Comparator
    {
        public const string PROPERTY_NAME = "[PROPERTY_NAME]";
        public const string OLD_VALUE = "[OLD_VALUE]";
        public const string NEW_VALUE = "[NEW_VALUE]";

        public static AuditTrail Compare<TEntity>(TEntity original, TEntity changed, EntityHeader user) where TEntity : class, IIDEntity
        {
            if (user == null || user.IsEmpty())
            {
                throw new InvalidOperationException("Must provide a valid user object when comparing two entities");
            }

            if (original == null && changed != null)
            {
                throw new InvalidOperationException("Can not compare original null object to not-null changed object");
            }

            if (original != null && changed == null)
            {
                throw new InvalidOperationException("Can not compare original not-null object to null changed object");
            }

            if (original == null && changed == null)
            {
                throw new InvalidOperationException("Can not compare, both original and compare objects are null");
            }

            if (original.GetType() != changed.GetType())
            {
                throw new InvalidOperationException($"Objects to be compared must be the same type, Object 1 => {original.GetType().FullName} == {changed.GetType().FullName}");
            }

            if (original.Id != changed.Id)
            {
                throw new InvalidOperationException($"Can not change, or compare two objects with different id's, original id => {original.Id} changed id => {changed.Id} record type => {typeof(TEntity).Name}");
            }

            var auditTrail = new AuditTrail();
            auditTrail.Changes = new List<Change>();
            auditTrail.User = user;
            auditTrail.Id = original.Id;
            auditTrail.EntityName = typeof(TEntity).Name;
            auditTrail.DateStamp = DateTime.Now.ToJSONString();

            var properties = original.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var prop in properties)
            {
                var propertyDisplay = prop.Name;
                var attr = prop.GetCustomAttributes(typeof(FormFieldAttribute), true).OfType<FormFieldAttribute>().FirstOrDefault(); ;
                if (attr != null && attr.ResourceType != null && !String.IsNullOrEmpty(attr.LabelDisplayResource))
                {
                    var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelDisplayResource);
                    propertyDisplay = (labelProperty != null) ? (string)labelProperty.GetValue(labelProperty.DeclaringType, null) : (string)null;
                }


                var oldValue = prop.GetValue(original);
                var newValue = prop.GetValue(changed);
                if (prop.PropertyType == typeof(string))
                {
                    var strOldValue = oldValue as String;
                    var strNewValue = newValue as String;
                    if (strOldValue != strNewValue)
                    {
                        if (!String.IsNullOrEmpty(strOldValue) || !String.IsNullOrEmpty(strNewValue))
                        {
                            if (String.IsNullOrEmpty(strOldValue))
                            {
                                auditTrail.Changes.Add(new Change()
                                {
                                    Name = propertyDisplay,
                                    OldValue = ComparatorResources.EmptyValue,
                                    NewValue = strNewValue,
                                    Message = ComparatorResources.Property_Empty_To_Value.Replace(PROPERTY_NAME, propertyDisplay).Replace(NEW_VALUE, strNewValue)
                                });
                            }
                            else if (String.IsNullOrEmpty(strNewValue))
                            {
                                auditTrail.Changes.Add(new Change()
                                {
                                    Name = propertyDisplay,
                                    NewValue = ComparatorResources.EmptyValue,
                                    OldValue = strOldValue,
                                    Message = ComparatorResources.Property_Value_To_Empty.Replace(PROPERTY_NAME, propertyDisplay).Replace(OLD_VALUE, strOldValue)
                                });
                            }
                            else
                            {
                                auditTrail.Changes.Add(new Change()
                                {
                                    Name = propertyDisplay,
                                    OldValue = strOldValue,
                                    NewValue = strNewValue,
                                    Message = ComparatorResources.PropertyValueChanged.Replace(PROPERTY_NAME, propertyDisplay).Replace(OLD_VALUE, strOldValue).Replace(NEW_VALUE, strNewValue)
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (oldValue == null && newValue != null)
                    {
                        auditTrail.Changes.Add(new Change()
                        {
                            Name = propertyDisplay,
                            OldValue = ComparatorResources.EmptyValue,
                            NewValue = newValue.ToString(),
                            Message = ComparatorResources.Property_Empty_To_Value.Replace(PROPERTY_NAME, propertyDisplay).Replace(NEW_VALUE, newValue.ToString())
                        });
                    }
                    else if (newValue == null && oldValue != null)
                    {
                        auditTrail.Changes.Add(new Change()
                        {
                            Name = propertyDisplay,
                            NewValue = ComparatorResources.EmptyValue,
                            OldValue = oldValue.ToString(),
                            Message = ComparatorResources.Property_Value_To_Empty.Replace(PROPERTY_NAME, propertyDisplay).Replace(OLD_VALUE, oldValue.ToString())
                        });
                    }
                    else if (newValue != null && oldValue != null && !oldValue.Equals(newValue))
                    {
                        auditTrail.Changes.Add(new Change()
                        {
                            Name = propertyDisplay,
                            OldValue = oldValue.ToString(),
                            NewValue = newValue.ToString(),
                            Message = ComparatorResources.PropertyValueChanged.Replace(PROPERTY_NAME, propertyDisplay).Replace(OLD_VALUE, oldValue.ToString()).Replace(NEW_VALUE, newValue.ToString())
                        });
                    }
                }

            }
            return auditTrail;
        }
    }
}
