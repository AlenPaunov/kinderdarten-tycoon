using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumberGenerator_BasicHash : RandomNumberGenerator
{
	private const uint Prime1 = 2654435761;

	private const uint Prime2 = 2246822519;

	private const uint Prime3 = 3266489917;

	private const uint Prime4 = 668265263;

	private const uint Prime5 = 374761393;

	public RandomNumberGenerator_BasicHash()
	{
	}

	private uint GetHash(int buffer)
	{
		uint num = this.seed + 374761393;
		num += 4;
		num = (uint)(num + buffer * -1028477379);
		num = RandomNumberGenerator_BasicHash.Rotate(num, 17) * 668265263;
		num = num ^ num >> 15;
		unchecked{
			num *= (uint)-2048144777;
		}
		num = num ^ num >> 13;
		unchecked{
			num *= (uint)-1028477379;
		}
		num = num ^ num >> 16;
		return num;
	}

	public override int GetInt(uint iterations)
	{
		return (int)this.GetHash((int)iterations);
	}

	private static uint Rotate(uint value, int count)
	{
		return value << (count & 31) | value >> (32 - count & 31);
	}
}
