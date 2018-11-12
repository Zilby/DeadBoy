using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundOnCollision : MonoBehaviour
{
	public ParticleSystem part;

	[StringInList(typeof(SFXManager), "GetSoundFXList")]
	public string clip;

	[Range(0.01f, 10f)]
	public float volume = 1;

	void Reset()
	{
		part = GetComponent<ParticleSystem>();
	}

	void OnParticleCollision(GameObject other)
	{
		List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>(); ;
		part.GetCollisionEvents(other, collisionEvents);
		for (int i = 0; i < collisionEvents.Count; ++i)
		{
			SFXManager.instance.PlayClip(clip, volume, location: collisionEvents[i].intersection);
		}
	}
}
