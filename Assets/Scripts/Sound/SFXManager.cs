using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Manages in-game SFX
/// </summary>
[ExecuteInEditMode]
public class SFXManager : MonoBehaviour
{
	public static SFXManager instance;

	public AudioMixerGroup sfxGroup;
	[Serializable] public class ClipList : ListWrapper<AudioClip> { }
	[Serializable] public class ClipDict : SerializableDictionary<string, ClipList> { }
	public ClipDict clips;
	private Dictionary<string, AudioClip> lastPlayedClips;

	[InspectorButton("LoadClips")]
	public bool SetUpClips;

	/// <summary>
	/// Sets up the audio clip list
	/// </summary>
	private void LoadClips()
	{
		clips = new ClipDict();
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
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			if (instance == null)
			{
				instance = this;
			}
		}
	}


	void Awake()
	{
		if (Application.isPlaying)
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
				lastPlayedClips = new Dictionary<string, AudioClip>();
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}


	/// <summary>
	/// Plays the clip at the given index. 
	/// </summary>
	public void PlayClip(string clip, float volume = 1, float pitch = 1, ulong delay = 0, Vector3? location = null,
	                     float spread = 360, float doppler = 1, AudioRolloffMode rm = AudioRolloffMode.Linear, float maxD = 20, float minD = 1)
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
		StartCoroutine(PlayClip(a, volume, pitch, delay, location, spread, doppler, rm, maxD, minD));
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
		s.clip = a;
		s.volume = volume;
		s.pitch = pitch;
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
		s.Play(delay);
		while (s.isPlaying)
		{
			yield return null;
		}
		Destroy(g);
	}

	public static string[] GetSoundFXList()
	{
		return instance.clips.Keys.ToArray();
	}
}