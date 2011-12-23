/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
/* ORIGINAL (MIT) license: (from http://techblog.procurios.nl/k/n618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or 
 * substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Runtime.Serialization;

namespace System451.Communication.Dashboard.Utils
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// 
    /// JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
    /// All numbers are parsed to doubles.
    /// </summary>
    public class JSON
    {
        public const int TOKEN_NONE = 0;
        public const int TOKEN_CURLY_OPEN = 1;
        public const int TOKEN_CURLY_CLOSE = 2;
        public const int TOKEN_SQUARED_OPEN = 3;
        public const int TOKEN_SQUARED_CLOSE = 4;
        public const int TOKEN_COLON = 5;
        public const int TOKEN_COMMA = 6;
        public const int TOKEN_STRING = 7;
        public const int TOKEN_NUMBER = 8;
        public const int TOKEN_TRUE = 9;
        public const int TOKEN_FALSE = 10;
        public const int TOKEN_NULL = 11;

        private const int BUILDER_CAPACITY = 2000;

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
        public static JsonObject JsonDecode(string json)
        {
            bool success = true;

            return JsonDecode(json, ref success);
        }

        /// <summary>
        /// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="success">Successful parse?</param>
        /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
        public static JsonObject JsonDecode(string json, ref bool success)
        {
            success = true;
            if (json != null)
            {
                char[] charArray = json.ToCharArray();
                int index = 0;
                JsonObject value = ParseValue(charArray, ref index, ref success);
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a Hashtable / ArrayList object into a JSON string
        /// </summary>
        /// <param name="json">A Hashtable / ArrayList</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string JsonEncode(JsonObject json)
        {
            StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
            bool success = SerializeValue(json, builder);
            return (success ? builder.ToString() : null);
        }

        protected static JsonObject ParseObject(char[] json, ref int index, ref bool success)
        {
            Hashtable table = new Hashtable();
            int token;

            // {
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                token = LookAhead(json, index);
                if (token == JSON.TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == JSON.TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == JSON.TOKEN_CURLY_CLOSE)
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
                    if (token != JSON.TOKEN_COLON)
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

        protected static JsonObject ParseArray(char[] json, ref int index, ref bool success)
        {
            ArrayList array = new ArrayList();

            // [
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == JSON.TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == JSON.TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == JSON.TOKEN_SQUARED_CLOSE)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }

                    array.Add(value);
                }
            }

            return array;
        }

        protected static JsonObject ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case JSON.TOKEN_STRING:
                    return ParseString(json, ref index, ref success);
                case JSON.TOKEN_NUMBER:
                    return ParseNumber(json, ref index, ref success);
                case JSON.TOKEN_CURLY_OPEN:
                    return ParseObject(json, ref index, ref success);
                case JSON.TOKEN_SQUARED_OPEN:
                    return ParseArray(json, ref index, ref success);
                case JSON.TOKEN_TRUE:
                    NextToken(json, ref index);
                    return true;
                case JSON.TOKEN_FALSE:
                    NextToken(json, ref index);
                    return false;
                case JSON.TOKEN_NULL:
                    NextToken(json, ref index);
                    return null;
                case JSON.TOKEN_NONE:
                    break;
            }

            success = false;
            return null;
        }

        protected static JsonObject ParseString(char[] json, ref int index, ref bool success)
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
                {
                    break;
                }

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {

                    if (index == json.Length)
                    {
                        break;
                    }
                    c = json[index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        int remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                            {
                                return "";
                            }
                            // convert the integer codepoint to a unicode char and add to string
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                            // skip 4 chars
                            index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    s.Append(c);
                }

            }

            if (!complete)
            {
                success = false;
                return null;
            }

            return s.ToString();
        }

        protected static JsonObject ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);

            int lastIndex = GetLastIndexOfNumber(json, index);
            int charLength = (lastIndex - index) + 1;

            double number;
            success = Double.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);

            index = lastIndex + 1;
            return number;
        }

        protected static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;

            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }
            return lastIndex - 1;
        }

        protected static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                if (" \t\n\r".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
        }

        protected static int LookAhead(char[] json, int index)
        {
            int saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        protected static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);

            if (index == json.Length)
            {
                return JSON.TOKEN_NONE;
            }

            char c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return JSON.TOKEN_CURLY_OPEN;
                case '}':
                    return JSON.TOKEN_CURLY_CLOSE;
                case '[':
                    return JSON.TOKEN_SQUARED_OPEN;
                case ']':
                    return JSON.TOKEN_SQUARED_CLOSE;
                case ',':
                    return JSON.TOKEN_COMMA;
                case '"':
                    return JSON.TOKEN_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return JSON.TOKEN_NUMBER;
                case ':':
                    return JSON.TOKEN_COLON;
            }
            index--;

            int remainingLength = json.Length - index;

            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' &&
                    json[index + 1] == 'a' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 's' &&
                    json[index + 4] == 'e')
                {
                    index += 5;
                    return JSON.TOKEN_FALSE;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' &&
                    json[index + 1] == 'r' &&
                    json[index + 2] == 'u' &&
                    json[index + 3] == 'e')
                {
                    index += 4;
                    return JSON.TOKEN_TRUE;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' &&
                    json[index + 1] == 'u' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 'l')
                {
                    index += 4;
                    return JSON.TOKEN_NULL;
                }
            }

            return JSON.TOKEN_NONE;
        }

        protected static bool SerializeValue(object value, StringBuilder builder)
        {
            bool success = true;

            if (value is string || (value is JsonObject && (value as JsonObject).Type == typeof(string)))
            {
                success = SerializeString((string)value, builder);
            }
            else if (value is Hashtable || (value is JsonObject && (value as JsonObject).Type == typeof(Hashtable)))
            {
                success = SerializeObject((Hashtable)value, builder);
            }
            else if (value is ArrayList || (value is JsonObject && (value as JsonObject).Type == typeof(ArrayList)))
            {
                success = SerializeArray((ArrayList)value, builder);
            }
            else if (IsNumeric(value) || (value is JsonObject && (value as JsonObject).Type == typeof(double)))
            {
                success = SerializeNumber(Convert.ToDouble(value), builder);
            }
            else if ((value is Boolean) && ((Boolean)value == true) || (value is JsonObject && (value as JsonObject).Type == typeof(bool) && (value as JsonObject).bValue))
            {
                builder.Append("true");
            }
            else if ((value is Boolean) && ((Boolean)value == false) || (value is JsonObject && (value as JsonObject).Type == typeof(bool) && !(value as JsonObject).bValue))
            {
                builder.Append("false");
            }
            else if (value == null || (value is JsonObject && (value as JsonObject).Null))
            {
                builder.Append("null");
            }
            else
            {
                success = false;
            }
            return success;
        }

        protected static bool SerializeObject(Hashtable anObject, StringBuilder builder)
        {
            builder.Append("{");

            IDictionaryEnumerator e = anObject.GetEnumerator();
            bool first = true;
            while (e.MoveNext())
            {
                string key = e.Key.ToString();
                object value = e.Value;

                if (!first)
                {
                    builder.Append(", ");
                }

                SerializeString(key, builder);
                builder.Append(":");
                if (!SerializeValue(value, builder))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("}");
            return true;
        }

        protected static bool SerializeArray(ArrayList anArray, StringBuilder builder)
        {
            builder.Append("[");

            bool first = true;
            for (int i = 0; i < anArray.Count; i++)
            {
                object value = anArray[i];

                if (!first)
                {
                    builder.Append(", ");
                }

                if (!SerializeValue(value, builder))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("]");
            return true;
        }

        protected static bool SerializeString(string aString, StringBuilder builder)
        {
            builder.Append("\"");

            char[] charArray = aString.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                char c = charArray[i];
                if (c == '"')
                {
                    builder.Append("\\\"");
                }
                else if (c == '\\')
                {
                    builder.Append("\\\\");
                }
                else if (c == '\b')
                {
                    builder.Append("\\b");
                }
                else if (c == '\f')
                {
                    builder.Append("\\f");
                }
                else if (c == '\n')
                {
                    builder.Append("\\n");
                }
                else if (c == '\r')
                {
                    builder.Append("\\r");
                }
                else if (c == '\t')
                {
                    builder.Append("\\t");
                }
                else
                {
                    int codepoint = Convert.ToInt32(c);
                    if ((codepoint >= 32) && (codepoint <= 126))
                    {
                        builder.Append(c);
                    }
                    else
                    {
                        builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                    }
                }
            }

            builder.Append("\"");
            return true;
        }

        protected static bool SerializeNumber(double number, StringBuilder builder)
        {
            builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
            return true;
        }

        /// <summary>
        /// Determines if a given object is numeric in any way
        /// (can be integer, double, null, etc). 
        /// 
        /// Thanks to mtighe for pointing out Double.TryParse to me.
        /// </summary>
        protected static bool IsNumeric(object o)
        {
            double result;

            return (o == null) ? false : Double.TryParse(o.ToString(), out result);
        }
    }

    /// <summary>
    /// Easy wrapper for Dynamic Json Objects
    /// </summary>
    [Serializable]
    public class JsonObject : ISerializable
    {
        public JsonObject(Hashtable obj)
        {
            Obj = obj;
            Null = false;
            Type = typeof(Hashtable);
        }

        public JsonObject(ArrayList obj)
        {
            Array = obj;
            Null = false;
            Type = typeof(ArrayList);
        }

        public JsonObject(double obj)
        {
            Dbl = obj;
            Null = false;
            Type = typeof(double);
        }

        public JsonObject(int obj)
        {
            Dbl = obj;
            Null = false;
            Type = typeof(double);
        }

        public JsonObject(bool obj)
        {
            Bool = obj;
            Null = false;
            Type = typeof(bool);
        }

        public JsonObject(string obj)
        {
            Str = obj;
            Null = false;
            Type = typeof(string);
        }

        public JsonObject()
        {
            Null = true;
        }

        public JsonObject(object obj)
        {
            InitWithObject(obj);
        }

        protected void InitWithObject(object obj)
        {
            Null = obj == null;
            if (!Null)
            {
                Type = obj.GetType();
                if (Type == typeof(JsonObject))
                {
                    var jo = obj as JsonObject;
                    this.Array = jo.Array;
                    this.Bool = jo.Bool;
                    this.Dbl = jo.Dbl;
                    this.Null = jo.Null;
                    this.Obj = jo.Obj;
                    this.Str = jo.Str;
                    this.Type = jo.Type;
                }
                else if (Type == typeof(Hashtable))
                {
                    Obj = (Hashtable)obj;
                }
                else if (Type == typeof(ArrayList))
                {
                    Array = (ArrayList)obj;
                }
                else if (Type == typeof(string))
                {
                    Str = obj.ToString();
                }
                else if (Type == typeof(double) || Type == typeof(float) || Type == typeof(int))
                {
                    Dbl = (double)obj;
                }
                else if (Type == typeof(bool))
                {
                    Bool = (bool)obj;
                }
                else
                    Null = true;
            }
        }

        public Hashtable Obj { get; protected set; }
        public ArrayList Array { get; protected set; }
        public string Str { get; protected set; }
        public double? Dbl { get; protected set; }
        public bool? Bool { get; protected set; }
        public bool Null { get; protected set; }
        public Type Type { get; protected set; }

        public object ToRaw()
        {
            if (!Null)
            {
                if (Type == typeof(Hashtable))
                {
                    return Obj;
                }
                else if (Type == typeof(ArrayList))
                {
                    return Array;
                }
                else if (Type == typeof(string))
                {
                    return Str;
                }
                else if (Type == typeof(double) || Type == typeof(float) || Type == typeof(int))
                {
                    return (double)Dbl;
                }
                else if (Type == typeof(bool))
                {
                    return (bool)Bool;
                }
            }
            return null;
        }

        public JsonObject this[int id]
        {
            get
            {
                return new JsonObject(this.Array[id]);
            }
            set
            {
                this.Array[id] = value;
            }
        }

        public JsonObject this[string id]
        {
            get
            {
                return new JsonObject(this.Obj[id]);
            }
            set
            {
                this.Obj[id] = value;
            }
        }

        public string Value
        {
            get
            {
                if (Str == null)
                {
                    if (Bool != null)
                        return Bool.ToString();
                    return dValue.ToString();
                }
                return Str;
            }
        }

        public double dValue
        {
            get
            {
                if (Dbl == null)
                    return Bool == true ? 1 : 0;
                return (double)Dbl;
            }
        }

        public int iValue
        {
            get
            {
                return (int)dValue;
            }
        }

        public bool bValue
        {
            get
            {
                return dValue == 0;
            }
        }

        public static implicit operator Hashtable(JsonObject jobj)
        {
            return jobj.Obj;
        }

        public static implicit operator ArrayList(JsonObject jobj)
        {
            return jobj.Array;
        }

        public static implicit operator string(JsonObject jobj)
        {
            return jobj.Str;
        }

        public static implicit operator bool(JsonObject jobj)
        {
            return (bool)jobj.Bool;
        }

        public static implicit operator double(JsonObject jobj)
        {
            return (double)jobj.Dbl;
        }

        public static implicit operator int(JsonObject jobj)
        {
            return (int)jobj.Dbl;
        }



        public static implicit operator JsonObject(int jobj)
        {
            return new JsonObject(jobj);
        }

        public static implicit operator JsonObject(double jobj)
        {
            return new JsonObject(jobj);
        }

        public static implicit operator JsonObject(bool jobj)
        {
            return new JsonObject(jobj);
        }

        public static implicit operator JsonObject(string jobj)
        {
            return new JsonObject(jobj);
        }

        public static implicit operator JsonObject(ArrayList jobj)
        {
            return new JsonObject(jobj);
        }

        public static implicit operator JsonObject(Hashtable jobj)
        {
            return new JsonObject(jobj);
        }

        private JsonObject(SerializationInfo si, StreamingContext ctx)
        {
            bool success = true;
            object obj = JSON.JsonDecode(si.GetString("root"), ref success);
            if (!success)
                throw new InvalidOperationException("Serialization is invalid");
            InitWithObject(obj);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctx)
        {
            info.AddValue("root", JSON.JsonEncode(this));
        }
    }
}