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

	/// <summary>
	/// Speed at which the player is repositioned. 
	/// </summary>
	protected override float REPOSITION_SPEED
	{
		get { return 8f; }
	}


	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);

		StartCoroutine(DelayedClimb(p));
	}

	protected IEnumerator DelayedClimb(PlayerController p) {
		while (moving)
		{
			yield return null;
		}
		yield return p.ClimbLedge(transform);
	}
}
