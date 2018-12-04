using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for lever interactables. 
/// </summary>
public abstract class Lever : Interactable
{
	[Header("Lever Fields")]
	public Vector3 movePosition;

	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Pull";
	}

	protected abstract List<Func<IEnumerator>> ToggleActions { get; }

	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		p.GrabAndDrag(transform, movePosition, delegate
		{
			foreach(Func<IEnumerator> f in ToggleActions) {
				p.StartCoroutine(f());
			}
			p.StartCoroutine(CameraController.RedirectCamera(look));
		});

		EndInteraction();
		Destroy(this);
	}
}
