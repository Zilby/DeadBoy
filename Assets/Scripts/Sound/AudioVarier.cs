using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using System;

/// <summary>
/// This class varies the pitch for sounds so that
/// they don't all overlap and drastically increase the amplitude.
/// </summary>
public class AudioVarier : MonoBehaviour
{
	/// <summary>
	/// Audio source to be varied. 
	/// </summary>
	private AudioSource a;

	void Start()
	{
		a = GetComponent<AudioSource>();
		a.pitch += -0.1f + (UnityEngine.Random.Range(0, 20000000) / 100000000.0f);
		string clipname = a.clip.name;
		if (Array.IndexOf(SFXManager.GetClipList(), clipname) >= 0)
		{
			a.volume = SFXManager.instance.clips[clipname].volume;
		}
	}
}
