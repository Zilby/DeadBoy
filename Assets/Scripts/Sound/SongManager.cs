using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoopableClip
{
	public AudioClip clip;
	public float loopTime;
}

/// <summary>
/// Manages the in-game songs. 
/// </summary>
public class SongManager : AudioManager<SongManager, LoopableClip>
{
	[Serializable] public class ClipDict : SerializableDictionary<string, LoopableClip> { }
	public ClipDict clips;

	protected override Dictionary<string, LoopableClip> Clips
	{
		get { return clips; }
	}

	/// <summary>
	/// Sets up the audio clip list
	/// </summary>
	protected override void LoadClips()
	{
		ClipDict newClips = new ClipDict();
		newClips[NO_CLIP] = null;
		AudioClip[] loaded = Resources.LoadAll<AudioClip>("Audio/Music");
		foreach (AudioClip c in loaded)
		{
			LoopableClip l = new LoopableClip();
			l.clip = c;
			if (clips != null && clips.ContainsKey(c.name))
			{
				l.loopTime = clips[c.name].loopTime;
			}
			newClips[c.name] = l;
		}
		clips = newClips;
	}

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

	/// <summary>
	/// The AudioSetting.dspTime at the start of the current song / loop. 
	/// </summary>
	private double time;

	protected override void Initialize()
	{
		base.Initialize();
		s = GetComponents<AudioSource>();
	}

	/// <summary>
	/// Plays the clip at the given index. 
	/// </summary>
	public void PlaySong(string song)
	{
		LoopableClip l = clips[song];
		if (song != NO_CLIP && s[currentSource].clip != l.clip)
		{
			s[currentSource].Stop();
			s[currentSource].clip = l.clip;
			time = AudioSettings.dspTime;
			loopTime = l.loopTime;
			s[currentSource].loop = loopTime == 0;
			s[currentSource].Play();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!Application.isPlaying)
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
}
