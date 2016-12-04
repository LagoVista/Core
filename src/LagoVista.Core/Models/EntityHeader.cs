using System;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Exceptions;

namespace LagoVista.Core.Models
{
    public class EntityHeader : IEntityHeader
    {
        public String Id { get; set; }
        public String Text { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(Id) && String.IsNullOrEmpty(Text);
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
            if(header == null)
            {
                return true;
            }

            if(header.IsEmpty())
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
}
