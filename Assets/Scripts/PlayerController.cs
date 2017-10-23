using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

/// <summary>
/// This class controls the player's movement and abilities. 
/// </summary>
public class PlayerController : MonoBehaviour {

	[Header("References")]
	public SpriteRenderer sprite;
	public Rigidbody2D rBody;
	public CapsuleCollider2D cCollider;

	[Header("Movement")]
	/// <summary>
	/// Speed of the player. 
	/// </summary>
	public float speed;
	/// <summary>
	/// How fast the player accelerates to max speed on the ground. 
	/// </summary>
	public float acceleration;
	/// <summary>
	/// The strength of deadboy's jump.
	/// </summary>
	public float jumpHeight;
	/// <summary>
	/// Level of aerial control for the player. 
	/// Higher numbers equate to more control. 
	/// Should be between 0 and 1 unless you want 
	/// more control in air than the ground. 
	/// </summary>
	public float aerialControl;

	/// <summary>
	/// Whether or not the player is in the air. 
	/// </summary>
	protected bool grounded;


	protected virtual void Start() {
		sprite = sprite == null ? GetComponent<SpriteRenderer>() : sprite;
		rBody = rBody == null ? GetComponent<Rigidbody2D>() : rBody;
		cCollider = cCollider == null ? GetComponent<CapsuleCollider2D>() : cCollider;
	}


	protected virtual void Update () {
		InAir();
		Move();
		Jump();
	}


	/// <summary>
	/// Moves the player left or right if given input. 
	/// </summary>
	protected virtual void Move() {
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
			acceleratedMove = movement == 0.0f ? rBody.velocity.x * (1 - acceleration) : rBody.velocity.x + (movement * acceleration);
		} 
		else 
		{
			acceleratedMove = rBody.velocity.x + (movement * aerialControl);
		}
		//grounded ? rBody.velocity.x + movement * acceleration : rBody.velocity.x + (movement * aerialControl);
		// Clamp the accelerated move to the maximum speeds. 
		movement = Mathf.Clamp(acceleratedMove, speed * Time.deltaTime * -1, speed * Time.deltaTime);
		sprite.flipX = movement == 0 ? sprite.flipX : movement > 0;
		rBody.velocity = new Vector2(movement, rBody.velocity.y);
	}


	/// <summary>
	/// Makes the player jump if given input. 
	/// </summary>
	protected virtual void Jump() {
		if(Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
			//rBody.AddForce(new Vector2(0, jumpHeight));
		}
	}


	/// <summary>
	/// Determines whether the player is currently in the air. 
	/// Do note: This will only detect ground as objects with layers specified in the layermask. 
	/// </summary>
	protected void InAir() {
		grounded = Physics2D.CapsuleCast(cCollider.bounds.center, new Vector2(cCollider.size.x * transform.localScale.x, cCollider.size.y * transform.localScale.y),
									   cCollider.direction, transform.rotation.z, Vector2.down, 0.1f, LayerMask.GetMask(new string[] { "Surface" }));
	}
}
