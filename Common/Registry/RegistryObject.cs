using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public abstract class RegistryObject
    {
        /// <summary>
        /// A unique domain + code of the object. Must be globally unique for all items / all blocks / all entities.
        /// </summary>
        public AssetLocation Code = null;

        /// <summary>
        /// Variant values as resolved from blocktype/itemtype or entitytype
        /// </summary>
        public OrderedDictionary<string, string> VariantStrict = new OrderedDictionary<string, string>();

        /// <summary>
        /// Variant values as resolved from blocktype/itemtype or entitytype. Will not throw an null pointer exception when the key does not exist, but return null instead.
        /// </summary>
        public RelaxedReadOnlyDictionary<string, string> Variant;

        /// <summary>
        /// The class handeling the object
        /// </summary>
        public string Class;


        public RegistryObject()
        {
            Variant = new RelaxedReadOnlyDictionary<string, string>(VariantStrict);
        }

        /// <summary>
        /// Returns a new assetlocation with an equal domain and the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetLocation CodeWithPath(string path)
        {
            return Code.CopyWithPath(path);
        }

        /// <summary>
        /// Removes componentsToRemove parts from the blocks code end by splitting it up at every occurence of a dash ('-'). Right to left.
        /// </summary>
        /// <param name="componentsToRemove"></param>
        /// <returns></returns>
        public string CodeWithoutParts(int componentsToRemove)
        {
            int i = Code.Path.Length;
            int index = 0;
            while (--i > 0 && componentsToRemove > 0)
            {
                if (Code.Path[i] == '-')
                {
                    index = i;
                    componentsToRemove--;
                }
            }

            return Code.Path.Substring(0, index);
        }


        /// <summary>
        /// Removes componentsToRemove parts from the blocks code beginning by splitting it up at every occurence of a dash ('-'). Left to Right
        /// </summary>
        /// <param name="componentsToRemove"></param>
        /// <returns></returns>
        public string CodeEndWithoutParts(int componentsToRemove)
        {
            int i = 0;
            int index = 0;
            while (++i < Code.Path.Length && componentsToRemove > 0)
            {
                if (Code.Path[i] == '-')
                {
                    index = i + 1;
                    componentsToRemove--;
                }
            }

            return Code.Path.Substring(index, Code.Path.Length - index);
        }


        /// <summary>
        /// Replaces the last parts from the blocks code and replaces it with components by splitting it up at every occurence of a dash ('-')
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public AssetLocation CodeWithParts(params string[] components)
        {
            if (Code == null) return null;

            AssetLocation newCode = Code.CopyWithPath(CodeWithoutParts(components.Length));
            for (int i = 0; i < components.Length; i++) newCode.Path += "-" + components[i];
            return newCode;
        }

        public AssetLocation CodeWithVariant(string type, string value)
        {
            StringBuilder sb = new StringBuilder(FirstCodePart());

            foreach (var val in Variant)
            {
                sb.Append("-");

                if (val.Key == type)
                {
                    sb.Append(value);
                } else
                {
                    sb.Append(val.Value);
                }
            }

            return new AssetLocation(Code.Domain, sb.ToString());
        }


        public AssetLocation CodeWithVariants(Dictionary<string, string> valuesByType)
        {
            StringBuilder sb = new StringBuilder(FirstCodePart());

            foreach (var val in Variant)
            {
                sb.Append("-");

                string value;

                if (valuesByType.TryGetValue(val.Key, out value))
                {
                    sb.Append(value);
                }
                else
                {
                    sb.Append(val.Value);
                }
            }

            return new AssetLocation(Code.Domain, sb.ToString());
        }

        public AssetLocation CodeWithVariants(string[] types, string[] values)
        {
            StringBuilder sb = new StringBuilder(FirstCodePart());

            foreach (var val in Variant)
            {
                sb.Append("-");

                int index = types.IndexOf(val.Key);

                if (index >= 0)
                {
                    sb.Append(values[index]);
                }
                else
                {
                    sb.Append(val.Value);
                }
            }

            return new AssetLocation(Code.Domain, sb.ToString());
        }


        /// <summary>
        /// Replaces one part from the blocks code and replaces it with components by splitting it up at every occurence of a dash ('-')
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public AssetLocation CodeWithPart(string part, int atPosition = 0)
        {
            if (Code == null) return null;

            AssetLocation newCode = Code.Clone();
            string[] parts = newCode.Path.Split('-');
            parts[atPosition] = part;
            newCode.Path = String.Join("-", parts);

            return newCode;
        }


        /// <summary>
        /// Returns the n-th code part in inverse order. If the code contains no dash ('-') the whole code is returned. Returns null if posFromRight is too high.
        /// </summary>
        /// <param name="posFromRight"></param>
        /// <returns></returns>
        public string LastCodePart(int posFromRight = 0)
        {
            if (Code == null) return null;
            if (posFromRight == 0 && !Code.Path.Contains('-')) return Code.Path;

            string[] parts = Code.Path.Split('-');
            return parts.Length - 1 - posFromRight >= 0 ? parts[parts.Length - 1 - posFromRight] : null;
        }

        /// <summary>
        /// Returns the n-th code part. If the code contains no dash ('-') the whole code is returned. Returns null if posFromLeft is too high.
        /// </summary>
        /// <param name="posFromLeft"></param>
        /// <returns></returns>
        public string FirstCodePart(int posFromLeft = 0)
        {
            if (Code == null) return null;
            if (posFromLeft == 0 && !Code.Path.Contains('-')) return Code.Path;

            string[] parts = Code.Path.Split('-');
            return posFromLeft <= parts.Length - 1 ? parts[posFromLeft] : null;
        }

        /// <summary>
        /// Returns true if any given wildcard matches the blocks/items code. E.g. water-* will match all water blocks
        /// </summary>
        /// <param name="wildcards"></param>
        /// <returns></returns>
        public bool WildCardMatch(AssetLocation[] wildcards)
        {
            foreach (AssetLocation wildcard in wildcards)
            {
                if (WildCardMatch(wildcard)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if given wildcard matches the blocks/items code. E.g. water-* will match all water blocks
        /// </summary>
        /// <param name="wildCard"></param>
        /// <returns></returns>
        public bool WildCardMatch(AssetLocation wildCard)
        {
            return WildcardUtil.Match(wildCard, Code);
        }


        /// <summary>
        /// Used by the block loader to replace wildcards with their final values
        /// </summary>
        /// <param name="input"></param>
        /// <param name="searchReplace"></param>
        /// <returns></returns>
        public static AssetLocation FillPlaceHolder(AssetLocation input, OrderedDictionary<string, string> searchReplace)
        {
            foreach (var val in searchReplace)
            {
                input.Path = FillPlaceHolder(input.Path, val.Key, val.Value);
            }

            return input;
        }

        /// <summary>
        /// Used by the block loader to replace wildcards with their final values
        /// </summary>
        /// <param name="input"></param>
        /// <param name="searchReplace"></param>
        /// <returns></returns>
        public static string FillPlaceHolder(string input, OrderedDictionary<string, string> searchReplace)
        {
            foreach (var val in searchReplace)
            {
                input = FillPlaceHolder(input, val.Key, val.Value);
            }

            return input;
        }

        /// <summary>
        /// Used by the block loader to replace wildcards with their final values
        /// </summary>
        /// <param name="input"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string FillPlaceHolder(string input, string search, string replace)
        {
            string pattern = @"\{((" + search + @")|([^\{\}]*\|" + search + @")|(" + search + @"\|[^\{\}]*)|([^\{\}]*\|" + search + @"\|[^\{\}]*))\}";

            return Regex.Replace(input, pattern, replace);
        }
    }

}
