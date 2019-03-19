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
	[Range(0, 50)]
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

	/// <summary>
	/// Whether or not DG is starting underwater. 
	/// </summary>
	public bool startingUnder;


	public override Character CharID 
	{ 
		get { return Character.DrownedGirl; } 
	}
	public override string Name 
	{ 
		get { return "Drowned Girl"; } 
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
	private bool surfaced
	{
		get
		{
			return swimming &&
					this.gameObject.transform.position.y + this.cCollider.size.y * this.gameObject.transform.lossyScale.y / 2 >
					waterCollider.transform.position.y + waterCollider.size.y * waterCollider.transform.lossyScale.y / 2;
		}
	}

	private float MAX_RISE = MAX_Y_VELOCITY + 1.0f;

	private float timeSinceDive;

	protected override void Awake()
	{
		base.Awake();
		diving = startingUnder;
	}

	protected override void Update()
	{
		base.Update();

		if (swimming)
		{
			if (!diving && DBInputManager.GetInput(this, PlayerInput.Down, InputType.Pressed))
			{
				diving = true;
				timeSinceDive = 0;
			}

			timeSinceDive += Time.deltaTime;
			if (diving && surfaced && timeSinceDive > 0.8f)
			{
				diving = false;
			}
		}
	}

	protected override void EnterWater(Collider2D water)
	{
		swimming = true;
		this.waterCollider = water.GetComponent<BoxCollider2D>();
	}

	protected override void StayWater(Collider2D water)
	{
	}

	protected override void ExitWater(Collider2D water)
	{
		swimming = false;
		this.waterCollider = null;
		this.diving = false;
	}

	protected override void Move()
	{
		base.Move();

		if (swimming)
		{
			float surface = waterCollider.transform.position.y + waterCollider.size.y * waterCollider.transform.lossyScale.y / 2;
			float feetHeight = this.gameObject.transform.position.y - settleDepth;

			float adjustedDivingBuoyancy = divingBuoyancy;

			float analog = 0;
			InControl.InputDevice device = DBInputManager.players[this]?.Device;
			if (device != null)
			{
				analog = Mathf.Clamp(device.LeftStick.Y + device.DPadY, -1, 1);
			}
			else if (diving && DBInputManager.GetInput(this, PlayerInput.Down, InputType.Held))
			{
				analog = -1;
			}
			else if (diving && DBInputManager.GetInput(this, PlayerInput.Up, InputType.Held))
			{
				analog = 1;
			}
			float movespeed = verticalControl * analog;
			adjustedDivingBuoyancy += movespeed;

			float buoyantForce = (diving ? adjustedDivingBuoyancy : (surface - feetHeight) * surfaceBuoyancy) * Time.deltaTime;
			float swimSpeed = rBody.velocity.y * momentum + buoyantForce /* *(1-momentum) */;

			if (!diving && !surfaced)
			{
				swimSpeed = Mathf.Min(swimSpeed, MAX_RISE);
			}

			rBody.velocity = new Vector2(rBody.velocity.x * waterDrag, swimSpeed);
		}
	}


	/// <summary>
	/// Sets the current state of the animation.
	/// </summary>
	protected override void SetAnimationState()
	{
		base.SetAnimationState();

		anim.SetBool("WasSwimming", anim.GetBool("Swimming"));
		anim.SetBool("Swimming", swimming);
		anim.SetBool("Diving", diving);
	}


	/// <summary>
	/// Tries to pull an object. 
	/// </summary>
	public override bool Pulling(Transform t)
	{
		if (grounded || pulling || swimming)
		{
			pulling = !pulling;
			if (pulling)
			{
				settingIK[(int)IK.RightArm] = true;
				settingIK[(int)IK.LeftArm] = true;
				SetUpLimbMovement(t);
			}
		}
		return pulling;
	}
}
