using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GenList
{
	public static int CountAllowNull<T>(this IList<T> list)
	{
		return (list == null ? 0 : list.Count);
	}

	public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison)
	{
		int j;
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			T item = list[i];
			for (j = i - 1; j >= 0 && comparison(list[j], item) > 0; j--)
			{
				list[j + 1] = list[j];
			}
			list[j + 1] = item;
		}
	}

	public static List<T> ListFullCopy<T>(this List<T> source)
	{
		List<T> ts = new List<T>(source.Count);
		for (int i = 0; i < source.Count; i++)
		{
			ts.Add(source[i]);
		}
		return ts;
	}

	public static List<T> ListFullCopyOrNull<T>(this List<T> source)
	{
		if (source == null)
		{
			return null;
		}
		return source.ListFullCopy<T>();
	}

	public static bool NullOrEmpty<T>(this IList<T> list)
	{
		return (list == null ? true : list.Count == 0);
	}

	public static void RemoveDuplicates<T>(this List<T> list)
		where T : class
	{
		if (list.Count <= 1)
		{
			return;
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			int num = 0;
			while (num < i)
			{
				if ((object)list[i] != (object)list[num])
				{
					num++;
				}
				else
				{
					list.RemoveAt(i);
					break;
				}
			}
		}
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int count = list.Count;
		while (count > 1)
		{
			count--;
			int item = Rand.RangeInclusive(0, count);
			T t = list[item];
			list[item] = list[count];
			list[count] = t;
		}
	}
}

