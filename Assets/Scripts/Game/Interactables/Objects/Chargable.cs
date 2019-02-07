using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for button interactables. 
/// </summary>
public class Chargable : Interactable
{
	[Header("Chargable Fields")]
	public bool startsCharged;
    public ParticleSystem ps;

    [HideInInspector]
    public bool charged;

    protected void Start() {
        charged = startsCharged;
        //if (startsCharged) {ps.Play();}

        var e = ps.emission;
        e.enabled = charged;
    }
	
	protected override string Tip(PlayerController p)
	{
		return "Press " + DBInputManager.GetInputName(p, InteractInput) + " To Grab";
	}

	protected override void InteractAction(PlayerController p)
	{
		
        
        p.StartCoroutine(RepositionPlayer(p));
		SFXManager.instance.PlayClip(clip, delay: (ulong)0.2, location: transform.position);
		p.Touch(transform, delegate
		{
            if (p.HoldingCharge != charged) {
                foreach (InteractAction a in actions)
                {
                    p.StartCoroutine(a.Act(p));
                }
            }
            
		});

		EndInteraction();
	}
}
