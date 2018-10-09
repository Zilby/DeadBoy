using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pullable objects. 
/// </summary>
public class Pullable : Grabable
{
	protected override string Tip
	{
		get { return "Press " + GrabInput.ToString() + " To Pull"; }
	}

	private bool pulling = false;

	protected override void GrabAction()
	{
		pulling = !pulling;
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
