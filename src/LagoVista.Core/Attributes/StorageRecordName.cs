using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    public class StorageRecordNameAttribute : Attribute
    {

        public StorageRecordNameAttribute(string name) 
        {
            StorageName = name;
        }
  
        public string StorageName { get; }
    }
}
