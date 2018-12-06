using System;
using System.Collections;
using System.Collections.Generic;
using Anima2D;
using UnityEngine;
using UnityEngine.Sprites;

/// <summary>
/// This class controls the player's movement and abilities. 
/// </summary>
public abstract class PlayerController : MonoBehaviour
{
	public enum IK
	{
		RightArm = 0,
		LeftArm = 1,
		RightLeg = 2,
		LeftLeg = 3,
		RightFoot = 4,
		LeftFoot = 5
	}

	#region Fields

	#region Public

	[Header("Automatic References")]
	public Rigidbody2D rBody;
	public CapsuleCollider2D cCollider;
	public Animator anim;

	[Header("Other References")]
	public Fadeable indicator;

	[Header("InverseKinematics")]
	public IkLimb2D[] iKLimbs = new IkLimb2D[IKCount];

	[InspectorButton("SetDefaultIKs")]
	public bool SetUpDefaultLimbs;

	[Header("Characteristics")]
	/// <summary>
	/// The initial controller starting with this player.
	/// </summary>
	public int initialPlayer = 0;

	[Header("Movement")]
	/// <summary>
	/// Speed of the player. 
	/// </summary>
	[Range(0, 2000)]
	public float speed;
	/// <summary>
	/// How fast the player accelerates to max speed on the ground. 
	/// </summary>
	[Range(0, 1)]
	public float acceleration;
	/// <summary>
	/// The strength of deadboy's jump.
	/// </summary>
	[Range(0, 100)]
	public float jumpHeight;
	/// <summary>
	/// The interval in which you can increase deadboy's jump. 
	/// </summary>
	[Range(0, 1)]
	public float jumpInterval;
	/// <summary>
	/// Level of aerial control for the player. 
	/// Higher numbers equate to more control. 
	/// Should be between 0 and 1 unless you want 
	/// more control in air than the ground. 
	/// </summary>
	[Range(0, 1)]
	public float aerialControl;

	[Header("Sprites")]
	/// <summary>
	/// Inverts the sprite direction (flips the sprites)
	/// </summary>
	public bool invertDirection = false;

	#endregion

	#region Protected

	/// <summary>
	/// The last checkpoint reached. 
	/// </summary>
	protected Transform checkpoint;

	/// <summary>
	/// Whether or not the player is in the air. 
	/// </summary>
	protected bool grounded
	{
		get { return gCounter < 6; }
	}

	/// <summary>
	/// Number of frames since last grounded. 
	/// </summary>
	protected uint gCounter = 0;

	/// <summary>
	/// Whether or not the player is in water.
	/// <summary>
	protected bool swimming;

	/// <summary>
	/// The time of the start of the jump.
	/// </summary>
	protected float jumpStart;

	/// <summary>
	/// Whether or not jump has been held. 
	/// </summary>
	protected bool jumpHeld;

	/// <summary>
	/// Whether or not the iks are returning to their original position.
	/// </summary>
	protected bool[] returningToPosition = new bool[IKCount];

	/// <summary>
	/// The location of an objet to be interacted with.
	/// </summary>
	protected Transform objectLocation = null;

	/// <summary>
	/// The offsets of the object location. 
	/// </summary>
	protected Vector3[] ObjectOffsets = new Vector3[IKCount];

	/// <summary>
	/// Whether or not the player is currently pulling. 
	/// </summary>
	protected bool pulling;

	/// <summary>
	/// Whether or not the player is currently climbing. 
	/// </summary>
	protected bool climbing;

	/// <summary>
	/// The current pickup held.
	/// </summary>
	protected Pickup.Type currentPickup = Pickup.Type.none;

	/// <summary>
	/// The held object.
	/// </summary>
	protected GameObject heldObject;

	/// <summary>
	/// Action to be called once an object is interacted with. 
	/// </summary>
	protected Action interactAction;

	/// <summary>
	/// The location to drag the object to.
	/// </summary>
	protected Vector3 dragLocation = Vector3.zero;

	/// <summary>
	/// The speed of dragging an object. 
	/// </summary>
	protected float dragSpeed;

	/// <summary>
	/// Whether or not the ik location is currently being set. 
	/// </summary>
	protected bool[] settingIK = new bool[IKCount];

	/// <summary>
	/// The last location of the ik.
	/// </summary>
	protected Vector3[] lastIKLocation = new Vector3[IKCount];

	/// <summary>
	/// All of the connected sprite mesh instances
	/// </summary>
	protected SpriteMeshInstance[] sprites;

	/// <summary>
	/// The max y velocity before no longer being considered grounded.
	/// </summary>
	protected const float MAX_Y_VELOCITY = 5;

	/// <summary>
	/// The minimum y velocity before no longer being considered grounded.
	/// </summary>
	protected const float MIN_Y_VELOCITY = -1;

	/// <summary>
	/// The limb move speed.
	/// </summary>
	protected const float LIMB_MOVE_SPEED = 3f;

	/// <summary>
	/// The smoothed out animation speed.
	/// </summary>
	private const float SMOOTH_ANIM_SPEED = 15F;

	#endregion

	#region Properties

	/// <summary>
	/// The character's name 
	/// </summary>
	public abstract string Name { get; }

	/// <summary>
	/// Whether or not the player is currently grounded
	/// </summary>
	public bool Grounded
	{
		get { return grounded; }
	}

	/// <summary>
	/// Whether or not the player is currently swimming
	/// </summary>
	public bool Swimming
	{
		get { return swimming; }
	}

	/// <summary>
	/// Whether or not the player is currently climbing
	/// </summary>
	public bool Climbing
	{
		get { return climbing; }
		set { climbing = value; }
	}

	/// <summary>
	/// Gets the current pickup.
	/// </summary>
	public Pickup.Type CurrentPickup
	{
		get { return currentPickup; }
	}

	/// <summary>
	/// Gets the IK Count.
	/// </summary>
	protected static int IKCount
	{
		get { return Enum.GetValues(typeof(IK)).Length; }
	}


	/// <summary>
	/// Gets the limb move speed.
	/// </summary>
	protected float LimbMoveSpeed
	{
		get
		{
			return LIMB_MOVE_SPEED * (rBody.velocity.magnitude + 1) * Time.deltaTime;
		}
	}

	/// <summary>
	/// Whether or not this player is currently accepting movement input. 
	/// </summary>
	public bool AcceptingMoveInput
	{
		get
		{
			bool output = !climbing;
			for (int i = 0; i < IKCount; ++i)
			{
				// Allow move input when pulling with arms
				output &= !settingIK[i] || (pulling && i < 2);
			}
			return output;
		}
	}

	/// <summary>
	/// The sort value that determines which player gets selected next when toggling between players. 
	/// It also determines their corresponding number key. 
	/// </summary>
	public abstract int SORT_VALUE { get; }

	/// <summary>
	/// Gets all of the connected sprite mesh instances
	/// </summary>
	public SpriteMeshInstance[] Sprites
	{
		get
		{
			if (sprites == null)
			{
				sprites = GetComponentsInChildren<SpriteMeshInstance>();
			}
			return sprites;
		}
	}

	#endregion

	#endregion

	#region Functions

	#region General


	protected virtual void Reset()
	{
		SetDefaultIKs();
	}

	protected virtual void Awake()
	{
		rBody = rBody == null ? GetComponent<Rigidbody2D>() : rBody;
		cCollider = cCollider == null ? GetComponent<CapsuleCollider2D>() : cCollider;
		anim = anim == null ? GetComponent<Animator>() : anim;
	}

	protected virtual void OnDestroy()
	{
		DBInputManager.Unregister(this);
	}

	protected virtual void Start()
	{
		DBInputManager.Register(this, initialPlayer);
		jumpStart = Time.fixedTime - 100f;
		foreach (SpriteMeshInstance s in Sprites)
		{
			s.sortingOrder += 100 * SORT_VALUE;
		}
	}

	protected virtual void FixedUpdate()
	{
		if (rBody.velocity.y >= MAX_Y_VELOCITY)
		{
			gCounter += 10;
		}
		else if (rBody.velocity.y < MIN_Y_VELOCITY && cCollider.GetContacts(new Collider2D[0]) == 0)
		{
			gCounter++;
		}
		Move();
	}

	protected virtual void Update()
	{
		Jump();
		SetAnimationState();
		PreserveIndicatorDirection();
	}

	protected virtual void LateUpdate()
	{
		UpdateKinematics();
	}

	/// <summary>
	/// Moves the player left or right if given input. 
	/// </summary>
	protected virtual void Move()
	{
		float analog = 0f;
		InControl.InputDevice device = DBInputManager.players[this]?.Device;
		if (device != null)
		{
			analog = Mathf.Clamp(device.LeftStick.X + device.DPadX, -1, 1);
		}
		else if (DBInputManager.GetInput(this, PlayerInput.Left, InputType.Held))
		{
			analog = -1f;
		}
		else if (DBInputManager.GetInput(this, PlayerInput.Right, InputType.Held))
		{
			analog = 1f;
		}
		float movespeed = speed * analog * Time.fixedDeltaTime;

		float acceleratedMove;
		if (grounded)
		{
			acceleratedMove = movespeed == 0.0f ? rBody.velocity.x * (1 - (acceleration / 2f)) : rBody.velocity.x + (movespeed * acceleration);
			rBody.velocity = new Vector2(rBody.velocity.x, rBody.velocity.y + ((rBody.gravityScale * -Physics2D.gravity.y * Mathf.Abs(analog)) / 100f));
		}
		else
		{
			acceleratedMove = rBody.velocity.x + (movespeed * aerialControl);
		}
		// Clamp the accelerated move to the maximum speeds. 
		movespeed = Mathf.Clamp(acceleratedMove, -Mathf.Abs(movespeed), Mathf.Abs(movespeed));
		rBody.velocity = new Vector2(movespeed, rBody.velocity.y);
	}

	/// <summary>
	/// Determines if the character is able to jump
	/// </summary>
	protected virtual bool CanJump()
	{
		return grounded;
	}

	/// <summary>
	/// Makes the player jump if given input. 
	/// </summary>
	protected virtual void Jump()
	{
		if (!pulling)
		{
			if (DBInputManager.GetInput(this, PlayerInput.Jump, InputType.Pressed) && CanJump())
			{
				rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
				jumpStart = Time.fixedTime;
				jumpHeld = true;
			}
			if (!DBInputManager.GetInput(this, PlayerInput.Jump, InputType.Held) && !grounded)
			{
				jumpHeld = false;
			}
			// Add more force if jump held for longer. 
			else if (DBInputManager.GetInput(this, PlayerInput.Jump, InputType.Held) &&
					 (Time.fixedTime - jumpStart < jumpInterval) &&
					 !grounded && jumpHeld)
			{
				rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
			}
		}
	}

	/// <summary>
	/// Sets the current state of the animation.
	/// </summary>
	protected virtual void SetAnimationState()
	{
		if (!pulling && !climbing)
		{
			float flip = transform.localEulerAngles.y;
			if (rBody.velocity.x > 1f)
			{
				flip = 180 + (invertDirection ? 180 : 0);
			}
			else if (rBody.velocity.x < -1f)
			{
				flip = 0 + (invertDirection ? 180 : 0);
			}
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, flip, transform.localEulerAngles.z);
		}
		bool flipped = transform.localEulerAngles.y == 180;
		anim.SetFloat("OldXVel", anim.GetFloat("XVel"));
		anim.SetFloat("OldYVel", anim.GetFloat("YVel"));
		anim.SetFloat("XVel", rBody.velocity.x);
		anim.SetFloat("YVel", rBody.velocity.y);
		anim.SetFloat("XMag", Mathf.Abs(rBody.velocity.x));
		anim.SetFloat("YMag", Mathf.Abs(rBody.velocity.y));
		anim.SetFloat("RelativeXVel", flipped ? -rBody.velocity.x : rBody.velocity.x);
		anim.SetFloat("SmoothXVel", Mathf.MoveTowards(anim.GetFloat("SmoothXVel"), rBody.velocity.x, SMOOTH_ANIM_SPEED * Time.deltaTime));
		anim.SetFloat("SmoothYVel", Mathf.MoveTowards(anim.GetFloat("SmoothYVel"), rBody.velocity.y, SMOOTH_ANIM_SPEED * Time.deltaTime));
		anim.SetFloat("SmoothXMag", Mathf.MoveTowards(anim.GetFloat("SmoothXMag"), Mathf.Abs(rBody.velocity.x), SMOOTH_ANIM_SPEED * Time.deltaTime));
		anim.SetFloat("SmoothYMag", Mathf.MoveTowards(anim.GetFloat("SmoothYMag"), Mathf.Abs(rBody.velocity.y), SMOOTH_ANIM_SPEED * Time.deltaTime));
		anim.SetFloat("SmoothRelXVel", Mathf.MoveTowards(anim.GetFloat("SmoothRelXVel"), anim.GetFloat("RelativeXVel"), SMOOTH_ANIM_SPEED * Time.deltaTime));
		anim.SetFloat("SmootherRelXVel", Mathf.Clamp(Mathf.MoveTowards(anim.GetFloat("SmootherRelXVel"), anim.GetFloat("RelativeXVel"), (SMOOTH_ANIM_SPEED / 5) * Time.deltaTime), -1, 1));



		if (anim.GetBool("Grounded") != grounded && grounded)
		{
			anim.SetTrigger("Fell");
		}
		if (!grounded)
		{
			anim.ResetTrigger("Fell");
		}
		anim.SetBool("Grounded", grounded);
		anim.SetInteger("GCounter", (int)gCounter);
		anim.SetBool("Pulling", pulling && !climbing);
		anim.SetBool("Climbing", climbing);
		anim.SetBool("PullingUp", climbing && pulling);
		anim.SetBool("Flipped", transform.localEulerAngles.y == 180);

		bool left = DBInputManager.GetInput(this, PlayerInput.Left, InputType.Held);
		bool right = DBInputManager.GetInput(this, PlayerInput.Right, InputType.Held);
		anim.SetBool("RightInput", right);
		anim.SetBool("LeftInput", left);
		anim.SetBool("AnyInput", left || right);
		anim.SetBool("BackInput", (flipped && right) || (!flipped && left));
		anim.SetBool("ForwardInput", (flipped && left) || (!flipped && right));
	}

	/// <summary>
	/// Plays a footstep sound effect for the given IK
	/// </summary>
	public virtual void Footstep(IK ik)
	{
		SFXManager.instance.PlayClip("DBFootstepsRock", 0.2f, 0.25f, location: iKLimbs[(int)ik].transform.position);
	}

	public virtual void SwitchedTo()
	{
		indicator.Hide();//Incase players swap quickly and it's still there
		indicator.Show();
		indicator.DelayedFadeOut();
	}

	protected void PreserveIndicatorDirection()
	{
		indicator.gameObject.GetComponent<SpriteRenderer>().flipX = transform.localEulerAngles.y > 90;
	}

	protected virtual IEnumerator Die()
	{
		CameraController.Deactivate();
		DBInputManager.instance.enabled = false;
		yield return Fader.FadeIn();
		CameraController.Activate();
		transform.position = checkpoint.transform.position;
		rBody.velocity = Vector3.zero;
		yield return new WaitForSeconds(0.2f);
		DBInputManager.instance.enabled = true;
		yield return Fader.FadeOut();
		if (checkpoint == null)
		{
			LevelManager.instance.RestartLevel();
			yield return null;
		}
	}

	#endregion

	#region CollisionDetection

	void OnCollisionStay2D(Collision2D collision)
	{
		CheckGrounded(collision);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		CheckGrounded(collision);
	}

	protected virtual void CheckGrounded(Collision2D collision)
	{
		foreach (ContactPoint2D contact in collision.contacts)
		{
			gCounter = rBody.velocity.y < MAX_Y_VELOCITY && (grounded || TouchingGround(contact)) && !swimming ? 0 : gCounter;
			/*
			print(contact.collider.name + " hit " + contact.otherCollider.name + " " + 
			      (Vector2.Distance(transform.position, contact.point) / (transform.localScale.x * cCollider.size.y)).ToString());
			*/
		}
	}

	bool TouchingGround(ContactPoint2D contact)
	{
		//print((contact.point.y < transform.position.y) + " " + (Vector2.Distance(transform.position, contact.point) / (transform.localScale.y)) + " " + Time.time.ToString());
		return contact.point.y < transform.position.y &&
				Vector2.Distance(transform.position, contact.point) /
								(transform.localScale.y) > 4.2f;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			swimming = true;
			this.EnterWater(collision);
		}
		if (collision.tag == "Checkpoint") {
			checkpoint = collision.gameObject.transform;
		}
	}

	// void OnTriggerStay2D(Collider2D collision)
	// {
	// 	if(collision.gameObject.layer == LayerMask.NameToLayer("Water")) {
	// 		this.InWater(collision);
	// 	}
	// }

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			swimming = false;
			this.ExitWater(collision);
		}
	}

	#endregion

	#region Water

	protected virtual void EnterWater(Collider2D water)
	{
		StartCoroutine(Die());
	}

	protected virtual void ExitWater(Collider2D water)
	{
	}

	#endregion

	#region KinematicMovements

	/// <summary>
	/// Sets up moving limbs to their given position.
	/// </summary>
	protected void SetUpLimbMovement(Transform t)
	{
		for (int i = 0; i < IKCount; ++i)
		{
			if (settingIK[i])
			{
				lastIKLocation[i] = iKLimbs[i].transform.position;
				returningToPosition[i] = false;
			}
		}
		objectLocation = t;
	}


	/// <summary>
	/// Cancels the kinematics.
	/// </summary>
	private void CancelKinematics()
	{
		for (int i = 0; i < IKCount; ++i)
		{
			if (settingIK[i])
			{
				settingIK[i] = false;
				returningToPosition[i] = false;
			}
		}
		objectLocation = null;
	}


	/// <summary>
	/// Sets the pulling for the limb in the animation.
	/// </summary>
	protected void SetLocation(IK ik)
	{
		int i = (int)ik;
		float s = LimbMoveSpeed / (returningToPosition[i] && (i == (int)IK.LeftLeg || i == (int)IK.RightLeg) ? 3f : 1f);
		lastIKLocation[i] = Vector3.MoveTowards(lastIKLocation[i], returningToPosition[i] ? iKLimbs[i].transform.position :
												objectLocation.position + ObjectOffsets[i], s);
		iKLimbs[i].transform.position = lastIKLocation[i];
		iKLimbs[i].UpdateIK();
	}


	/// <summary>
	/// Sets the pulling for the limbs in the animation.
	/// </summary>
	protected void SetAllLocations()
	{
		foreach (IK ik in Enum.GetValues(typeof(IK)))
		{
			if (settingIK[(int)ik])
			{
				SetLocation(ik);
			}
		}
	}


	/// <summary>
	/// Whether or not the ik has reached the location it was moving towards. 
	/// </summary>
	protected bool IkAtLocation(IK ik)
	{
		int i = (int)ik;
		return !settingIK[i] || Vector2.Distance(lastIKLocation[i], returningToPosition[i] ? iKLimbs[i].transform.position : objectLocation.position + ObjectOffsets[i]) < 0.01f;
	}


	/// <summary>
	/// Whether or not all the iks have reached the location they were moving towards. 
	/// </summary>
	protected bool AllIKsAtLocation()
	{
		bool output = true;
		foreach (IK ik in Enum.GetValues(typeof(IK)))
		{
			output &= IkAtLocation(ik);
		}
		return output;
	}


	/// <summary>
	/// Updates the IKs to go where they're set to go.
	/// </summary>
	protected void UpdateKinematics()
	{
		if (dragSpeed != 0)
		{
			DragToLocation();
		}
		foreach (IK ik in Enum.GetValues(typeof(IK)))
		{
			int i = (int)ik;
			if (settingIK[i] && IkAtLocation(ik))
			{
				if (returningToPosition[i])
				{
					returningToPosition[i] = false;
					settingIK[i] = false;
				}
				else
				{
					if (!pulling && !climbing && dragSpeed == 0)
					{
						SetLocation(ik);
						returningToPosition[i] = true;
						ObjectOffsets[i] = Vector3.zero;
					}
				}
			}
		}
		SetAllLocations();
		if (AllIKsAtLocation())
		{
			interactAction?.Invoke();
			interactAction = null;
		}
	}


	/// <summary>
	/// Drags the object to the set drag location, and then sets the hands to return to the default position. 
	/// </summary>
	protected void DragToLocation()
	{
		if (Vector2.Distance(Vector3.zero, dragLocation) > 0.01f)
		{
			float speedx = dragLocation.x * dragSpeed * Time.deltaTime;
			float speedy = dragLocation.y * dragSpeed * Time.deltaTime;
			objectLocation.position = new Vector3(objectLocation.position.x + speedx, objectLocation.position.y + speedy, objectLocation.position.z);
			dragLocation.x -= speedx;
			dragLocation.y -= speedy;
		}
		else
		{
			dragSpeed = 0;
			dragLocation = Vector3.zero;
		}
	}


	/// <summary>
	/// Finds and sets the default IK limbs
	/// </summary>
	public void SetDefaultIKs()
	{
		iKLimbs = new IkLimb2D[IKCount];
		foreach (IK ik in Enum.GetValues(typeof(IK)))
		{
			int i = (int)ik;
			iKLimbs[i] = Utils.FindDeepChild<IkLimb2D>(transform, ik.ToString());
		}
	}

	#endregion

	#region Interactables

	/// <summary>
	/// Tries to pull an object. 
	/// </summary>
	public virtual bool Pulling(Transform t)
	{
		if (grounded || pulling)
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

	/// <summary>
	/// Picks up the given pickup at the given transform.
	/// </summary>
	public void PickUp(Transform t, Pickup.Type p)
	{
		settingIK[(int)IK.RightArm] = true;
		SetUpLimbMovement(t);
		interactAction = delegate
		{
			objectLocation.parent = iKLimbs[(int)IK.RightArm].transform;
			currentPickup = p;
			heldObject = objectLocation.gameObject;
		};
	}

	/// <summary>
	/// Presses the given location
	/// </summary>
	public void Press(Transform t, Action a)
	{
		settingIK[(int)IK.RightArm] = true;
		SetUpLimbMovement(t);
		interactAction = a;
	}

	/// <summary>
	/// Grabs the given object and drags it to the given location..
	/// </summary>
	public void GrabAndDrag(Transform t, Vector3 position, Action a, float speed = 2)
	{
		settingIK[(int)IK.RightArm] = true;
		//settingLA = true;
		SetUpLimbMovement(t);
		dragLocation = position;
		dragSpeed = speed;
		interactAction = a;
	}

	/// <summary>
	/// Uses the current pickup.
	/// </summary>
	public void UsePickup()
	{
		currentPickup = Pickup.Type.none;
		Destroy(heldObject);
	}

	/// <summary>
	/// Climbs a ledge
	/// </summary>
	public IEnumerator ClimbLedge(Transform t)
	{
		bool side = transform.position.x > t.position.x;
		climbing = true;
		pulling = true;
		rBody.simulated = false;
		rBody.velocity = Vector3.zero;
		settingIK[(int)IK.RightArm] = true;
		settingIK[(int)IK.LeftArm] = true;
		// for initialization
		SetUpLimbMovement(t);
		while (transform.position.y < t.position.y)
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 3f * Time.deltaTime);
			yield return null;
			if (pulling && transform.position.y > t.position.y - 1.5f)
			{
				pulling = false;
			}
		}
		while (cCollider.bounds.min.y < t.position.y)
		{
			if ((!returningToPosition[(int)IK.RightArm] || !returningToPosition[(int)IK.LeftArm]) && cCollider.bounds.min.y > t.position.y - 1f)
			{
				returningToPosition[(int)IK.RightArm] = true;
				returningToPosition[(int)IK.LeftArm] = true;
			}
			if (!settingIK[(int)IK.LeftLeg] && cCollider.bounds.min.y > t.position.y - 2f)
			{
				lastIKLocation[(int)IK.LeftLeg] = iKLimbs[(int)IK.LeftLeg].transform.position;
				settingIK[(int)IK.LeftLeg] = true;
			}
			transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 3f * Time.deltaTime);
			yield return null;
		}
		transform.position = new Vector3(transform.position.x, t.position.y + transform.position.y - cCollider.bounds.min.y, transform.position.z);

		lastIKLocation[(int)IK.RightLeg] = iKLimbs[(int)IK.RightLeg].transform.position;
		settingIK[(int)IK.RightLeg] = true;
		ObjectOffsets[(int)IK.RightLeg] = new Vector3(side ? -0.5f : 0.5f, 0, 0);
		while (side ? (transform.position.x + 0.4f > t.position.x) : (transform.position.x - 0.4f < t.position.x))
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + (side ? Vector3.left : Vector3.right), 3f * Time.deltaTime);
			yield return null;
		}
		rBody.simulated = true;
		returningToPosition[(int)IK.RightLeg] = true;
		returningToPosition[(int)IK.LeftLeg] = true;
		while (!grounded)
		{
			if (CancelClimbOnInput())
			{
				yield break;
			}
			yield return null;
		}
		yield return null;
		climbing = false;
	}

	/// <summary>
	/// Cancels climbing given player input. 
	/// </summary>
	/// <returns><c>true</c>, if climb on input was canceled, <c>false</c> otherwise.</returns>
	private bool CancelClimbOnInput()
	{
		if (DBInputManager.GetInput(this, PlayerInput.Left, InputType.Held, false) ||
			DBInputManager.GetInput(this, PlayerInput.Right, InputType.Held, false) ||
			DBInputManager.GetInput(this, PlayerInput.Jump, InputType.Pressed, false))
		{
			CancelKinematics();
			rBody.simulated = true;
			gCounter = 0;
			climbing = false;
		}
		return !climbing;
	}

	#endregion

	#endregion
}
