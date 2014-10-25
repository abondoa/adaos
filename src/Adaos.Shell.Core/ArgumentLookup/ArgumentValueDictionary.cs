using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Core.ArgumentLookup
{

    /// <summary>
    /// A dictionary class used to hold arguments of type object, mapped to their argument names.
    /// </summary>
    public class ArgumentValueDictionary : ArgumentValueDictionary<object>
    {
        /// <summary>
        /// A constructor for the ArgumentValueDictionary
        /// </summary>
        public ArgumentValueDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// A constructor for the ArgumentValueDictionary
        /// </summary>
        /// <param name="dictionary">Default values for the dictionary.</param>
        public ArgumentValueDictionary(Dictionary<string, object> dictionary)
            : base(dictionary)
        { }

        /// <summary>
        /// A constructor for the ArgumentValueDictionary
        /// </summary>
        /// <param name="values">Default values for the dictionary, given as an anonymous type.
        /// The if the type of a value in the anonymous type doesn't match the type 
        /// of the dictionary typeparameter, the values is discarded.
        /// </param>
        public ArgumentValueDictionary(object values)
            : base(values)
        { }
    }

    /// <summary>
    /// A dictionary class used to hold arguments mapped to their argument names.
    /// </summary>
    /// <typeparam name="TArgumentType">The type of the argument to hold.</typeparam>
    public class ArgumentValueDictionary<TArgumentType> : Dictionary<string, TArgumentType>
    {
        /// <summary>
        /// A constructor for the ArgumentValueDictionary
        /// </summary>
        public ArgumentValueDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// A constructor for the ArgumentValueDictionary
        /// </summary>
        /// <param name="dictionary">Default values for the dictionary.</param>
        public ArgumentValueDictionary(Dictionary<string, TArgumentType> dictionary)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (dictionary != null)
            {
                foreach (var current in dictionary)
                {
                    Add(current.Key, current.Value);
                }
            }
        }

        /// <summary>
        /// A constructor for the ArgumentValueDictionary
        /// </summary>
        /// <param name="values">Default values for the dictionary, given as an anonymous type.
        /// The if the type of a value in the anonymous type doesn't match the type 
        /// of the dictionary typeparameter, the values is discarded.
        /// </param>
        public ArgumentValueDictionary(object values)
        {
            var valuesAsDictionary = values as IDictionary<string, TArgumentType>;
            if(valuesAsDictionary!=null)
            {
                foreach (var current in valuesAsDictionary)
                {
                    Add(current.Key, current.Value);
                }
            }
            else if (values != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                {
                    object obj2 = descriptor.GetValue(values);
                    if(obj2 is TArgumentType)
                        Add(descriptor.Name, (TArgumentType)obj2);
                }
            }
        }
    }
}
