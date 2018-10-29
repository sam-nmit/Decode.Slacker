using System;

namespace Slacker.Helpers.Attributes{

    /// <summary>
    /// Attribute used on DatabModels to override default table name generation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TableAttribute : Attribute {
        public string Name { get; private set; }
        public string Alias { get; private set; }

        public TableAttribute(string name, string alias = null) {
            this.Name = name;
            this.Alias = alias ?? name;
        }
    }
}
