using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for toggling particle systems emission state. 
/// </summary>
public class ParticleSystemToggleAction : InteractAction
{
	public List<ParticleSystem> toggled;
	public float delay = 1.5f;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		yield return new WaitForSeconds(delay);
		foreach (ParticleSystem ps in toggled)
		{
			var e = ps.emission;
			e.enabled = !e.enabled;
		}
	}
}
