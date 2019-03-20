using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [StringInList(typeof(TutorialManager), "TutorialList")]
	public string tutorial;
	public Character triggerer;

	void OnTriggerEnter2D(Collider2D other)
	{
		PlayerController pc = other.gameObject.GetComponent<PlayerController>();
		if (pc != null && pc.CharID == triggerer)
		{
			TutorialManager.instance.RunTutorial(tutorial);
			Destroy(gameObject);
		}
	}
}
