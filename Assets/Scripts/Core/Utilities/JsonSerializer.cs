using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class JsonSerializer
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

	public static object JsonDecode(byte[] json)
	{
		return JsonDecode(System.Text.ASCIIEncoding.ASCII.GetString(json));
	}

	public static object JsonDecode(string json)
	{
		bool success = true;
		return JsonDecode(json, ref success);
	}

	public static object JsonDecode(string json, ref bool success)
	{
		success = true;
		if (json != null)
		{
			char[] charArray = json.ToCharArray();
			int index = 0;
			object value = ParseValue(charArray, ref index, ref success);
			return value;
		}
			
		return null;
	}

	public static string JsonEncode(object json, bool isPretty = false)
	{
		StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
		bool success = SerializeValue(json, builder, isPretty);
		return success ? builder.ToString() : null;
	}

	protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
	{
		Hashtable table = new Hashtable();
		int token;

		NextToken(json, ref index);

		bool done = false;
		while (!done)
		{
			token = LookAhead(json, index);
			if (token == JsonSerializer.TOKEN_NONE)
			{
				success = false;
				return null;
			}
			else if (token == TOKEN_COMMA)
			{
				NextToken(json, ref index);
			}
			else if (token == TOKEN_CURLY_CLOSE)
			{
				NextToken(json, ref index);
				return table;
			}
			else
			{
				string name = ParseString(json, ref index, ref success);
				if (!success)
				{
					success = false;
					return null;
				}

				token = NextToken(json, ref index);
				if (token != TOKEN_COLON)
				{
					success = false;
					return null;
				}

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

	protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
	{
		ArrayList array = new ArrayList();

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
			{
				NextToken(json, ref index);
			}
			else if (token == TOKEN_SQUARED_CLOSE)
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

	protected static object ParseValue(char[] json, ref int index, ref bool success)
	{
		switch(LookAhead(json, index))
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

	protected static string ParseString(char[] json, ref int index, ref bool success)
	{
		StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
		char c;

		EatWhitespace(json, ref index);

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
						uint codePoint;

						const int lowerRange = 0xD800;
						const int upperRange = 0xDBFF;
						string highString = new string(json, index, 4);
						int highSurrogate = Convert.ToInt32(highString, 16);

						if (highSurrogate >= lowerRange && highSurrogate <= upperRange)
						{
							index += 6;
							string lowString = new string(json, index, 4);
							int lowSurrogate = Convert.ToInt32(lowString, 16);
							int codePointTest = Char.ConvertToUtf32((char)highSurrogate, (char)lowSurrogate);

							string converted = Char.ConvertFromUtf32(codePointTest);

							s.Append(converted);

							index += 4;
						}
						else
						{
							if (!(success = UInt32.TryParse(new string(json, index, 4), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
							{
								return "";
							}

							s.Append(Char.ConvertFromUtf32((int)codePoint));
							index += 4;
						}
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

	protected static object ParseNumber(char[] json, ref int index, ref bool success)
	{
		EatWhitespace(json, ref index);

		int lastIndex = GetLastIndexOfNumber(json, index);
		int charLength = (lastIndex - index) + 1;

		string token = new string(json, index, charLength);
		index = lastIndex + 1;
		if (token.Contains("."))
		{
			success = float.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out float number);
			return number;
		}
		else if (token.Length < 10)
		{
			success = int.TryParse(token, out int number);
			return number;
		}
		else if (token.Length < 12)
		{
			success = long.TryParse(token, out long longNumber);
			if (Math.Abs(longNumber) <= int.MaxValue)
			{
				int intNumber = (int)longNumber;
				return intNumber;
			}
			else
			{
				return longNumber;
			}
		}
		else
		{
			success = long.TryParse(token, out long number);
			return number;
		}
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

	private static void EatWhitespace(char[] json, ref int index)
	{
		for (; index < json.Length; index++)
		{
			if (" \t\n\r".IndexOf(json[index]) == -1)
			{
				break;
			}
		}
	}

	private static int NextToken(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);

		if (index == json.Length)
		{
			return TOKEN_NONE;
		}

		char c = json[index];
		index++;
		switch (c)
		{
			case '{':
				return TOKEN_CURLY_OPEN;
			case '}':
				return TOKEN_CURLY_CLOSE;
			case '[':
				return TOKEN_SQUARED_OPEN;
			case ']':
				return TOKEN_SQUARED_CLOSE;
			case ',':
				return TOKEN_COMMA;
			case '"':
				return TOKEN_STRING;
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
				return TOKEN_NUMBER;
			case ':':
				return TOKEN_COLON;
		}
		index--;

		int remainingLength = json.Length - index;

		if (remainingLength >= 5)
		{
			if (json[index] == 'f' && 
				json[index + 1] == 'a' &&
				json[index + 2] == 'l' &&
				json[index + 3] == 's' &&
				json[index + 4] == 'e')
			{
				index += 5;
				return TOKEN_FALSE;
			}
		}

		if (remainingLength >= 4)
		{

			if (json[index] == 't' &&
				json[index + 1] == 'r' &&
				json[index + 2] == 'u' &&
				json[index + 3] == 'e')
			{
				index += 4;
				return TOKEN_TRUE;
			}
		}

		if (remainingLength >= 4)
		{
			if (json[index] == 'n' &&
				json[index + 1] == 'u' &&
				json[index + 2] == 'l' &&
				json[index + 3] == 'l')
			{
				index += 4;
				return TOKEN_NULL;
			}
		}

		return TOKEN_NONE;
	}

	protected static int LookAhead(char[] json, int index)
	{
		int saveIndex = index;
		return NextToken(json, ref saveIndex);
	}

	protected static bool SerializeValue(object value, StringBuilder builder, bool isPretty = false, int prettyLevel = 0)
	{
		bool success = true;

		if (value is string)
		{
			success = SerializeString((string)value, builder);
		}
		else if (value is Hashtable)
		{
			success = SerializeObject((Hashtable)value, builder, isPretty, prettyLevel);
		}
		else if (value is ArrayList)
		{
			success = SerializeArray((ArrayList)value, builder);
		}
		else if (IsNumeric(value))
		{
			success = SerializeNumber(Convert.ToDouble(value), builder);
		}
		else if ((value is Boolean) && ((Boolean)value == true))
		{
			builder.Append("true");
		}
		else if ((value is Boolean) && ((Boolean)value == false))
		{
			builder.Append("false");
		}
		else if (value == null)
		{
			builder.Append("null");
		}
		else if (value.GetType().IsArray)
		{
			success = SerializeArray((object[])value, builder);
		}
		else
		{
			success = false;
		}

		return success;
	}

	protected static bool SerializeObject(Hashtable anObject, StringBuilder builder, bool isPretty = false, int prettyLevel = 0)
	{
		builder.Append("{");
		if (isPretty)
		{
			builder.Append("\n");
		}

		List<string> keys = new List<string>(anObject.Keys.Cast<string>().ToList());
		if (isPretty)
		{
			keys.Sort();
		}
		bool first = true;
		foreach (string key in keys)
		{
			object value = anObject[key];

			if (!first)
			{
				if (isPretty)
				{
					builder.Append(",\n");
				}
				else
				{
					builder.Append(", ");
				}
			}
			if (isPretty)
			{
				builder.Append(new String(' ', (prettyLevel + 1) * 3));
			}

			SerializeString(key, builder);
			builder.Append(":");
			if (isPretty)
			{
				builder.Append(" ");
			}

			if (!SerializeValue(value, builder, isPretty, prettyLevel + 1))
			{
				return false;
			}

			first = false;
		}

		if (isPretty)
		{
			builder.Append("\n" + new string(' ', prettyLevel * 3));
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
	protected static bool SerializeArray(object[] anArray, StringBuilder builder)
	{
		builder.Append("[");

		bool first = true;
		for (int i = 0; i < anArray.Length; i++)
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
				int codePoint = Convert.ToInt32(c);
				if ((codePoint >= 32) && (codePoint <= 126))
				{
					builder.Append(c);
				}
				else
				{
					builder.Append("\\u" + Convert.ToString(codePoint, 16).PadLeft(4, '0'));
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

	protected static bool IsNumeric(object o)
	{
		double result;

		return (o == null) ? false : Double.TryParse(o.ToString(), out result);
	}
}
