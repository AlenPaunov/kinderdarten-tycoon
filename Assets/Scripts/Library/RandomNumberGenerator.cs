using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RandomNumberGenerator
{
	public uint seed = (uint)DateTime.Now.GetHashCode();

	protected RandomNumberGenerator()
	{
	}

	public float GetFloat(uint iterations)
	{
		return (float)(((double)this.GetInt(iterations) - -2147483648) / 4294967295);
	}

	public abstract int GetInt(uint iterations);
}

