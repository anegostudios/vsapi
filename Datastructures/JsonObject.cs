using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Elegant, yet somewhat inefficently designed (because wasteful with heap objects) wrapper class to abstract away the type-casting nightmare of JToken O.O
    /// </summary>
    public class JsonObject : IReadOnlyCollection<JsonObject>
    {
        JToken token;

        public static JsonObject FromJson(string jsonCode)
        {
            return new JsonObject(JToken.Parse(jsonCode));
        }

        /// <summary>
        /// Create a new instance of a JsonObject
        /// </summary>
        /// <param name="token"></param>
        public JsonObject(JToken token) {
            this.token = token;
        }

        /// <summary>
        /// Create a new instance of a JsonObject
        /// </summary>
        /// <param name="original"></param>
        /// <param name="unused">Only present so that the Constructor with a sole null parameter has an unambiguous signature</param>
        public JsonObject(JsonObject original, bool unused)
        {
            this.token = original.token;
        }

        /// <summary>
        /// Access a tokens element with given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JsonObject this[string key] {
            get
            {
                if ((token == null || !(token is JObject))) return new JsonObject(null);

                (token as JObject).TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var value);
                return new JsonObject(value);
            }
        }

        /// <summary>
        /// True if the token is not null
        /// </summary>
        public bool Exists
        {
            get { return token != null; }
        }

        public virtual JToken Token { 
            get { return token; } 
            set { token = value; } 
        }

        

        /// <summary>
        /// True if the token has an element with given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            return token[key] != null;
        }

        /// <summary>
        /// Deserialize the token to an object of the specified type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AsObject<T>(T defaultValue = default(T))
        {
            JsonSerializerSettings settings = null;

            return token == null ? defaultValue : JsonConvert.DeserializeObject<T>(token.ToString(), settings);
        }

        /// <summary>
        /// Deserialize the token to an object of the specified type T, with the specified domain for any AssetLocation which needs to be parsed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AsObject<T>(T defaultValue, string domain)
        {
            JsonSerializerSettings settings = null;

            if (domain != GlobalConstants.DefaultDomain)
            {
                settings = new JsonSerializerSettings();
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }

            return token == null ? defaultValue : JsonConvert.DeserializeObject<T>(token.ToString(), settings);
        }

        public T AsObject<T>(JsonSerializerSettings settings, T defaultValue, string domain = GlobalConstants.DefaultDomain)
        {
            if (domain != GlobalConstants.DefaultDomain)
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }

            return token == null ? defaultValue : JsonConvert.DeserializeObject<T>(token.ToString(), settings);
        }


        /// <summary>
        /// Turn the token into an array of JsonObjects
        /// </summary>
        /// <returns></returns>
        public JsonObject[] AsArray()
        {
            if (!(token is JArray)) return null;
            JArray arr = (JArray)token;

            JsonObject[] objs = new JsonObject[arr.Count];

            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = new JsonObject(arr[i]);
            }

            return objs;
        }
        
        /// <summary>
        /// Turn the token into a string
        /// </summary>
        /// <param name="defaultValue">If the conversion fails, this value is used instead</param>
        /// <returns></returns>
        public string AsString(string defaultValue = null)
        {
            return GetValue<string>(defaultValue);
        }

        [Obsolete("Use AsArray<string>() instead")]
        public string[] AsStringArray(string[] defaultValue = null, string defaultDomain = null)
        {
            return AsArray<string>(defaultValue, defaultDomain);
        }

        [Obsolete("Use AsArray<float>() instead")]
        public float[] AsFloatArray(float[] defaultValue = null)
        {
            return AsArray<float>(defaultValue);
        }

        /// <summary>
        /// Turn the token into an array
        /// </summary>
        /// <param name="defaultValue">If the conversion fails, this value is used instead</param>
        /// <param name="defaultDomain"></param>
        /// <returns></returns>
        public T[] AsArray<T>(T[] defaultValue = null, string defaultDomain = null)
        {
            if (!(token is JArray)) return defaultValue;
            JArray arr = (JArray)token;

            T[] objs = new T[arr.Count];

            for (int i = 0; i < objs.Length; i++)
            {
                JToken token = arr[i];
                if (token is JValue || token is JObject) {
                    objs[i] = token.ToObject<T>(defaultDomain);
                } else
                {
                    return defaultValue;
                }
                
            }

            return objs;
        }


        /// <summary>
        /// Turn the token into a boolean
        /// </summary>
        /// <param name="defaultValue">If the conversion fails, this value is used instead</param>
        /// <returns></returns>
        public bool AsBool(bool defaultValue = false)
        {
            if (!(token is JValue)) return defaultValue;

            object value = ((JValue)token).Value;

            if (value is bool) return (bool)value;
            if (value is string)
            {
                if (!bool.TryParse("" + value, out bool val))
                {
                    val = defaultValue;
                }
                return val;
            }

            return defaultValue;
        }


        /// <summary>
        /// Turn the token into an integer
        /// </summary>
        /// <param name="defaultValue">If the conversion fails, this value is used instead</param>
        /// <returns></returns>
        public int AsInt(int defaultValue = 0)
        {
            if (!(token is JValue)) return defaultValue;
            
            object value = ((JValue)token).Value;

            if (value is long) return (int)((long)value);
            if (value is int) return (int)value;
            if (value is float) return (int)((float)value);
            if (value is double) return (int)((double)value);
            if (value is string)
            {
                if (!int.TryParse("" + value, out int val))
                {
                    val = defaultValue;
                }
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Turn the token into a float
        /// </summary>
        /// <param name="defaultValue">If the conversion fails, this value is used instead</param>
        /// <returns></returns>
        public float AsFloat(float defaultValue = 0)
        {
            if (!(token is JValue)) return defaultValue;

            object value = ((JValue)token).Value;

            if (value is int) return (int)value;
            if (value is float) return (float)value;
            if (value is long) return (long)value;
            if (value is double) return (float)((double)value);
            if (value is string)
            {
                if (!float.TryParse("" + value, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out float val))
                {
                    val = defaultValue;
                }
                return val;
            }

            return defaultValue;
        }

        
        /// <summary>
        /// Turn the token into a double
        /// </summary>
        /// <param name="defaultValue">If the conversion fails, this value is used instead</param>
        /// <returns></returns>
        public double AsDouble(double defaultValue = 0)
        {
            if (!(token is JValue)) return defaultValue;

            object value = ((JValue)token).Value;

            if (value is int) return (int)value;
            if (value is long) return (long)value;
            if (value is double) return (double)value;
            if (value is string)
            {
                if (!double.TryParse("" + value, out double val))
                {
                    val = defaultValue;
                }
                return val;
            }
            return defaultValue;
        }
        
        T GetValue<T>(T defaultValue = default(T))
        {
            if (!(token is JValue)) return defaultValue;

            object value = ((JValue)token).Value;

            if (!(value is T)) return defaultValue;

            return token.ToObject<T>();
        }


        /// <summary>
        /// Calls token.ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Util.StringExtensions.DeDuplicate(token?.ToString());
        }

        /// <summary>
        /// True if the token is a JArray
        /// </summary>
        /// <returns></returns>
        public bool IsArray()
        {
            return token is JArray;
        }

        /// <summary>
        /// Turns the token into an IAttribute with all its child elements, if it has any.<br/>
        /// Note: If you converting this to a tree attribute, a subsequent call to tree.GetInt() might not work because Newtonsoft.JSON seems to load integers as long, so use GetDecimal() or GetLong() instead. Similar things might happen with float&lt;-&gt;double
        /// </summary>
        /// <returns></returns>
        public IAttribute ToAttribute()
        {
            return ToAttribute(token);
        }

        public virtual void FillPlaceHolder(string key, string value)
        {
            FillPlaceHolder(token, key, value);
        }

        static internal void FillPlaceHolder(JToken token, string key, string value)
        {
            JValue jval = token as JValue;
            if (jval != null && jval.Value is string)
            {
                jval.Value = (jval.Value as string).Replace("{" + key + "}", value);
            }

            JArray jarr = token as JArray;
            if (jarr != null)
            {
                foreach (JToken subtoken in jarr)
                {
                    FillPlaceHolder(subtoken, key, value);
                }
            }

            JObject jobj = token as JObject;
            if (jobj != null)
            {
                foreach (var val in jobj)
                {
                    FillPlaceHolder(val.Value, key, value);
                }
            }
        }


        static IAttribute ToAttribute(JToken token)
        {
            JValue jval = token as JValue;

            // Scalar

            if (jval != null)
            {
                if (jval.Value is int) return new IntAttribute((int)jval.Value);
                if (jval.Value is long) return new LongAttribute((long)jval.Value);
                if (jval.Value is float) return new FloatAttribute((float)jval.Value);
                if (jval.Value is double) return new DoubleAttribute((double)jval.Value);
                if (jval.Value is bool) return new BoolAttribute((bool)jval.Value);
                if (jval.Value is string) return new StringAttribute((string)jval.Value);
            }

            // Object, but an itemstack?
            /*JObject jobj = token as JObject;
            if (jobj != null)
            {
                if (jobj["type"] != null && jobj["code"] != null)
                {
                    JValue typeVal = jobj["type"] as JValue;
                    if (typeVal != null && typeVal.Value is string)
                    {

                    }
                }

                return tree;
            }*/

            // Object 

            JObject jobj = token as JObject;
            if (jobj != null)
            {
                TreeAttribute tree = new TreeAttribute();

                foreach (var val in jobj)
                {
                    tree[val.Key] = ToAttribute(val.Value);
                }

                return tree;
            }

            // Array

            JArray jarr = token as JArray;
            if (jarr != null)
            {
                if (!jarr.HasValues)
                    return new TreeArrayAttribute(Array.Empty<TreeAttribute>());

                JToken first = jarr[0];

                JValue jvalFirst = first as JValue;
                if (jvalFirst != null)
                {
                    if (jvalFirst.Value is int) return new IntArrayAttribute(ToPrimitiveArray<int>(jarr));
                    if (jvalFirst.Value is long) return new LongArrayAttribute(ToPrimitiveArray<long>(jarr));
                    if (jvalFirst.Value is float) return new FloatArrayAttribute(ToPrimitiveArray<float>(jarr));
                    if (jvalFirst.Value is double) return new DoubleArrayAttribute(ToPrimitiveArray<double>(jarr));
                    if (jvalFirst.Value is bool) return new BoolArrayAttribute(ToPrimitiveArray<bool>(jarr));
                    if (jvalFirst.Value is string) return new StringArrayAttribute(ToPrimitiveArray<string>(jarr));
                    return null;
                }

                TreeAttribute[] attrs = new TreeAttribute[jarr.Count];
                for (int i = 0; i < attrs.Length; i++)
                {
                    attrs[i] = (TreeAttribute)ToAttribute(jarr[i]);
                }

                return new TreeArrayAttribute(attrs);
            }

            return null;
        }

        /// <summary>
        /// Turn a JArray into a primitive array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] ToPrimitiveArray<T>(JArray array)
        {
            T[] values = new T[array.Count];
            for (int i = 0; i < values.Length; i++)
            {
                JValue val = array[i] as JValue;
                values[i] = array[i].ToObject<T>();
            }
            return values;
        }
        
        
        /// <summary>
        /// Returns a deep clone
        /// </summary>
        /// <returns></returns>
        public JsonObject Clone()
        {
            JsonObject cloned = new JsonObject(token.DeepClone());
            return cloned;
        }


        /// <summary>
        /// Returns true if this object has the named bool attribute, and it is true
        /// </summary>
        public bool IsTrue(string attrName)
        {
            if (token == null || !(token is JObject)) return false;
            if (token[attrName] is JValue jvalue)
            {
                if (jvalue.Value is bool result) return result;
                if (jvalue.Value is string boolString)
                {
                    bool.TryParse(boolString, out bool val);
                    return val;
                }
            }
            return false;
        }

        public int Count
        {
            get
            {
                if (token == null) throw new InvalidOperationException("Cannot count a null token");

                if (token is JObject jobj)
                {
                    return jobj.Count;
                }
                if (token is JArray jarr)
                {
                    return jarr.Count;
                }

                throw new InvalidOperationException("Can iterate only over a JObject or JArray, this token is of type " + token.Type);
            }
        }

        public IEnumerator<JsonObject> GetEnumerator()
        {
            if (token == null) throw new InvalidOperationException("Cannot iterate over a null token");

            if (token is JObject jobj)
            {
                foreach (var val in jobj)
                {
                    yield return new JsonObject(val.Key);
                }
            } else
            if (token is JArray jarr)
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return new JsonObject(jarr[i]);
                }
            } else

            throw new InvalidOperationException("Can iterate only over a JObject or JArray, this token is of type " + token.Type);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

