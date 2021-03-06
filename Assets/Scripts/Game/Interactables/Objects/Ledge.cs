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
	protected override PlayerInput InteractInput
	{
		get { return PlayerInput.Jump; }
	}

	/// <summary>
	/// Speed at which the player is repositioned. 
	/// </summary>
	protected override float REPOSITION_SPEED
	{
		get { return 16f; }
	}

	/// <summary>
	/// Gets the tip.
	/// </summary>
	protected override string Tip(PlayerController p) { return null; }

	/// <summary>
	/// The main object this ledge is attached to. 
	/// </summary>
	public Rigidbody2D mainObject;

	/// <summary>
	/// Checks for player input. 
	/// </summary>
	protected override IEnumerator CheckForInput(PlayerController p)
	{
		for (; ; )
		{
			yield return null;
			if (!activateOnExit && DBInputManager.GetInput(p, InteractInput, InputType.Held)
				&& (p.rBody.velocity.y >= -20 || p.Grounded || p.Swimming)
				&& p.transform.position.y < transform.position.y
			    && !((p as SquishController)?.BlobMode ?? false))
			{
				InteractAction(p);
				yield break;
			}
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision)
	{
		base.OnTriggerExit2D(collision);
		PlayerController p = collision.attachedRigidbody?.GetComponent<PlayerController>();
		if (p == null)
		{
			activateOnExit = false;
		}
	}


	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		p.rBody.velocity = Vector2.zero;
		p.rBody.simulated = false;
		p.Climbing = true;
		if (p.transform.position.y < transform.position.y) {
			p.IsPulling = true;
		}
		StartCoroutine(DelayedClimb(p));
	}


	protected IEnumerator DelayedClimb(PlayerController p)
	{
		p.cCollider.isTrigger = true;
		if (mainObject != null)
		{
			mainObject.mass += p.rBody.mass * p.rBody.gravityScale;
		}
		while (moving)
		{
			yield return null;
		}
		p.cCollider.isTrigger = false;
		yield return p.ClimbLedge(transform);
		if (mainObject != null)
		{
			mainObject.mass -= p.rBody.mass * p.rBody.gravityScale;
		}
	}
}
