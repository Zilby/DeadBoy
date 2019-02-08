using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueWriter
{
	public static DialogueTree LoadTree(string path)
	{
#if UNITY_EDITOR
		AssetDatabase.Refresh();
#endif
		TextAsset file = Resources.Load<TextAsset>(path);
		if (file == null)
		{
			Debug.LogErrorFormat("There was a problem loading {0}", path);
			return null;
		}
		TextReader reader = new StringReader(file.text);
		DialogueTree d = XMLUtility.ReadXML<DialogueTree>(reader, file.name);
		if (d == null)
		{
			Debug.LogErrorFormat("There was a problem parsing {0}", file.name);
		} else {
			Debug.LogFormat("Successfully Loaded {0}", file.name);
		}
		return d;
	}

#if UNITY_EDITOR
	public static void WriteTree(DialogueTree d, string fileName, string path)
	{
		Debug.LogFormat("Writing out {0}", fileName);
		XMLUtility.WriteXML<DialogueTree>(d, fileName, "Assets/Resources/" + path);
		AssetDatabase.Refresh();
		Debug.LogFormat("Finished writing {0}", fileName);
	}
#endif
}
