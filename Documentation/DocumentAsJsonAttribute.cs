using System;

#nullable disable

namespace Vintagestory.API
{
    /// <summary>
    /// Specifies that this class or property should be inlcuded in the JSON-only documentation.
    ///     <br/>Has no functional use and can be ommited if the property is marked with [JsonProperty].
    ///     <br/>When using with enum types, only specify this attribute on the type itself, not each value.
    ///
    /// <br/><br/>
    /// Note that most fields will also contain the following inside their summary tag:<br/>
    /// &lt;jsonoptional&gt;Optional&lt;/jsonoptional&gt;&lt;jsondefault&gt;None&lt;/jsondefault&gt;
    ///
    /// <br/><br/>
    /// The attribute now has the ability to add in required, default, and attribute parameters.
    /// These will be analysed by the docFx plugin.
    /// </summary>
    public class DocumentAsJsonAttribute : Attribute
    {

        public DocumentAsJsonAttribute() { }

        public DocumentAsJsonAttribute(string requiredStatus, string defaultValue = "", bool isAttribute = false) { }

    }
}
