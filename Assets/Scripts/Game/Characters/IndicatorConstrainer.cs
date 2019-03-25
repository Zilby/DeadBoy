using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorConstrainer : MonoBehaviour
{
	private float defaultY;

	private const float RANGE = 4.5f;

	private void Awake()
	{
		defaultY = transform.localPosition.y;
	}

	private void Update()
	{
		SetPosition();
	}

	private void FixedUpdate()
	{
		SetPosition();
	}

	private void SetPosition()
	{
		if (transform.position.y > Camera.main.transform.position.y + RANGE ||
			transform.position.y < Camera.main.transform.position.y - RANGE)
		{
			transform.position = transform.position.Y(Mathf.Clamp(transform.position.y,
				Camera.main.transform.position.y - RANGE, Camera.main.transform.position.y + RANGE));
		}
		else
		{
			transform.localPosition = transform.localPosition.Y(defaultY);
		}
	}
}
