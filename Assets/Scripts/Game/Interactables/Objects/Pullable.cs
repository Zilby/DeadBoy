﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pullable objects. 
/// </summary>
public class Pullable : Interactable
{

	public Collider2D enableCollider;

	/// <summary>
	/// The distance joint for this pullable. 
	/// </summary>
	private DistanceJoint2D joint;


	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Pull";
	}

	protected override void Awake()
	{
		base.Awake();
		joint = GetComponentInChildren<DistanceJoint2D>();
	}


	protected override void InteractAction(PlayerController p)
	{
		bool pulling = p.Pulling(transform);
		joint.enabled = pulling;
		joint.connectedBody = p.rBody;
		enableCollider.enabled = pulling;
		if (pulling)
		{
			base.InteractAction(p);
			ToolTips.instance.SetTooltipString(tooltip, "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Release");
		}
		else
		{
			ToolTips.instance.SetTooltipString(tooltip, Tip(p));
		}
	}
}
