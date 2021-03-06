﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DialogueTree : IWriteable, IReadable
{

	[XmlIgnore]
	public string scene = "Test";

	[XmlElement]
	public bool leftCharEnabled;

	[XmlElement]
	public bool rightCharEnabled;

	[XmlElement]
	public Character leftChar;

	[XmlElement]
	public Character rightChar;

	[XmlElement]
	public DialogueManager.Expression leftExpr;

	[XmlElement]
	public DialogueManager.Expression rightExpr;

	[XmlElement]
	public bool warmTint = false;

	[XmlElement]
	public bool coldTint = false;

	public const string DIRECTORY = "Dialogues";

	public const string PREFIX = "Dialogue";


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
		return DIRECTORY;
	}

	public string GetFileName()
	{
		return PREFIX + scene;
	}
}
