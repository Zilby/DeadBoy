using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnAction : InteractAction
{
    public Fadeable preBurnFader;
    public Fadeable postBurnFader;
    public ParticleSystem ps;

    public void Start() {
        var e = ps.emission;
        e.enabled = false;
    }

    public override IEnumerator Act(PlayerController p)
	{
        StartCoroutine(preBurnFader.DelayedFadeOut());//disable on fade
        postBurnFader.gameObject.SetActive(true);//fade on enable
		yield return base.Act(p);
	}

}
