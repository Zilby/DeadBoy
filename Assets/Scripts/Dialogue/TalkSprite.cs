﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for talk sprites. 
/// </summary>
public class TalkSprite : FadeableUI
{
	/// <summary>
	/// The warm color.
	/// </summary>
	public Color warmColor;
	/// <summary>
	/// The cold color. 
	/// </summary>
	public Color coldColor;

	/// <summary>
	/// The image.
	/// </summary>
	private Image image;
	/// <summary>
	/// The current talk sprites. 
	/// </summary>
	private Sprite[] sprites;
	/// <summary>
	/// Last character used. 
	/// </summary>
	private DialogueManager.Character? character = null;
	/// <summary>
	/// The talk routine.
	/// </summary>
	private Coroutine talkRoutine;

	public const float PARTIAL_FADE = 0.6f;

	/// <summary>
	/// Last character used. 
	/// </summary>
	public DialogueManager.Character? Character {
		get { return character; }
	}

	private void Awake()
	{
		image = GetComponent<Image>();
		Hide();
	}

	/// <summary>
	/// Displays the talk sprite with the given conditions. 
	/// </summary>
	public void DisplayTalkSprite(DialogueManager.Character c, DialogueManager.Expression e, bool warm = false, bool cold = false)
	{
		sprites = Resources.LoadAll<Sprite>(Path.Combine("DialogueSprites", c.ToString(), e.ToString()));
		if (sprites.Length == 0)
		{
			sprites = Resources.LoadAll<Sprite>(Path.Combine("DialogueSprites", c.ToString(), DialogueManager.Expression.Neutral.ToString()));
		}
		image.sprite = sprites[0];
		if (warm)
		{
			image.color = warmColor;
		}
		else if (cold)
		{
			image.color = coldColor;
		}
		else
		{
			image.color = Color.white;
		}
		character = c;
		talkRoutine = StartCoroutine(Talk());
	}

	/// <summary>
	/// Talks after a delay. 
	/// </summary>
	private IEnumerator Talk(float delay = 0.8f)
	{
		if (sprites.Length > 1)
		{
			for (int i = 0; ; ++i)
			{
				if (i >= sprites.Length)
				{
					i = 0;
				}
				image.sprite = sprites[i];
				yield return new WaitForSeconds(delay);
			}
		}
	}

	/// <summary>
	/// Stops the talking coroutine. 
	/// </summary>
	public void StopTalking()
	{
		if (talkRoutine != null)
		{
			StopCoroutine(talkRoutine);
			talkRoutine = null;
		}
	}

	public void PartialFade() 
	{
		SelfFadeOut(endAlpha: PARTIAL_FADE);
		IsVisible = true;
	}
}
