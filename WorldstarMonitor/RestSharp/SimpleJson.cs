//-----------------------------------------------------------------------
// <copyright file="SimpleJson.cs" company="The Outercurve Foundation">
//    Copyright (c) 2011, The Outercurve Foundation.
//
//    Licensed under the MIT License (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.opensource.org/licenses/mit-license.php
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <author>Nathan Totten (ntotten.com), Jim Zimmerman (jimzimmerman.com) and Prabir Shrestha (prabir.me)</author>
// <website>https://github.com/facebook-csharp-sdk/simple-json</website>
//-----------------------------------------------------------------------

// VERSION: 0.26.0

// NOTE: uncomment the following line to make SimpleJson class internal.
//#define SIMPLE_JSON_INTERNAL

// NOTE: uncomment the following line to make JsonArray and JsonObject class internal.
//#define SIMPLE_JSON_OBJARRAYINTERNAL

// NOTE: uncomment the following line to enable dynamic support.
//#define SIMPLE_JSON_DYNAMIC

// NOTE: uncomment the following line to enable DataContract support.
//#define SIMPLE_JSON_DATACONTRACT

// NOTE: uncomment the following line to disable linq expressions/compiled lambda (better performance) instead of method.invoke().
// define if you are using .net framework <= 3.0 or < WP7.5
//#define SIMPLE_JSON_NO_LINQ_EXPRESSION

// NOTE: uncomment the following line if you are compiling under Window Metro style application/library.
// usually already defined in properties
//#define NETFX_CORE;

// If you are targetting WinStore, WP8 and NET4.5+ PCL make sure to #define SIMPLE_JSON_TYPEINFO;

// original json parsing code from http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html

#if NETFX_CORE
#define SIMPLE_JSON_TYPEINFO
#endif

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
#if !SIMPLE_JSON_NO_LINQ_EXPRESSION
using System.Linq.Expressions;
#endif
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
#if SIMPLE_JSON_DYNAMIC
using System.Dynamic;
#endif
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using RestSharp.Reflection;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable RedundantExplicitArrayCreation
// ReSharper disable SuggestUseVarKeywordEvident
namespace RestSharp
{
    /// <summary>
    /// Represents the json array.
    /// </summary>
    [GeneratedCode("simple-json", "1.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
#if SIMPLE_JSON_OBJARRAYINTERNAL
    internal
#else
    public
#endif
 class JsonArray : List<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class. 
        /// </summary>
        public JsonArray() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class. 
        /// </summary>
        /// <param name="capacity">The capacity of the json array.</param>
        public JsonArray(int capacity) : base(capacity) { }

        /// <summary>
        /// The json representation of the array.
        /// </summary>
        /// <returns>The json representation of the array.</returns>
        public override string ToString()
        {
            return SimpleJson.SerializeObject(this) ?? string.Empty;
        }
    }

    /// <summary>
    /// Represents the json object.
    /// </summary>
    [GeneratedCode("simple-json", "1.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
#if SIMPLE_JSON_OBJARRAYINTERNAL
    internal
#else
    public
#endif
 class JsonObject :
#if SIMPLE_JSON_DYNAMIC
 DynamicObject,
#endif
 IDictionary<string, object>
    {
        /// <summary>
        /// The internal member dictionary.
        /// </summary>
        private readonly Dictionary<string, object> _members;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/>.
        /// </summary>
        public JsonObject()
        {
            _members = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the key.</param>
        public JsonObject(IEqualityComparer<string> comparer)
        {
            _members = new Dictionary<string, object>(comparer);
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public object this[int index]
        {
            get { return GetAtIndex(_members, index); }
        }

        internal static object GetAtIndex(IDictionary<string, object> obj, int index)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (index >= obj.Count)
                throw new ArgumentOutOfRangeException("index");
            int i = 0;
            foreach (KeyValuePair<string, object> o in obj)
                if (i++ == index) return o.Value;
            return null;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, object value)
        {
            _members.Add(key, value);
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return _members.ContainsKey(key);
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<string> Keys
        {
            get { return _members.Keys; }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return _members.Remove(key);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return _members.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<object> Values
        {
            get { return _members.Values; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return _members[key]; }
            set { _members[key] = value; }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<string, object> item)
        {
            _members.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _members.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return _members.ContainsKey(item.Key) && _members[item.Key] == item.Value;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");
            int num = Count;
            foreach (KeyValuePair<string, object> kvp in this)
            {
                array[arrayIndex++] = kvp;
                if (--num <= 0)
                    return;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _members.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return _members.Remove(item.Key);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        /// <summary>
        /// Returns a json <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A json <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return SimpleJson.SerializeObject(this);
        }

#if SIMPLE_JSON_DYNAMIC
        /// <summary>
        /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
        /// </summary>
        /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
        /// <param name="result">The result of the type conversion operation.</param>
        /// <returns>
        /// Alwasy returns true.
        /// </returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            // <pex>
            if (binder == null)
                throw new ArgumentNullException("binder");
            // </pex>
            Type targetType = binder.Type;

            if ((targetType == typeof(IEnumerable)) ||
                (targetType == typeof(IEnumerable<KeyValuePair<string, object>>)) ||
                (targetType == typeof(IDictionary<string, object>)) ||
                (targetType == typeof(IDictionary)))
            {
                result = this;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that delete an object member. This method is not intended for use in C# or Visual Basic.
        /// </summary>
        /// <param name="binder">Provides information about the deletion.</param>
        /// <returns>
        /// Alwasy returns true.
        /// </returns>
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            // <pex>
            if (binder == null)
                throw new ArgumentNullException("binder");
            // </pex>
            return _members.Remove(binder.Name);
        }

        /// <summary>
        /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for indexing operations.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, <paramref name="indexes"/> is equal to 3.</param>
        /// <param name="result">The result of the index operation.</param>
        /// <returns>
        /// Alwasy returns true.
        /// </returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes == null) throw new ArgumentNullException("indexes");
            if (indexes.Length == 1)
            {
                result = ((IDictionary<string, object>)this)[(string)indexes[0]];
                return true;
            }
            result = null;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// Alwasy returns true.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            object value;
            if (_members.TryGetValue(binder.Name, out value))
            {
                result = value;
                return true;
            }
            result = null;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="indexes"/> is equal to 3.</param>
        /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="value"/> is equal to 10.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes == null) throw new ArgumentNullException("indexes");
            if (indexes.Length == 1)
            {
                ((IDictionary<string, object>)this)[(string)indexes[0]] = value;
                return true;
            }
            return base.TrySetIndex(binder, indexes, value);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // <pex>
            if (binder == null)
                throw new ArgumentNullException("binder");
            // </pex>
            _members[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var key in Keys)
                yield return key;
        }
#endif
    }
}

namespace RestSharp
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// 
    /// JSON uses Arrays and Objects. These correspond here to the datatypes JsonArray(IList&lt;object>) and JsonObject(IDictionary&lt;string,object>).
    /// All numbers are parsed to doubles.
    /// </summary>
    [GeneratedCode("simple-json", "1.0.0")]
#if SIMPLE_JSON_INTERNAL
    internal
#else
    public
#endif
 static class SimpleJson
    {
        private const int TOKEN_NONE = 0;
        private const int TOKEN_CURLY_OPEN = 1;
        private const int TOKEN_CURLY_CLOSE = 2;
        private const int TOKEN_SQUARED_OPEN = 3;
        private const int TOKEN_SQUARED_CLOSE = 4;
        private const int TOKEN_COLON = 5;
        private const int TOKEN_COMMA = 6;
        private const int TOKEN_STRING = 7;
        private const int TOKEN_NUMBER = 8;
        private const int TOKEN_TRUE = 9;
        private const int TOKEN_FALSE = 10;
        private const int TOKEN_NULL = 11;
        private const int BUILDER_CAPACITY = 2000;

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An IList&lt;object>, a IDictionary&lt;string,object>, a double, a string, null, true, or false</returns>
        public static object DeserializeObject(string json)
        {
            object obj;
            if (TryDeserializeObject(json, out obj))
                return obj;
            throw new SerializationException("Invalid JSON string");
        }

        /// <summary>
        /// Try parsing the json string into a value.
        /// </summary>
        /// <param name="json">
        /// A JSON string.
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// Returns true if successfull otherwise false.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        public static bool TryDeserializeObject(string json, out object obj)
        {
            bool success = true;
            if (json != null)
            {
                char[] charArray = json.ToCharArray();
                int index = 0;
                obj = ParseValue(charArray, ref index, ref success);
            }
            else
                obj = null;

            return success;
        }

        public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            object jsonObject = DeserializeObject(json);
            return type == null || jsonObject != null && ReflectionUtils.IsAssignableFrom(jsonObject.GetType(), type)
                       ? jsonObject
                       : (jsonSerializerStrategy ?? CurrentJsonSerializerStrategy).DeserializeObject(jsonObject, type);
        }

        public static object DeserializeObject(string json, Type type)
        {
            return DeserializeObject(json, type, null);
        }

        public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            return (T)DeserializeObject(json, typeof(T), jsonSerializerStrategy);
        }

        public static T DeserializeObject<T>(string json)
        {
            return (T)DeserializeObject(json, typeof(T), null);
        }

        /// <summary>
        /// Converts a IDictionary&lt;string,object> / IList&lt;object> object into a JSON string
        /// </summary>
        /// <param name="json">A IDictionary&lt;string,object> / IList&lt;object></param>
        /// <param name="jsonSerializerStrategy">Serializer strategy to use</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
            bool success = SerializeValue(jsonSerializerStrategy, json, builder);
            return (success ? builder.ToString() : null);
        }

        public static string SerializeObject(object json)
        {
            return SerializeObject(json, CurrentJsonSerializerStrategy);
        }

        public static string EscapeToJavascriptString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return jsonString;

            StringBuilder sb = new StringBuilder();
            char c;

            for (int i = 0; i < jsonString.Length; )
            {
                c = jsonString[i++];

                if (c == '\\')
                {
                    int remainingLength = jsonString.Length - i;
                    if (remainingLength >= 2)
                    {
                        char lookahead = jsonString[i];
                        if (lookahead == '\\')
                        {
                            sb.Append('\\');
                            ++i;
                        }
                        else if (lookahead == '"')
                        {
                            sb.Append("\"");
                            ++i;
                        }
                        else if (lookahead == 't')
                        {
                            sb.Append('\t');
                            ++i;
                        }
                        else if (lookahead == 'b')
                        {
                            sb.Append('\b');
                            ++i;
                        }
                        else if (lookahead == 'n')
                        {
                            sb.Append('\n');
                            ++i;
                        }
                        else if (lookahead == 'r')
                        {
                            sb.Append('\r');
                            ++i;
                        }
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
        {
            IDictionary<string, object> table = new JsonObject();
            int token;

            // {
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                token = LookAhead(json, index);
                if (token == TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == TOKEN_COMMA)
                    NextToken(json, ref index);
                else if (token == TOKEN_CURLY_CLOSE)
                {
                    NextToken(json, ref index);
                    return table;
                }
                else
                {
                    // name
                    string name = ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }
                    // :
                    token = NextToken(json, ref index);
                    if (token != TOKEN_COLON)
                    {
                        success = false;
                        return null;
                    }
                    // value
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }
                    table[name] = value;
                }
            }
            return table;
        }

        static JsonArray ParseArray(char[] json, ref int index, ref bool success)
        {
            JsonArray array = new JsonArray();

            // [
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == TOKEN_COMMA)
                    NextToken(json, ref index);
                else if (token == TOKEN_SQUARED_CLOSE)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                        return null;
                    array.Add(value);
                }
            }
            return array;
        }

        static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case TOKEN_STRING:
                    return ParseString(json, ref index, ref success);
                case TOKEN_NUMBER:
                    return ParseNumber(json, ref index, ref success);
                case TOKEN_CURLY_OPEN:
                    return ParseObject(json, ref index, ref success);
                case TOKEN_SQUARED_OPEN:
                    return ParseArray(json, ref index, ref success);
                case TOKEN_TRUE:
                    NextToken(json, ref index);
                    return true;
                case TOKEN_FALSE:
                    NextToken(json, ref index);
                    return false;
                case TOKEN_NULL:
                    NextToken(json, ref index);
                    return null;
                case TOKEN_NONE:
                    break;
            }
            success = false;
            return null;
        }

        static string ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
            char c;

            EatWhitespace(json, ref index);

            // "
            c = json[index++];
            bool complete = false;
            while (!complete)
            {
                if (index == json.Length)
                    break;

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {
                    if (index == json.Length)
                        break;
                    c = json[index++];
                    if (c == '"')
                        s.Append('"');
                    else if (c == '\\')
                        s.Append('\\');
                    else if (c == '/')
                        s.Append('/');
                    else if (c == 'b')
                        s.Append('\b');
                    else if (c == 'f')
                        s.Append('\f');
                    else if (c == 'n')
                        s.Append('\n');
                    else if (c == 'r')
                        s.Append('\r');
                    else if (c == 't')
                        s.Append('\t');
                    else if (c == 'u')
                    {
                        int remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;

#if PocketPC
                            try {
                                codePoint = UInt32.Parse(new string(json, index, 4), NumberStyles.HexNumber);
                            } catch (Exception ex) {
                                return "";
                            }
#else
                            if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                                return "";
#endif
                            // convert the integer codepoint to a unicode char and add to string
                            if (0xD800 <= codePoint && codePoint <= 0xDBFF)  // if high surrogate
                            {
                                index += 4; // skip 4 chars
                                remainingLength = json.Length - index;
                                if (remainingLength >= 6)
                                {
                                    uint lowCodePoint = 0;

#if PocketPC
                                    bool lowCodePointPassed = false;
                                    try {
                                        lowCodePoint = UInt32.Parse(new string(json, index + 2, 4), NumberStyles.HexNumber);
                                        lowCodePointPassed = true;
                                    } catch (Exception) { }
                                    if (new string(json, index, 2) == "\\u" && lowCodePointPassed)
#else
                                    if (new string(json, index, 2) == "\\u" && UInt32.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lowCodePoint))
#endif
                                    {
                                        if (0xDC00 <= lowCodePoint && lowCodePoint <= 0xDFFF)    // if low surrogate
                                        {
                                            s.Append((char)codePoint);
                                            s.Append((char)lowCodePoint);
                                            