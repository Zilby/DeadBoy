using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMenus : MonoBehaviour
{
	private void Awake()
	{
		SongManager.instance.PlaySong(SongManager.Songs.Sewers);
	}

	public void LoadDemo() {
		Fader.SceneEvent("DemoLevel", 1.5f);
	}

	public void LoadMenu() {
		Fader.SceneEvent("DemoSplash");
	}
}
