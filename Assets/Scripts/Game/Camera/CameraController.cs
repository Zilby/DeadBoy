using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of the camera. 
/// </summary>
public class CameraController : MonoBehaviour
{
	public static Action DeactivateEvent;
	public static Action ActivateEvent;
	public static Action<PlayerController> NewPositionEvent;

	public Vector2 xRange = new Vector2(-100, 100);
	public Vector2 yRange = new Vector2(-100, 100);

	private const float CAMERA_MOVE_SPEED = 1.5f;

	private const float MAX_X_DIFF = 5F;
	private const float MAX_Y_DIFF = 2F;

	private float oldDiffX;
	private float oldDiffY;

	private bool movingToNewPosition;

	public bool MovingToNewPosition
	{
		get
		{
			return movingToNewPosition;
		}
		set
		{
			movingToNewPosition = value;
			newMoveSpeed = 2f;
		}
	}

	private static float newMoveSpeed = 2f;

	private static bool p2Added = false;

	public static Transform followTransform;

	protected virtual void Awake()
	{
		if (!p2Added)
		{
			AddPlayer2Cam();
		}
		Initialize();
	}

	protected void Initialize()
	{
		DeactivateEvent += Deactivate;
		ActivateEvent += Activate;
		NewPositionEvent += NewPosition;
	}

	private void Deactivate() {
		enabled = false;
	}

	private void Activate()
	{
		enabled = true;
	}

	private void NewPosition(PlayerController p) {
		if (p == null || p.transform == PlayerTransform)
		{
			MovingToNewPosition = true;
		}
	}

	/// <summary>
	/// Gets the player transform for this camera controller. 
	/// </summary>
	protected virtual Transform PlayerTransform
	{
		get { return DBInputManager.MainPlayer.transform; }
	}

	/// <summary>
	/// Gets the relative player position.
	/// </summary>
	private Vector3 RelativePlayerPosition
	{
		get
		{
			Transform t = PlayerTransform;
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
	protected virtual void Update()
	{
		ClampCamera();
	}

	protected virtual void FixedUpdate()
	{
		MoveToPlayer(Time.fixedDeltaTime);
	}

	/// <summary>
	/// Updates camera to the player position on fixed update (since that is when the player moves)
	/// </summary>
	void MoveToPlayer(float t)
	{
		float moveSpeed = movingToNewPosition ? CAMERA_MOVE_SPEED * newMoveSpeed : CAMERA_MOVE_SPEED;
		if (Mathf.Abs(transform.position.x - RelativePlayerPosition.x) <= MAX_X_DIFF || movingToNewPosition || Mathf.Abs(oldDiffX) > MAX_X_DIFF)
		{
			transform.position = transform.position.X(Mathf.Lerp(transform.position.x, RelativePlayerPosition.x, moveSpeed * t));
		}
		if (Mathf.Abs(transform.position.y - RelativePlayerPosition.y) <= MAX_Y_DIFF || movingToNewPosition || Mathf.Abs(oldDiffY) > MAX_Y_DIFF)
		{

			transform.position = transform.position.Y(Mathf.Lerp(transform.position.y, RelativePlayerPosition.y, moveSpeed * t));
		}
		if ((Mathf.Abs(transform.position.x - RelativePlayerPosition.x) <= MAX_X_DIFF || (transform.position.x == xRange.x || transform.position.x == xRange.y)) &&
			(Mathf.Abs(transform.position.y - RelativePlayerPosition.y) <= MAX_Y_DIFF || (transform.position.y == yRange.x || transform.position.y == yRange.y)) &&
			followTransform == null)
		{
			movingToNewPosition = false;
		}
	}

	void ClampCamera()
	{
		if (Mathf.Abs(transform.position.x - RelativePlayerPosition.x) > MAX_X_DIFF && !movingToNewPosition && Mathf.Abs(oldDiffX) <= MAX_X_DIFF)
		{
			transform.position = transform.position.X(RelativePlayerPosition.x - oldDiffX);
		}
		else
		{
			oldDiffX = RelativePlayerPosition.x - transform.position.x;
		}
		if (Mathf.Abs(transform.position.y - RelativePlayerPosition.y) > MAX_Y_DIFF && !movingToNewPosition && Mathf.Abs(oldDiffY) <= MAX_Y_DIFF)
		{
			transform.position = transform.position.Y(RelativePlayerPosition.y - oldDiffY);
		}
		else
		{
			oldDiffY = RelativePlayerPosition.y - transform.position.y;
		}
		LockTransform();
	}

	/// <summary>
	/// Locks the transform to the range. 
	/// </summary>
	void LockTransform()
	{
		transform.position = transform.position.XY(Mathf.Clamp(transform.position.x, xRange.x, xRange.y),
												   Mathf.Clamp(transform.position.y, yRange.x, yRange.y));
	}

	/// <summary>
	/// Redirects the camera to the look transform.
	/// </summary>
	/// <returns>The camera.</returns>
	public static IEnumerator RedirectCamera(List<Transform> looks, float delay = 0.8f, float duration = 2.3f, float speed = 2f)
	{
		DBInputManager.instance.restrictInput = true;
		if (looks != null)
		{
			newMoveSpeed = speed;
			yield return new WaitForSeconds(delay);
			foreach (Transform t in looks)
			{
				NewPositionEvent(null);
				followTransform = t;
				yield return new WaitForSeconds(duration);
				followTransform = null;
			}
		}
		DBInputManager.instance.restrictInput = false;
	}

	/// <summary>
	/// Adds the player2 cam.
	/// </summary>
	protected void AddPlayer2Cam()
	{
		p2Added = true;
		GameObject g = Instantiate(gameObject);
		Destroy(g.GetComponent<CameraController>());
		Destroy(g.GetComponent<AudioListener>());
		g.tag = "Untagged";

		Player2Camera c = g.AddComponent<Player2Camera>();
		// Not sure why this number works out, but it does. 
		float extraDist = transform.position.z / -2f;
		c.xRange = new Vector2(xRange.x - extraDist, xRange.y + extraDist);
		c.oldXrange = xRange;
		c.yRange = yRange;
		c.player1 = this;
	}

	private void OnDestroy()
	{
		DeactivateEvent -= Deactivate;
		ActivateEvent -= Activate;
		NewPositionEvent -= NewPosition;
		p2Added = false;
	}
}
