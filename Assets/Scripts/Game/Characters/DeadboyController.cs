using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadboyController : PlayerController
{
	protected override int SORT_VALUE
	{
		get { return 1; }
	}
}
