using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu instance;

	public FadeableUI fadeable;

	public Button resume;
	public Button menu;
	public Button quit;


	void Awake()
	{
		instance = this;
		resume.onClick.AddListener(Pause);
		menu.onClick.AddListener(delegate { Fader.SceneEvent("DemoSplash"); });
		quit.onClick.AddListener(delegate { StartCoroutine(Fader.Quit()); } );
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
			Time.timeScale = oldTimeScale;
			fadeable.SelfFadeIn();
		}
		else
		{
			oldTimeScale = Time.timeScale;
			Time.timeScale = 0;
			fadeable.SelfFadeOut();
		}
	}
}
