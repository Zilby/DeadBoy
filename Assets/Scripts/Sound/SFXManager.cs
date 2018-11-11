using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Manages in-game SFX
/// </summary>
public class SFXManager : MonoBehaviour
{
	public static SFXManager instance;

	public AudioMixerGroup sfxGroup;
	[Serializable] public class ClipList : ListWrapper<AudioClip> { }
	[Serializable] public class ClipDict : SerializableDictionary<string, ClipList> { }
	public ClipDict clips;
	private Dictionary<string, AudioClip> lastPlayedClips;

	[Space(10)]
	[InspectorButton("SetUpClips")]
	public bool SetUpClipKeys;

	/// <summary>
	/// Sets up the audio clip list
	/// </summary>
	private void SetUpClips()
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


	void Awake()
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


	/// <summary>
	/// Plays the clip at the given index. 
	/// </summary>
	public void PlayClip(string clip, Vector3 location, float volume = 1, float pitch = 1)
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