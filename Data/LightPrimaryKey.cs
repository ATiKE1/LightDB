using System;

namespace LightDB.Data
{
    public class LightPrimaryKeyAttribute : Attribute
    {
        public object PrimaryKey { get; set; }

        public LightPrimaryKeyAttribute(object primaryKey)
        {
            PrimaryKey = primaryKey;
        }   
    }
}
