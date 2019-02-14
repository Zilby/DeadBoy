#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System;
using System.IO;

public class DialogueNodeEditor : EditorWindow
{
	public DialogueTree tree;

	public static Action<ConnectionPoint> ClickInPointEvent;

	public static Action<ConnectionPoint> ClickOutPointEvent;

	public static Action<DialogueNode> RemoveNodeEvent;

	public static Action<Connection> RemoveConnectionEvent;

	private ConnectionPoint selectedInPoint;
	private ConnectionPoint selectedOutPoint;

	private Vector2 offset;
	private Vector2 drag;

	private bool needsConnectionFuse = false;

	private const float kZoomMin = 0.2f;
	private const float kZoomMax = 3.0f;

	private static DialogueNodeEditor instance;

	private int scene;
	
	public static Rect _zoomArea
	{
		get
		{
			return new Rect(0.0f, 0.0f, instance.position.width, instance.position.height);
		}
	}

	public static float _zoom = 1.0f;
	public static Vector2 _zoomCoordsOrigin = Vector2.zero;

	public static Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
	{
		return (screenCoords - _zoomArea.TopLeft()) / _zoom + _zoomCoordsOrigin;
	}

	public static Vector2 ConvertScreenCoordsToNodeCoords(Vector2 screenCoords)
	{
		return (screenCoords - _zoomArea.TopLeft()) / _zoom;
	}


	[MenuItem("Window/Dialogue Editor")]
	private static void OpenWindow()
	{
		DialogueNodeEditor window = GetWindow<DialogueNodeEditor>();
		window.titleContent = new GUIContent("Dialogue Editor");
		window.wantsMouseMove = true;
	}

	private void OnEnable()
	{
		instance = this;
		tree = new DialogueTree();
		ClickInPointEvent = OnClickInPoint;
		ClickOutPointEvent = OnClickOutPoint;
		RemoveNodeEvent = OnClickRemoveNode;
		RemoveConnectionEvent = OnClickRemoveConnection;
	}

	private void OnGUI()
	{
		// Within the zoom area all coordinates are relative to the top left corner of the zoom area
		// with the width and height being scaled versions of the original/unzoomed area's width and height.
		EditorZoomArea.Begin(_zoom, _zoomArea);

		DrawGrid(20, 0.2f, Color.gray);
		DrawGrid(100, 0.4f, Color.gray);
		DrawNodes();
		DrawConnections();

		EditorZoomArea.End();

		DrawConnectionLine(Event.current);

		DrawControls();

		ProcessNodeEvents(Event.current);
		ProcessEvents(Event.current);

		if (GUI.changed) Repaint();
	}

	private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
	{
		int widthDivs = Mathf.CeilToInt(position.width / (_zoom * gridSpacing));
		int heightDivs = Mathf.CeilToInt(position.height / (_zoom * gridSpacing));

		Handles.BeginGUI();
		Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

		offset += drag * 0.5f / _zoom;
		Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

		for (int i = 0; i < widthDivs; i++)
		{
			Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height / _zoom, 0f) + newOffset);
		}

		for (int j = 0; j < heightDivs; j++)
		{
			Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width / _zoom, gridSpacing * j, 0f) + newOffset);
		}

		Handles.color = Color.white;
		Handles.EndGUI();
	}


	private void DrawControls()
	{
		GUIContent content = new GUIContent("Scene");
		EditorGUI.LabelField(new Rect(5, 5, 40, 20), content);
		DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + tree.GetDirectory());
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		string[] scene_names = new string[fileInfo.Length];
		for (int i = 0; i < fileInfo.Length; ++i)
		{
			scene_names[i] = fileInfo[i].Name.Substring(DialogueTree.PREFIX.Length, fileInfo[i].Name.Length - (DialogueTree.PREFIX.Length + fileInfo[i].Extension.Length));
		}
		int old_scene = scene;
		scene = EditorGUI.Popup(new Rect(45, 5, 120, 15), scene, scene_names);
		if (scene != old_scene) {
			tree.scene = scene_names[scene];
		}
		tree.scene = EditorGUI.TextField(new Rect(5, 25, 160, 15), tree.scene);
		content = new GUIContent("Left Initial Character");
		EditorGUI.LabelField(new Rect(5, 45, 140, 20), content);
		tree.leftCharEnabled = EditorGUI.Toggle(new Rect(150, 45, 20, 20), tree.leftCharEnabled);
		tree.leftChar = (DialogueManager.Character)EditorGUI.EnumPopup(
				new Rect(5, 65, 80, 15), tree.leftChar);
		tree.leftExpr = (DialogueManager.Expression)EditorGUI.EnumPopup(
			new Rect(90, 65, 80, 15), tree.leftExpr);
		content = new GUIContent("Right Initial Character");
		EditorGUI.LabelField(new Rect(5, 85, 140, 20), content);
		tree.rightCharEnabled = EditorGUI.Toggle(new Rect(150, 85, 20, 20), tree.rightCharEnabled);
		tree.rightChar = (DialogueManager.Character)EditorGUI.EnumPopup(
				new Rect(5, 105, 80, 15), tree.rightChar);
		tree.rightExpr = (DialogueManager.Expression)EditorGUI.EnumPopup(
			new Rect(90, 105, 80, 15), tree.rightExpr);
		content = new GUIContent("Warm Tint");
		EditorGUI.LabelField(new Rect(5, 125, 80, 15), content);
		tree.warmTint = EditorGUI.Toggle(new Rect(70, 125, 20, 15), tree.warmTint);
		content = new GUIContent("Cold Tint");
		EditorGUI.LabelField(new Rect(90, 125, 80, 15), content);
		tree.coldTint = EditorGUI.Toggle(new Rect(155, 125, 20, 15), tree.coldTint);
		content = new GUIContent("Load Dialogue");
		if (GUI.Button(new Rect(5, 145, 160, 20), content))
		{
			DialogueTree d = DialogueWriter.LoadTree(Path.Combine(tree.GetDirectory(), tree.GetFileName()));
			if (d != null)
			{
				tree.Nodes = d.Nodes;
				tree.Connections = d.Connections;
				needsConnectionFuse = true;
				tree.warmTint = d.warmTint;
				tree.coldTint = d.coldTint;
				tree.leftCharEnabled = d.leftCharEnabled;
				tree.rightCharEnabled = d.rightCharEnabled;
				tree.leftChar = d.leftChar;
				tree.rightChar = d.rightChar;
				tree.leftExpr = d.leftExpr;
				tree.rightExpr = d.rightExpr;
			}
		}
		content = new GUIContent("Save Dialogue");
		if (GUI.Button(new Rect(5, 170, 160, 20), content))
		{
			DialogueWriter.WriteTree(tree, tree.GetFileName(), tree.GetDirectory());
		}
		if (needsConnectionFuse)
		{
			FuseConnections();
		}
	}


	private void FuseConnections()
	{
		foreach (DialogueNode n in tree.Nodes)
		{
			if (!n.initialized || !n.inPoint.initialized || !n.outPoint.initialized)
			{
				return;
			}
		}
		foreach (Connection c in tree.Connections)
		{
			if (!c.initialized || !c.inPoint.initialized || !c.outPoint.initialized)
			{
				return;
			}
		}
		foreach (DialogueNode n in tree.Nodes)
		{
			foreach (Connection c in tree.Connections)
			{
				if (n.inPoint.id == c.inPoint.id)
				{
					c.inPoint = n.inPoint;
				}
			}
			foreach (Connection c in tree.Connections)
			{
				if (n.outPoint.id == c.outPoint.id)
				{
					c.outPoint = n.outPoint;
				}
			}
		}
		needsConnectionFuse = false;
		GUI.UnfocusWindow();
		Repaint();
	}

	private void DrawNodes()
	{
		if (tree.Nodes != null)
		{
			for (int i = 0; i < tree.Nodes.Count; i++)
			{
				tree.Nodes[i].Draw();
			}
		}
	}

	private void DrawConnections()
	{
		if (tree.Connections != null)
		{
			for (int i = 0; i < tree.Connections.Count; i++)
			{
				tree.Connections[i].Draw();
			}
		}
	}

	private void DrawConnectionLine(Event e)
	{
		Vector2 zoomCoordsMousePos = e.mousePosition;

		if (selectedInPoint != null && selectedOutPoint == null)
		{
			Vector2 start = selectedInPoint.rect.center * _zoom;
			Handles.DrawBezier(
				start,
				zoomCoordsMousePos,
				start + Vector2.down * 50f,
				zoomCoordsMousePos - Vector2.down * 50f,
				Color.white,
				null,
				2f
			);

			GUI.changed = true;
		}

		if (selectedOutPoint != null && selectedInPoint == null)
		{
			Vector2 start = selectedOutPoint.rect.center * _zoom;
			Handles.DrawBezier(
				start,
				zoomCoordsMousePos,
				start - Vector2.down * 50f,
				zoomCoordsMousePos + Vector2.down * 50f,
				Color.white,
				null,
				2f
			);

			GUI.changed = true;
		}
	}

	private void ProcessEvents(Event e)
	{
		drag = Vector2.zero;
		Vector2 screenCoordsMousePos = e.mousePosition;
		Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
		switch (e.type)
		{
			case EventType.MouseDown:
				if (e.button == 1)
				{
					ProcessContextMenu(screenCoordsMousePos);
				}
				break;

			case EventType.MouseDrag:
				if (e.button == 0)
				{
					OnDrag(e.delta);
				}
				break;
			case EventType.ScrollWheel:
				Vector2 delta = e.delta;
				float zoomDelta = -delta.y / 150.0f;
				float oldZoom = _zoom;
				_zoom += zoomDelta;
				_zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
				_zoomCoordsOrigin = zoomCoordsMousePos - (oldZoom / _zoom) * (zoomCoordsMousePos - _zoomCoordsOrigin);
				if (oldZoom != _zoom)
				{
					float change = (oldZoom - _zoom);
					Vector2 dragDelta = (new Vector2(change * position.width, change * position.height)) /* + (new Vector2(position.width, position.height).XYDiv(2) - e.mousePosition) / 100.0f) */ / (_zoom + oldZoom);
					OnDrag(dragDelta);
				}

				Event.current.Use();
				break;
		}
	}

	private void ProcessNodeEvents(Event e)
	{
		if (tree.Nodes != null)
		{
			for (int i = tree.Nodes.Count - 1; i >= 0; i--)
			{
				bool guiChanged = tree.Nodes[i].ProcessEvents(e);

				if (guiChanged)
				{
					GUI.changed = true;
				}
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		drag = delta;
		delta /= _zoom;
		_zoomCoordsOrigin += delta;

		if (tree.Nodes != null)
		{
			for (int i = 0; i < tree.Nodes.Count; i++)
			{
				tree.Nodes[i].Drag(delta);
			}
		}

		GUI.changed = true;
	}

	private void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
		genericMenu.ShowAsContext();
	}

	private void OnClickAddNode(Vector2 mousePosition)
	{
		if (tree.Nodes == null)
		{
			tree.Nodes = new List<DialogueNode>();
		}

		tree.Nodes.Add(new DialogueNode(ConvertScreenCoordsToNodeCoords(mousePosition), 200, 140));
	}

	private void OnClickInPoint(ConnectionPoint inPoint)
	{
		selectedInPoint = inPoint;

		if (selectedOutPoint != null)
		{
			if (selectedOutPoint.nodeRect != selectedInPoint.nodeRect)
			{
				CreateConnection();

			}
			ClearConnectionSelection();
		}
	}

	private void OnClickOutPoint(ConnectionPoint outPoint)
	{
		selectedOutPoint = outPoint;

		if (selectedInPoint != null)
		{
			if (selectedOutPoint.nodeRect != selectedInPoint.nodeRect)
			{
				CreateConnection();
			}
			ClearConnectionSelection();
		}
	}

	private void OnClickRemoveConnection(Connection connection)
	{
		tree.Connections.Remove(connection);
	}

	private void CreateConnection()
	{
		if (tree.Connections == null)
		{
			tree.Connections = new List<Connection>();
		}

		tree.Connections.Add(new Connection(selectedInPoint, selectedOutPoint));
	}

	private void ClearConnectionSelection()
	{
		selectedInPoint = null;
		selectedOutPoint = null;
	}

	private void OnClickRemoveNode(DialogueNode node)
	{
		if (tree.Connections != null)
		{
			List<Connection> connectionsToRemove = new List<Connection>();

			for (int i = 0; i < tree.Connections.Count; i++)
			{
				if (tree.Connections[i].inPoint == node.inPoint || tree.Connections[i].outPoint == node.outPoint)
				{
					connectionsToRemove.Add(tree.Connections[i]);
				}
			}

			for (int i = 0; i < connectionsToRemove.Count; i++)
			{
				tree.Connections.Remove(connectionsToRemove[i]);
			}

			connectionsToRemove = null;
		}

		tree.Nodes.Remove(node);
	}
}
#endif