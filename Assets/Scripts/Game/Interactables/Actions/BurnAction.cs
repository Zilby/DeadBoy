using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnAction : InteractAction
{
    public Fadeable preBurnFader;
    public Fadeable postBurnFader;

    public override IEnumerator Act(PlayerController p)
	{
        StartCoroutine(preBurnFader.FadeOut());//disable on fade
        postBurnFader.gameObject.SetActive(true);//fade on enable
		yield return base.Act(p);
	}

}
