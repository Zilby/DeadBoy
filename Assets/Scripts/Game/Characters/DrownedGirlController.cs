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
	/// Upward force when diving in water
	/// </summary>
	[Range(0, 7)]
	public float divingBuoyancy;
    
	/// <summary>
	/// Horixontal speed reduction while in water
	/// </summary>
	[Range(0, 1)]
	public float waterDrag;

    /// <summary>
	/// The collider for the body of water currently in
	/// </summary>
    private BoxCollider2D waterCollider;
    /// <summary>
	/// Whether drowned girl is diving under water
	/// </summary>
    private bool diving;

	private float MAX_RISE = MAX_Y_VELOCITY + 1.0f;



	protected override void Update() {
        base.Update();

        if (swimming)
        {
            if (!diving && Input.GetKeyDown(KeyCode.S)) 
            {
                diving = true;
            }
            else if (diving && Input.GetKeyDown(KeyCode.W)) 
            {
                diving = false;

                grounded = false;
                checkForGrounded = true;
            }
        }
    } 

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
            float buoyantForce = (surface-feetHeight) * (diving ? divingBuoyancy : surfaceBuoyancy)  * Time.deltaTime;
            // Debug.Log(surface + "  " + feetHeight + "  " + this.gameObject.transform.position.y);

			rBody.velocity = new Vector2(rBody.velocity.x * waterDrag, Mathf.Min(rBody.velocity.y * momentum + buoyantForce /* *(1-momentum) */, MAX_RISE)); 
            // float speed = rBody.velocity.y * momentum + buoyantForce /* *(1-momentum) */;
            // if (!diving) {
            //     speed = Mathf.Min(speed, MAX_RISE);
            // }
            // float tmp = rBody.velocity.x;
            // rBody.velocity = new Vector2(rBody.velocity.x * waterDrag, speed);
            // if (grounded) {Debug.Log(tmp +"   " + rBody.velocity.x);}
        }
    }
}
