using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for toggling an object's enabled state. 
/// </summary>
public class ToggleEnabledAction : InteractAction
{
	public MonoBehaviour toggled;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		toggled.enabled = !toggled.enabled;
	}
}
