using System;
using System.Collections;
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
			StartCoroutine(SwapClip(l));
		}
	}

	/// <summary>
	/// Swaps the current audio clip for a new one.
	/// </summary>
	protected IEnumerator SwapClip(LoopableClip l)
	{
		bool fade = s[currentSource].isPlaying || s[1 - currentSource].isPlaying;
		int nextSource = 1 - currentSource;
		if (fade)
		{
			StartCoroutine(Utils.FadeOut(s[nextSource], 0.6f));
			yield return Utils.FadeOut(s[currentSource], 0.6f);
		}
		s[currentSource].clip = l.clip;
		time = AudioSettings.dspTime;
		loopTime = l.loopTime;
		s[currentSource].loop = loopTime == 0;
		if (fade)
		{
			yield return Utils.FadeIn(s[currentSource], 0.3f);
		}
		else
		{
			s[currentSource].Play();
		}
	}

	protected override void Update()
	{
		base.Update();
		int nextSource = 1 - currentSource;
		if (loopTime != 0)
		{
			if (!s[nextSource].isPlaying && AudioSettings.dspTime > time)
			{
				time = time + loopTime;
				s[nextSource].clip = s[currentSource].clip;
				s[nextSource].loop = false;
				s[nextSource].PlayScheduled(time);
				currentSource = nextSource;
				//print("Loop time: " + time);
			}
			if (!s[currentSource].isPlaying)
			{
				s[currentSource].PlayScheduled(time);
				//print("Not playing????");
			}
		}
		//print((int)AudioSettings.dspTime);
	}
}
