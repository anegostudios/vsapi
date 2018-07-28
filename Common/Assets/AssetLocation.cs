using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Vintagestory.API.Config;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Defines a complete path to an assets, including it's domain
    /// </summary>
    [TypeConverterAttribute(typeof(StringAssetLocationConverter))]
    public class AssetLocation : IEquatable<AssetLocation>
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

        /// <summary>
        /// Create a new AssetLocation. If no domain is prefixed, the default 'game' domain is used.
        /// </summary>
        /// <param name="path"></param>
        public AssetLocation(string path)
        {
            path = path.ToLowerInvariant();
            var colonIndex = path.IndexOf(':');
            if (colonIndex == -1)
            {
                this.domain = null;
                this.path   = path;
            }
            else if (path.IndexOf(':', colonIndex + 1) == -1)
            {
                this.domain = path.Substring(0, colonIndex);
                this.path   = path.Substring(colonIndex + 1);
            }
            else throw new ArgumentException($"'{ path }' is not a valid asset location!");
            Validate();
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
            Validate();
        }

        private void Validate()
        {
            if ((domain?.Length == 0) || (path.Length == 0) || (domain?.Contains('/') ?? false) ||
                (path[0] == '/') || (path[path.Length - 1] == '/') || path.Contains("//"))
            {
                throw new ArgumentException($"'{ path }' is not a valid asset location!");
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

        public AssetCategory Category
        {
            get { return AssetCategory.FromCode(FirstPathPart()); }
        }

        public AssetLocation WithPathPrefix(string prefix)
        {
            path = prefix + path;
            Validate();
            return this;
        }

        public AssetLocation WithPathAppendix(string appendix)
        {
            path += appendix;
            Validate();
            return this;
        }

        public AssetLocation WithPathAppendixOnce(string appendix)
        {
            if (!path.EndsWith(appendix))
            {
                path += appendix;
                Validate();
            }
            return this;
        }

        public virtual bool HasDomain()
        {
            return domain != null;
        }

        public virtual string GetName()
        {
            var index = Path.LastIndexOf('/');
            return (index >= 0) ? Path.Substring(index + 1) : Path;
        }

        public virtual void RemoveEnding()
        {
            path = path.Substring(0, path.LastIndexOf("."));
            Validate();
        }

        public virtual AssetLocation Clone()
        {
            return new AssetLocation(this.domain, this.path);
        }

        public virtual AssetLocation CopyWithPath(string path)
        {
            return Clone().WithPath(path);
        }

        public virtual AssetLocation WithPath(string path)
        {
            this.Path = path;
            Validate();
            return this;
        }

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
