using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Manages in-game SFX
/// </summary>
public class SFXManager : MonoBehaviour
{
	public static SFXManager instance;
	public enum Sounds
	{
		//None = -1,
		DBFootsteps = 0,
		DGFootsteps = 1,
		WaterDrops = 2,
	}
	public AudioMixerGroup sfxGroup;
	[Serializable] public class ClipList : ListWrapper<AudioClip> { }
	[Serializable] public class ClipDict : SerializableDictionary<Sounds, ClipList> { }
	public ClipDict clips;
	// public Dictionary<Sounds, List<AudioClip>> clips;
	// Make a custom property attribute
	[InspectorButton("SetUpClips")]
	public bool SetUpClipKeys;
	private AudioClip[] lastPlayedClips;
	private void SetUpClips()
	{
		foreach (Sounds s in Enum.GetValues(typeof(Sounds)))
		{
			if (!clips.ContainsKey(s))
			{
				clips[s] = null;
			}
		}
	}
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			lastPlayedClips = new AudioClip[Enum.GetValues(typeof(Sounds)).Length];
		}
		else
		{
			Destroy(gameObject);
		}
	}
	/// <summary>
	/// Plays the clip at the given index. 
	/// </summary>
	public void PlayClip(Sounds sound, Vector3 location, float volume = 1, float pitch = 1)
	{
		ClipList clipList = clips[sound];
		// Get random clip if list is greater than 1.
		AudioClip a = clipList[UnityEngine.Random.Range(0, clipList.Count - 1)];
		if (clipList.Count > 1)
		{
			while (a == lastPlayedClips[(int)sound])
			{
				a = clipList[UnityEngine.Random.Range(0, clipList.Count - 1)];
			}
		}
		lastPlayedClips[(int)sound] = a;
		StartCoroutine(PlayClip(a, location, volume, pitch));
	}
	/// <summary>
	/// Plays the given audio clip.
	/// </summary>
	public IEnumerator PlayClip(AudioClip a, Vector3 location, float volume = 1, float pitch = 1)
	{
		GameObject g = new GameObject(a.ToString(), typeof(AudioSource));
		yield return null;
	}
}