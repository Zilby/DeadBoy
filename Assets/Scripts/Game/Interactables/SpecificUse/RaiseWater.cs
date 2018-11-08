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

	protected override void InteractAction(PlayerController p)
	{
		base.InteractAction(p);
		p.GrabAndDrag(transform, movePosition, delegate
		{
			p.StartCoroutine(Raise());
			p.StartCoroutine(Waterfall());
			p.StartCoroutine(RedirectCamera());
		});

		OnTriggerExit2D(null);
		Destroy(this);
	}

	private IEnumerator Raise()
	{
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

	private IEnumerator Waterfall()
	{
		yield return new WaitForSeconds(1.5f);
		look.gameObject.SetActive(true);
	}
}
