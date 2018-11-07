using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterDroplet : MonoBehaviour
{
	public ParticleSystem part;

	void Reset()
	{
		part = GetComponent<ParticleSystem>();
	}

	void OnParticleCollision(GameObject other)
	{
		// Play soundfx
	}
}
