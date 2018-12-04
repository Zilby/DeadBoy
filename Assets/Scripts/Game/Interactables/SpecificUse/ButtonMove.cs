using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : PressableButton
{
	[Header("ButtonMove Fields")]

	public float speed = 0.01f;
	public float xdist = 0;
	public float ydist = 0;

	public Transform t;


	protected override List<Func<IEnumerator>> ToggleActions
	{
		get
		{
			return new List<Func<IEnumerator>>() { MoveToLocation };
		}
	}

	private IEnumerator MoveToLocation()
	{
		while (Mathf.Abs(xdist) > 0 || Mathf.Abs(ydist) > 0)
		{
			float xspeed = xdist * speed;
			float yspeed = ydist * speed;
			t.position = new Vector3(t.position.x + xspeed, t.position.y + yspeed, t.position.z);
			xdist -= xspeed;
			ydist -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}
}
