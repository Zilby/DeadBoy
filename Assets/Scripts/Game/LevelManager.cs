using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	public enum Levels {
		Sewer,
	}

	public Levels level;


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

}
