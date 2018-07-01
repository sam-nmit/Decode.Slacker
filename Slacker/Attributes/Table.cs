﻿using System;

namespace Slacker.Helpers.Attributes{

    /// <summary>
    /// Attribute used on DatabModels to override default table name generation
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class Table : System.Attribute {
        public string Name { get; private set; }
        public string Alias { get; private set; }

        public Table(string name, string alias = null) {
            this.Name = name;
            this.Alias = alias ?? name;
        }
    }
}