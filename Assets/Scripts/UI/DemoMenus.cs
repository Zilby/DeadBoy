using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoMenus : MonoBehaviour
{
	[StringInList(typeof(SongManager), "GetClipList")]
	public string song;
	[StringInList(typeof(LevelManager), "GetLoadedLevels")]
	public string firstLevel;
	[StringInList(typeof(LevelManager), "GetLoadedLevels")]
	public string menuScene;


	public Button play;
	public Button menu;
	public Button quit;
	public Button levelSelect;
	public Button settings;
	public List<Button> mainMenu;

	public Fadeable mainf;
	public Fadeable settingsf;
	public Fadeable levelSelectf;

	private void Start()
	{
		if (SongManager.instance != null)
		{
			SongManager.instance.PlaySong(song);
		}

		play?.onClick.AddListener(delegate { Fader.SceneEvent(firstLevel); });
		menu?.onClick.AddListener(delegate { Fader.SceneEvent(menuScene); });
		quit?.onClick.AddListener(delegate { StartCoroutine(Fader.Quit()); });
		settings?.onClick.AddListener(delegate { mainf.SelfFadeOut(); settingsf.SelfFadeIn(); });
		levelSelect?.onClick.AddListener(delegate { mainf.SelfFadeOut(); levelSelectf.SelfFadeIn(); });

		foreach (Button b in mainMenu)
		{
			b.onClick.AddListener(delegate
			{
				if (settingsf.IsVisible)
				{
					settingsf.SelfFadeOut();
				}
				if (levelSelectf.IsVisible)
				{
					levelSelectf.SelfFadeOut();
				}
				mainf.SelfFadeIn();
			});
		}
	}
}
