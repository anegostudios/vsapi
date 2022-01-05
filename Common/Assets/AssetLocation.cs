using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public interface ITextureLocationDictionary
    {
        bool AddTextureLocation(AssetLocationAndSource textureLoc);

        int this[AssetLocationAndSource textureLoc] { get; }

        bool ContainsKey(AssetLocation loc);
        void AddTextureLocation_Checked(AssetLocationAndSource assetLocationAndSource);
    }

    /// <summary>
    /// For performance, don't build and store new concatenated strings for every block variant, item and entity, when these will only be used (if ever) for error logging
    /// </summary>
    public struct SourceStringComponents
    {
        private readonly string text;
        private readonly string domain;
        private readonly string path;
        private readonly int alternate;

        /// <summary>
        /// Store references to the source strings, to be able to build a logging string later if necessary
        /// </summary>
        public SourceStringComponents(string message, string sourceDomain, string sourcePath, int sourceAlt)
        {
            text = message;
            domain = sourceDomain;
            path = sourcePath;
            alternate = sourceAlt;
        }

        public override string ToString()
        {
            if (alternate >= 0) return text + domain + AssetLocation.LocationSeparator + path + " alternate:" + alternate;
            return text + domain + AssetLocation.LocationSeparator + path;
        }
    }
    
    /// <summary>
    /// Defines a complete path to an assets, including it's domain. Includes an extra Source field for debugging.
    /// </summary>
    [TypeConverterAttribute(typeof(StringAssetLocationConverter))]
    public class AssetLocationAndSource : AssetLocation, IEquatable<AssetLocation>
    {
        /// <summary>
        /// The source of a given asset.
        /// </summary>
        public SourceStringComponents Source;

        public AssetLocationAndSource(string location) : base(location)
        {
        }

        public AssetLocationAndSource(AssetLocation loc) : base(loc.Domain, loc.Path)
        {
        }

        public AssetLocationAndSource(AssetLocation loc, string message, AssetLocation sourceLoc, int alternateNo = -1) : base(loc.Domain, loc.Path)
        {
            this.Source = new SourceStringComponents(message, sourceLoc.Domain, sourceLoc.Path, alternateNo);
        }

        public AssetLocationAndSource(string domain, string path, string message, string sourceDomain, string sourcePath, int alternateNo = -1) : base(domain, path)
        {
            this.Source = new SourceStringComponents(message, sourceDomain, sourcePath, alternateNo);
        }

        public AssetLocationAndSource(AssetLocation loc, SourceStringComponents source) : base(loc.Domain, loc.Path)
        {
            this.Source = source;
        }

        public AssetLocationAndSource(string domain, string path, SourceStringComponents source) : base(domain, path)
        {
            this.Source = source;
        }

        [Obsolete("For reduced RAM usage please use newer overloads e.g. AssetLocationAndSource(loc, message, sourceAssetLoc)", false)]
        public AssetLocationAndSource(AssetLocation loc, string oldStyleSource) : base(loc.Domain, loc.Path)
        {
            this.Source = new SourceStringComponents(oldStyleSource, "", "", -1);
        }

        [Obsolete("For reduced RAM usage please use newer overloads e.g. AssetLocationAndSource(domain, path, message, sourceAssetLoc)", false)]
        public AssetLocationAndSource(string domain, string path, string oldStyleSource) : base(domain, path)
        {
            this.Source = new SourceStringComponents(oldStyleSource, "", "", -1);
        }
    }

    /// <summary>
    /// Defines a complete path to an assets, including it's domain
    /// </summary>
    [TypeConverterAttribute(typeof(StringAssetLocationConverter))]
    [ProtoContract]
    public class AssetLocation : IEquatable<AssetLocation>, IComparable<AssetLocation>
    {
        public const char LocationSeparator = ':';

        [ProtoMember(1)]
        private string domain;
        [ProtoMember(2)]
        private string path;

        public string Domain {
            get { return domain ?? GlobalConstants.DefaultDomain; }
            set { domain = value == null ? null : string.Intern(value.ToLowerInvariant()); }
        }

        public string Path {
            get { return path; }
            set { path = value; }
        }

        public bool IsWildCard => Path.IndexOf('*') >= 0;
        public bool EndsWithWildCard => path.Length > 1 && path[path.Length - 1] == '*';

        // Needed for ProtoBuf
        public AssetLocation()
        {

        }

        /// <summary>
        /// Create a new AssetLocation from a single string (e.g. when parsing an AssetLocation in a JSON file). If no domain is prefixed, the default 'game' domain is used.
        /// This ensures the domain and path in the created AssetLocation are lowercase (as the input string could have any case)
        /// </summary>
        /// <param name="domainAndPath"></param>
        public AssetLocation(string domainAndPath)
        {
            ResolveToDomainAndPath(domainAndPath, out domain, out path);
        }
        
        /// <summary>
        /// Helper function to resolve path dependancies.
        /// </summary>
        /// <param name="domainAndPath">Full path</param>
        /// <param name="domain">The mod domain to get</param>
        /// <param name="path">The resulting path to get</param>
        static void ResolveToDomainAndPath(string domainAndPath, out string domain, out string path)
        {
            domainAndPath = domainAndPath.ToLowerInvariant();
            var colonIndex = domainAndPath.IndexOf(':');

            if (colonIndex == -1)
            {
                domain = null;
                path = domainAndPath;
            }
            else
            {
                domain = string.Intern(domainAndPath.Substring(0, colonIndex));
                path = domainAndPath.Substring(colonIndex + 1);
            }
        }

        /// <summary>
        /// Create a new AssetLocation with given domain and path: for efficiency it is the responsibility of calling code to ensure these are lowercase
        /// </summary>
        public AssetLocation(string domain, string path)
        {
            this.domain = domain == null ? null : string.Intern(domain);
            this.path   = path;
        }


        /// <summary>
        /// Create an Asset Location from a string which may optionally have no prefixed domain: - in which case the defaultDomain is used.
        /// This may be used to create an AssetLocation from any string (e.g. from custom Attributes in a JSON file).  For safety and consistency it ensures the domainAndPath string is lowercase.
        /// BUT: the calling code has the responsibility to ensure the defaultDomain parameter is lowercase (normally the defaultDomain will be taken from another existing AssetLocation, in which case it should already be lowercase).
        /// </summary>
        public static AssetLocation Create(string domainAndPath, string defaultDomain = GlobalConstants.DefaultDomain)
        {
            if (!domainAndPath.Contains(":"))
            {
                return new AssetLocation(defaultDomain, domainAndPath.ToLowerInvariant());
            }

            return new AssetLocation(domainAndPath);
        }


        /// <summary>
        /// Returns true if this is a valid path. For an asset location to be valid it needs to 
        /// have any string as domain, any string as path, the domain may not contain slashes, and the path may not contain 2 consecutive slashes
        /// </summary>
        public bool Valid
        {
            get {
                bool invalid = ((domain?.Length == 0) || (path.Length == 0) || (domain?.Contains('/') ?? false) ||
                (path[0] == '/') || (path[path.Length - 1] == '/') || path.Contains("//"));

                return !invalid;
            }
        }


        public virtual bool IsChild(AssetLocation Location)
        {
            return Location.Domain.Equals(Domain) && Location.path.StartsWithFast(path);
        }


        public virtual bool BeginsWith(string domain, string partialPath)
        {
            return path.StartsWithFast(partialPath) && (domain == null || domain.Equals(Domain));
        }

        public string ToShortString()
        {
            if (domain == null || domain.Equals(GlobalConstants.DefaultDomain))
            {
                return path;
            }

            return ToString();
        }

        public string ShortDomain()
        {
            return (domain == null || domain.Equals(GlobalConstants.DefaultDomain)) ? "" : domain;
        }


        /// <summary>
        /// Returns the n-th path part
        /// </summary>
        /// <param name="posFromLeft"></param>
        /// <returns></returns>
        public string FirstPathPart(int posFromLeft = 0)
        {
            string[] parts = path.Split('/');
            return parts[posFromLeft];
        }

        /// <summary>
        /// Gets the category of the asset.
        /// </summary>
        public AssetCategory Category
        {
            get { return AssetCategory.FromCode(FirstPathPart()); }
        }

        public AssetLocation WithPathPrefix(string prefix)
        {
            path = prefix + path;
            return this;
        }

        public AssetLocation WithPathPrefixOnce(string prefix)
        {
            if (!path.StartsWithFast(prefix))
            {
                path = prefix + path;
            }
            return this;
        }

        public AssetLocation WithPathAppendix(string appendix)
        {
            path += appendix;
            return this;
        }

        public AssetLocation WithoutPathAppendix(string appendix)
        {
            if (path.EndsWith(appendix))
            {
                path = path.Substring(0, path.Length - appendix.Length);
            }
            return this;
        }

        public AssetLocation WithPathAppendixOnce(string appendix)
        {
            if (!path.EndsWith(appendix))
            {
                path += appendix;
            }
            return this;
        }

        /// <summary>
        /// Whether or not the Asset has a domain.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasDomain()
        {
            return domain != null;
        }

        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <returns></returns>
        public virtual string GetName()
        {
            var index = Path.LastIndexOf('/');
            return (index >= 0) ? Path.Substring(index + 1) : Path;
        }

        /// <summary>
        /// Removes the file ending from the asset path.
        /// </summary>
        public virtual void RemoveEnding()
        {
            path = path.Substring(0, path.LastIndexOf('.'));
        }

        /// <summary>
        /// Clones this asset.
        /// </summary>
        /// <returns>the cloned asset.</returns>
        public virtual AssetLocation Clone()
        {
            return new AssetLocation(this.domain, this.path);
        }

        public virtual AssetLocation CloneWithoutPrefixAndEnding(int prefixLength)
        {
            int i = path.LastIndexOf('.');
            string newPath = i >= prefixLength ? path.Substring(prefixLength, i - prefixLength) : path.Substring(prefixLength);
            return new AssetLocation(this.domain, newPath);
        }

        /// <summary>
        /// Makes a copy of the asset with a modified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual AssetLocation CopyWithPath(string path)
        {
            return new AssetLocation(this.domain, path);
        }

        /// <summary>
        /// Sets the path of the asset location
        /// </summary>
        /// <param name="path">the new path to set.</param>
        /// <returns>The modified AssetLocation</returns>
        public virtual AssetLocation WithPath(string path)
        {
            this.Path = path;
            return this;
        }

        /// <summary>
        /// Converts a collection of paths to AssetLocations.
        /// </summary>
        /// <param name="names">The names of all of the locations</param>
        /// <returns>The AssetLocations for all the names given.</returns>
        public static AssetLocation[] toLocations(string[] names)
        {
            AssetLocation[] locations = new AssetLocation[names.Length];
            for(int i = 0; i < locations.Length; i++)
            {
                locations[i] = new AssetLocation(names[i]);
            }
            return locations;
        }

        public override int GetHashCode()
        {
            return Domain.GetHashCode() ^ path.GetHashCode();
        }

        public bool Equals(AssetLocation other)
        {
            if (other == null) return false;
            return path.EqualsFast(other.path) && Domain.Equals(other.Domain);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AssetLocation);
        }

        public override string ToString()
        {
            return Domain + LocationSeparator + Path;
        }

        public int CompareTo(AssetLocation other)
        {
            return ToString().CompareTo(other.ToString());
        }
    }

    class StringAssetLocationConverter : TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string && value as string != "")
            {
                return new AssetLocation(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as AssetLocation).ToShortString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
