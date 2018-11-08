﻿using System;
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

	#region Fields

	#region Public

	[Header("References")]
	public Rigidbody2D rBody;
	public CapsuleCollider2D cCollider;
	public Animator anim;

	[Header("InverseKinematics")]
	public IkLimb2D rightArm;
	public IkLimb2D leftArm;
	public IkLimb2D rightLeg;
	public IkLimb2D leftLeg;
	public IkLimb2D rightFoot;
	public IkLimb2D leftFoot;

	[Header("Characteristics")]
	/// <summary>
	/// Whether or not this is the main player when starting the level. 
	/// </summary>
	public bool isMainPlayer = false;

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
	/// Whether or not the player is in the air. 
	/// </summary>
	protected bool grounded;

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
	/// Whether or not the hands are returning to their original position.
	/// </summary>
	protected bool returningArmPosition;

	/// <summary>
	/// Whether or not the legs are returning to their original position.
	/// </summary>
	protected bool returningLegPosition;

	/// <summary>
	/// The location of an objet to be interacted with.
	/// </summary>
	protected Transform objectLocation = null;

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
	/// Whether or not the right arm location is currently being set. 
	/// </summary>
	protected bool settingRA = false;

	/// <summary>
	/// Whether or not the left arm location is currently being set. 
	/// </summary>
	protected bool settingLA = false;

	/// <summary>
	/// Whether or not the right leg location is currently being set. 
	/// </summary>
	protected bool settingRL = false;

	/// <summary>
	/// Whether or not the left leg location is currently being set. 
	/// </summary>
	protected bool settingLL = false;

	/// <summary>
	/// The last Right arm location.
	/// </summary>
	protected Vector3 lastRALocation = Vector3.zero;

	/// <summary>
	/// The last Left arm location.
	/// </summary>
	protected Vector3 lastLALocation = Vector3.zero;

	/// <summary>
	/// The last Right leg location.
	/// </summary>
	protected Vector3 lastRLLocation = Vector3.zero;

	/// <summary>
	/// The last Left leg location.
	/// </summary>
	protected Vector3 lastLLLocation = Vector3.zero;

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
	protected const float MIN_Y_VELOCITY = -3;

	/// <summary>
	/// The limb move speed.
	/// </summary>
	protected const float LIMB_MOVE_SPEED = 3f;

	#endregion

	#region Properties

	/// <summary>
	/// Gets the current pickup.
	/// </summary>
	public Pickup.Type CurrentPickup
	{
		get { return currentPickup; }
	}

	/// <summary>
	/// Whether or not the right arm has reached the location it was moving towards. 
	/// </summary>
	protected bool rightArmAtLocation
	{
		get { return !settingRA || Vector2.Distance(lastRALocation, returningArmPosition ? rightArm.transform.position : objectLocation.position) < 0.001f; }
	}

	/// <summary>
	/// Whether or not the left arm has reached the location it was moving towards. 
	/// </summary>
	protected bool leftArmAtLocation
	{
		get { return !settingLA || Vector2.Distance(lastLALocation, returningArmPosition ? leftArm.transform.position : objectLocation.position) < 0.001f; }
	}

	/// <summary>
	/// Whether or not the right leg has reached the location it was moving towards. 
	/// </summary>
	protected bool rightLegAtLocation
	{
		get { return !settingRL || Vector2.Distance(lastRLLocation, returningLegPosition ? rightLeg.transform.position : objectLocation.position) < 0.001f; }
	}

	/// <summary>
	/// Whether or not the left leg has reached the location it was moving towards. 
	/// </summary>
	protected bool leftLegAtLocation
	{
		get { return !settingLL || Vector2.Distance(lastLALocation, returningLegPosition ? leftLeg.transform.position : objectLocation.position) < 0.001f; }
	}

	public bool AcceptingMoveInput 
	{
		get { return ((!settingRA && !settingLA) || pulling) && !climbing; }
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

	protected virtual void Awake()
	{
		rBody = rBody == null ? GetComponent<Rigidbody2D>() : rBody;
		cCollider = cCollider == null ? GetComponent<CapsuleCollider2D>() : cCollider;
		anim = anim == null ? GetComponent<Animator>() : anim;
		InputManager.Register(this, isMainPlayer);
	}

	protected virtual void OnDestroy()
	{
		InputManager.Unregister(this);
	}

	protected virtual void Start()
	{
		jumpStart = Time.fixedTime - 100f;
		foreach (SpriteMeshInstance s in Sprites)
		{
			s.sortingOrder += 100 * SORT_VALUE;
		}
	}

	protected virtual void FixedUpdate()
	{
		if (rBody.velocity.y >= MAX_Y_VELOCITY || (rBody.velocity.y < MIN_Y_VELOCITY && cCollider.GetContacts(new Collider2D[0]) == 0))
		{
			grounded = false;
		}
		Move();
	}

	protected virtual void Update()
	{
		Jump();
		SetAnimationState();
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
		float movement = 0.0f;
		if (InputManager.GetInputHeld(this, PlayerInput.Left))
		{
			movement -= speed * Time.deltaTime;
		}
		if (InputManager.GetInputHeld(this, PlayerInput.Right))
		{
			movement += speed * Time.deltaTime;
		}
		float acceleratedMove;
		if (grounded)
		{
			acceleratedMove = movement == 0.0f ? rBody.velocity.x * (1 - (acceleration / 2f)) : rBody.velocity.x + (movement * acceleration);
		}
		else
		{
			acceleratedMove = rBody.velocity.x + (movement * aerialControl);
		}
		// Clamp the accelerated move to the maximum speeds. 
		movement = Mathf.Clamp(acceleratedMove, speed * Time.fixedDeltaTime * -1, speed * Time.fixedDeltaTime);
		rBody.velocity = new Vector2(movement, rBody.velocity.y);
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
			if (InputManager.GetInputStart(this, PlayerInput.Jump) && CanJump())
			{
				rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
				jumpStart = Time.fixedTime;
				jumpHeld = true;
			}
			if (!InputManager.GetInputHeld(this, PlayerInput.Jump) && !grounded)
			{
				jumpHeld = false;
			}
			// Add more force if jump held for longer. 
			else if (InputManager.GetInputHeld(this, PlayerInput.Jump) &&
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
		if (!pulling)
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
		anim.SetFloat("XVel", rBody.velocity.x);
		anim.SetFloat("YVel", rBody.velocity.y);
		anim.SetFloat("XMag", Mathf.Abs(rBody.velocity.x));
		anim.SetFloat("YMag", Mathf.Abs(rBody.velocity.y));
		anim.SetBool("Fell", anim.GetBool("Grounded") != grounded && grounded);
		anim.SetBool("Grounded", grounded);
		anim.SetBool("Pulling", pulling);
		anim.SetBool("Climbing", climbing);
		anim.SetBool("Flipped", transform.localEulerAngles.y == 180);

		bool left = InputManager.GetInputHeld(this, PlayerInput.Left);
		bool right = InputManager.GetInputHeld(this, PlayerInput.Right);
		anim.SetBool("RightInput", right);
		anim.SetBool("LeftInput", left);
		anim.SetBool("AnyInput", left || right);
	}

	///

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

	void CheckGrounded(Collision2D collision)
	{
		foreach (ContactPoint2D contact in collision.contacts)
		{
			grounded = rBody.velocity.y < MAX_Y_VELOCITY && (grounded || TouchingGround(contact));
			/*
			print(contact.collider.name + " hit " + contact.otherCollider.name + " " + 
			      (Vector2.Distance(transform.position, contact.point) / (transform.localScale.x * cCollider.size.y)).ToString());
			*/
		}
	}

	bool TouchingGround(ContactPoint2D contact)
	{
		return contact.point.y < transform.position.y &&
				Vector2.Distance(transform.position, contact.point) /
								(transform.localScale.y) > 4.5f;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			swimming = true;
			this.EnterWater(collision);
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
		CameraController.Deactivate();
		LevelManager.instance.RestartLevel();
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
		if (settingRA)
		{
			lastRALocation = rightArm.transform.position;
		}
		if (settingLA)
		{
			lastLALocation = leftArm.transform.position;
		}
		if (settingRL)
		{
			lastRLLocation = rightLeg.transform.position;
		}
		if (settingLL)
		{
			lastLLLocation = leftLeg.transform.position;
		}
		objectLocation = t;
		returningArmPosition = false;
		returningLegPosition = false;
	}


	/// <summary>
	/// Sets the pulling for the limbs in the animation.
	/// </summary>
	protected void SetLimbLocations()
	{
		float s = LIMB_MOVE_SPEED * (rBody.velocity.magnitude + 1) * Time.deltaTime;
		if (settingRA)
		{
			lastRALocation = Vector3.MoveTowards(lastRALocation, returningArmPosition ? rightArm.transform.position : objectLocation.position, s);
			rightArm.transform.position = lastRALocation;
			rightArm.UpdateIK();
		}
		if (settingLA)
		{
			lastLALocation = Vector3.MoveTowards(lastLALocation, returningArmPosition ? leftArm.transform.position : objectLocation.position, s);
			leftArm.transform.position = lastLALocation;
			leftArm.UpdateIK();
		}
		if (settingRL)
		{
			lastRLLocation = Vector3.MoveTowards(lastRLLocation, returningLegPosition ? rightLeg.transform.position : objectLocation.position, s);
			rightLeg.transform.position = lastRLLocation;
			rightLeg.UpdateIK();
		}
		if (settingLL)
		{
			lastLLLocation = Vector3.MoveTowards(lastLLLocation, returningLegPosition ? leftLeg.transform.position : objectLocation.position, s);
			leftLeg.transform.position = lastLLLocation;
			leftLeg.UpdateIK();
		}
	}

	/// <summary>
	/// Updates the IKs to go where they're set to go.
	/// </summary>
	protected void UpdateKinematics()
	{
		if (settingLA || settingRA || settingLL || settingRL)
		{
			if (rightArmAtLocation && leftArmAtLocation)
			{
				if (returningArmPosition)
				{
					returningArmPosition = false;
					settingRA = false;
					settingLA = false;
					interactAction = null;
					dragSpeed = 0;
				}
				else
				{
					if (dragSpeed != 0)
					{
						DragToLocation();
					}
					else if (!pulling && !climbing)
					{
						SetUpReturningHandPositions();
					}
				}
			}
			if (rightLegAtLocation && leftLegAtLocation) {
				if (returningLegPosition)
				{
					returningLegPosition = false;
					settingRL = false;
					settingLL = false;
				}
				else
				{
					if (!pulling && !climbing)
					{
						returningLegPosition = true;
					}
				}
			}
			SetLimbLocations();
		}
	}


	/// <summary>
	/// Sets up returning hands to their default positions.
	/// </summary>
	protected void SetUpReturningHandPositions()
	{
		returningArmPosition = true;
		interactAction?.Invoke();
		dragLocation = Vector3.zero;
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
			SetUpReturningHandPositions();
		}
	}

	#endregion

	#region Interactables

	/// <summary>
	/// Tries to pull an object. 
	/// </summary>
	public bool Pulling(Transform t)
	{
		if (grounded || pulling)
		{
			pulling = !pulling;
			if (pulling)
			{
				settingRA = true;
				settingLA = true;
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
		settingRA = true;
		SetUpLimbMovement(t);
		interactAction = delegate
		{
			objectLocation.parent = rightArm.transform;
			currentPickup = p;
		};
	}

	/// <summary>
	/// Presses the given location
	/// </summary>
	public void Press(Transform t, Action a)
	{
		settingRA = true;
		SetUpLimbMovement(t);
		interactAction = a;
	}

	/// <summary>
	/// Grabs the given object and drags it to the given location..
	/// </summary>
	public void GrabAndDrag(Transform t, Vector3 position, Action a, float speed = 2)
	{
		settingRA = true;
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
		Destroy(objectLocation.gameObject);
	}

	/// <summary>
	/// Climbs a ledge
	/// </summary>
	public IEnumerator ClimbLedge(Transform t)
	{
		climbing = true;
		rBody.simulated = false;
		settingRA = true;
		settingLA = true;
		SetUpLimbMovement(t);
		while(transform.position.y < t.position.y) {
			transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 3f * Time.deltaTime);
			yield return null;
		}
		returningArmPosition = true;
		settingRL = true;
		while (cCollider.bounds.min.y < t.position.y) {
			transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 3f * Time.deltaTime);
			yield return null;
		}
		returningLegPosition = true;
		rBody.simulated = true;
		climbing = false;
	}

	#endregion

	#endregion
}
