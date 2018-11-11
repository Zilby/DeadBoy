using System.Collections.Generic;
using UnityEngine;
 /// <summary>
/// Used for making lists of lists in the inspector. 
/// </summary>
[System.Serializable]
public class ListWrapper<T>
{
	public List<T> list;
 	public T this[int key]
	{
		get
		{
			return list[key];
		}
		set
		{
			list[key] = value;
		}
	}
 	public int Count
	{
		get
		{
			return list.Count;
		}
	}
} 