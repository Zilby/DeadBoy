using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Parses dialogue from data files. 
/// </summary>
public class DialogueParser
{
	/// <summary>
	/// A single line of dialogue. 
	/// </summary>
	public struct DialogueLine
	{
		public DialogueNode node;
		public List<DialogueLine> connections;
		public List<DialogueLine> parents;

		public DialogueLine(DialogueNode n)
		{
			node = n;
			connections = new List<DialogueLine>();
			parents = new List<DialogueLine>();
		}
	}

	/// <summary>
	/// The starting line of dialogue for this scene. 
	/// </summary>
	private DialogueLine head;

	/// <summary>
	/// The starting line of dialogue for this scene. 
	/// </summary>
	public DialogueLine Head
	{
		get { return head; }
	}

	/// <summary>
	/// The lines of dialogue for this scene. 
	/// </summary>
	private DialogueLine[] lines;

	/// <summary>
	/// The lines of dialogue for this scene. 
	/// </summary>
	public DialogueLine[] Lines
	{
		get { return lines; }
	}

	/// <summary>
	/// Loads a dialogue scene file into dialogue line structs. 
	/// </summary>
	/// <param name="path">The scene file to be loaded</param>
	public IEnumerator LoadDialogue(string path)
	{
		lines = new DialogueLine[1000];
		DialogueTree tree = DialogueWriter.LoadTree(path);
		yield return new WaitForSecondsRealtime(0.2f);
		foreach (DialogueNode n in tree.Nodes)
		{
			lines[n.id] = new DialogueLine(n);
		}
		foreach (Connection c in tree.Connections)
		{
			DialogueLine inD = lines[c.inPoint.id];
			DialogueLine outD = lines[c.outPoint.id];
			lines[c.outPoint.id].connections.Add(inD);
			lines[c.inPoint.id].parents.Add(outD);
		}
		head = lines[tree.Nodes[0].id];
		while (head.parents.Count > 0)
		{
			head = head.parents[0];
		}
	}
}