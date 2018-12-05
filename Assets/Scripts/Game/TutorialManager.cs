using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(RunTutorial());
	}

	private IEnumerator RunTutorial() {
		yield return new WaitForSeconds(1.5f);
		yield return DBInputManager.instance.SwapTutorial();
	}
}
