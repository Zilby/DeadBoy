using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class for showing tooltips in-game. 
/// </summary>
public class ToolTips : MonoBehaviour
{
	/// <summary>
	/// The instance of the tooltips class.
	/// </summary>
	public static ToolTips instance;

	/// <summary>
	/// The individual tooltips. 
	/// </summary>
	private FadeableUI[] tips;

	private void Awake()
	{
		instance = this;
		tips = GetComponentsInChildren<FadeableUI>();
	}

	/// <summary>
	/// Sets the tooltip active.
	/// </summary>
	/// <returns>The index of the active tooltip.</returns>
	/// <param name="s">String of the tooltip.</param>
	/// <param name="location">Location of the tooltip in worldspace.</param>
	public int SetTooltipActive(string s, Vector3 location)
	{
		int i = 0;
		while (tips[i].IsVisible)
		{
			++i;
		}
		SetTooltipPosition(i, location);
		SetTooltipString(i, s);
		StartCoroutine(tips[i].FadeIn(dur: 0.1f));
		return i;
	}

	/// <summary>
	/// Sets the tooltip string.
	/// </summary>
	/// <param name="i">The index of the active tooltip.</param>
	/// <param name="s">String of the tooltip.</param>
	public void SetTooltipString(int i, string s) 
	{
		tips[i].GetComponentInChildren<TextMeshProUGUI>().text = s;
	}

	/// <summary>
	/// Sets the tooltip's position.
	/// </summary>
	/// <param name="i">The index of the tooltip.</param>
	/// <param name="location">The location to set the position to in world space.</param>
	public void SetTooltipPosition(int i, Vector3 location)
	{
		tips[i].transform.position = location;
	}

	/// <summary>
	/// Sets the given tooltip inactive.
	/// </summary>
	/// <param name="i">The index of the tooltip.</param>
	public void SetTooltipInactive(int i)
	{
		StartCoroutine(tips[i].FadeOut(dur: 0.25f));
	}
}
