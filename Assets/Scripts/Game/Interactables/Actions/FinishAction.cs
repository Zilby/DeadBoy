using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for being able to finish a level. 
/// </summary>
public class FinishAction : InteractAction
{
	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		LevelManager.instance.CanNowFinish();
	}

}
