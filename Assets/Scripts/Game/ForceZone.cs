using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For applying force to players. 
/// </summary>
public class ForceZone : MonoBehaviour
{
	public Vector2 forceOverTime;

	public List<string> ignorePlayers;

	private List<PlayerController> players;

	private void Awake()
	{
		players = new List<PlayerController>();
	}

	void OnTriggerEnter2D(Collider2D collision) {
		PlayerController p = collision.gameObject.GetComponent<PlayerController>();
		if (p && !ignorePlayers.Contains(p.Name)) {
			players.Add(p);
		}
	}

	void OnTriggerExit2D(Collider2D collision) {
		PlayerController p = collision.gameObject.GetComponent<PlayerController>();
		if (p && players.Contains(p))
		{
			players.Remove(p);
		}
	}

	private void FixedUpdate()
	{
		foreach(PlayerController p in players) {
			p.rBody.AddForce(forceOverTime);
		}
	}
}
