using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// For managing individual levels in the game. 
/// </summary>
public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	public GameObject pitGlow;

	[StringInList(typeof(SongManager), "GetClipList")]
	public string song;
#if UNITY_EDITOR
	[StringInList(typeof(LevelManager), "GetLoadedLevels")]
#endif
	public string nextLevel;

	[Range(0, 10)]
	public float timescale = 1;

	/// <summary>
	/// The number of players required to proceed to the next level. 
	/// </summary>
	public int requiredPlayers = 1;

	private int players;

	[SerializeField]
	private bool canFinish;

#if UNITY_EDITOR
	public static string[] GetLoadedLevels()
	{
		string[] temp = new string[SceneManager.sceneCountInBuildSettings];
		for (int i = 0; i < temp.Length; ++i)
		{
			EditorBuildSettingsScene s = EditorBuildSettings.scenes[i];
			if (s.enabled)
			{
				string name = s.path.Substring(s.path.LastIndexOf('/') + 1);
				name = name.Substring(0, name.Length - 6);
				temp[i] = name;
			}
		}
		return temp;
	}
#endif

	void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		Time.timeScale = timescale;
		Time.fixedDeltaTime = timescale * 0.02f;
		SongManager.instance.PlaySong(song);
	}

	/// <summary>
	/// Whether or not the player can now finish the level. 
	/// </summary>
	public void CanNowFinish()
	{
		canFinish = true;
		pitGlow?.SetActive(true);
	}

	/// <summary>
	/// Restarts the level.
	/// </summary>
	public void RestartLevel()
	{
		Fader.SceneEvent(SceneManager.GetActiveScene().name);
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		PlayerController p = other.GetComponent<PlayerController>();
		if (p)
		{
			if (canFinish)
			{
				players += 1;
				if (players == requiredPlayers)
				{
					Fader.SceneEvent(nextLevel);
				} 
				else 
				{
					if (DBInputManager.IsControlled(p))
					{
						DBInputManager.CyclePlayers(p);
					}
				}
			}
			else
			{
				p.StartCoroutine(p.Die());
			}
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (canFinish)
		{
			players -= 1;
		}
	}

	public void Update()
	{
		// For testing in slowmo in-editor.
#if UNITY_EDITOR
		if (!PauseMenu.instance.Paused)
		{
			Time.timeScale = timescale;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
		}
#endif
	}
}
