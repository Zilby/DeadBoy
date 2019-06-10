using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for objects that can hold a charge. Possibly interactible. 
/// </summary>
public class Chargable : Interactable
{
	[Header("Chargable Fields")]
	public bool startsCharged;
    public ParticleSystem ps;

    public bool interactable = true;

    [HideInInspector]
    public bool Charged { 
        get { return charged; } 
        set {
            charged = value;
            setPS();

            foreach (Chargable c in connected) {
                if (c.Charged != value) {
                    c.Charged = value;
                    c.setPS();
                }
            }
        } 
    }
    private bool charged;

    private List<Chargable> connected = new List<Chargable>();

    protected void Start() {
        Charged = startsCharged;
    }

    private void setPS() {
        if (ps != null) {
            var e = ps.emission;
            e.enabled = Charged;
        }
    }
	
    /// <summary>
	/// Checks if the given player is valid. 
	/// </summary>
	protected override bool PlayerCheck(PlayerController p)
	{
		return interactable && p.CharID == Character.ElectricBaby && base.PlayerCheck(p);
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
            if (p.HoldingCharge != Charged) {
                foreach (InteractAction a in actions)
                {
                    p.StartCoroutine(a.Act(p));
                }
            }
            
		});

		EndInteraction();
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController pc = other.gameObject.GetComponent<PlayerController>();
        if (pc != null) {
            pc.TouchedCharged(Charged, false);
        }
        CollisionStart(other.gameObject);
    }
    void OnCollisionExit2D(Collision2D other)
    {
        CollisionStop(other.gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CollisionStart(other.gameObject);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        CollisionStop(other.gameObject);
    }


    void CollisionStart(GameObject other) 
    {
        Chargable c = other.GetComponent<Chargable>();
        if (c != null) { 
            connected.Add(c);
            if (Charged) {
                c.Charged = true;
            }    
        }
    }
    void CollisionStop(GameObject other)
    {
        Chargable c = other.GetComponent<Chargable>();
        if (c != null) { connected.Remove(c); }
    }
}
