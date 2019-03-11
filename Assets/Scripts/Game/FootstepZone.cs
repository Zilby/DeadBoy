using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes player's footsteps while in the zone. 
/// </summary>
public class FootstepZone : MonoBehaviour
{
	public SFXManager.FootstepType footstepType;

	private Dictionary<PlayerController, SFXManager.FootstepType> previousFootsteps;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			PlayerController p = other.GetComponent<PlayerController>();
			if (p)
			{
				previousFootsteps[p] = p.FootstepType;
				p.FootstepType = footstepType;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			PlayerController p = other.GetComponent<PlayerController>();
			if (p && previousFootsteps.ContainsKey(p))
			{
				p.FootstepType = previousFootsteps[p];
			}
		}
	}
}
