using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for toggling an object's enabled state. 
/// </summary>
public class ToggleGameObjectAction : InteractAction
{
	public GameObject toggled;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		toggled.SetActive(!toggled.activeSelf);
	}
}
