using System;
using LagoVista.Core.Interfaces;
using System.Reflection;
using System.Linq;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Attributes;

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
       
        public override String Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                if(typeof(T).GetTypeInfo().IsEnum)
                {
                    var enumValues = Enum.GetValues(typeof(T));

                    for (var idx = 0; idx < enumValues.GetLength(0); ++idx)
                    {
                        var enumValue = enumValues.GetValue(idx).ToString();

                        var enumMember = typeof(T).GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == enumValue.ToString()).FirstOrDefault();
                        var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();
                        if(enumAttr.Key == Id)
                        {
                            _value = (T)(enumValues.GetValue(idx));
                        }
                    }
                }
            }
        }
    }

}
