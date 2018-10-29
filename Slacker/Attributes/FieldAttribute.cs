using System;

namespace Slacker.Helpers.Attributes
{
    /// <summary>
    /// Attribute used in DataModels to override default mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute {

        public string Name { get; set; }
        public bool IsPrimary { get; set; }
        public bool Ignored { get; set; }

        private bool? _isGenerated;
        /// <summary>
        /// Whether this Field is a generated key field. Defaults to IsPrimary unless specifically set
        /// </summary>
        public bool IsGenerated {
            get => _isGenerated ?? IsPrimary;

            set {
                _isGenerated = value;
            }
        }


    }
}
