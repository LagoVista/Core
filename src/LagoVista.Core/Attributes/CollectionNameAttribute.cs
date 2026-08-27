using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string name) 
        {
            CollectionName = name;
        }

        public CollectionNameAttribute(string name, bool mongo, bool cosmos)
        {
            CollectionName = name;
            Mongo = mongo;
            Cosmos = cosmos;
        }

        public bool Mongo { get;  } = true;

        public bool Cosmos { get; } = true;
        public string CollectionName { get; }
    }

    public class CosmosPartitionKeyAttribute : Attribute
    {
        public CosmosPartitionKeyAttribute(string name)
        {
            PartitionKey = name;
        }

   

        public string PartitionKey { get; }
    }
}
