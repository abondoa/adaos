using Adaos.Shell.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Core.ArgumentLookup
{

    /// <summary>
    /// A lookup-table used to store argument values of type object, indexed by the argument name.
    /// </summary>
    public class ArgumentValueLookup : ValueLookup<IArgument>, IArgumentValueLookup
    {
        /// <summary>
        /// A constructor for the ArgumentValueLookup taking an anonymous object as values.
        /// </summary>
        /// <param name="values">The collection of key-value pairs to create the lookup from.</param>
        public ArgumentValueLookup(IEnumerable<KeyValuePair<string, IArgument>> values)
            : base(values)
        { }

        /// <summary>
        /// A constructor for the ArgumentValueLookup taking an anonymous object as values.
        /// </summary>
        /// <param name="dictionary">The dictionary to create the lookup from.</param>
        public ArgumentValueLookup(IDictionary<string, IArgument> dictionary)
            : base(dictionary)
        { }
        
        /// <summary>
        /// A constructor for the ArgumentValueLookup taking an anonymous object as values.
        /// </summary>
        /// <param name="values">Default values for the lookup, given as an anonymous type.
        /// The if the type of a value in the anonymous type doesn't match the type 
        /// of the dictionary typeparameter, the values is discarded.
        /// </param>
        public ArgumentValueLookup(IArgument values)
            : base(values)
        { }
    }

    /// <summary>
    /// A lookup-table used to store argument values, indexed by the argument name.
    /// </summary>
    /// <typeparam name="TArgumentType">The type of the argument.</typeparam>
    public class ValueLookup<TArgumentType> : IValueLookup<TArgumentType>
    {
        private IEnumerable<KeyValuePair<string, TArgumentType>> _table;
        private ILookup<string, TArgumentType> _lookup;

        public ILookup<string, TArgumentType> Lookup { get { return _lookup; } }

        /// <summary>
        /// A constructor for the ValueLookup taking an anonymous object as values.
        /// </summary>
        /// <param name="values">The collection of key-value pairs to create the lookup from.</param>
        public ValueLookup(IEnumerable<KeyValuePair<string, TArgumentType>> values)
        {
            _table = values;
            _lookup = _table.ToLookup(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// A constructor for the ValueLookup taking an anonymous object as values.
        /// </summary>
        /// <param name="dictionary">The dictionary to create the lookup from.</param>
        public ValueLookup(IDictionary<string, TArgumentType> dictionary)
        { 
            if(dictionary != null)
            {
                _table = dictionary.AsEnumerable();
            }
            else
            {
                // Create empty lookup to avoid checking for null-reference all over.
                _table = new KeyValuePair<string, TArgumentType>[0].AsEnumerable();
            }
            _lookup = _table.ToLookup(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// A constructor for the ValueLookup taking an anonymous object as values.
        /// </summary>
        /// <param name="values">Default values for the lookup, given as an anonymous type.
        /// The if the type of a value in the anonymous type doesn't match the type 
        /// of the dictionary typeparameter, the values is discarded.
        /// </param>
        public ValueLookup(object values)
        {

            var valuesAsDictionary = values as IDictionary<string, TArgumentType>;
            if (valuesAsDictionary != null)
            {
                _table = valuesAsDictionary.AsEnumerable();
            }
            else if (values != null)
            {
                var objectAsDictionary = new List<KeyValuePair<string, TArgumentType>>();
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                {
                    object obj2 = descriptor.GetValue(values);
                    if (obj2 is TArgumentType)
                        objectAsDictionary.Add(new KeyValuePair<string, TArgumentType>(descriptor.Name, (TArgumentType)obj2));
                }
                _table = objectAsDictionary.AsEnumerable();
            }
            _lookup = _table.ToLookup(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Get the first value associated with the given name.
        /// Returns null if value wasn't found.
        /// </summary>
        /// <param name="name">The name of the object to search for.</param>
        public TArgumentType this[string name]
        {
            get
            {
                var result = _lookup[name];
                if (result != null)
                    return result.FirstOrDefault();
                else
                    return default(TArgumentType);
            }
        }

        /// <summary>
        /// A method that returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<string, TArgumentType>> GetEnumerator()
        {
            return _table.GetEnumerator();
        }

        /// <summary>
        /// A method that returns an enumerator that iterates through the collection.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _table.GetEnumerator();
        }
    }
}
