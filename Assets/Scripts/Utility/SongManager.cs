using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the in-game songs. 
/// </summary>
public class SongManager : MonoBehaviour
{
	public List<AudioClip> clips;

	public static Action<int> PlayClip;

	private static SongManager instance;

	private AudioSource s;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			s = GetComponent<AudioSource>();
			PlayClip = delegate (int i)
			{
				s.Stop();
				s.clip = clips[i];
				s.Play();
			};
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
