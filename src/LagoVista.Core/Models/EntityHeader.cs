using System;
using LagoVista.Core.Interfaces;
using System.Reflection;
using System.Linq;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Attributes;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityHeader : IEntityHeader
    {

        private String _id;

        public virtual String Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public String Text { get; set; }

        public string Key { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public bool IsEmpty()
        {
            return (String.IsNullOrEmpty(Id) && String.IsNullOrEmpty(Text)) || Id == "-1";
        }

        public static EntityHeader Empty
        {
            get
            {
                return new EntityHeader() { Id = "-1" };
            }
        }

        public static EntityHeader Create(String id, string text)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new InvalidDataException("Id is required for creating an entity header.");
            }

            if (String.IsNullOrEmpty(text))
            {
                throw new InvalidDataException("Text is required for creating an entity header.");
            }

            return new EntityHeader()
            {
                Id = id,
                Text = text,
            };
        }

        public static bool IsNullOrEmpty(EntityHeader header)
        {
            if (header == null)
            {
                return true;
            }

            if (header.IsEmpty())
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(EntityHeader hdr1, EntityHeader hdr2)
        {
            if ((object)hdr1 == null && (object)hdr2 == null)
            {
                return false;
            }

            if ((object)hdr1 == null && hdr2.IsEmpty())
            {
                return false;
            }

            if ((object)hdr2 == null && hdr1.IsEmpty())
            {
                return false;
            }

            if ((object)hdr1 == null && !hdr2.IsEmpty())
            {
                return true;
            }

            if ((object)hdr2 == null && !hdr1.IsEmpty())
            {
                return true;
            }

            return (hdr1.Id != hdr2.Id || hdr1.Text != hdr2.Text);
        }

        public static bool operator ==(EntityHeader hdr1, EntityHeader hdr2)
        {
            if ((object)hdr1 == null && (object)hdr2 == null)
            {
                return true;
            }

            if ((object)hdr1 == null && hdr2.IsEmpty())
            {
                return true;
            }

            if ((object)hdr2 == null && hdr1.IsEmpty())
            {
                return true;
            }

            if ((object)hdr1 == null && !hdr2.IsEmpty())
            {
                return false;
            }

            if ((object)hdr2 == null && !hdr1.IsEmpty())
            {
                return false;
            }

            return (hdr1.Id == hdr2.Id && hdr1.Text == hdr2.Text);
        }

        public EntityHeader Clone()
        {
            return new EntityHeader() { Id = Id, Text = Text, Key = Key };
        }
    }

    public class EntityHeader<T> : EntityHeader, IEntityHeader<T>
    {
        public bool HasValue
        {
            get { return !String.IsNullOrEmpty(Id); }
        }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                /* If this is an enum the actual type should get set with the key from the enum rather than this value */
                if (!typeof(T).GetTypeInfo().IsEnum)
                {
                    _value = value;
                }
            }
        }

        private static bool HasInterface<TType, TInterface>()
        {
            return typeof(TType).GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(TInterface));
        }

        /// <summary>
        /// Create an Entity Header based on something that implements IIDEntity and INamedEntity 
        /// or an enum that has an EnumLabel attribte.
        /// </summary>
        public static EntityHeader<T> Create(T value)
        {
            if (HasInterface<T, IIDEntity>() && HasInterface<T, INamedEntity>())
            {
                var eh = new EntityHeader<T>
                {
                    Id = ((IIDEntity)value).Id,
                    Text = ((INamedEntity)value).Name,
                    Value = value,
                };
                return eh;
            }

            if (typeof(T).GetTypeInfo().IsEnum)
            {
                var valueMember = typeof(T).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == value.ToString()).FirstOrDefault();

                var enumValues = Enum.GetValues(typeof(T));
                for (var idx = 0; idx < enumValues.GetLength(0); ++idx)
                {
                    var enumValue = enumValues.GetValue(idx);
                    if (enumValue.ToString() == value.ToString())
                    {
                        var enumMember = typeof(T).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == enumValue.ToString()).FirstOrDefault();
                        var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();
                        if (enumAttr != null)
                        {
                            var eh = new EntityHeader<T>
                            {
                                Id = enumAttr.Key,
                                Value = value
                            };
                            var labelProperty = enumAttr.ResourceType.GetTypeInfo().GetDeclaredProperty(enumAttr.LabelResource);
                            eh.Text = (string)labelProperty.GetValue(labelProperty.DeclaringType, null);
                            return eh;
                        }
                        else
                        {
                            var eh = new EntityHeader<T>
                            {
                                Id = value.ToString(),
                                Text = value.ToString(),
                                Value = value
                            };
                            return eh;
                        }
                    }
                }
            }

            throw new Exception($"Not an enum type, Cannot map type {typeof(T).Name} to Entity Header.");
        }

        public new EntityHeader<T> Clone()
        {
            return new EntityHeader<T>()
            {
                Id = Id,
                Text = Text,
                Value = Value
            };
        }


        public override String Id
        {
            get { return base.Id; }
            set
            {
                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        base.Id = String.Empty;
                        base.Text = String.Empty;
                        return;
                    }
                    var str = new StringBuilder();
                    var enumValues = Enum.GetValues(typeof(T));
                    for (var idx = 0; idx < enumValues.GetLength(0); ++idx)
                    {
                        var enumValue = enumValues.GetValue(idx).ToString();

                        var enumMember = typeof(T).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == enumValue.ToString()).FirstOrDefault();
                        var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();
                        if (enumAttr != null)
                        {
                            str.Append(enumAttr.Key);
                            str.Append(",");
                            if (enumAttr.Key == value)
                            {
                                base.Id = value;
                                _value = (T)(enumValues.GetValue(idx));
                                return;
                            }
                        }
                        else if (value == enumMember.Name)
                        {
                            base.Id = value;
                            _value = (T)(enumValues.GetValue(idx));
                            return;
                        }

                    }

                    /* If we made it here, we attempted to assign an invalid id, so reset values to null */
                    throw new InvalidCastException($"Attempt to Assign Incorrect Value to an Enum Based EntityHeader.  EnumType: [{typeof(T)}], Assgined Value [{value}], Allowable Values: [{str.ToString().TrimEnd(',')}].");
                }
                else
                {
                    base.Id = value;
                }

            }
        }
    }

}
