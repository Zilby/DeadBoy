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
	#region StaticVariables

	/// <summary>
	/// The main player (ie: controlled player).
	/// </summary>
	public static PlayerController MainPlayer
	{
		get { return mainPlayer; }
		// Set mainplayer in front
		set
		{
			if (mainPlayer != null)
			{
				mainPlayer.transform.position = new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y, 0);
			}
			mainPlayer = value;
			mainPlayer.transform.position = new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y, -1f);
			CameraController.movingToNewPosition = true;
		}
	}
	/// <summary>
	/// The main player (ie: controlled player).
	/// </summary>
	protected static PlayerController mainPlayer;
	/// <summary>
	/// All of the available players.
	/// </summary>
	public static List<PlayerController> players = new List<PlayerController>();

	#endregion

	#region Fields

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
	protected bool returningPosition;

	/// <summary>
	/// The location of an objet to be interacted with.
	/// </summary>
	protected Transform objectLocation = null;

	/// <summary>
	/// Whether or not the player is currently pulling. 
	/// </summary>
	protected bool pulling;

	/// <summary>
	/// Whether or not the player is currently picking up a pickup.
	/// </summary>
	protected bool pickingUp;

	/// <summary>
	/// The current pickup held.
	/// </summary>
	protected Pickup.Type currentPickup = Pickup.Type.none;

	/// <summary>
	/// Action to be called once an object is pressed. 
	/// </summary>
	protected Action pressAction;

	/// <summary>
	/// The location to drag the object to.
	/// </summary>
	protected Vector3 dragLocation = Vector3.zero;

	/// <summary>
	/// The speed of dragging an object. 
	/// </summary>
	protected float dragSpeed;

	/// <summary>
	/// Gets the current pickup.
	/// </summary>
	public Pickup.Type CurrentPickup
	{
		get { return currentPickup; }
	}

	/// <summary>
	/// The last Right arm location.
	/// </summary>
	protected Vector3 lastRALocation = Vector3.zero;

	/// <summary>
	/// The last Left arm location.
	/// </summary>
	protected Vector3 lastLALocation = Vector3.zero;

	/// <summary>
	/// The original Right arm location.
	/// </summary>
	protected Vector3 originalRALocation = Vector3.zero;

	/// <summary>
	/// The original Left arm location.
	/// </summary>
	protected Vector3 originalLALocation = Vector3.zero;

	/// <summary>
	/// The max y velocity before no longer being considered grounded.
	/// </summary>
	protected const float MAX_Y_VELOCITY = 5;

	/// <summary>
	/// The minimum y velocity before no longer being considered grounded.
	/// </summary>
	protected const float MIN_Y_VELOCITY = -3;

	/// <summary>
	/// The sort value that determines which player gets selected next when toggling between players. 
	/// It also determines their corresponding number key. 
	/// </summary>
	protected abstract int SORT_VALUE { get; }

	protected bool AcceptingMoveInput
	{
		get
		{
			return MainPlayer == this && (originalRALocation == Vector3.zero && originalLALocation == Vector3.zero || pulling);
		}
	}


	#endregion

	#region Functions

	#region General

	protected virtual void Awake()
	{
		players.Add(this);
	}

	protected virtual void OnDestroy()
	{
		players.Remove(this);
	}

	protected virtual void Start()
	{
		rBody = rBody == null ? GetComponent<Rigidbody2D>() : rBody;
		cCollider = cCollider == null ? GetComponent<CapsuleCollider2D>() : cCollider;
		anim = anim == null ? GetComponent<Animator>() : anim;
		jumpStart = Time.fixedTime - 100f;
		if (isMainPlayer)
		{
			MainPlayer = this;
			players.Sort(delegate (PlayerController p1, PlayerController p2)
			{
				if (p1.SORT_VALUE < p2.SORT_VALUE)
				{
					return 1;
				}
				else if (p1.SORT_VALUE > p2.SORT_VALUE)
				{
					return -1;
				}
				return 0;
			});
			StartCoroutine(SwapPlayers());
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
		if (AcceptingMoveInput)
		{
			Jump();
		}
		SetAnimationState();
	}

	protected virtual void LateUpdate()
	{
		if (pulling)
		{
			SetArmLocations();
		}
		if (pickingUp)
		{
			SetArmLocations(l: false);
			if (Vector2.Distance(lastRALocation, returningPosition ? originalRALocation : objectLocation.position) < 0.1f)
			{
				if (returningPosition)
				{
					pickingUp = false;
					returningPosition = false;
					originalRALocation = Vector3.zero;
				}
				else
				{
					objectLocation.parent = rightArm.transform;
					returningPosition = true;
				}
			}
		}
		if (pressAction != null)
		{
			SetArmLocations(l: false);
			if (Vector2.Distance(lastRALocation, returningPosition ? originalRALocation : objectLocation.position) < 0.1f)
			{
				if (returningPosition)
				{
					pressAction = null;
					returningPosition = false;
					originalRALocation = Vector3.zero;
				}
				else
				{
					pressAction();
					returningPosition = true;
				}
			}
		}
		if (dragSpeed != 0)
		{
			SetArmLocations();
			if (Vector2.Distance(lastRALocation, returningPosition ? originalRALocation : objectLocation.position) < 0.01f &&
				Vector2.Distance(lastLALocation, returningPosition ? originalLALocation : objectLocation.position) < 0.01f)
			{
				if (returningPosition)
				{
					dragSpeed = 0;
					returningPosition = false;
					originalRALocation = Vector3.zero;
					originalLALocation = Vector3.zero;
				}
				else
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
						dragLocation = Vector3.zero;
						returningPosition = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Changes the current player. 
	/// </summary>
	private IEnumerator SwapPlayers()
	{
		while (MainPlayer == this)
		{
			yield return null;
			// Go through each player with return;
			if (Input.GetKeyDown(KeyCode.Return))
			{
				int curr = players.IndexOf(MainPlayer);
				curr += 1;
				if (curr >= players.Count)
				{
					curr = 0;
				}
				MainPlayer = players[curr];
				MainPlayer.StartCoroutine(MainPlayer.SwapPlayers());
			}
			// Set individual player based on hitting their sort value number key. 
			for (int i = 0; i < Utils.keyCodes.Length; i++)
			{
				if (Input.GetKeyDown(Utils.keyCodes[i]))
				{
					foreach (PlayerController p in players)
					{
						if (p.SORT_VALUE == i && p != MainPlayer)
						{
							MainPlayer = p;
							MainPlayer.StartCoroutine(MainPlayer.SwapPlayers());
							break;
						}
					}
				}
			}
		}
	}


	/// <summary>
	/// Moves the player left or right if given input. 
	/// </summary>
	protected virtual void Move()
	{
		float movement = 0.0f;
		if (Input.GetKey(KeyCode.A) && AcceptingMoveInput)
		{
			movement -= speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D) && AcceptingMoveInput)
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
			if (Input.GetKeyDown(KeyCode.Space) && CanJump())
			{
				rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
				jumpStart = Time.fixedTime;
				jumpHeld = true;
			}
			if (!Input.GetKey(KeyCode.Space) && !grounded)
			{
				jumpHeld = false;
			}
			// Add more force if jump held for longer. 
			else if (Input.GetKey(KeyCode.Space) &&
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
		if (!pulling && AcceptingMoveInput)
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
		anim.SetBool("Flipped", transform.localEulerAngles.y == 180);
		if (AcceptingMoveInput)
		{
			anim.SetBool("RightInput", Input.GetKey(KeyCode.D));
			anim.SetBool("LeftInput", Input.GetKey(KeyCode.A));
			anim.SetBool("AnyInput", Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A));
		}
		else
		{
			anim.SetBool("RightInput", false);
			anim.SetBool("LeftInput", false);
			anim.SetBool("AnyInput", false);
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
								(transform.localScale.y * cCollider.size.y) > 0.47f;
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

	#region Interactables

	/// <summary>
	/// Tries to pull an object. 
	/// </summary>
	public bool Pulling(Transform position)
	{
		if (grounded || pulling)
		{
			pulling = !pulling;
			objectLocation = position;
			lastRALocation = rightArm.transform.position;
			lastLALocation = leftArm.transform.position;
		}
		return pulling;
	}


	/// <summary>
	/// Sets the pulling for the limbs in the animation.
	/// </summary>
	public void SetArmLocations(bool r = true, bool l = true)
	{
		float s = 5f * (rBody.velocity.magnitude + 1) * Time.deltaTime;
		if (r)
		{
			lastRALocation = Vector3.MoveTowards(lastRALocation, returningPosition ? originalRALocation : objectLocation.position, s);
			rightArm.transform.position = lastRALocation;
			rightArm.UpdateIK();
		}
		if (l)
		{
			lastLALocation = Vector3.MoveTowards(lastLALocation, returningPosition ? originalLALocation : objectLocation.position, s);
			leftArm.transform.position = lastLALocation;
			leftArm.UpdateIK();
		}
	}

	/// <summary>
	/// Picks up the given pickup at the given transform.
	/// </summary>
	public void PickUp(Transform t, Pickup.Type p)
	{
		lastRALocation = rightArm.transform.position;
		originalRALocation = rightArm.transform.position;
		objectLocation = t;
		pickingUp = true;
		currentPickup = p;
	}

	/// <summary>
	/// Presses the given location
	/// </summary>
	public void Press(Transform t, Action a)
	{
		lastRALocation = rightArm.transform.position;
		originalRALocation = rightArm.transform.position;
		objectLocation = t;
		pressAction = a;
	}

	/// <summary>
	/// Grabs the given object and drags it to the given location..
	/// </summary>
	public void GrabAndDrag(Transform t, Vector3 position, float speed = 2)
	{
		lastRALocation = rightArm.transform.position;
		originalRALocation = rightArm.transform.position;
		lastLALocation = leftArm.transform.position;
		originalLALocation = leftArm.transform.position;
		objectLocation = t;
		dragLocation = position;
		dragSpeed = speed;
	}

	/// <summary>
	/// Uses the current pickup.
	/// </summary>
	public void UsePickup()
	{
		currentPickup = Pickup.Type.none;
		Destroy(objectLocation.gameObject);
	}

	#endregion
	#endregion
}
