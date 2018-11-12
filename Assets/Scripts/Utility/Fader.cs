using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for fading in and out transitions.
/// </summary>
public class Fader : MonoBehaviour
{
	public delegate void SelfFade(float f = 0.3f);
	/// <summary>
	/// Self fades in the fader.
	/// </summary>
	public static SelfFade SelfFadeIn;
	/// <summary>
	/// Self fades out the fader. 
	/// </summary>
	public static SelfFade SelfFadeOut;

	public delegate IEnumerator Fade(float f = 0.3f);
	/// <summary>
	/// Fades in the fader.
	/// </summary>
	public static Fade FadeIn;
	/// <summary>
	/// Fades out the fader.
	/// </summary>
	public static Fade FadeOut;

	public delegate void SceneLoad(string s, float wait = 0.5f, bool save = false);
	/// <summary>
	/// Fades in the given scene.
	/// </summary>
	public static SceneLoad SceneEvent;

	public static Func<IEnumerator> Quit;

	private static Fader instance;

	private FadeableUI fadeable;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			fadeable = GetComponent<FadeableUI>();
			FadeIn = delegate (float f)
			{
				fadeable.Hide();
				return fadeable.FadeIn(dur: f);
			};
			FadeOut = delegate (float f)
			{
				fadeable.Show();
				return fadeable.FadeOut(dur: f);
			};
			SelfFadeIn = delegate (float f)
			{
				fadeable.Hide();
				fadeable.SelfFadeIn(dur: f);
			};
			SelfFadeOut = delegate (float f)
			{
				fadeable.Show();
				fadeable.SelfFadeOut(dur: f);
			};
			SceneEvent = delegate (string s, float w, bool b)
			{
				if (b)
				{
					//FileData.Save();
				}
				gameObject.SetActive(true);
				StartCoroutine(FadeInScene(s, w));
			};
			Quit = QuitGame;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Fades in the given scene.
	/// </summary>
	private IEnumerator FadeInScene(string scene, float wait)
	{
		yield return fadeable.FadeIn();
		SceneManager.LoadScene(scene);
		yield return new WaitForSecondsRealtime(wait);
		yield return fadeable.FadeOut(dur: 0.5f);
	}

	private IEnumerator QuitGame() 
	{
		yield return fadeable.FadeIn();
		Utils.Quit();
	}

}
