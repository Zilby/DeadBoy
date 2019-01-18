using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action for toggling an object's enabled state. 
/// </summary>
public class ToggleEnableAction : InteractAction
{
	public GameObject toggled;
	public float delay = 1.5f;

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		yield return new WaitForSeconds(1.5f);
		toggled.SetActive(!toggled.activeSelf);
	}
}
