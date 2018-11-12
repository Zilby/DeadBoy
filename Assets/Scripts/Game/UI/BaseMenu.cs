using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{

	public virtual void LoadDemo() {
		Fader.SceneEvent("DemoLevel", 1.5f);
	}

	public virtual void LoadMenu() {
		Fader.SceneEvent("DemoSplash");
	}
}
