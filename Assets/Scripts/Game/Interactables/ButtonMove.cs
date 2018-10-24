using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : Interactable
{
	public float speed = 0.01f;
	public float xdist = 0;
	public float ydist = 0; // -4.4

	public Transform t;
	public Transform follow;

	protected override string Tip
	{
		get { return "Press " + InteractInput.ToString() + " To Press"; }
	}

	protected override void InteractAction(Rigidbody2D r)
	{
		PlayerController p = r.GetComponent<PlayerController>();
		p.StartCoroutine(MoveToLocation());
		p.StartCoroutine(RedirectCamera());
		OnTriggerExit2D(null);
		Destroy(this);
	}

	private IEnumerator MoveToLocation() {
		while(Mathf.Abs(xdist) > 0 || Mathf.Abs(ydist) > 0) {
			float xspeed = xdist * speed;
			float yspeed = ydist * speed;
			t.position = new Vector3(t.position.x + xspeed, t.position.y + yspeed, t.position.z);
			xdist -= xspeed;
			ydist -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator RedirectCamera() 
	{
		CameraController.movingToNewPosition = true;
		CameraController.followTransform = follow;
		yield return new WaitForSeconds(1.5f);
		CameraController.followTransform = null;
	}
}
