using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownedGirlController : PlayerController
{
    [Header("Swimming")]
	/// <summary>
	/// Upward force when at surface level in water
    /// Affects frequency of bobbing at surface
	/// </summary>
	[Range(0, 200)]
	public float surfaceBuoyancy;

	/// <summary>
	/// Modifier for the depth at witch drowned girl will settle.
    /// Surface Bouancy also affects this, so tweak that first.
	/// </summary>
	[Range(0, 3)]
	public float settleDepth;

	/// <summary>
	/// Vertical momentum when in water
    /// Affects the duration it will take drowned girl to settle in the water
	/// </summary>
	[Range(0.7f, 0.999f)]
	public float momentum;

	/// <summary>
	/// Horixontal speed reduction while in water
	/// </summary>
	[Range(0, 1)]
	public float waterDrag;

    private BoxCollider2D waterCollider;

    protected override void EnterWater(Collider2D water) {
        this.waterCollider = water.GetComponent<BoxCollider2D>();
    }
    
    protected override void ExitWater(Collider2D water) {
        this.waterCollider = null;
    }

    protected override void Move() {
        base.Move();

        if (swimming) {
            float surface = waterCollider.transform.position.y + waterCollider.size.y*waterCollider.transform.lossyScale.y/2;
            float feetHeight = this.gameObject.transform.position.y - settleDepth;
            float buoyantForce = surface - feetHeight;
            Debug.Log(buoyantForce + "     " + rBody.velocity.y + "    " +  buoyantForce*Time.deltaTime);
            rBody.velocity = new Vector2(rBody.velocity.x * waterDrag, rBody.velocity.y * momentum + buoyantForce*surfaceBuoyancy*Time.deltaTime);
        }
    }
}
