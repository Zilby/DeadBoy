using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of the camera. 
/// </summary>
public class CameraController : MonoBehaviour
{
	public static Action Deactivate;

	public Vector2 xRange = new Vector2(-100, 100);
	public Vector2 yRange = new Vector2(-100, 100);

	private const float CAMERA_MOVE_SPEED = 2;

	private const float MAX_X_DIFF = 5F;
	private const float MAX_Y_DIFF = 2F;

	private float oldDiffX;
	private float oldDiffY;

	public static bool movingToNewPosition;

	public static Transform followTransform; 

	private void Awake()
	{
		Deactivate = delegate {
			enabled = false;
		};
	}

	/// <summary>
	/// Gets the relative player position.
	/// </summary>
	private Vector3 RelativePlayerPosition
	{
		get
		{
			Transform t = DBInputManager.player1.transform;
			if (followTransform != null) 
			{
				t = followTransform;
			}
			float x = t.position.x;
			float y = t.position.y;
			return new Vector3(x, y, transform.position.z);
		}
	}

	/// <summary>
	/// Sets transform to the player if they are moving extremely fast
	/// </summary>
	private void Update()
	{
		if (Mathf.Abs(transform.position.x - RelativePlayerPosition.x) > MAX_X_DIFF && !movingToNewPosition && Mathf.Abs(oldDiffX) <= MAX_X_DIFF)
		{
			transform.position = new Vector3(RelativePlayerPosition.x - oldDiffX,
											  transform.position.y, transform.position.z);
		}
		else
		{
			oldDiffX = RelativePlayerPosition.x - transform.position.x;
		}
		if (Mathf.Abs(transform.position.y - RelativePlayerPosition.y) > MAX_Y_DIFF && !movingToNewPosition && Mathf.Abs(oldDiffY) <= MAX_Y_DIFF)
		{
			transform.position = new Vector3(transform.position.x, RelativePlayerPosition.y - oldDiffY,
											 transform.position.z);
		}
		else
		{
			oldDiffY = RelativePlayerPosition.y - transform.position.y;
		}
		LockTransform();
	}

	/// <summary>
	/// Updates camera to the player position on fixed update (since that is when the player moves)
	/// </summary>
	void FixedUpdate()
	{
		float moveSpeed = movingToNewPosition ? CAMERA_MOVE_SPEED * 2: CAMERA_MOVE_SPEED;
		if (Mathf.Abs(transform.position.x - RelativePlayerPosition.x) <= MAX_X_DIFF || movingToNewPosition || Mathf.Abs(oldDiffX) > MAX_X_DIFF)
		{
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, RelativePlayerPosition.x, moveSpeed * Time.fixedDeltaTime),
											 transform.position.y, transform.position.z);
		}
		if (Mathf.Abs(transform.position.y - RelativePlayerPosition.y) <= MAX_Y_DIFF || movingToNewPosition || Mathf.Abs(oldDiffY) > MAX_Y_DIFF)
		{

			transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, RelativePlayerPosition.y, moveSpeed * Time.fixedDeltaTime),
										 transform.position.z);
		}
		if ((Mathf.Abs(transform.position.x - RelativePlayerPosition.x) <= MAX_X_DIFF || (transform.position.x == xRange.x || transform.position.x == xRange.y)) &&
			(Mathf.Abs(transform.position.y - RelativePlayerPosition.y) <= MAX_Y_DIFF || (transform.position.y == yRange.x || transform.position.y == yRange.y)) &&
		    followTransform == null)
		{
			movingToNewPosition = false;
		}
	}

	/// <summary>
	/// Locks the transform to the range. 
	/// </summary>
	void LockTransform()
	{
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, xRange.x, xRange.y), Mathf.Clamp(transform.position.y, yRange.x, yRange.y), transform.position.z);
	}
}
