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
		get { return KeyCode.J; }
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
		checkInput = StartCoroutine(CheckForInput(collision.attachedRigidbody));
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
	private IEnumerator CheckForInput(Rigidbody2D r)
	{
		for (; ; )
		{
			yield return null;
			if (Input.GetKeyDown(InteractInput))
			{
				InteractAction(r);
			}
		}
	}

	/// <summary>
	/// The action to be taken when the interactable is grabbed. 
	/// </summary>
	protected abstract void InteractAction(Rigidbody2D r);
}
