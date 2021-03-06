﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pickups
/// </summary>
public class Pickup : Interactable
{
	public enum Type
	{
		none = -1,
		key = 0,
	}

	public Type type;
	public Rigidbody2D rbody;
	public Collider2D col;


	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Grab";
	}

	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		if (rbody != null)
		{
			Destroy(rbody);
		}
		Transform t = col.transform;
		Destroy(col);
		p.PickUp(t, type);
		EndInteraction();
		SelfDestruct();
	}
}
