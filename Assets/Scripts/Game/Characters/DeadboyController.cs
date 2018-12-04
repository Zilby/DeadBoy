using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadboyController : PlayerController
{
	public static Action activate;

	[Header("Animator")]
	public bool getUp = false;

	public override int SORT_VALUE
	{
		get { return 1; }
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
	}
}
