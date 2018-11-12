using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : BaseMenu
{
	public static PauseMenu instance;
    void Awake()
	{
        Debug.Log("awake");
		instance = this;
	}

	private bool paused;
    private float oldTimeScale;
	public void Pause() {
		if (paused) {
			Time.timeScale = oldTimeScale;
			ShowMenu(false);
			paused = false;
		} else {
            oldTimeScale = Time.timeScale;
			Time.timeScale = 0;
			ShowMenu(true);
			paused = true;
		}
	}

    void OnDestroy()
    {
        /// to reset the timescale if the scene changes
        if(paused) {
            Pause();
        }
    }

    /// <summary>
    /// Activate each of the child objects that comprise the menu.
    /// </summary>
    private void ShowMenu(bool show) {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(show);
        }
    }
}
