using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Vintagestory.API.Config;

namespace Vintagestory.API.Common
{
    public interface IBlockTextureLocationDictionary
    {
        void AddTextureLocation(AssetLocationAndSource textureLoc);

        int this[AssetLocationAndSource textureLoc] { get; }
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
        public string Source;

        public AssetLocationAndSource(string location) : base(location)
        {
        }

        public AssetLocationAndSource(AssetLocation loc) : base(loc.Domain, loc.Path)
        {
        }

        public AssetLocationAndSource(AssetLocation loc, string source) : base(loc.Domain, loc.Path)
        {
            this.Source = source;
        }

        public AssetLocationAndSource(string domain, string path, string source) : base(domain, path)
        {
            this.Source = source;
        }
    }

    /// <summary>
    /// Defines a complete path to an assets, including it's domain
    /// </summary>
    [TypeConverterAttribute(typeof(StringAssetLocationConverter))]
    public class AssetLocation : IEquatable<AssetLocation>, IComparable<AssetLocation>
    {
        public const char LocationSeparator = ':';

        private string domain;
        private string path;

        public string Domain {
            get { return domain ?? "game"; }
            set { domain = value; }
        }

        public string Path {
            get { return path; }
            set { path = value; }
        }

        public bool IsWildCard => Path.Contains("*");

        /// <summary>
        /// Create a new AssetLocation. If no domain is prefixed, the default 'game' domain is used.
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
                domain = domainAndPath.Substring(0, colonIndex);
                path = domainAndPath.Substring(colonIndex + 1);
            }
        }

        /// <summary>
        /// Create a new AssetLocation with given domain
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        public AssetLocation(string domain, string path)
        {
            this.domain = domain?.ToLowerInvariant();
            this.path   = path.ToLowerInvariant();
        }


        public static AssetLocation Create(string domainAndPath, string defaultDomain = "game")
        {
            if (!domainAndPath.Contains(":"))
            {
                return new AssetLocation(defaultDomain, domainAndPath);
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
            return Location.Domain.Equals(Domain) && Location.Path.StartsWith(Path);
        }


        public virtual bool BeginsWith(string domain, string partialPath)
        {
            return domain.Equals(Domain) && Path.StartsWith(partialPath);
        }

        public string ToShortString()
        {
            if (Domain != "game")
            {
                return ToString();
            }

            return Path;
        }


        /// <summary>
        /// Returns the n-th path part
        /// </summary>
        /// <param name="posFromLeft"></param>
        /// <returns></returns>
        public string FirstPathPart(int posFromLeft = 0)
        {
            string[] parts = Path.Split('/');
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
            if (!path.StartsWith(prefix))
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
            path = path.Substring(0, path.LastIndexOf("."));
        }

        /// <summary>
        /// Clones this asset.
        /// </summary>
        /// <returns>the cloned asset.</returns>
        public virtual AssetLocation Clone()
        {
            return new AssetLocation(this.domain, this.path);
        }

        /// <summary>
        /// Makes a copy of the asset with a modified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual AssetLocation CopyWithPath(string path)
        {
            return Clone().WithPath(path);
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
            return Domain.GetHashCode() ^ Path.GetHashCode();
        }

        public bool Equals(AssetLocation other)
        {
            if (other == null) return false;
            return (Domain == other.Domain) && (Path == other.Path);
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
