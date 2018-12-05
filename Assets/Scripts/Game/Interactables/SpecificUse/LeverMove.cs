using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverMove : Lever
{
	[Header("LeverSlide Fields")]
	public float speed = 0.01f;
	public Vector2 location;
	public Transform t;

	protected override List<Func<IEnumerator>> ToggleActions
	{
		get
		{
			return new List<Func<IEnumerator>>() { delegate { return Utils.MoveToLocation(t, location, speed); } };
		}
	}
}
