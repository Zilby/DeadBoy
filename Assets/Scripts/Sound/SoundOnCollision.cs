using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundOnCollision : MonoBehaviour
{
	public ParticleSystem part;

	[StringInList(typeof(SFXManager), "GetClipList")]
	public string clip;

	public bool useCustomVolume = false;

	[ConditionalHide("useCustomVolume", min:0f, max:1f)]
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
			SFXManager.instance.PlayClip(clip, useCustomVolume ? (float?)volume : null, location: collisionEvents[i].intersection);
		}
	}
}
