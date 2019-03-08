using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [StringInList(typeof(DialogueManager), "GetDialogueList")]
	public string name;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			DialogueManager.instance.StartCoroutine(DialogueManager.instance.BeginDialogue(name));
			Destroy(gameObject);
		}
	}
}
