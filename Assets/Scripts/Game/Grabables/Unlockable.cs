using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for unlockable objects
/// </summary>
public class Unlockable : Interactable
{
	protected override string Tip
	{
		get { return "Press " + InteractInput.ToString() + " To Unlock"; }
	}
	protected override void InteractAction(Rigidbody2D r)
	{
		throw new System.NotImplementedException();
	}
}
