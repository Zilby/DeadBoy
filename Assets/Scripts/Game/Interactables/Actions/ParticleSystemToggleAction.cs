using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for toggling particle systems emission state. 
/// </summary>
public class ParticleSystemToggleAction : InteractAction
{
	public List<ParticleSystem> toggled;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		foreach (ParticleSystem ps in toggled)
		{
			var e = ps.emission;
			e.enabled = !e.enabled;
		}
	}
}
