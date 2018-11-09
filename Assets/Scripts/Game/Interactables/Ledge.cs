﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for climbable ledges.
/// </summary>
public class Ledge : Interactable
{
	/// <summary>
	/// Activates the ledge upon exiting its current non-player collider (eg: water). 
	/// </summary>
	public bool activateOnExit = false;

	/// <summary>
	/// The input needed to activate this interactable. 
	/// </summary>
	protected override List<KeyCode> InteractInput
	{
		get { return InputManager.KeyBindings[(int)PlayerInput.Jump]; }
	}

	/// <summary>
	/// Speed at which the player is repositioned. 
	/// </summary>
	protected override float REPOSITION_SPEED
	{
		get { return 8f; }
	}

	/// <summary>
	/// Gets the tip.
	/// </summary>
	protected override string Tip { get { return null; }}

	/// <summary>
	/// Checks for player input. 
	/// </summary>
	protected override IEnumerator CheckForInput(PlayerController p)
	{
		for (; ; )
		{
			yield return null;
			foreach (KeyCode k in InteractInput)
			{
				if (!activateOnExit && Input.GetKey(k) && p.rBody.velocity.y >= 0)
				{
					InteractAction(p);
					yield break;
				}
			}
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision)
	{
		base.OnTriggerExit2D(collision);
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (p == null)
		{
			activateOnExit = false;
		}
	}


	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);

		p.Climbing = true;
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
