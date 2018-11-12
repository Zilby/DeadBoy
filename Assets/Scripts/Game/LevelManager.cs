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

	public Levels level;

	public float timescale = 1;

	private int players;

	void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		switch (level)
		{
			case Levels.Sewer:
				SongManager.instance.PlaySong(SongManager.Songs.Sewers);
				break;
			default:
				break;
		}

#if UNITY_EDITOR
		Time.timeScale = timescale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
#endif
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
		if (players == InputManager.players.Count)
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
	}
}
