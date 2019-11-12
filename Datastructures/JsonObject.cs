using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API
{
    /// <summary>
    /// Elegant, yet somewhat inefficently designed (because wasteful with heap objects) wrapper class to abstract away the type-casting nightmare of JToken O.O
    /// </summary>
    public class JsonObject
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
        /// Access a tokens element with given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JsonObject this[string key] {
            get
            {
                return new JsonObject((token == null || !(token is JObject)) ? null : token[key]);
            }
        }

        /// <summary>
        /// True if the token is not null
        /// </summary>
        public bool Exists
        {
            get { return token != null; }
        }

        public JToken Token { get { return token; } set { token = value; } }

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
        /// Deserialize the token to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AsObject<T>(T defaultValue = default(T), string domain = "game")
        {
            JsonSerializerSettings settings = null;

            if (domain != "game")
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
        public string[] AsStringArray(string[] defaultValue = null)
        {
            return AsArray<string>(defaultValue);
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
        /// <returns></returns>
        public T[] AsArray<T>(T[] defaultValue = null)
        {
            if (!(token is JArray)) return defaultValue;
            JArray arr = (JArray)token;

            T[] objs = new T[arr.Count];

            for (int i = 0; i < objs.Length; i++)
            {
                JToken token = arr[i];
                if (token is JValue) {
                    objs[i] = token.ToObject<T>();
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
                bool val = defaultValue;
                bool.TryParse("" + value, out val);
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
                int val = defaultValue;
                int.TryParse("" + value, out val);
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
                float val = defaultValue;
                float.TryParse(""+value, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out val);
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
                double val = defaultValue;
                double.TryParse("" + value, out val);
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
            return token.ToString();
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
        /// Turns the token into an IAttribute with all its child elements, if it has any
        /// </summary>
        /// <returns></returns>
        public IAttribute ToAttribute()
        {
            return ToAttribute(token);
        }

        public void FillPlaceHolder(string key, string value)
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
                    return new TreeArrayAttribute(new TreeAttribute[0]);

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
    }
}

