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
	public static DialogueManager instance;

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
		Shocked = 6,
		Determined = 7,
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
		instance = this;
	}


	/// <summary>
	/// Begins the dialogue scene.
	/// </summary>
	public IEnumerator BeginDialogue(string scene)
	{
		SelfFadeIn();
		yield return StartCoroutine(dParser.LoadDialogue(Path.Combine("Dialogues", "Dialogue" + scene)));
		bool warm = dParser.Tree.warmTint;
		bool cold = dParser.Tree.coldTint;

		DialogueParser.DialogueLine current = dParser.Head;
		bool? wasRight = null;
		bool bottom = false;

		if (dParser.Tree.leftCharEnabled)
		{
			leftSprite.SelfFadeIn();
			leftSprite.DisplayTalkSprite(dParser.Tree.leftChar, dParser.Tree.leftExpr, warm, cold);
			leftSprite.StopTalking();
		}
		if (dParser.Tree.rightCharEnabled)
		{
			rightSprite.SelfFadeIn();
			rightSprite.DisplayTalkSprite(dParser.Tree.rightChar, dParser.Tree.rightExpr, warm, cold);
			rightSprite.StopTalking();
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
		yield return DBInputManager.WaitForKeypress(PlayerInput.Submit);
	}

	/// <summary>
	/// Gets the list of all dialogue scenes. 
	/// </summary>
	public static string[] GetDialogueList()
	{
		DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + DialogueTree.DIRECTORY);
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		string[] scene_names = new string[fileInfo.Length];
		for (int i = 0; i < fileInfo.Length; ++i)
		{
			scene_names[i] = fileInfo[i].Name.Substring(DialogueTree.PREFIX.Length, fileInfo[i].Name.Length - (DialogueTree.PREFIX.Length + fileInfo[i].Extension.Length));
		}
		return scene_names;
	}
}