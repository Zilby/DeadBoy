using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pickups
/// </summary>
public class Pickup : Interactable
{
	protected override string Tip
	{
		get { return "Press " + InteractInput.ToString() + " To Grab"; }
	}
	protected override void InteractAction(Rigidbody2D r)
	{
		throw new System.NotImplementedException();
	}
}
