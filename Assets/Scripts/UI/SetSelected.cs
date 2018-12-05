using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Sets the current selected gameobject to this. 
/// </summary>
public class SetSelected : MonoBehaviour
{
	public static Action Select;
    // Start is called before the first frame update
    void Awake()
    {
		Select += SelectThis;
    }

	void Start() 
	{
		if(!DBInputManager.MainIsKeyboard) 
		{
			SelectThis();
		}
	}

	private void OnDestroy()
	{
		Select -= SelectThis;
	}

	void SelectThis() {
		EventSystem.current.SetSelectedGameObject(gameObject);
	}
}
