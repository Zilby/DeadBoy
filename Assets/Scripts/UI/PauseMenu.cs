using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu instance;

	public FadeableUI fadeable;

	public Button resume;
    public Button restart;
	public Button menu;
	public Button quit;


	void Awake()
	{
		instance = this;
		resume.onClick.AddListener(Pause);
		restart.onClick.AddListener(delegate { LevelManager.instance.RestartLevel(); });
		menu.onClick.AddListener(delegate { Fader.SceneEvent("DemoStart"); });
		quit.onClick.AddListener(delegate { StartCoroutine(Fader.Quit()); });
	}

	public bool Paused 
	{
		get { return paused; }
	}

	private bool paused;
	private float oldTimeScale;

	public void Pause()
	{
		paused = !paused;
		if (paused)
		{
			oldTimeScale = Time.timeScale;
			Time.timeScale = 0;
			fadeable.SelfFadeIn();
		}
		else
		{
			Time.timeScale = oldTimeScale;
			fadeable.SelfFadeOut();
		}
	}
}
