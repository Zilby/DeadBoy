using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBabyController : PlayerController
{
    [Header("EB Fields")]
    public bool startsCharged;

    private bool charged;



	public override int SORT_VALUE
	{
		get { return 4; }
	}

	public override string Name
	{
		get { return "Electric Baby"; }
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
        
        var e = electricPS.emission;
        e.enabled = charged;
    }

    
	public override void TouchedCharged(bool c) {
		charged = c;
        
        
        var e = electricPS.emission;
        e.enabled = !e.enabled;
	}
}
