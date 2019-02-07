using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferChargeAction : InteractAction
{
    public Chargable chargableObj;
    public float delay = 0;
    
	public override IEnumerator Act(PlayerController p) {
        yield return new WaitForSeconds(delay);
        p.TouchedCharged(chargableObj.charged);
        chargableObj.charged = !chargableObj.charged;
    }
}
