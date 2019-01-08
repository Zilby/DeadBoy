using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for button interactables. 
/// </summary>
public abstract class PressableButton : Interactable
{
	[Header("Button Fields")]
	public Sprite sprite;

	protected abstract List<Func<IEnumerator>> ToggleActions { get; }

	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Press";
	}

	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		foreach (Func<IEnumerator> f in ToggleActions)
		{
			p.StartCoroutine(f());
		}
		p.StartCoroutine(CameraController.RedirectCamera(lookAts));

		p.Press(transform, delegate
		{
			GetComponent<SpriteRenderer>().sprite = sprite;
			SelfDestruct();
		});
		EndInteraction();
	}
}
