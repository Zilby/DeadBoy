using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inheritable class for interact actions. 
/// </summary>
public abstract class InteractAction : MonoBehaviour
{
	public List<Transform> lookAts;

	public void Reset()
	{
		GetComponent<Interactable>().actions.Add(this);
	}

	public virtual IEnumerator Act(PlayerController p)
	{
		p.StartCoroutine(CameraController.RedirectCamera(lookAts));
		yield return null;
	}
}
