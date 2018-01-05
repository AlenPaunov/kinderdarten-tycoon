using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

public static class Rand
{
	private static Stack<ulong> stateStack;

	private static RandomNumberGenerator random;

	private static uint iterations;

	private static List<int> tmpRange;

	public static bool Bool
	{
		get
		{
			return Rand.Value < 0.5f;
		}
	}

	public static int Int
	{
		get
		{
			RandomNumberGenerator randomNumberGenerator = Rand.random;
			uint num = Rand.iterations;
			Rand.iterations = num + 1;
			return randomNumberGenerator.GetInt(num);
		}
	}

	public static Vector3 PointOnDisc
	{
		get
		{
			Vector3 vector3;
			while (true)
			{
				vector3 = new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f) * 2f;
				if (vector3.sqrMagnitude <= 1f)
				{
					break;
				}
			}
			return vector3;
		}
	}

	public static Vector3 PointOnSphere
	{
		get
		{
			Vector3 vector3 = new Vector3(Rand.Gaussian(0f, 1f), Rand.Gaussian(0f, 1f), Rand.Gaussian(0f, 1f));
			return vector3.normalized;
		}
	}

	public static int Seed
	{
		set
		{
			Rand.random.seed = (uint)value;
			Rand.iterations = 0;
		}
	}

	private static ulong StateCompressed
	{
		get
		{
			return (ulong)Rand.random.seed | (ulong)Rand.iterations << 32;
		}
		set
		{
			unchecked {
				Rand.random.seed = (uint)(value & (ulong)-1);
				Rand.iterations = (uint)(value >> 32 & (ulong)-1);
			}

		}
	}

	public static float Value
	{
		get
		{
			RandomNumberGenerator randomNumberGenerator = Rand.random;
			uint num = Rand.iterations;
			Rand.iterations = num + 1;
			return randomNumberGenerator.GetFloat(num);
		}
	}

	static Rand()
	{
		Rand.stateStack = new Stack<ulong>();
		Rand.random = new RandomNumberGenerator_BasicHash();
		Rand.iterations = 0;
		Rand.tmpRange = new List<int>();
	}

	public static bool Chance(float chance)
	{
		if (chance >= 1f)
		{
			return true;
		}
		return Rand.Value < chance;
	}

	public static bool ChanceSeeded(float chance, int specialSeed)
	{
		ulong stateCompressed = Rand.StateCompressed;
		Rand.Seed = specialSeed;
		bool flag = Rand.Chance(chance);
		Rand.StateCompressed = stateCompressed;
		return flag;
	}

	public static T Element<T>(T a, T b)
	{
		return (!Rand.Bool ? b : a);
	}

	public static T Element<T>(T a, T b, T c)
	{
		float value = Rand.Value;
		if (value < 0.33333f)
		{
			return a;
		}
		if (value < 0.66666f)
		{
			return b;
		}
		return c;
	}

	public static T Element<T>(T a, T b, T c, T d)
	{
		float value = Rand.Value;
		if (value < 0.25f)
		{
			return a;
		}
		if (value < 0.5f)
		{
			return b;
		}
		if (value < 0.75f)
		{
			return c;
		}
		return d;
	}

	public static T Element<T>(T a, T b, T c, T d, T e)
	{
		float value = Rand.Value;
		if (value < 0.2f)
		{
			return a;
		}
		if (value < 0.4f)
		{
			return b;
		}
		if (value < 0.6f)
		{
			return c;
		}
		if (value < 0.8f)
		{
			return d;
		}
		return e;
	}

	public static T Element<T>(T a, T b, T c, T d, T e, T f)
	{
		float value = Rand.Value;
		if (value < 0.16666f)
		{
			return a;
		}
		if (value < 0.33333f)
		{
			return b;
		}
		if (value < 0.5f)
		{
			return c;
		}
		if (value < 0.66666f)
		{
			return d;
		}
		if (value < 0.83333f)
		{
			return e;
		}
		return f;
	}

	public static void EnsureStateStackEmpty()
	{
		if (Rand.stateStack.Any<ulong>())
		{
			Debug.LogWarning ("Random state stack is not empty. There were more calls to PushState than PopState. Fixing.");
			Rand.stateStack.Clear();
		}
	}

	public static float Gaussian(float centerX = 0, float widthFactor = 1)
	{
		float value = Rand.Value;
		float single = Rand.Value;
		float single1 = Mathf.Sqrt(-2f * Mathf.Log(value)) * Mathf.Sin(6.28318548f * single);
		return single1 * widthFactor + centerX;
	}

	public static float GaussianAsymmetric(float centerX = 0, float lowerWidthFactor = 1, float upperWidthFactor = 1)
	{
		float value = Rand.Value;
		float single = Rand.Value;
		float single1 = Mathf.Sqrt(-2f * Mathf.Log(value)) * Mathf.Sin(6.28318548f * single);
		if (single1 <= 0f)
		{
			return single1 * lowerWidthFactor + centerX;
		}
		return single1 * upperWidthFactor + centerX;
	}

	internal static void LogRandTests()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = Rand.Int;
		stringBuilder.AppendLine(string.Concat("Repeating single ValueSeeded with seed ", num, ". This should give the same result:"));
		for (int i = 0; i < 4; i++)
		{
			stringBuilder.AppendLine(string.Concat("   ", Rand.ValueSeeded(num)));
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Long-term tests");
		for (int j = 0; j < 3; j++)
		{
			int num1 = 0;
			for (int k = 0; k < 5000000; k++)
			{
				if (Rand.MTBEventOccurs(250f, 60000f, 60f))
				{
					num1++;
				}
			}
			stringBuilder.AppendLine(string.Concat(new object[] { "MTB=", 250, " days, MTBUnit=", 60000, ", check duration=", 60, " Simulated ", 5000, " days (", 5000000, " tests). Got ", num1, " events." }));
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Short-term tests");
		for (int l = 0; l < 5; l++)
		{
			int num2 = 0;
			for (int m = 0; m < 10000; m++)
			{
				if (Rand.MTBEventOccurs(1f, 24000f, 12000f))
				{
					num2++;
				}
			}
			stringBuilder.AppendLine(string.Concat(new object[] { "MTB=", 1f, " days, MTBUnit=", 24000f, ", check duration=", 12000f, ", ", 10000, " tests got ", num2, " events." }));
		}
		for (int n = 0; n < 5; n++)
		{
			int num3 = 0;
			for (int o = 0; o < 10000; o++)
			{
				if (Rand.MTBEventOccurs(2f, 24000f, 6000f))
				{
					num3++;
				}
			}
			stringBuilder.AppendLine(string.Concat(new object[] { "MTB=", 2f, " days, MTBUnit=", 24000f, ", check duration=", 6000f, ", ", 10000, " tests got ", num3, " events." }));
		}
		Debug.Log(stringBuilder.ToString());
	}

	public static bool MTBEventOccurs(float mtb, float mtbUnit, float checkDuration)
	{
		if (mtb == Single.PositiveInfinity)
		{
			return false;
		}
		if (mtb <= 0f)
		{
			Debug.LogError(string.Concat("MTBEventOccurs with mtb=", mtb));
			return true;
		}
		if (mtbUnit <= 0f)
		{
			Debug.LogError(string.Concat("MTBEventOccurs with mtbUnit=", mtbUnit));
			return false;
		}
		if (checkDuration <= 0f)
		{
			Debug.LogError(string.Concat("MTBEventOccurs with checkDuration=", checkDuration));
			return false;
		}
		double num = (double)checkDuration / ((double)mtb * (double)mtbUnit);
		if (num > 0)
		{
			double num1 = 1;
			if (num < 0.0001)
			{
				while (num < 0.0001)
				{
					num *= 8;
					num1 /= 8;
				}
				if ((double)Rand.Value > num1)
				{
					return false;
				}
			}
			return (double)Rand.Value < num;
		}
		Debug.LogError(string.Concat(new object[] { "chancePerCheck is ", num, ". mtb=", mtb, ", mtbUnit=", mtbUnit, ", checkDuration=", checkDuration }));
		return false;
	}

	public static Vector3 PointOnSphereCap(Vector3 center, float angle)
	{
		if (angle <= 0f)
		{
			return center;
		}
		if (angle >= 180f)
		{
			return Rand.PointOnSphere;
		}
		float single = Rand.Range(Mathf.Cos(angle * 0.0174532924f), 1f);
		float single1 = Rand.Range(0f, 6.28318548f);
		Vector3 vector3 = new Vector3(Mathf.Sqrt(1f - single * single) * Mathf.Cos(single1), Mathf.Sqrt(1f - single * single) * Mathf.Sin(single1), single);
		return Quaternion.FromToRotation(Vector3.forward, center) * vector3;
	}

	public static void PopState()
	{
		Rand.StateCompressed = Rand.stateStack.Pop();
	}

	public static void PushState()
	{
		Rand.stateStack.Push(Rand.StateCompressed);
	}

	public static void PushState(int replacementSeed)
	{
		Rand.PushState();
		Rand.Seed = replacementSeed;
	}

	public static void RandomizeStateFromTime()
	{
		Rand.Seed = DateTime.Now.GetHashCode();
	}

//	public static int RandSeedForHour(this Thing t, int salt)
//	{
//		int num = t.HashOffset();
//		num = Gen.HashCombineInt(num, Find.TickManager.TicksAbs / 2500);
//		return Gen.HashCombineInt(num, salt);
//	}

	public static int Range(int min, int max)
	{
		if (max <= min)
		{
			return min;
		}
		RandomNumberGenerator randomNumberGenerator = Rand.random;
		uint num = Rand.iterations;
		Rand.iterations = num + 1;
		return min + Mathf.Abs(randomNumberGenerator.GetInt(num) % (max - min));
	}

	public static float Range(float min, float max)
	{
		if (max <= min)
		{
			return min;
		}
		return Rand.Value * (max - min) + min;
	}

	public static int RangeInclusive(int min, int max)
	{
		if (max <= min)
		{
			return min;
		}
		return Rand.Range(min, max + 1);
	}

	public static bool TryRangeInclusiveWhere(int from, int to, Predicate<int> predicate, out int value)
	{
		int num = to - from + 1;
		int num1 = Mathf.Max(Mathf.RoundToInt(Mathf.Sqrt((float)num)), 5);
		for (int i = 0; i < num1; i++)
		{
			int num2 = Rand.RangeInclusive(from, to);
			if (predicate(num2))
			{
				value = num2;
				return true;
			}
		}
		Rand.tmpRange.Clear();
		for (int j = from; j <= to; j++)
		{
			Rand.tmpRange.Add(j);
		}
		Rand.tmpRange.Shuffle<int>();
		int num3 = 0;
		int count = Rand.tmpRange.Count;
		while (num3 < count)
		{
			if (predicate(Rand.tmpRange[num3]))
			{
				value = Rand.tmpRange[num3];
				return true;
			}
			num3++;
		}
		value = 0;
		return false;
	}

	public static float ValueSeeded(int specialSeed)
	{
		ulong stateCompressed = Rand.StateCompressed;
		Rand.Seed = specialSeed;
		float value = Rand.Value;
		Rand.StateCompressed = stateCompressed;
		return value;
	}
}
