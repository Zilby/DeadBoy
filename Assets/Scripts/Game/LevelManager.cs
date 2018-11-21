using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// For managing individual levels in the game. 
/// </summary>
public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	public enum Levels
	{
		Sewer,
	}

	[StringInList(typeof(SongManager), "GetClipList")]
	public string song;

	public Levels level;

	[Range(0, 10)]
	public float timescale = 1;

	/// <summary>
	/// The number of players required to proceed to the next level. 
	/// </summary>
	public int requiredPlayers = 1;

	private int players;

	void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		Time.timeScale = timescale;
		Time.fixedDeltaTime = timescale * 0.02f;
		switch (level)
		{
			case Levels.Sewer:
				SongManager.instance.PlaySong(song);
				StartCoroutine(DBInputManager.instance.GeneralTutorial());
				break;
			default:
				break;
		}
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
		players += 1;
		if (players == requiredPlayers)
		{
			switch (level)
			{
				case Levels.Sewer:
					Fader.SceneEvent("DemoEnd");
					break;
				default:
					break;
			}
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		players -= 1;
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
