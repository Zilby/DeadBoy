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
			CameraController.movingToNewPlayer = true;
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
	/// Whether or not the player is currently pulling. 
	/// </summary>
	protected bool pulling;

	/// <summary>
	/// The pull location.
	/// </summary>
	protected Transform pullLocation = null;

	/// <summary>
	/// The last Right arm location.
	/// </summary>
	protected Vector3 lastRAlocation = Vector3.zero;

	/// <summary>
	/// The last Left arm location.
	/// </summary>
	protected Vector3 lastLAlocation = Vector3.zero;

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

	//===FUNCTIONS================================================================================================================

	protected virtual void Awake()
	{
		players.Add(this);
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
		if (MainPlayer == this)
		{
			Jump();
		}
		SetAnimationState();
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
		if (Input.GetKey(KeyCode.A) && MainPlayer == this)
		{
			movement -= speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D) && MainPlayer == this)
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
	/// Makes the player jump if given input. 
	/// </summary>
	protected virtual void Jump()
	{
		if (!pulling)
		{
			if (Input.GetKeyDown(KeyCode.Space) && grounded)
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
		anim.SetBool("Flipped", transform.localEulerAngles.y == 180);
		if (MainPlayer == this)
		{
			anim.SetBool("RightInput", Input.GetKey(KeyCode.D));
			anim.SetBool("LeftInput", Input.GetKey(KeyCode.A));
			anim.SetBool("AnyInput", Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A));
		}
	}

	protected virtual void EnterWater(Collider2D water)
	{
		LevelManager.RestartLevel();
	}

	protected virtual void ExitWater(Collider2D water)
	{
	}

	//===COLLISION=DETECTION=======================================================================================================

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

	//===PULLING=======================================================================================================================

	/// <summary>
	/// Tries to pull an object. 
	/// </summary>
	public bool Pulling(Transform position)
	{
		if (grounded || pulling)
		{
			pulling = !pulling;
			pullLocation = position;
			lastRAlocation = rightArm.transform.position;
			lastLAlocation = leftArm.transform.position;
			float flip = transform.localEulerAngles.y;
			if (transform.position.x < position.position.x)
			{
				flip = 180;
			}
			else
			{
				flip = 0;
			}
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, flip, transform.localEulerAngles.z);
		}
		return pulling;
	}


	/// <summary>
	/// Sets the pulling for the limbs in the animation.
	/// </summary>
	public void SetPullingLocations()
	{
		lastRAlocation = Vector3.MoveTowards(lastRAlocation, pullLocation.position, 5f * (rBody.velocity.magnitude + 1) * Time.deltaTime);
		lastLAlocation = Vector3.MoveTowards(lastLAlocation, pullLocation.position, 5f * (rBody.velocity.magnitude + 1) * Time.deltaTime);
		rightArm.transform.position = lastRAlocation;
		leftArm.transform.position = lastLAlocation;
		rightArm.UpdateIK();
		leftArm.UpdateIK();
	}

	protected virtual void LateUpdate()
	{
		if (pulling)
		{
			SetPullingLocations();
		}
	}
}
