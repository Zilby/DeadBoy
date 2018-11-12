using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadboyController : PlayerController
{
	public override int SORT_VALUE
	{
		get { return 1; }
	}


	public override string Name { 
		get { return "Deadboy"; } 
	}
}
