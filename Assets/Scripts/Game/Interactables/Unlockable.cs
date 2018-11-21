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

	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Unlock";
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
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (p.CurrentPickup == requiredPickup)
		{
			base.OnTriggerEnter2D(collision);
		}
	}

	protected override void OnTriggerStay2D(Collider2D collision)
	{
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (p.CurrentPickup == requiredPickup)
		{
			base.OnTriggerStay2D(collision);
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision)
	{
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (p.CurrentPickup == requiredPickup)
		{
			base.OnTriggerExit2D(collision);
		}
	}
}
