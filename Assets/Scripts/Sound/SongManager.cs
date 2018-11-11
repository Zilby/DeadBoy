using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the in-game songs. 
/// </summary>
public class SongManager : MonoBehaviour
{
	public enum Songs
	{
		Sewers = 0,
	}

	public static SongManager instance;

	[Serializable]
	public struct LoopableClip
	{
		public AudioClip clip;
		public float loopTime;
	}

	public List<LoopableClip> clips;

	/// <summary>
	/// List of audio sources. 
	/// </summary>
	private AudioSource[] s;

	/// <summary>
	/// The current audio source index.
	/// </summary>
	private int currentSource = 0;

	/// <summary>
	/// The looptime for the current source. 
	/// </summary>
	private float loopTime = 0;

	private double time;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			s = GetComponents<AudioSource>();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Plays the clip at the given index. 
	/// </summary>
	public void PlaySong(Songs song)
	{
		LoopableClip l = clips[(int)song];
		if (s[currentSource].clip != l.clip)
		{
			s[currentSource].Stop();
			s[currentSource].clip = l.clip;
			time = AudioSettings.dspTime;
			loopTime = l.loopTime;
			s[currentSource].loop = loopTime == 0;
			s[currentSource].Play();
		}
	}

	void Update()
	{
		int nextSource = 1 - currentSource;
		if (loopTime != 0)
		{
			if (!s[nextSource].isPlaying)
			{
				time = time + loopTime;
				s[nextSource].clip = s[currentSource].clip;
				s[nextSource].loop = false;
				s[nextSource].PlayScheduled(time);
				currentSource = nextSource;
			}
		}
	}
}
