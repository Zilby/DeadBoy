using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadboyController : PlayerController
{
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

	protected override void Awake()
	{
		base.Awake();
		if (getUp)
		{
			anim.SetTrigger("GetUp");
		}
	}
}
