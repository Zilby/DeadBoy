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

	public Transform t;

	protected override List<Func<IEnumerator>> ToggleActions
	{
		get
		{
			return new List<Func<IEnumerator>>() { Raise, Waterfall };
		}
	}

	private IEnumerator Raise()
	{
		while (Mathf.Abs(height) > 0)
		{
			float hspeed = height * speed;
			float yspeed = width * speed;
			t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y + hspeed, t.localPosition.z);
			t.localScale = new Vector3(t.localScale.x, t.localScale.y + yspeed, t.localScale.z);
			height -= hspeed;
			width -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator Waterfall()
	{
		yield return new WaitForSeconds(1.5f);
		look.gameObject.SetActive(true);
	}
}
