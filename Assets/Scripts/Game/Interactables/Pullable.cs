using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pullable objects. 
/// </summary>
public class Pullable : Interactable
{

	/// <summary>
	/// The distance joint for this pullable. 
	/// </summary>
	private DistanceJoint2D joint;


	protected override string Tip
	{
		get { return "Press " + InteractInput[0].ToString() + " To Pull"; }
	}

	private void Awake()
	{
		joint = GetComponentInChildren<DistanceJoint2D>();
	}


	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		bool pulling = p.Pulling(transform);
		joint.enabled = pulling;
		joint.connectedBody = p.rBody;
		if (pulling)
		{
			ToolTips.instance.SetTooltipString(tooltip, "Press " + InteractInput[0].ToString() + " To Release");
		}
		else
		{
			ToolTips.instance.SetTooltipString(tooltip, Tip);
		}
	}
}
