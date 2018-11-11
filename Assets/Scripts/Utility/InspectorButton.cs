using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

/// <summary>
/// This attribute can only be applied to fields because its
/// associated PropertyDrawer only operates on fields (either
/// public or tagged with the [SerializeField] attribute) in
/// the target MonoBehaviour.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
	public readonly string MethodName;

	public InspectorButtonAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer
{
	private MethodInfo _eventMethodInfo = null;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 30.0f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
		Rect buttonRect = new Rect(position.x + (position.width - (Screen.width / 1.2f)) * 0.5f, position.y + 5, (Screen.width / 1.2f), position.height - 10);
		if (GUI.Button(buttonRect, label.text))
		{
			Object eventOwner = property.serializedObject.targetObject;
			System.Type eventOwnerType = eventOwner.GetType();
			string eventName = inspectorButtonAttribute.MethodName;

			Undo.RecordObject(eventOwner, eventName);

			if (_eventMethodInfo == null)
				_eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (_eventMethodInfo != null)
				_eventMethodInfo.Invoke(property.serializedObject.targetObject, null);
			else
				Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
		}
	}
}
#endif