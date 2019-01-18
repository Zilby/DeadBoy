using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for lever interactables. 
/// </summary>
public class Lever : Interactable
{
	[Header("Lever Fields")]
	public Vector3 movePosition;

	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Pull";
	}
	
	protected override void InteractAction(PlayerController p)
	{
		p.StartCoroutine(RepositionPlayer(p));
		SFXManager.instance.PlayClip(clip, delay: (ulong)0.2, location: transform.position);
		p.GrabAndDrag(transform, movePosition, delegate
		{
			foreach (InteractAction a in actions)
			{
				p.StartCoroutine(a.Act(p));
			}
			SelfDestruct();
		});

		EndInteraction();
	}
}
