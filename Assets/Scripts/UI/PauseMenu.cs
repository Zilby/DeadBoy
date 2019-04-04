using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu instance;

	public Fadeable fadeable;

	public Button resume;
    public Button restart;
	public Button menu;
	public Button quit;
	public Button settings;
	public Button returnB;

	public Fadeable mainMenu;
	public Fadeable settingsMenu;


	void Awake()
	{
		instance = this;
		resume.onClick.AddListener(Pause);
		restart.onClick.AddListener(delegate { LevelManager.instance.RestartLevel(); });
		menu.onClick.AddListener(delegate { Fader.SceneEvent("DemoStart"); });
		quit.onClick.AddListener(delegate { StartCoroutine(Fader.Quit()); });
		settings.onClick.AddListener(delegate { StartCoroutine(SwapMenues()); });
		returnB.onClick.AddListener(delegate { StartCoroutine(SwapBack()); });
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

	private IEnumerator SwapMenues() {
		yield return mainMenu.FadeOut();
		yield return settingsMenu.FadeIn();
	}

	private IEnumerator SwapBack()
	{
		yield return settingsMenu.FadeOut();
		yield return mainMenu.FadeIn();
	}
}
