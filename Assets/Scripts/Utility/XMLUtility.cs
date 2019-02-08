using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// XMLUtility can only write objects with this interface.
/// </summary>
public interface IWriteable
{
	string GetDirectory();
	string GetFileName();
}


/// <summary>
/// XMLUtility can only read objects with this interface.
/// </summary>
public interface IReadable
{
}


public static class XMLUtility
{
	
	public const string XML_EXTENSION = ".xml";


	public static T ReadXML<T>(string filename, string directory, bool ignoreErrors = false)
		where T : class, IReadable
	{
		if (!Directory.Exists(directory))
		{
			Debug.LogErrorFormat("Directory {0} does not exist.", directory);
			return null;
		}

		// Get the full filepath.
		string path = GetPath(directory, filename);
		return ReadXML<T>(path, ignoreErrors);
	}


	public static T ReadXML<T>(string path, bool ignoreErrors = false)
		where T : class, IReadable
	{
		if (!File.Exists(path))
		{
			Debug.LogErrorFormat("File {0} does not exist.", path);
			return null;
		}

		// Deserialize the file and return the object.

		StreamReader reader = new StreamReader(path, encoding: System.Text.Encoding.UTF8);
		T file = ReadXML<T>(reader, path, ignoreErrors);

		return file;
	}


	public static T ReadXML<T>(TextReader reader, string filePath, bool ignoreErrors = false)
		where T : class, IReadable
	{
		// Deserialize the file and return the object.
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		T o = null;
		try
		{
			o = (T)serializer.Deserialize(reader);
		}
		catch (Exception e)
		{
			if (!ignoreErrors)
			{
				Debug.LogErrorFormat("Failed to Deserialize file {0}, Exception: {1}", filePath, e);
			}
		}
		reader.Close();
		return o;
	}


	public static void WriteXML<T>(T o)
		where T : class, IWriteable
	{
		string directory = o.GetDirectory();
		string filename = o.GetFileName();

		//Get the full filepath.
		string filePath = GetPath(directory, filename);

		try
		{
			// If the directory does not exist yet, create it.
			if (!Directory.Exists(directory))
			{
				Debug.LogFormat("Directory {0} does not exist. Creating now.", directory);
				Directory.CreateDirectory(directory);
			}


			// Do not fill the XML namespace with junk.
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			// Write the file.
			FileStream file = File.Create(filePath);
			using (StreamWriter writer = new StreamWriter(file, System.Text.Encoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(writer, o, ns);
			}
			file.Close();
			Debug.LogFormat("Created file: {0} ", filePath);
		}
		catch (Exception e)
		{
			Debug.LogErrorFormat("Failed to save file: {0}, Exception: {1}", filePath, e);
		}
	}


	public static void WriteXML<T>(T o, string filename, string directory, bool ignoreErrors = false)
		where T : class, IWriteable
	{
		// If the directory does not exist yet, create it.
		if (!Directory.Exists(directory))
		{
			Debug.LogFormat("Directory {0} does not exist. Creating now.", directory);
			Directory.CreateDirectory(directory);
		}

		//Get the full filepath.
		string path = GetPath(directory, filename);

		// Do not fill the XML namespace with junk.
		XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
		ns.Add("", "");

		// Write the file.
		FileStream file = File.Create(path);
		using (StreamWriter writer = new StreamWriter(file, System.Text.Encoding.UTF8))
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(writer, o, ns);
		}

		file.Close();
	}

	/// <summary>
	/// Returns the filepath as a string.
	/// </summary>
	static string GetPath(string directory, string filename)
	{
		string path = Path.Combine(directory, filename);
		// If the filename does not end with .xml, add it.
		if (!filename.EndsWith(XML_EXTENSION))
		{
			path += XML_EXTENSION;
		}
		return path;
	}

	/// <summary>
	/// This function is attempting to make a clone of the given object using the xml serializer. It serializes the
	/// source object to a string. Then deserializes the string and returns the resulting object.
	/// </summary>
	/// <typeparam name="T">The type of object to serialize</typeparam>
	/// <param name="source">The object being cloned</param>
	/// <returns>A fresh clone of the given source</returns>
	public static T Clone<T>(T source)
	{
		if (source == null)
		{
			return source;
		}


		XmlSerializer serializer = new XmlSerializer(typeof(T));
		StringWriter writer = new StringWriter();
		serializer.Serialize(writer, source);
		writer.Close();
		string xmlData = writer.ToString();
		StringReader reader = new StringReader(xmlData);
		T session = (T)serializer.Deserialize(reader);

		return session;
	}
}
