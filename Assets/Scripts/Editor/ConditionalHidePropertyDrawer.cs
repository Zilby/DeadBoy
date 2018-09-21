using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;


//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: Alex Zilbersher

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            if (condHAtt.range.min == condHAtt.range.max)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                // Draw the property as a Slider or an IntSlider based on whether it's a float or integer.
                if (property.propertyType == SerializedPropertyType.Float)
                    EditorGUI.Slider(position, property, condHAtt.range.min, condHAtt.range.max, label);
                else if (property.propertyType == SerializedPropertyType.Integer)
                    EditorGUI.IntSlider(position, property, Convert.ToInt32(condHAtt.range.min), Convert.ToInt32(condHAtt.range.max), label);
                else
                    EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
            }
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            //The property is not being drawn
            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
            //return 0.0f;
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = false;

        string conditionPath = GetConditionPath(condHAtt, property);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = CheckPropertyType(sourcePropertyValue);
        }
        else
        {
            object propertyParent = GetParentObject(property);
            enabled = (bool)(propertyParent.GetType().GetProperty(condHAtt.ConditionalSourceField)
                             .GetValue(propertyParent, new object[0]));
        }

        if (condHAtt.Inverse) enabled = !enabled;

        return enabled;
    }

    private bool CheckPropertyType(SerializedProperty sourcePropertyValue)
    {
        switch (sourcePropertyValue.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return sourcePropertyValue.boolValue;
            case SerializedPropertyType.ObjectReference:
                return sourcePropertyValue.objectReferenceValue != null;
            default:
                break;
        }
        Debug.LogError("Data type of the property used for conditional hiding [" + sourcePropertyValue.propertyType + "] is currently not supported");
        return true;
    }

    public static object GetParentObject(SerializedProperty property)
    {
        string[] path = property.propertyPath.Split('.');

        object propertyObject = property.serializedObject.targetObject;
        object propertyParent = null;
        for (int i = 0; i < path.Length; ++i)
        {
            if (path[i] == "Array")
            {
                int index = (int)(path[i + 1][path[i + 1].Length - 2] - '0');
                propertyObject = ((IList)propertyObject)[index];
                ++i;
            }
            else
            {
                propertyParent = propertyObject;
                propertyObject = propertyObject.GetType().GetField(path[i]).GetValue(propertyObject);
            }
        }

        return propertyParent;
    }

    public static string GetConditionPath(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        //Get the full relative property path of the sourcefield so we can have nested hiding
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        if (property.name == "data")
        {
            string[] path = propertyPath.Split('.');

            if (path[path.Length - 2] == "Array")
            {
                conditionPath = propertyPath.Remove(propertyPath.Length - 
                                                    (".Array.data[*]".Length + path[path.Length - 3].Length)) + 
                                                    condHAtt.ConditionalSourceField; //special case for arrays
            }
        }
        return conditionPath;
    }
}
