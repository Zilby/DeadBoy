using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseWater : Lever
{
	[Header("RaiseWater Fields")]
	public float speed = 0.01f;
	public float height = 1;
	public float width = 1;

	public Transform water;
	public Transform wall;

	protected override List<Func<IEnumerator>> ToggleActions
	{
		get
		{
			return new List<Func<IEnumerator>>() { Raise, Waterfall, Lower };
		}
	}

	private IEnumerator Raise()
	{
		while (Mathf.Abs(height) > 0)
		{
			float hspeed = height * speed;
			float yspeed = width * speed;
			water.localPosition = water.localPosition.YAdd(hspeed);
			water.localScale = water.localScale.YAdd(yspeed);
			height -= hspeed;
			width -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator Waterfall()
	{
		yield return new WaitForSeconds(1.5f);
		lookAts[0].gameObject.SetActive(true);
	}

	private IEnumerator Lower()
	{
		while (wall.localPosition.y > -2f)
		{
			wall.localPosition = wall.localPosition.YAdd(-speed * 2);
			yield return new WaitForFixedUpdate();
		}
	}
}
