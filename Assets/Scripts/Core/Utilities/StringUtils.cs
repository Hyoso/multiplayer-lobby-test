using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public static class StringUtils
{
	public static string ConvertToConstCase(this string input)
	{
		char separatedChar = '_';
		string output = "";
		bool canAdd = false;

		for (int i = 0; i < input.Length; i++)
		{
			if (char.IsLower(input[i]))
			{
				canAdd = true;
			}

			if (i != 0 && char.IsUpper(input[i]) && canAdd)
			{
				output += separatedChar;
				canAdd = false;
			}

			output += input[i];
		}

		output = output.ToUpper();
		output = Regex.Replace(output, @"\s+", "");
		return output;
	}

	public static string ToReadableString(this TimeSpan span)
	{
		string formatted = string.Format("{0}{1}{2}{3}",
			span.Duration().Days > 0 ? string.Format("{0:0}:", span.Days) : string.Empty,
			span.Duration().Hours > 0 ? string.Format("{0:0}:", span.Hours) : string.Empty,
			span.Duration().Minutes > 0 ? string.Format("{0:0}:", span.Minutes) : string.Empty,
			span.Duration().Seconds > 0 ? string.Format("{0:0}", span.Seconds) : string.Empty);

		if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

		if (string.IsNullOrEmpty(formatted)) formatted = "";

		return formatted;
	}

	public static string ToCondensedNumber(this float num)
	{
		string formatted = "";
		if (num == float.MaxValue)
		{
			formatted = "MAX";
		}
		else
		{
			formatted =
			num < 1000 ? ((int)num).ToString() :
			num < 1000000 ? ((int)(num / 1000)).ToString() + "K" :
			num < 1000000000 ? ((int)(num / 1000000)).ToString() + "M" :
			num < 1000000000000 ? ((int)(num / 1000000000)).ToString() + "B" :
			((int)(num / 1000000000000)).ToString() + "E";
		}


		if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

		if (string.IsNullOrEmpty(formatted)) formatted = "";

		return formatted;
	}
	public static string FirstCharToUpper(this string input)
	{
		switch (input)
		{
			case null: throw new ArgumentNullException(nameof(input));
			case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
			default: return input[0].ToString().ToUpper() + input.Substring(1);
		}
	}
}
