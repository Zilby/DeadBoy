﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [StringInList(typeof(DialogueManager), "GetDialogueList")]
	public string dialogueName;
	public Character triggerer;

	void OnTriggerEnter2D(Collider2D other)
	{
		PlayerController pc = other.gameObject.GetComponent<PlayerController>();
		if (pc != null && pc.CharID == triggerer)
		{
			DialogueManager.instance.StartCoroutine(DialogueManager.instance.BeginDialogue(dialogueName));
			Destroy(gameObject);
		}
	}
}
