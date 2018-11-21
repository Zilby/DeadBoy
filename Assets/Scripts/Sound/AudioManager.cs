using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generic inherited class for managing in-game audio. 
/// </summary>
[ExecuteInEditMode]
public abstract class AudioManager<T, D> : MonoBehaviour
	where T : AudioManager<T, D>
{
	/// <summary>
	/// The instance of this audio manager
	/// </summary>
	public static T instance;

	[InspectorButton("LoadClips")]
	public bool SetUpClips;

	/// <summary>
	/// The dictionary of clips. 
	/// </summary>
	protected abstract Dictionary<string, D> Clips { get; }

	/// <summary>
	/// Default clip name if there's none selected. 
	/// </summary>
	protected string NO_CLIP
	{
		get { return "None"; }
	}

	/// <summary>
	/// Gets the list of clip names.
	/// </summary>
	public static string[] GetClipList()
	{
		return instance.Clips.Keys.ToArray();
	}


	/// <summary>
	/// Sets up the audio clip list
	/// </summary>
	protected abstract void LoadClips();


	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			if (instance == null)
			{
				Initialize();
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}


	/// <summary>
	/// Initializes the audiomanager instance.
	/// </summary>
	protected virtual void Initialize() 
	{
		instance = this as T;
		DontDestroyOnLoad(gameObject);
	}


	protected virtual void Update()
	{
		if (!Application.isPlaying)
		{
			if (instance == null)
			{
				instance = this as T;
			}
		}
	}
}
