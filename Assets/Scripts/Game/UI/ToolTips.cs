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
	/// Called when setting a tooltip active. 
	/// </summary>
	public static Func<string, Vector3, int> TooltipActiveEvent;
	/// <summary>
	/// Called when the tooltip is currently displayed. 
	/// </summary>
	public static Action<int, Vector3> TooltipStayEvent;
	/// <summary>
	/// Called when the tooltip text needs changing. 
	/// </summary>
	public static Action<int, string> TooltipTextEvent;
	/// <summary>
	/// Called when setting a tooltip inactive. 
	/// </summary>
	public static Action<int> TooltipInactiveEvent;

	/// <summary>
	/// The individual tooltips. 
	/// </summary>
	private FadeableUI[] tips;

	private void Awake()
	{
		tips = GetComponentsInChildren<FadeableUI>();
		TooltipActiveEvent = SetTooltipActive;
		TooltipTextEvent = SetTooltipString;
		TooltipStayEvent = SetTooltipPosition;
		TooltipInactiveEvent = SetTooltipInactive;
	}

	/// <summary>
	/// Sets the tooltip active.
	/// </summary>
	/// <returns>The index of the active tooltip.</returns>
	/// <param name="s">String of the tooltip.</param>
	/// <param name="location">Location of the tooltip in worldspace.</param>
	private int SetTooltipActive(string s, Vector3 location)
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
	private void SetTooltipString(int i, string s) 
	{
		tips[i].GetComponentInChildren<TextMeshProUGUI>().text = s;
	}

	/// <summary>
	/// Sets the tooltip's position.
	/// </summary>
	/// <param name="i">The index of the tooltip.</param>
	/// <param name="location">The location to set the position to in world space.</param>
	private void SetTooltipPosition(int i, Vector3 location)
	{
		tips[i].transform.position = location;
	}

	/// <summary>
	/// Sets the given tooltip inactive.
	/// </summary>
	/// <param name="i">The index of the tooltip.</param>
	private void SetTooltipInactive(int i)
	{
		StartCoroutine(tips[i].FadeOut(dur: 0.25f));
	}
}
