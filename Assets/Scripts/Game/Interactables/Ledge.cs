using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for climbable ledges.
/// </summary>
public class Ledge : Interactable
{
	protected override string Tip
	{
		get { return "Press " + InteractInput.ToString() + " To Climb Up"; }
	}


	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		StartCoroutine(p.ClimbLedge(transform));
	}
}
