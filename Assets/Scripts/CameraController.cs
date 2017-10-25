using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of the camera. 
/// </summary>
public class CameraController : MonoBehaviour {

	private const float CAMERA_MOVE_SPEED = 2;


	// Update is called once per frame
	void FixedUpdate()
	{
		MoveWithPlayer();
	}


	/// <summary>
	/// Moves the camera with the player.
	/// </summary>
	private void MoveWithPlayer() {
		float px = PlayerController.mainPlayer.transform.position.x;
		float py = PlayerController.mainPlayer.transform.position.y;
		float moveSpeed = CAMERA_MOVE_SPEED * (Vector3.Distance(transform.position, new Vector3(px, py, transform.position.z)));
		transform.position = Vector3.Lerp(transform.position, new Vector3(px, py, transform.position.z), CAMERA_MOVE_SPEED * Time.fixedDeltaTime);
	}
}
