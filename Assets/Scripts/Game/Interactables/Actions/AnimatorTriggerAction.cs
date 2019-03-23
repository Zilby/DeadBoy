using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTriggerAction : InteractAction
{

	public Animator anim;
	public string trigger;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		anim.SetTrigger(trigger);
	}
}
