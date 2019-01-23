using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for button interactables. 
/// </summary>
public class PressableButton : Interactable
{
	[Header("Button Fields")]
	public Sprite sprite;
	
	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Press";
	}

	protected override void InteractAction(PlayerController p)
	{
		p.Press(transform, delegate
		{
			GetComponent<SpriteRenderer>().sprite = sprite;
			SelfDestruct();
		});
		base.InteractAction(p);
		EndInteraction();
	}
}
