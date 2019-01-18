using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for moving an object. 
/// </summary>
public class MoveAction : InteractAction
{
	[Header("Move Action Fields")]
	public float speed = 0.01f;
	public Vector2 location;
	public Transform t;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		yield return Utils.MoveToLocation(t, location, speed);
	}

}
