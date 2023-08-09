using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class TagsGenerator : MonoBehaviour
{
	[MenuItem("Tools/Generate Tags")]
	public static void GenerateTags()
	{
		string baseClassPath = "Assets/Editor/TagsBase.txt";
		string outputPath = "Assets/Scripts/Misc/Generated/Tags.cs";

		StreamReader reader = new StreamReader(baseClassPath);
		StreamWriter writer = new StreamWriter(outputPath, false);

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			writer.WriteLine(line);

			if (line == "// TAGS")
			{
				string tagsString = GetAllTagsString();
				writer.WriteLine(tagsString);
			}
		}

		writer.Close();
		reader.Close();

		Debug.Log("Generated tags and saved to : " + outputPath);

		AssetDatabase.ImportAsset(outputPath);
	}

	private static string GetAllTagsString()
	{
		string typePrepend = "public const string ";
		string tagsString = "";
		string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
		for (int i = 0; i < allTags.Length; i++)
		{
			string tag = allTags[i];
			if (!string.IsNullOrEmpty(tag))
			{
				string tagNoWhitespace = Regex.Replace(tag, @"\s+", ""); // remove white space
				tagsString += typePrepend + tagNoWhitespace + " = " + "\"" + tag + "\";\r\n";
			}
		}

		return tagsString;
	}
}
