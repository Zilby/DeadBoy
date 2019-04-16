using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBabyController : PlayerController
{
    [Header("EB Fields")]
    public bool startsCharged;

    private bool charged;


	public override Character CharID 
	{ 
		get { return Character.ElectricBaby; } 
	}
	
    /// <summary>
	/// Whether the player is holding an electrical charge.
	/// </summary>
	public override bool HoldingCharge 
	{
		get { return charged; }
	}

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        charged = startsCharged;

        electricPS.Play();
        var e = electricPS.emission;
        e.enabled = charged;
        
    }

	public override void TouchedCharged(bool c, bool transfer) {
        if (transfer) {
            charged = c;

            var e = electricPS.emission;
            e.enabled = charged;
        }
    }
}
