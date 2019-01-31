using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundSwapper : MonoBehaviour
{
	public bool underground = false;

	public static Action<bool> SwapEvent;

	private Fadeable[] fadeables;

	void Awake()
	{
		fadeables = GetComponentsInChildren<Fadeable>();
		SwapEvent += Swap;
	}

	private void OnDestroy()
	{
		SwapEvent -= Swap;
	}

	void Swap(bool down)
	{
		foreach (Fadeable f in fadeables)
		{
			if (down && !underground)
			{
				f.SelfFadeIn();
			}
			else if (!down)
			{
				f.SelfFadeOut();
			}
		}
	}
}
