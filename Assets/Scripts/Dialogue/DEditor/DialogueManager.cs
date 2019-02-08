using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;

/// <summary>
/// Manages the main game dialogue. 
/// </summary>
public class DialogueManager : FadeableUI
{
	/// <summary>
	/// The expressions assignable to each character.
	/// </summary>
	public enum Expression
	{
		Neutral = 0,
		Happy = 1,
		Sad = 2,
		Anxious = 3,
		Excited = 4,
		Angry = 5,
	}


	/// <summary>
	/// The characters that have in-game portraits. 
	/// </summary>
	public enum Character
	{
		Deadboy = 0,
		DrownedGirl = 1,
		Squish = 2,
		ElectricBaby = 3,
		FireKid = 4,
	}

	public List<Color> textColors;

	public TalkSprite leftSprite;
	public TalkSprite rightSprite;


	/// <summary>
	/// The moving dialogue text.
	/// </summary>
	private MoveableText mText;

	/// <summary>
	/// The dialogue parser for parsing text files. 
	/// </summary>
	private DialogueParser dParser = new DialogueParser();

	// Use this for initialization
	void Awake()
	{
		mText = GetComponentInChildren<MoveableText>();
	}


	/// <summary>
	/// Begins the dialogue scene.
	/// </summary>
	public IEnumerator BeginDialogue(int scene)
	{
		SelfFadeIn();
		yield return dParser.LoadDialogue(Path.Combine("Dialogues", "Dialogue" + scene));
		bool warm = dParser.Tree.warmTint;
		bool cold = dParser.Tree.coldTint;

		DialogueParser.DialogueLine current = dParser.Head;
		bool? wasRight = null;
		bool bottom = false;

		if (dParser.Tree.leftCharEnabled)
		{
			leftSprite.SelfFadeIn();
			leftSprite.DisplayTalkSprite(dParser.Tree.leftChar, dParser.Tree.leftExpr, warm, cold);
		}
		if (dParser.Tree.rightCharEnabled)
		{
			rightSprite.SelfFadeIn();
			rightSprite.DisplayTalkSprite(dParser.Tree.rightChar, dParser.Tree.rightExpr, warm, cold);
		}

		TalkSprite disabled = current.node.rightSide ? leftSprite : rightSprite;
		if (disabled.IsVisible)
		{
			disabled.PartialFade();
		}

		yield return new WaitForSeconds(0.5f);

		while (!bottom)
		{
			TalkSprite t = current.node.rightSide ? rightSprite : leftSprite;
			if (current.node.rightSide != wasRight || t.Character != current.node.character)
			{
				if (t.Character != current.node.character)
				{
					yield return t.FadeOut();
				}
				t.SelfFadeIn(startAlpha: t.Alpha);
			}
			t.DisplayTalkSprite(current.node.character, current.node.expression, warm, cold);
			yield return CharacterDialogue(current.node.character, current.node.dialogue);
			t.StopTalking();
			bottom = current.connections.Count <= 0;
			if (!bottom)
			{
				wasRight = current.node.rightSide;
				DialogueParser.DialogueLine newer = current.connections[0];
				if (newer.node.rightSide != current.node.rightSide)
				{
					t.PartialFade();
				}
				current = newer;
			}
		}
		leftSprite.Hide();
		rightSprite.Hide();
		yield return FadeOut();
	}


	/// <summary>
	/// Waits for the given key to be pressed.
	/// </summary>
	/// <param name="k">The key to be pressed.</param>
	public static IEnumerator WaitForKeypress()
	{
		while (!DBInputManager.GetInput(DBInputManager.controllers[0], PlayerInput.Submit, InputType.Pressed))
		{
			yield return null;
		}
	}


	/// <summary>
	/// Displays the dialogue for a given character. 
	/// </summary>
	/// <param name="c">The character given.</param>
	/// <param name="s">The dialogue string. </param>
	private IEnumerator CharacterDialogue(Character c, string s)
	{
		yield return CharacterDialogue((int)c, s);
	}


	/// <summary>
	/// Displays the dialogue for a given character. 
	/// </summary>
	/// <param name="c">The character index given.</param>
	/// <param name="s">The dialogue string. </param>
	private IEnumerator CharacterDialogue(int c, string s)
	{
		mText.Text.color = textColors[c];
		yield return mText.TypeText(s);
		yield return new WaitForSecondsRealtime(0.1f);
		yield return WaitForKeypress();
	}
}