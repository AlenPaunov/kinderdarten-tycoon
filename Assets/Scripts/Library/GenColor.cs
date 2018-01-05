using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public static class GenColor
{
	public static Color FromBytes(int r, int g, int b, int a = 255)
	{
		Color color = new Color()
		{
			r = (float)r / 255f,
			g = (float)g / 255f,
			b = (float)b / 255f,
			a = (float)a / 255f
		};
		return color;
	}

	public static Color FromHex(string hex)
	{
		if (hex.StartsWith("#"))
		{
			hex = hex.Substring(1);
		}
		if (hex.Length != 6 && hex.Length != 8)
		{
			Debug.LogError(string.Concat(hex, " is not a valid hex color."));
			return Color.white;
		}
		int num = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		int num1 = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		int num2 = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		int num3 = 255;
		if (hex.Length == 8)
		{
			num3 = int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
		}
		return GenColor.FromBytes(num, num1, num2, num3);
	}

	public static bool IndistinguishableFrom(this Color colA, Color colB)
	{
		Color color = colA - colB;
		return Mathf.Abs(color.r) + Mathf.Abs(color.g) + Mathf.Abs(color.b) + Mathf.Abs(color.a) < 0.001f;
	}

	public static Color RandomColorOpaque()
	{
		return new Color(Rand.Value, Rand.Value, Rand.Value, 1f);
	}

	public static Color SaturationChanged(this Color col, float change)
	{
		float single = col.r;
		float single1 = col.g;
		float single2 = col.b;
		float single3 = Mathf.Sqrt(single * single * 0.299f + single1 * single1 * 0.587f + single2 * single2 * 0.114f);
		single = single3 + (single - single3) * change;
		single1 = single3 + (single1 - single3) * change;
		single2 = single3 + (single2 - single3) * change;
		return new Color(single, single1, single2);
	}
}

