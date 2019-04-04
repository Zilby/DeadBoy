using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player2 camera.
/// </summary>
public class Player2Camera : CameraController
{
	public Vector2 oldXrange;
	public CameraController player1;
	public Image divider;

	private Camera cam;

	protected override Transform PlayerTransform {
		get {
			return DBInputManager.controllers.Count > 1 ? 
			                   DBInputManager.players.Keys.FirstOrDefault(p => DBInputManager.players[p] == DBInputManager.controllers[1])?.transform : 
			                   null; }
	}

	protected override void Awake()
	{
		Initialize();
		divider = GameObject.Find("Divider").GetComponent<Image>();
		cam = GetComponent<Camera>();
		Rect r = cam.rect;
		r.x = r.width = 0.5f;
		cam.rect = r;
	}

	protected override void Update()
	{
		bool player2Active = PlayerTransform != null;
		cam.enabled = player2Active;
		divider.enabled = player2Active;

		Rect rMain = Camera.main.rect;

		if (player2Active) {
			base.Update();
			rMain.width = 0.5f;
			player1.xRange = xRange;
		} else {
			transform.position = Camera.main.transform.position;
			rMain.width = 1f;
			player1.xRange = oldXrange;
		}

		Camera.main.rect = rMain;
	}

	protected override void FixedUpdate()
	{
		if (PlayerTransform != null)
		{
			base.FixedUpdate();
		}
	}
}
