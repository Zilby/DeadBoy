using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
	private void Awake()
	{
		Transform areas = transform.Find("Areas");
		foreach(Transform area in areas)
		{
			foreach(Transform level in area)
			{
				level.GetComponent<Button>().onClick.AddListener(delegate { Fader.SceneEvent(area.name + level.name); });
			}
		}
	}
}
