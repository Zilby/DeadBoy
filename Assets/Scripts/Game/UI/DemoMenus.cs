using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMenus : BaseMenu
{
	private void Awake()
	{
		SongManager.instance.PlaySong(SongManager.Songs.Sewers);
	}

}
