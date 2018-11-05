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
	/// Upward force when diving in water
	/// </summary>
	[Range(0, 200)]
	public float divingBuoyancy;

    /// <summary> 
    /// How fast W and S move up and down in the water.
    /// </summary>
    [Range(0,50)]
    public float verticalControl;

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

	protected override int SORT_VALUE
	{
		get { return 2; }
	}

	/// <summary>
	/// The collider for the body of water currently in
	/// </summary>
	private BoxCollider2D waterCollider;
    /// <summary>
	/// Whether drowned girl is diving under water
	/// </summary>
    private bool diving;
    /// <summary>
    /// Whether drowned girl is poking above the water's surface
    /// </summary>
    private bool surfaced { get { 
        return swimming && 
                this.gameObject.transform.position.y +  this.cCollider.size.y*this.gameObject.transform.lossyScale.y/2 > 
                waterCollider.transform.position.y + waterCollider.size.y*waterCollider.transform.lossyScale.y/2; 
    }}

	private float MAX_RISE = MAX_Y_VELOCITY + 1.0f;

    private float timeSinceDive;

	protected override void Update() {
        base.Update();

		if (swimming && MainPlayer == this)
        {
            if (!diving && Input.GetKeyDown(KeyCode.S)) 
            {
                diving = true;
                timeSinceDive = 0;
            }
            else if (diving && Input.GetKeyDown(KeyCode.W)) 
            {
                grounded = false;
            }

            timeSinceDive += Time.deltaTime;
            if (diving && surfaced && timeSinceDive > 0.8f) {
                diving = false;
            }
        }
    } 

    protected override void EnterWater(Collider2D water) {
        this.waterCollider = water.GetComponent<BoxCollider2D>();
    }
    
    protected override void ExitWater(Collider2D water) {
        this.waterCollider = null;
        this.diving = false;
    }

    protected override bool CanJump() {
        return swimming ? surfaced : base.CanJump();
    }

    protected override void Move() {
        base.Move();

        if (swimming) 
		{
            float surface = waterCollider.transform.position.y + waterCollider.size.y*waterCollider.transform.lossyScale.y/2;
            float feetHeight = this.gameObject.transform.position.y - settleDepth;

            float adjustedDivingBuoyancy = divingBuoyancy;
            if (diving && Input.GetKey(KeyCode.S)) 
            {
                adjustedDivingBuoyancy -= verticalControl;
            }
            else if (diving && Input.GetKey(KeyCode.W)) 
            {
                adjustedDivingBuoyancy += verticalControl;
            }

            float buoyantForce = (diving ? adjustedDivingBuoyancy : (surface - feetHeight) * surfaceBuoyancy)  * Time.deltaTime;
            float speed = rBody.velocity.y * momentum + buoyantForce /* *(1-momentum) */;

            if (!diving && !surfaced) {
                speed = Mathf.Min(speed, MAX_RISE);
            }

            rBody.velocity = new Vector2(rBody.velocity.x * waterDrag, speed);
        }
    }
}
