using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kickable : Interactable
{
	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Kick";
	}

	protected override void InteractAction(PlayerController p)
	{
        p.invertDirection = true;
		base.InteractAction(p);
		p.Kick(transform, delegate
		{
			SelfDestruct();
		});
		EndInteraction();
	}

    protected virtual void OnTriggerStay2D(Collider2D collision)
	{
		PlayerController p = collision.attachedRigidbody?.GetComponent<PlayerController>();
		if (p != null && PlayerCheck(p))
		{
            if (p.Grounded) {
                EndInteraction();
            }
        }
        
    }
}
