using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for breaking a joint. 
/// </summary>
public class BreakJointAction : InteractAction
{
	public Joint2D joint;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		Destroy(joint);
        yield return null;
	}
}
