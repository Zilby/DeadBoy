using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inheritable class for interactable objects. 
/// </summary>
public abstract class Interactable : MonoBehaviour
{
	/// <summary>
	/// The tip position.
	/// </summary>
	[Range(-10, 10)]
	public float tipPos = 3;
	/// <summary>
	/// Transform to look at after interacting.
	/// </summary>
	public Transform look;

	/// <summary>
	/// Gets the position above the interactable for the tooltip.
	/// </summary>
	protected Vector3 TipPos
	{
		get
		{
			Vector3 pos = transform.position;
			pos.y += tipPos;
			return pos;
		}
	}

	/// <summary>
	/// The tooltip for this interactable. 
	/// </summary>
	protected abstract string Tip { get; }

	/// <summary>
	/// The input needed to activate this interactable. 
	/// </summary>
	protected virtual KeyCode InteractInput
	{
		get { return KeyCode.F; }
	}

	/// <summary>
	/// The tooltip index for this interactable. 
	/// </summary>
	protected int tooltip = 0;

	/// <summary>
	/// Coroutine for checking input. 
	/// </summary>
	private Coroutine checkInput;


	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		tooltip = ToolTips.instance.SetTooltipActive(Tip, TipPos);
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (p != null)
		{
			checkInput = StartCoroutine(CheckForInput(p));
		}
	}

	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
		ToolTips.instance.SetTooltipPosition(tooltip, TipPos);
	}

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		ToolTips.instance.SetTooltipInactive(tooltip);
		StopCoroutine(checkInput);
	}

	/// <summary>
	/// Checks for player input. 
	/// </summary>
	private IEnumerator CheckForInput(PlayerController p)
	{
		for (; ; )
		{
			yield return null;
			if (Input.GetKeyDown(InteractInput))
			{
				InteractAction(p);
			}
		}
	}

	/// <summary>
	/// The action to be taken when the interactable is interacted with. 
	/// </summary>
	protected virtual void InteractAction(PlayerController p) {
		float flip;
		if (transform.position.x > p.transform.position.x)
		{
			flip = 180 + (p.invertDirection ? 180 : 0);
		}
		else
		{
			flip = 0 + (p.invertDirection ? 180 : 0);
		}
		p.transform.localEulerAngles = new Vector3(p.transform.localEulerAngles.x, flip, p.transform.localEulerAngles.z);
	}

	/// <summary>
	/// Redirects the camera to the look transform.
	/// </summary>
	/// <returns>The camera.</returns>
	protected virtual IEnumerator RedirectCamera()
	{
		yield return new WaitForSeconds(0.8f);
		CameraController.movingToNewPosition = true;
		CameraController.followTransform = look;
		yield return new WaitForSeconds(2f);
		CameraController.followTransform = null;
	}
}
