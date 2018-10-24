using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMenus : MonoBehaviour
{
	public void LoadDemo() {
		Fader.SceneEvent("DemoLevel", 1.5f);
	}

	public void LoadMenu() {
		Fader.SceneEvent("DemoSplash");
	}
}
