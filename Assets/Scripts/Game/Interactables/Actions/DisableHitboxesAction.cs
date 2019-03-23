using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableHitboxesAction : InteractAction
{
	public List<Collider2D> colliders;
	public override void Reset()
	{
		base.Reset();
		colliders = GetComponents<Collider2D>().ToList();
	}

	public override IEnumerator Act(PlayerController p)
	{
		yield return base.Act(p);
		foreach (Collider2D c in colliders)
		{
			c.enabled = false;
		}
	}
}
