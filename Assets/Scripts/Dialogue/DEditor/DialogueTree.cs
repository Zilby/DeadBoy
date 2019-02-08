using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DialogueTree : IWriteable, IReadable
{

	[XmlIgnore]
	public int scene = -1;

	[XmlElement]
	public bool leftCharEnabled;

	[XmlElement]
	public bool rightCharEnabled;

	[XmlElement]
	public DialogueManager.Character leftChar;

	[XmlElement]
	public DialogueManager.Character rightChar;

	[XmlElement]
	public DialogueManager.Expression leftExpr;

	[XmlElement]
	public DialogueManager.Expression rightExpr;

	[XmlElement]
	public bool warmTint = false;

	[XmlElement]
	public bool coldTint = false;


	[XmlArray]
	public List<DialogueNode> Nodes
	{
		get { return nodes; }
		set { nodes = value; }
	}

	private List<DialogueNode> nodes;

	[XmlArray]
	public List<Connection> Connections
	{
		get { return connections; }
		set { connections = value; }
	}

	private List<Connection> connections;

	public string GetDirectory()
	{
		return "Dialogues";
	}

	public string GetFileName()
	{
		return "Dialogue" + scene;
	}
}
