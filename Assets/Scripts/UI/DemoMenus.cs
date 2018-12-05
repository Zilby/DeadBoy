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

	private void Start()
	{
		if (SongManager.instance != null) 
		{
			SongManager.instance.PlaySong(song);
		}
		
		if (play != null) {
			play.onClick.AddListener(delegate { Fader.SceneEvent("Desert1"); });
		}
		if (menu != null) {
			menu.onClick.AddListener(delegate { Fader.SceneEvent("DemoStart"); });
		}
		if (quit != null) {
			quit.onClick.AddListener(delegate { StartCoroutine(Fader.Quit()); } );
		}
	}
}
