using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : Interactable
{
	public Sprite sprite;
	public float speed = 0.01f;
	public float xdist = 0;
	public float ydist = 0;

	public Transform t;

	protected override string Tip
	{
		get { return "Press " + InteractInput[0].ToString() + " To Press"; }
	}

	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		p.StartCoroutine(MoveToLocation());
		p.StartCoroutine(RedirectCamera());

		p.Press(transform, delegate
		{
			GetComponent<SpriteRenderer>().sprite = sprite;
			Destroy(this);
		});
		EndInteraction();
	}

	private IEnumerator MoveToLocation()
	{
		while (Mathf.Abs(xdist) > 0 || Mathf.Abs(ydist) > 0)
		{
			float xspeed = xdist * speed;
			float yspeed = ydist * speed;
			t.position = new Vector3(t.position.x + xspeed, t.position.y + yspeed, t.position.z);
			xdist -= xspeed;
			ydist -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}
}
