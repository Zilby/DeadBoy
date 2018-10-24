using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;


    void Awake()
    {
		instance = this;
    }

	/// <summary>
	/// Restarts the level.
	/// </summary>
	public void RestartLevel()
    {
		Fader.SceneEvent(SceneManager.GetActiveScene().name);
    }

}
