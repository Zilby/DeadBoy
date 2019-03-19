using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : Interactable
{
    /// <summary>
	/// Checks if the given player is valid. 
	/// </summary>
	protected override bool PlayerCheck(PlayerController p)
	{
        return p.CharID == Character.Firekid && base.PlayerCheck(p);
    }

    protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Burn";
	}

    protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		EndInteraction();
        SelfDestruct();
    }
}
