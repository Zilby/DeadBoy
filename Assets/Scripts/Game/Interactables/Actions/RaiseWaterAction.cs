using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for raising water. 
/// </summary>
public class RaiseWaterAction : InteractAction
{
	[Header("RaiseWater Fields")]
	public float speed = 0.01f;
	public float height = 1;
	public float width = 1;

	public Transform water;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
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
}
