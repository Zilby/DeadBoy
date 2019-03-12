using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class ClipList : ListWrapper<AudioClip>
{
	[Range(0f, 1f)]
	public float volume = 0.5f;
}

/// <summary>
/// Manages in-game SFX
/// </summary>
public class SFXManager : AudioManager<SFXManager, ClipList>
{
	public enum FootstepType { Hard, Soft };

	[Serializable] public class ClipDict : SerializableDictionary<string, ClipList> { }
	public ClipDict clips;

	protected override Dictionary<string, ClipList> Clips
	{
		get { return clips; }
	}

	public AudioMixerGroup sfxGroup;

	private Dictionary<string, AudioClip> lastPlayedClips;

	/// <summary>
	/// Sets up the audio clip list
	/// </summary>
	protected override void LoadClips()
	{
		ClipDict oldClips = clips;
		clips = new ClipDict();
		clips[NO_CLIP] = null;
		AudioClip[] loaded = Resources.LoadAll<AudioClip>("Audio/SFX/Loose");
		foreach (AudioClip c in loaded)
		{
			ClipList l = new ClipList();
			l.list.Add(c);
			clips[c.name] = l;
		}
		string path = "/Resources/Audio/SFX/Compiled";
		DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + path);
		DirectoryInfo[] dInfo = levelDirectoryPath.GetDirectories("*", SearchOption.TopDirectoryOnly);
		foreach (DirectoryInfo d in dInfo)
		{
			loaded = Resources.LoadAll<AudioClip>("Audio/SFX/Compiled/" + d.Name);
			ClipList l = new ClipList();
			foreach (AudioClip c in loaded)
			{
				l.list.Add(c);
			}
			clips[d.Name] = l;
		}
		foreach (string s in clips.Keys)
		{
			if (oldClips.ContainsKey(s) && clips[s] != null)
			{
				clips[s].volume = oldClips[s].volume;
			}
		}
	}

	protected override void Initialize()
	{
		base.Initialize();
		lastPlayedClips = new Dictionary<string, AudioClip>();
	}


	/// <summary>
	/// Plays the clip at the given index. 
	/// </summary>
	public void PlayClip(string clip, float? volume = null, float pitch = 1, ulong delay = 0, Vector3? location = null,
						 float spread = 360, float doppler = 1, AudioRolloffMode rm = AudioRolloffMode.Linear, float maxD = 20, float minD = 1)
	{
		if (clip != NO_CLIP && clip != null)
		{
			ClipList clipList = clips[clip];
			// Get random clip if list is greater than 1.
			AudioClip a = clipList[UnityEngine.Random.Range(0, clipList.Count - 1)];
			if (clipList.Count > 1)
			{
				if (lastPlayedClips.ContainsKey(clip))
				{
					while (a == lastPlayedClips[clip])
					{
						a = clipList[UnityEngine.Random.Range(0, clipList.Count - 1)];
					}
				}
			}
			lastPlayedClips[clip] = a;
			StartCoroutine(PlayClip(a, volume ?? clipList.volume, pitch, delay, location, spread, doppler, rm, maxD, minD));
		}
	}


	/// <summary>
	/// Plays the given audio clip.
	/// </summary>
	public IEnumerator PlayClip(AudioClip a, float volume = 1, float pitch = 1, ulong delay = 0, Vector3? location = null,
								float spread = 360, float doppler = 1, AudioRolloffMode rm = AudioRolloffMode.Linear, float maxD = 20, float minD = 1)
	{
		GameObject g = new GameObject(a.ToString(), typeof(AudioSource));
		g.transform.parent = transform;
		AudioSource s = g.GetComponent<AudioSource>();
		s.playOnAwake = false;
		s.clip = a;
		// Set 2D sound settings
		s.volume = volume;
		s.pitch = pitch;
		// Set 3D sound settings
		if (location != null)
		{
			g.transform.position = (Vector3)location;
			s.spatialBlend = 1;
			s.spread = spread;
			s.dopplerLevel = doppler;
			s.rolloffMode = rm;
			s.maxDistance = maxD;
			s.minDistance = minD;
		}
		s.loop = false;
		// Have to add after initializing audio source. 
		g.AddComponent<AudioVarier>();
		s.PlayDelayed(delay);
		while (s.isPlaying)
		{
			yield return null;
		}
		Destroy(g);
	}
}