using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseWater : Interactable
{
	public float speed = 0.01f;
	public float height = 1;
	public float width = 1;

	public Transform t;
	public Vector3 movePosition;

	protected override string Tip
	{
		get { return "Press " + InteractInput.ToString() + " To Pull"; }
	}

	protected override void InteractAction(Rigidbody2D r)
	{
		PlayerController p = r.GetComponent<PlayerController>();
		p.StartCoroutine(Raise());
		p.StartCoroutine(RedirectCamera());

		p.GrabAndDrag(transform, movePosition);

		OnTriggerExit2D(null);
		Destroy(this);
	}

	private IEnumerator Raise()
	{
		look.gameObject.SetActive(true);
		while (Mathf.Abs(height) > 0)
		{
			float hspeed = height * speed;
			float yspeed = width * speed;
			t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y + hspeed, t.localPosition.z);
			t.localScale = new Vector3(t.localScale.x, t.localScale.y + yspeed, t.localScale.z);
			height -= hspeed;
			width -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}
}
