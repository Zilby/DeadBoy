using System;
using UnityEngine;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[XmlInclude(typeof(ConnectionPoint))]
public class ConnectionPoint
{
	public int id = 0;

	public enum Type
	{
		In,
		Out
	}

	public bool readyToInit = false;
	[XmlIgnore]
	public bool initialized = false;

	public Rect rect;

	[XmlAttribute]
	public Type type;

	public Rect nodeRect;

	private GUIStyle style;

	private Action<ConnectionPoint> OnClickConnectionPoint;

	public ConnectionPoint()
	{
	}

	public ConnectionPoint(DialogueNode node, Type type)
	{
		this.nodeRect = node.rect;
		id = node.id;
		this.type = type;
		readyToInit = true;
	}

#if UNITY_EDITOR

	/// <summary>
	/// Initializes this connection point. 
	/// Late init allows the xml deserializer to load variables.
	/// </summary>
	public void Init()
	{
		style = new GUIStyle();
		style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
		style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
		style.border = new RectOffset(4, 4, 12, 12);
		switch (type)
		{
			case Type.In:
				this.OnClickConnectionPoint = DialogueNodeEditor.ClickInPointEvent;
				break;
			case Type.Out:
			default:
				this.OnClickConnectionPoint = DialogueNodeEditor.ClickOutPointEvent;
				break;
		}
		rect = new Rect(0, 0, 20f, 10f);
		initialized = true;
	}

	public void Draw()
	{
		if (readyToInit && !initialized)
		{
			Init();
		}
		else
		{
			rect.x = nodeRect.x + (nodeRect.width * 0.5f) - rect.width * 0.5f;

			switch (type)
			{
				case Type.In:
					rect.y = nodeRect.y - 5;
					break;

				case Type.Out:
					rect.y = nodeRect.y + nodeRect.height - 7;
					break;
			}

			if (GUI.Button(rect, "", style))
			{
				if (OnClickConnectionPoint != null)
				{
					OnClickConnectionPoint(this);
				}
			}
		}
	}

#endif

}