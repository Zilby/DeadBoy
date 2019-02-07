using System;
#if UNITY_EDITOR
using UnityEditor;
#endif 
using UnityEngine;
using System.Xml.Serialization;

[XmlInclude(typeof(Connection))]
public class Connection
{
	public bool readyToInit = false;
	[XmlIgnore]
	public bool initialized = false;

	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;

	private Action<Connection> OnClickRemoveConnection;

	public Connection()
	{
	}

	public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint)
	{
		this.inPoint = inPoint;
		this.outPoint = outPoint;
		readyToInit = true;
	}

#if UNITY_EDITOR

	public void Init()
	{
		this.OnClickRemoveConnection = DialogueNodeEditor.RemoveConnectionEvent;
		initialized = true;
	}

	public void Draw()
	{
		if (readyToInit && !initialized)
		{
			Init();
			inPoint.Init();
			outPoint.Init();
		}
		else
		{
			Handles.DrawBezier(
				inPoint.rect.center,
				outPoint.rect.center,
				inPoint.rect.center + Vector2.down * 50f,
				outPoint.rect.center - Vector2.down * 50f,
				Color.white,
				null,
				2f
			);

			if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
			{
				if (OnClickRemoveConnection != null)
				{
					OnClickRemoveConnection(this);
				}
			}
		}
	}

#endif
}