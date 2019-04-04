using System;
using System.Collections;
using System.Collections.Generic;
using Anima2D;
using UnityEngine;
using UnityEngine.Sprites;
using System.Text.RegularExpressions;

public enum Character
{
	Deadboy = 1,
	DrownedGirl = 2,
	Squish = 3,
	ElectricBaby = 4,
	Firekid = 5
}

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

	public static Action<SFXManager.FootstepType> SetFootstepType;

	#region Fields

	#region Public

	[Header("Automatic References")]
	public Rigidbody2D rBody;
	public CapsuleCollider2D cCollider;
	public Animator anim;

	[Header("Other References")]
	public Fadeable indicator;
	public ParticleSystem electricPS;

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
	/// How fast the player decelerates on the ground. 
	/// </summary>
	[Range(0, 1)]
	public float deceleration;
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
	/// Whether the last checkpoint reached is underground. 
	/// </summary>
	protected bool checkpointUnderground;

	/// <summary>
	/// The current footstep type. 
	/// </summary>
	protected SFXManager.FootstepType footstepType;

	/// <summary>
	/// The current footstep type. 
	/// </summary>
	public SFXManager.FootstepType FootstepType
	{
		get { return footstepType; }
		set { footstepType = value; }
	}

	/// <summary>
	/// Whether or not the player is in the air. 
	/// </summary>
	protected bool grounded
	{
		get { return gCounter < 5; }
	}

	/// <summary>
	/// Number of frames since last grounded. 
	/// </summary>
	protected uint gCounter = 0;

	/// <summary>
	/// The surface normal.
	/// </summary>
	protected Vector2 surfaceNormal = Vector2.up;

	/// <summary>
	/// The original physics material.
	/// </summary>
	protected PhysicsMaterial2D originalMat;

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
	public bool climbing { get; protected set; }

	/// <summary>
	/// Whether or not the current player is underground. 
	/// </summary>
	protected bool underground;

	/// <summary>
	/// Whether or not the player is dying. 
	/// </summary>
	protected bool dying;

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
	/// Distance from collider where the player is considered touching. 
	/// </summary>
	protected const float TOUCHING_DIST = 0.1F;

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
	public string Name
	{
		get
		{
			// Add space after letters that occur before capital letters.
			return Regex.Replace(CharID.ToString(), "\\w(?=[A-Z])", delegate (Match m) { return m.Value + " "; });
		}
	}

	public virtual float GetJumpHeight
	{
		get { return jumpHeight; }
	}

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
	/// Whether or not the player is currently pulling.
	/// </summary>
	public bool IsPulling
	{
		get { return pulling; }
		set { pulling = value; }
	}

	/// <summary>
	/// Whether or not the player is currently underground. 
	/// </summary>
	public bool Underground
	{
		get { return underground; }
		set
		{
			if (underground != value)
			{
				UndergroundSwapper.SwapEvent?.Invoke(value);
				underground = value;
			}
		}
	}

	/// <summary>
	/// Whether the player is holding an electrical charge.
	/// </summary>
	public virtual bool HoldingCharge
	{
		get { return false; }
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
	/// Height of the collider
	/// </summary>
	/// <value>The height of the collider.</value>
	protected float ColliderHeight
	{
		get
		{
			return (cCollider.size.y / 2.0f) * transform.localScale.y;
		}
	}

	/// <summary>
	/// Gets the collider offset.
	/// </summary>
	protected float ColliderOffset
	{
		get
		{
			return cCollider.offset.y * transform.localScale.y;
		}
	}

	/// <summary>
	/// Whether or not this player is currently accepting movement input. 
	/// </summary>
	public virtual bool AcceptingMoveInput
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

	public abstract Character CharID { get; }

	/// <summary>
	/// Determines which characters gets selected next when toggling between players. 
	/// Determines their corresponding number key. 
	/// Determines sprite sorting order.
	/// </summary>
	public int CharIDInt { get { return ((int)CharID); } }

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
		originalMat = cCollider.sharedMaterial;
		SetFootstepType += delegate (SFXManager.FootstepType f) { footstepType = f; };
	}

	protected virtual void OnDestroy()
	{
		DBInputManager.Unregister(this);
		SetFootstepType -= delegate (SFXManager.FootstepType f) { footstepType = f; };
	}

	protected virtual void Start()
	{
		DBInputManager.Register(this, initialPlayer);
		jumpStart = Time.fixedTime - 100f;
		foreach (SpriteMeshInstance s in Sprites)
		{
			s.sortingOrder += 100 * CharIDInt;
		}
	}

	protected virtual void FixedUpdate()
	{
		CheckGrounded();
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

		if (DBInputManager.instance.restrictInput)
		{
			//analog = Mathf.Clamp(analog, -0.3f, 0.3f);
			analog = 0;
		}

		float movespeed = speed * analog * Time.fixedDeltaTime;

		float acceleratedMove;
		if (grounded)
		{
			acceleratedMove = Mathf.Abs(movespeed) <= 0.01f ? rBody.velocity.x * (1 - deceleration) : rBody.velocity.x + (movespeed * acceleration);
		}
		else
		{
			acceleratedMove = rBody.velocity.x + (movespeed * aerialControl);
		}

		if (analog == 0f && grounded && (Mathf.Abs(acceleratedMove) < 1f || Mathf.Abs(surfaceNormal.x) > 0.1f))
		{
			PhysicsMaterial2D m = new PhysicsMaterial2D();
			m.bounciness = originalMat.bounciness;
			m.friction = 1f;
			cCollider.sharedMaterial = m;
		}
		else
		{
			cCollider.sharedMaterial = originalMat;
		}

		if (analog == 0f && grounded && Mathf.Abs(acceleratedMove) < 1f && Mathf.Abs(surfaceNormal.x) > 0.1f)
		{
			rBody.constraints = (RigidbodyConstraints2D)5;
		}
		else
		{
			rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
			// Clamp the accelerated move to the maximum speeds. 
			float maxSpeed = speed * Time.fixedDeltaTime;
			if (Mathf.Abs(movespeed) > 0.01f)
			{
				maxSpeed *= analog;
			}
			movespeed = Mathf.Clamp(acceleratedMove, -Mathf.Abs(maxSpeed), Mathf.Abs(maxSpeed));
			if (Mathf.Abs(surfaceNormal.x) < 0.2f)
			{
				rBody.velocity = rBody.velocity.X(movespeed);
			}
			else
			{
				bool down = surfaceNormal.x * movespeed > 0;
				rBody.velocity = new Vector2(movespeed * (1 - Mathf.Abs(surfaceNormal.x / 3f)),
											 rBody.velocity.y + (Mathf.Abs(movespeed) * (1 - Mathf.Abs(surfaceNormal.y))) * (down ? -1 : 1));
			}
		}
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
				rBody.velocity = rBody.velocity.Y(GetJumpHeight);
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
				rBody.velocity = rBody.velocity.Y(GetJumpHeight);
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
			transform.localEulerAngles = transform.localEulerAngles.Y(flip);
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

		if (rBody.velocity.y < -5)
		{
			anim.SetFloat("FallingYVelocity", rBody.velocity.y);
		}
		else if (rBody.velocity.y > 5)
		{
			anim.SetFloat("FallingYVelocity", 0);
		}

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

		anim.SetBool("Controlled", DBInputManager.IsControlled(this));
	}

	/// <summary>
	/// Plays a footstep sound effect for the given IK
	/// </summary>
	public virtual void Footstep(IK ik)
	{
		string key = "Footstep" + this.Name + footstepType.ToString();
		SFXManager.instance.PlayClip(key, 0.2f, location: iKLimbs[(int)ik].transform.position);
	}

	public virtual void SwitchedFrom()
	{
		indicator.Hide();
	}

	public virtual void SwitchedTo()
	{
		indicator.Show();
		indicator.DelayedFadeOut();
	}

	protected void PreserveIndicatorDirection()
	{
		indicator.gameObject.GetComponent<SpriteRenderer>().flipX = transform.localEulerAngles.y > 90;
	}

	public virtual IEnumerator Die()
	{
		if (!dying)
		{
			dying = true;
			Underground = checkpointUnderground;
			CameraController.DeactivateEvent();
			DBInputManager.instance.enabled = false;
			yield return Fader.FadeIn();
			CameraController.ActivateEvent();
			transform.position = checkpoint.transform.position;
			rBody.velocity = Vector3.zero;
			yield return new WaitForSeconds(0.2f);
			DBInputManager.instance.enabled = true;
			yield return Fader.FadeOut();
			dying = false;
			if (checkpoint == null)
			{
				LevelManager.instance.RestartLevel();
				yield return null;
			}
		}
	}

	#endregion

	#region CollisionDetection

	protected virtual void CheckGrounded()
	{
		if (rBody.velocity.y >= MAX_Y_VELOCITY)
		{
			gCounter += 10;
		}
		else if (rBody.velocity.y < MIN_Y_VELOCITY && !TouchingGround())
		{
			gCounter++;
		}
		else if (rBody.velocity.y < MAX_Y_VELOCITY && TouchingGround() && !swimming)
		{
			gCounter = 0;
		}
	}

	bool TouchingGround()
	{
		RaycastHit2D[] hits = new RaycastHit2D[10];
		cCollider.Raycast(Vector2.down, hits, ColliderHeight + TOUCHING_DIST, Physics2D.GetLayerCollisionMask(gameObject.layer));
		foreach (RaycastHit2D r in hits)
		{
			if (r.collider != null && !r.collider.isTrigger &&
				(anim.GetFloat("OldYVel") < -5 || r.point.y <= transform.position.y - ((ColliderHeight - ColliderOffset) * 7f / 8f)))
			{
				surfaceNormal = r.normal;
				return true;
			}
		}
		surfaceNormal = Vector2.zero;
		return false;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			this.EnterWater(collision);
		}
		if (collision.tag == "Grate")
		{
			this.EnterGrate(collision);
		}
		if (collision.tag == "Checkpoint")
		{
			checkpoint = collision.gameObject.transform;
			checkpointUnderground = false;
		}
		if (collision.tag == "UndergroundCheckpoint") {
			checkpoint = collision.gameObject.transform;
			checkpointUnderground = true;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			this.StayWater(collision);
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			this.ExitWater(collision);
		}
		if (collision.tag == "Grate")
		{
			this.ExitGrate(collision);
		}
	}

	#endregion

	#region Water

	protected virtual void EnterWater(Collider2D water)
	{
	}

	protected virtual void StayWater(Collider2D water)
	{
		if (water.OverlapPoint(transform.position.YAdd(ColliderHeight + ColliderOffset).YMul(7f / 8f)))
		{
			StartCoroutine(Die());
		}
	}

	protected virtual void ExitWater(Collider2D water)
	{
	}

	#endregion

	#region Grates

	protected virtual void EnterGrate(Collider2D trigger)
	{
	}

	protected virtual void ExitGrate(Collider2D trigger)
	{
	}

	#endregion

	#region Charge

	public virtual void TouchedCharged(bool charged)
	{
		if (charged)
		{
			StartCoroutine(Die());
		}
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
			ObjectOffsets[i] = Vector3.zero;
		}
		objectLocation = null;
	}


	/// <summary>
	/// Sets the pulling for the limb in the animation.
	/// </summary>
	protected void SetLocation(IK ik)
	{
		int i = (int)ik;
		float s = LimbMoveSpeed / (climbing && !returningToPosition[i] ? 0.5f : (returningToPosition[i] && (i == (int)IK.LeftLeg || i == (int)IK.RightLeg) ? 3f : 1f));
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
			objectLocation.position = objectLocation.position.XYAdd(speedx, speedy);
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
	/// Touches the given location
	/// </summary>
	public void Touch(Transform t, Action a)
	{
		Press(t, a); //same for now, might be different hands later.
	}

	/// <summary>
	/// Grabs the given object and drags it to the given location..
	/// </summary>
	public void GrabAndDrag(Transform t, Vector3 position, Action a, float speed = 2, bool bothArms = false)
	{
		settingIK[(int)IK.RightArm] = true;
		if (bothArms)
		{
			settingIK[(int)IK.LeftArm] = true;
		}
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
	public virtual IEnumerator ClimbLedge(Transform t)
	{
		bool side = transform.position.x > t.position.x;
		climbing = true;
		pulling = true;
		rBody.simulated = false;
		rBody.velocity = Vector3.zero;
		// Pull up character
		settingIK[(int)IK.RightArm] = true;
		settingIK[(int)IK.LeftArm] = true;
		ObjectOffsets[(int)IK.RightArm] = ObjectOffsets[(int)IK.LeftArm] = new Vector3(side ? -0.3f : 0.3f, -0.1f, 0);
		SetUpLimbMovement(t);
		while (transform.position.y < t.position.y)
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 5f * Time.deltaTime);
			yield return null;
			if (pulling && transform.position.y > t.position.y - 1.5f)
			{
				pulling = false;
			}
		}
		pulling = false;
		// Climb onto ledge
		while (cCollider.bounds.min.y < t.position.y)
		{
			if ((!returningToPosition[(int)IK.RightArm] || !returningToPosition[(int)IK.LeftArm]) && cCollider.bounds.min.y > t.position.y - 1f)
			{
				returningToPosition[(int)IK.RightArm] = true;
				returningToPosition[(int)IK.LeftArm] = true;
			}
			if (!settingIK[(int)IK.LeftLeg] && cCollider.bounds.min.y > t.position.y - 2f)
			{
				ObjectOffsets[(int)IK.LeftLeg] = new Vector3(side ? -0.2f : 0.2f, 0, 0);
				lastIKLocation[(int)IK.LeftLeg] = iKLimbs[(int)IK.LeftLeg].transform.position;
				settingIK[(int)IK.LeftLeg] = true;
			}
			transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 3f * Time.deltaTime);
			yield return null;
		}
		transform.position = transform.position.YAdd(t.position.y - cCollider.bounds.min.y);

		lastIKLocation[(int)IK.RightLeg] = iKLimbs[(int)IK.RightLeg].transform.position;
		settingIK[(int)IK.RightLeg] = true;
		ObjectOffsets[(int)IK.RightLeg] = new Vector3(side ? -0.5f : 0.5f, 0, 0);
		while (side ? (transform.position.x + 0.4f > t.position.x) : (transform.position.x - 0.4f < t.position.x))
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + (side ? Vector3.left : Vector3.right), 3f * Time.deltaTime);
			yield return null;
		}
		rBody.simulated = true;

		//returningToPosition[(int)IK.RightLeg] = true;
		//returningToPosition[(int)IK.LeftLeg] = true;
		CancelKinematics();
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
