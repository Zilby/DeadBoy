using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferChargeAction : InteractAction
{
    public Chargable chargableObj;
    
	public override IEnumerator Act(PlayerController p) {
		yield return base.Act(p);
        p.TouchedCharged(chargableObj.charged, true);
        chargableObj.charged = !chargableObj.charged;
    }
}
