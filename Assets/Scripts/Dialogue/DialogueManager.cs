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
public class DialogueManager : MonoBehaviour
{
	/// <summary>
	/// The expressions assignable to each character.
	/// </summary>
	public enum Expression
	{
		happy = 0,
		neutral = 1,
		sad = 2,
		anxious = 3,
		excited = 4,
		angry = 5,
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

	/// <summary>
	/// The overlay backdrop for the text
	/// </summary>
	public FadeableUI textOverlay;

	/// <summary>
	/// The moving dialogue text for each character
	/// </summary>
	public List<MoveableText> characterTexts;


	/// <summary>
	/// The dialogue parser for parsing text files. 
	/// </summary>
	private DialogueParser dParser = new DialogueParser();

	// Use this for initialization
	void Awake()
	{
		textOverlay.Hide();
	}


	/// <summary>
	/// Begins a dialogue stream. 
	/// </summary>
	/// <param name="i">The scene index for dialogue.</param>
	/// <param name="c">The number of conversations left for this character.</param>
	/// <param name="b">Whether or not there is available dialogue.</param>
	private void BeginText(int i, bool b, Character ch, bool trust = true)
	{
		ClearTexts();
		textOverlay.SelfFadeIn();
	}


	/// <summary>
	/// Ends a dialogue scene. 
	/// </summary>
	private void FinishText()
	{
		textOverlay.SelfFadeOut();
	}


	/// <summary>
	/// Waits for the given key to be pressed.
	/// </summary>
	/// <param name="k">The key to be pressed.</param>
	public static IEnumerator WaitForKeypress(KeyCode k)
	{
		while (!Input.GetKeyDown(k))
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
		ClearTexts();
		yield return characterTexts[c].TypeText(s);
		yield return new WaitForSecondsRealtime(0.1f);
		yield return WaitForKeypress(KeyCode.Space);
	}


	/// <summary>
	/// Clears the dialogue texts.
	/// </summary>
	private void ClearTexts()
	{
		foreach (MoveableText t in characterTexts)
		{
			t.ClearText();
		}
	}
}