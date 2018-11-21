using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoMenus : MonoBehaviour
{
	[StringInList(typeof(SongManager), "GetClipList")]
	public string song;

	public FadeableUI fadeable;

	public Button play;
	public Button menu;
	public Button quit;

	private void Awake()
	{
		if (SongManager.instance != null) 
		{
			SongManager.instance.PlaySong(song);
		}
		
		if (play != null) {
			play.onClick.AddListener(delegate { Fader.SceneEvent("DemoLevel"); });
		}
		if (menu != null) {
			menu.onClick.AddListener(delegate { Fader.SceneEvent("DemoSplash"); });
		}
		if (quit != null) {
			quit.onClick.AddListener(delegate { StartCoroutine(Fader.Quit()); } );
		}
	}
}
