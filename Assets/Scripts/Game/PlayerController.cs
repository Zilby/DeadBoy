using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

/// <summary>
/// This class controls the player's movement and abilities. 
/// </summary>
public class PlayerController : MonoBehaviour
{

	public static PlayerController mainPlayer;

	[Header("References")]
	public SpriteRenderer sprite;
	public Rigidbody2D rBody;
	public CapsuleCollider2D cCollider;

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

	/// <summary>
	/// Whether or not the player is in the air. 
	/// </summary>
	protected bool grounded;

	/// <summary>
	/// The time of the start of the jump.
	/// </summary>
	protected float jumpStart;

	/// <summary>
	/// Whether or not jump has been held. 
	/// </summary>
	protected bool jumpHeld;


	protected virtual void Start()
	{
		PlayerController.mainPlayer = this;
		sprite = sprite == null ? GetComponent<SpriteRenderer>() : sprite;
		rBody = rBody == null ? GetComponent<Rigidbody2D>() : rBody;
		cCollider = cCollider == null ? GetComponent<CapsuleCollider2D>() : cCollider;
		jumpStart = Time.fixedTime - 100f;
	}

	protected virtual void FixedUpdate()
	{
		Move();
		//Jump();
	}

	protected virtual void Update()
	{
		Jump();
	}


	/// <summary>
	/// Moves the player left or right if given input. 
	/// </summary>
	protected virtual void Move()
	{
		float movement = 0.0f;
		if (Input.GetKey(KeyCode.A))
		{
			movement -= speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D))
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
		movement = Mathf.Clamp(acceleratedMove, speed * Time.deltaTime * -1, speed * Time.deltaTime);
		sprite.flipX = Mathf.Abs(movement) <= 1f ? sprite.flipX : movement > 0;
		rBody.velocity = new Vector2(movement, rBody.velocity.y);
	}


	/// <summary>
	/// Makes the player jump if given input. 
	/// </summary>
	protected virtual void Jump()
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

	void OnCollisionStay2D(Collision2D collision)
	{
		CheckGrounded(collision);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		CheckGrounded(collision);
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		grounded = false;
	}

	void CheckGrounded(Collision2D collision)
	{
		foreach (ContactPoint2D contact in collision.contacts)
		{
			bool touchingGround = Vector2.Distance(transform.position, contact.point) / (transform.localScale.x * cCollider.size.y) > 0.47f;
			grounded = rBody.velocity.y <= 5 && (grounded || touchingGround);
			/*
			print(contact.collider.name + " hit " + contact.otherCollider.name + " " + 
			      (Vector2.Distance(transform.position, contact.point) / (transform.localScale.x * cCollider.size.y)).ToString());
			*/
		}
	}
}
