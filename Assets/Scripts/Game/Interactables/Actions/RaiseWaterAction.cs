using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for raising water. 
/// </summary>
public class RaiseWaterAction : InteractAction
{
	[Header("RaiseWater Fields")]
	/// <summary>
	/// The speed.
	/// </summary>
	public float speed = 0.01f;
	/// <summary>
	/// The percentage to raise the water. 
	/// </summary>
	[Range(-200f, 500f)]
	public float percentage = 50f;
	/// <summary>
	/// The water.
	/// </summary>
	public Transform water;


	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		percentage /= 100f;
		float width = percentage * water.localScale.y;
		float height = (percentage / 2) * water.localScale.y * water.GetComponent<BoxCollider2D>().size.y;
		while (Mathf.Abs(height) > 0)
		{
			float hspeed = height * speed;
			float yspeed = width * speed;
			water.localPosition = water.localPosition.YAdd(hspeed);
			water.localScale = water.localScale.YAdd(yspeed);
			height -= hspeed;
			width -= yspeed;
			yield return new WaitForFixedUpdate();
			if (water.localScale.y <= 0)
			{
				Destroy(water.gameObject);
				break;
			}
		}
	}
}
