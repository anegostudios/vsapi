using System;

namespace Vintagestory.API
{
    /// <summary>
    /// Adds a new property in the DocFX JSON documentation. Useful for attribute types which do not store JSON properties as variables.
    /// Properties need to be serializable by DocFX, hence why they're all strings. Sorry about that.    
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddDocumentationPropertyAttribute : Attribute
    {

        /// <summary>
        /// Adds a new property in the DocFX JSON documentation. Useful for attribute types which do not store JSON properties as variables.
        /// Properties need to be serializable by DocFX, hence why they're all strings. Sorry about that.    
        /// </summary>
        /// <param name="name">The name of the property in DocFX.</param>
        /// <param name="summary">The summary of the property in DocFX.</param>
        /// <param name="typeWithFullNamespace">The name of the type, along with its full namespace. e.g. "System.Single" or "VintageStory.API.Common.CompositeShape".</param>
        /// <param name="requiredStatus">The text to put inside the "required" tag in DocFX.</param>
        /// <param name="defaultStatus">The text to put inside the "default" tag in DocFX. Use an empty string for none.</param>
        public AddDocumentationPropertyAttribute(string name, string summary, string typeWithFullNamespace, string requiredStatus, string defaultStatus, bool attribute = false)
        {}

    }
}
