using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Executer.ArgumentLookup
{
    /// <summary>
    /// A class defining a segment of the argument template string. 
    /// </summary>
    public class ArgumentTemplateSegment
    {
        /// <summary>
        /// Get the name of the argument segment.
        /// </summary>
        public string[] Names { get; private set; }

        /// <summary>
        /// Get whether this argument segment is a catch-all segment.
        /// </summary>
        public bool IsCatchAll { get; private set; }

        /// <summary>
        /// Get whether this argument segment is required.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Get the default value of this segment
        /// </summary>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// A constructor for the ArgumentTemplateSegment.
        /// </summary>
        /// <param name="argumentTemplateSegment">The string template segment, 
        /// to checked for different flags (e.g. the catch-all flag '*')</param>
        public ArgumentTemplateSegment(string argumentTemplateSegment)
        {
            if (argumentTemplateSegment == null)
                throw new ArgumentNullException(
                    "ArgumentTemplateSegment cannot be null string.");
            if (String.IsNullOrWhiteSpace(argumentTemplateSegment))
                throw new ArgumentNullException(
                    "ArgumentTemplateSegment cannot be empty or all whitespace.");

            string[] namesValue = argumentTemplateSegment.Split("=".ToCharArray(),2);
            Names = namesValue.First().Split('/');

            if (namesValue.Count() == 2)
            {
                IsRequired = false;
                DefaultValue = namesValue[1];
            }
            else 
            {
                IsRequired = true;
                DefaultValue = String.Empty;
            }
            if (Names.Contains("*"))
            {
                IsCatchAll = true;
            }
        }

    }
}
