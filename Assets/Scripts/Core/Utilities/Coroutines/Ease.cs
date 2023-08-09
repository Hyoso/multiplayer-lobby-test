using System.Collections.Generic;
using UnityEngine;

public class Ease
{
	public delegate float Function(float value);

	public readonly static Dictionary<Type, Function> EaseFunctions = new Dictionary<Type, Function>
	{
		{ Type.LINEAR, Linear }
	};

	public enum Type
	{
		LINEAR
	}

	public static Function GetEase(Type type)
	{
		if (!EaseFunctions.ContainsKey(type))
		{
			Debug.LogError("No such easing function: " + type);
		}

		return EaseFunctions[type];
	}

	public static Function GetEase(AnimationCurve curve)
	{
		if (curve != null)
		{
			Function f = delegate (float t)
			{
				return curve.Evaluate(t);
			};

			return f;
		}

		Debug.LogError("Curve cannot be null");
		return null;
	}

	public static float Linear(float x)
	{
		return x;
	}
}
