using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inheritable class for grabable objects. 
/// </summary>
public abstract class Grabable : MonoBehaviour
{
	/// <summary>
	/// Gets the position above the grabable for the tooltip.
	/// </summary>
	protected Vector3 TipPos
	{
		get
		{
			Vector3 pos = transform.position;
			pos.y += 3;
			return pos;
		}
	}

	/// <summary>
	/// The tooltip for this grabable. 
	/// </summary>
	protected abstract string Tip { get; }

	/// <summary>
	/// The input needed to activate this grabable. 
	/// </summary>
	protected virtual KeyCode GrabInput
	{
		get { return KeyCode.J; }
	}

	/// <summary>
	/// The tooltip index for this grabable. 
	/// </summary>
	protected int tooltip = 0;

	/// <summary>
	/// Coroutine for checking input. 
	/// </summary>
	private Coroutine checkInput;


	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		tooltip = ToolTips.TooltipActiveEvent(Tip, TipPos);
		checkInput = StartCoroutine(CheckForInput());
	}

	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
		ToolTips.TooltipStayEvent(tooltip, TipPos);
	}

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		ToolTips.TooltipInactiveEvent(tooltip);
		StopCoroutine(checkInput);
	}

	/// <summary>
	/// Checks for player input. 
	/// </summary>
	private IEnumerator CheckForInput()
	{
		for (; ; )
		{
			yield return null;
			if (Input.GetKeyDown(GrabInput))
			{
				GrabAction();
			}
		}
	}

	/// <summary>
	/// The action to be taken when the grabable is grabbed. 
	/// </summary>
	protected abstract void GrabAction();
}
