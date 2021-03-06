﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnAction : InteractAction
{
    public float pause = 0f;
    public Fadeable preBurnFader;
    public GameObject postBurn;
    public ParticleSystem ps;

    public void Start() {
        var e = ps.emission;
        e.enabled = false;
    }

    public override IEnumerator Act(PlayerController p)
	{
        yield return new WaitForSeconds(delay);
        StartCoroutine(preBurnFader.DelayedFadeOut());//disable on fade
		yield return new WaitForSeconds(pause);;
        postBurn.SetActive(true);//fade on enable
		yield return base.Act(p);
	}

    

}
