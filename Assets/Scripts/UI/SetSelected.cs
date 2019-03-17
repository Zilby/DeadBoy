using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Sets the current selected gameobject to this. 
/// </summary>
public class SetSelected : MonoBehaviour
{
	public static Action Select;
	// Start is called before the first frame update
	void Start()
	{
		Select += SelectThis;
	}

	void OnEnable()
	{
		if (DBInputManager.instance != null && !DBInputManager.MainIsKeyboard)
		{
			SelectThis();
		}
	}

	private void OnDestroy()
	{
		Select -= SelectThis;
	}

	void SelectThis()
	{
		EventSystem.current.SetSelectedGameObject(gameObject);
		gameObject.GetComponent<Selectable>()?.OnSelect(null);
	}
}
