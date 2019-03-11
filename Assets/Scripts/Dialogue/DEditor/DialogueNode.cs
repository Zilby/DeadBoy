using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Xml.Serialization;

[XmlInclude(typeof(DialogueNode))]
public class DialogueNode
{
	private static int idCounter = 0;
	public int id = 0;
	public bool readyToInit = false;
	[XmlIgnore]
	public bool initialized = false;
	public Rect rect;
	public Rect defaultRect;
	public string title;
	public string dialogue = "";
	public DialogueManager.Character character = DialogueManager.Character.Deadboy;
	public DialogueManager.Expression expression = DialogueManager.Expression.Neutral;
	public bool rightSide = false;
	public bool animated = true;
	public bool isDragged;
	public bool isSelected;

	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;

	private GUIStyle style;
	private GUIStyle defaultNodeStyle;
	private GUIStyle selectedNodeStyle;

	private GUIStyle textfieldStyle;
	private GUIStyle toggleStyle;
	private GUIStyle popupStyle;
	private GUIStyle foldoutStyle;

	private Action<DialogueNode> OnRemoveNode;

	public DialogueNode()
	{
	}

	public DialogueNode(Vector2 position, float width, float height)
	{
		defaultRect = rect = new Rect(position.x, position.y, width, height);
		id = ++idCounter;
		inPoint = new ConnectionPoint(this, ConnectionPoint.Type.In);
		outPoint = new ConnectionPoint(this, ConnectionPoint.Type.Out);
		readyToInit = true;
	}

#if UNITY_EDITOR

	public void Init()
	{
		style = new GUIStyle();
		style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		style.border = new RectOffset(12, 12, 12, 12);

		defaultNodeStyle = style;

		selectedNodeStyle = new GUIStyle();
		selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
		selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

		EditorStyles.textField.wordWrap = false;
		EditorStyles.textField.stretchHeight = false;
		textfieldStyle = new GUIStyle(EditorStyles.textField);
		textfieldStyle.wordWrap = true;
		textfieldStyle.stretchHeight = true;
		textfieldStyle.stretchWidth = false;
		EditorStyles.label.normal.textColor = Color.black;
		EditorStyles.label.richText = true;
		foldoutStyle = new GUIStyle(EditorStyles.foldout);
		foldoutStyle.richText = true;
		OnRemoveNode = DialogueNodeEditor.RemoveNodeEvent;
		idCounter = Mathf.Max(id, idCounter);
		initialized = true;
	}

	public void Drag(Vector2 delta)
	{
		rect.position += delta;
	}

	public void Draw()
	{
		if (readyToInit && !initialized)
		{
			Init();
		}
		else
		{
			GUIContent content = new GUIContent(dialogue);
			float height = textfieldStyle.CalcHeight(content, rect.width - 20);
			rect.height = defaultRect.height + Mathf.Max(height, 40) - 20;
			inPoint.Draw();
			outPoint.Draw();
			GUI.Box(rect, title, style);
			dialogue = EditorGUI.TextField(new Rect(rect.x + 10, rect.y + 85, rect.width - 20, Mathf.Max(height, 40)), dialogue, textfieldStyle);
			character = (DialogueManager.Character)EditorGUI.EnumPopup(
				new Rect(rect.x + 10, rect.y + 25, rect.width - 20, 15), character);
			expression = (DialogueManager.Expression)EditorGUI.EnumPopup(
				new Rect(rect.x + 10, rect.y + 45, rect.width - 20, 15), expression);
			content = new GUIContent("<color=white>Right Side</color>");
			EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 65, 80, 15), content);
			rightSide = EditorGUI.Toggle(new Rect(rect.x + 80, rect.y + 65, 20, 15), rightSide);
			content = new GUIContent("<color=white>Animated</color>");
			EditorGUI.LabelField(new Rect(rect.x + 100, rect.y + 65, 80, 15), content);
			animated = EditorGUI.Toggle(new Rect(rect.x + 165, rect.y + 65, 20, 15), animated);
			inPoint.nodeRect = rect;
			outPoint.nodeRect = rect;
		}
	}

	public bool ProcessEvents(Event e)
	{
		switch (e.type)
		{
			case EventType.MouseDown:
				Vector2 mp = DialogueNodeEditor.ConvertScreenCoordsToNodeCoords(e.mousePosition);
				if (e.button == 0)
				{
					GUI.changed = true;
					isSelected = rect.Contains(mp);
					if (isSelected)
					{
						isDragged = true;
						style = selectedNodeStyle;
					}
					else
					{
						style = defaultNodeStyle;
					}
				}
				if (e.button == 1 && rect.Contains(mp))
				{
					ProcessContextMenu();
					e.Use();
				}
				break;

			case EventType.MouseUp:
				isDragged = false;
				break;

			case EventType.MouseDrag:
				if (e.button == 0 && isDragged)
				{
					Vector2 delta = e.delta / DialogueNodeEditor._zoom;
					Drag(delta);
					e.Use();
					return true;
				}
				break;
		}

		return false;
	}

	private void ProcessContextMenu()
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
		genericMenu.ShowAsContext();
	}

	private void OnClickRemoveNode()
	{
		if (OnRemoveNode != null)
		{
			OnRemoveNode(this);
		}
	}

#endif
}
