using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadboyController : PlayerController
{
	public static Action activate;

	[Header("Animator")]
	public bool getUp = false;


	public override Character CharID
	{
		get { return Character.Deadboy; }
	}

	public override string Name
	{
		get { return "Deadboy"; }
	}

	protected override void Start()
	{
		base.Start();
		if (getUp)
		{
			anim.SetTrigger("GetUp");
			activate = delegate { this.enabled = true; };
			this.enabled = false;
		}
		Interactable.TogglePhased?.Invoke(initialPlayer == 0);
	}

	public override void SwitchedFrom() {
		base.SwitchedFrom();
		Interactable.TogglePhased?.Invoke(false);
	}

	public override void SwitchedTo()
	{
		base.SwitchedTo();
		Interactable.TogglePhased?.Invoke(true);
	}
}
