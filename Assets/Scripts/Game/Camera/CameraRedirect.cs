using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRedirect : MonoBehaviour
{
	public List<Transform> locations;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			StartCoroutine(Redirect());
		}
	}

	IEnumerator Redirect()
	{
		yield return CameraController.RedirectCamera(locations, 0);
		Destroy(gameObject);
	}
}
