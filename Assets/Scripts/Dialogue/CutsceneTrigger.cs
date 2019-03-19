using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
	public Cutscene cutscene;
	public Character triggerer;

	void OnTriggerEnter2D(Collider2D other)
	{
		PlayerController pc = other.gameObject.GetComponent<PlayerController>();
		if (pc != null && pc.CharID == triggerer)
		{
			DialogueManager.instance.StartCoroutine(cutscene.ExecuteCutscene());
			Destroy(gameObject);
		}
	}
}
