using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pullable objects. 
/// </summary>
public class Pullable : Grabable
{

	/// <summary>
	/// The distance joint for this pullable. 
	/// </summary>
	private HingeJoint2D joint;


	protected override string Tip
	{
		get { return "Press " + GrabInput.ToString() + " To Pull"; }
	}

	private bool pulling = false;


	private void Awake()
	{
		joint = GetComponentInChildren<HingeJoint2D>();
	}


	protected override void GrabAction(Rigidbody2D r)
	{
		pulling = !pulling;
		joint.enabled = pulling;
		joint.connectedBody = r;
		if (pulling)
		{
			ToolTips.TooltipTextEvent(tooltip, "Press " + GrabInput.ToString() + " To Release");
		}
		else
		{
			ToolTips.TooltipTextEvent(tooltip, Tip);
		}
	}
}
