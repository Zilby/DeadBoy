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
		get { return "Press " + InteractInput.ToString() + " To Unlock"; }
	}


	protected override void InteractAction(Rigidbody2D r)
	{
		PlayerController p = r.GetComponent<PlayerController>();
		p.UsePickup();
		foreach (FadeableSprite s in sprites) 
		{
			s.SelfFadeOut();
		}
		foreach (FadeableSpriteMesh s in spriteMeshes)
		{
			s.SelfFadeOut();
		}
		base.OnTriggerExit2D(null);
		Destroy(this);
	}


	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		if (PlayerController.MainPlayer.CurrentPickup == requiredPickup)
		{
			base.OnTriggerEnter2D(collision);
		}
	}

	protected override void OnTriggerStay2D(Collider2D collision)
	{
		if (PlayerController.MainPlayer.CurrentPickup == requiredPickup)
		{
			base.OnTriggerStay2D(collision);
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision)
	{
		if (PlayerController.MainPlayer.CurrentPickup == requiredPickup)
		{
			base.OnTriggerExit2D(collision);
		}
	}
}
