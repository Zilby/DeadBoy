using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	public enum Levels
	{
		Sewer,
	}

	public Levels level;

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
		if (players == PlayerControllerManager.players.Count)
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
}
