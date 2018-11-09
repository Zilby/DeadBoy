using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for unlockable objects
/// </summary>
public class Unlockable : Interactable
{
	public Pickup.Type requiredPickup = Pickup.Type.none;
	public List<FadeableSprite> sprites;
	public List<FadeableSpriteMesh> spriteMeshes;

	protected override string Tip
	{
		get { return "Press " + InteractInput[0].ToString() + " To Unlock"; }
	}


	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		p.UsePickup();
		foreach (FadeableSprite s in sprites) 
		{
			s.SelfFadeOut();
		}
		foreach (FadeableSpriteMesh s in spriteMeshes)
		{
			s.SelfFadeOut();
		}
		EndInteraction();
		Destroy(this);
	}


	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		if (InputManager.MainPlayer.CurrentPickup == requiredPickup)
		{
			base.OnTriggerEnter2D(collision);
		}
	}

	protected override void OnTriggerStay2D(Collider2D collision)
	{
		if (InputManager.MainPlayer.CurrentPickup == requiredPickup)
		{
			base.OnTriggerStay2D(collision);
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision)
	{
		if (InputManager.MainPlayer.CurrentPickup == requiredPickup)
		{
			base.OnTriggerExit2D(collision);
		}
	}
}
