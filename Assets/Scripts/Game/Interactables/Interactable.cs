using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inheritable class for interactable objects. 
/// </summary>
public abstract class Interactable : MonoBehaviour
{
	public delegate void PhaseEvent(bool b, float f = 0);
	public static PhaseEvent TogglePhased;

	[Header("Interactable Fields")]
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
	/// Whether or not this interactable is phasing. 
	/// </summary>
	public List<FadeableSprite> phased;

	/// <summary>
	/// Whether or not to move the player left or right when interacting. 
	/// </summary>
	public bool movePlayer;

	/// <summary>
	/// The position to the left or right that the player is moved to. 
	/// </summary>
	[ConditionalHide("movePlayer", true, false, 0, 10)]
	public float playerMovePosition = 1;

	[StringInList(typeof(SFXManager), "GetClipList")]
	public string clip;

	[System.NonSerialized]
	public bool moving = false;

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
	/// The input needed to activate this interactable. 
	/// </summary>
	protected virtual PlayerInput InteractInput
	{
		get { return PlayerInput.Interact; }
	}

	/// <summary>
	/// Speed at which the player is repositioned. 
	/// </summary>
	protected virtual float REPOSITION_SPEED
	{
		get { return 2f; }
	}

	/// <summary>
	/// The tooltip index for this interactable. 
	/// </summary>
	protected int tooltip = -1;

	/// <summary>
	/// Coroutine for checking input. 
	/// </summary>
	private Coroutine checkInput;

	/// <summary>
	/// The tooltip for this interactable. 
	/// </summary>
	protected abstract string Tip(PlayerController p);


	protected virtual void Awake()
	{
		if (phased.Count > 0)
		{
			TogglePhased += DeactivatePhased;
		}
	}

	protected virtual void OnDestroy()
	{
		if (phased.Count > 0)
		{
			TogglePhased -= DeactivatePhased;
		}
	}

	protected void DeactivatePhased(bool b, float delay = 0)
	{
		foreach(FadeableSprite s in phased) {
			if (b) {
				s.SelfDelayedFadeIn(delay);
			} else {
				s.SelfDelayedFadeOut(delay);
			}
		}
	}

	protected void SelfDestruct() {
		DeactivatePhased(false, 1.5f);
		Destroy(this);
	}

	/// <summary>
	/// Checks if the given player is valid. 
	/// </summary>
	protected bool PlayerCheck(PlayerController p)
	{
		return p != null && (!(phased.Count > 0) || p is DeadboyController);
	}


	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (PlayerCheck(p))
		{
			if (Tip(p) != null)
			{
				tooltip = ToolTips.instance.SetTooltipActive(Tip(p), TipPos);
			}
			checkInput = StartCoroutine(CheckForInput(p));
		}
	}

	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (PlayerCheck(p))
		{
			if (tooltip >= 0)
			{
				ToolTips.instance.SetTooltipPosition(tooltip, TipPos);
			}
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		PlayerController p = collision.attachedRigidbody.GetComponent<PlayerController>();
		if (PlayerCheck(p))
		{
			EndInteraction();
		}
	}

	protected virtual void EndInteraction()
	{
		if (tooltip >= 0)
		{
			ToolTips.instance.SetTooltipInactive(tooltip);
		}
		StopCoroutine(checkInput);
	}

	/// <summary>
	/// Checks for player input. 
	/// </summary>
	protected virtual IEnumerator CheckForInput(PlayerController p)
	{
		for (; ; )
		{
			yield return null;
			if (DBInputManager.GetInput(p, InteractInput, InputType.Pressed))
			{
				InteractAction(p);
			}
		}
	}

	/// <summary>
	/// The action to be taken when the interactable is interacted with. 
	/// </summary>
	protected virtual void InteractAction(PlayerController p)
	{
		p.StartCoroutine(RepositionPlayer(p));
		SFXManager.instance.PlayClip(clip, delay: (ulong)0.2, location: transform.position);
	}

	private IEnumerator RepositionPlayer(PlayerController p)
	{
		moving = true;
		float flip;
		float mPos = transform.position.x;
		if (transform.position.x > p.transform.position.x)
		{
			flip = 180 + (p.invertDirection ? 180 : 0);
			mPos -= playerMovePosition;
		}
		else
		{
			flip = 0 + (p.invertDirection ? 180 : 0);
			mPos += playerMovePosition;
		}
		p.transform.localEulerAngles = new Vector3(p.transform.localEulerAngles.x, flip, p.transform.localEulerAngles.z);
		float t = 0.0f;
		while (Mathf.Abs(p.transform.position.x - mPos) > 0.01f && movePlayer)
		{
			p.transform.position = new Vector3(Mathf.Lerp(p.transform.position.x, mPos, t), p.transform.position.y, p.transform.position.z);
			t += REPOSITION_SPEED * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		moving = false;
		yield return null;
	}
}
