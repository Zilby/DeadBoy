using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of the camera. 
/// </summary>
public class CameraController : MonoBehaviour
{

	private const float CAMERA_MOVE_SPEED = 2;
	private float oldDiffX;
	private float oldDiffY;


	private Vector3 RelativePlayerPosition
	{
		get
		{
			float x = PlayerController.mainPlayer.transform.position.x;
			float y = PlayerController.mainPlayer.transform.position.y;
			return new Vector3(x, y, transform.position.z);
		}
	}

	private void Update()
	{
		if (Mathf.Abs(transform.position.x - RelativePlayerPosition.x) > 5f)
		{
			transform.position = new Vector3 (RelativePlayerPosition.x - oldDiffX, 
			                                  transform.position.y, transform.position.z);
		}
		else
		{
			oldDiffX = RelativePlayerPosition.x - transform.position.x;
		}
		if (Mathf.Abs(transform.position.y - RelativePlayerPosition.y) > 2f)
		{
			transform.position = new Vector3(transform.position.x, RelativePlayerPosition.y - oldDiffY,
											 transform.position.z);
		}
		else
		{
			oldDiffY = RelativePlayerPosition.y - transform.position.y;
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (Mathf.Abs(transform.position.x - RelativePlayerPosition.x) < 5)
		{
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, RelativePlayerPosition.x, CAMERA_MOVE_SPEED * Time.fixedDeltaTime),
											 transform.position.y, transform.position.z);
		}
		if (Mathf.Abs(transform.position.y - RelativePlayerPosition.y) < 2)
		{

			transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, RelativePlayerPosition.y, CAMERA_MOVE_SPEED * Time.fixedDeltaTime),
										 transform.position.z);
		}
	}
}
