using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class LayersGenerator : MonoBehaviour
{
	[MenuItem("Tools/Generate Layers")]
	public static void GeneratePhysicsLayers()
	{
		string baseClassPath = "Assets/Editor/LayerBase.txt";
		string outputPath = "Assets/Scripts/Misc/Generated/Layers.cs"; 

		StreamReader reader = new StreamReader(baseClassPath);
		StreamWriter writer = new StreamWriter(outputPath, false);

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			writer.WriteLine(line);

			if (line == "// LAYERS")
			{
				string layersString = GetAllLayersString();
				writer.WriteLine(layersString);
			}
		}

		writer.Close();
		reader.Close();

		Debug.Log("Generated layers and saved to : " + outputPath);

		AssetDatabase.ImportAsset(outputPath);
	}

	private static string GetAllLayersString()
	{
		string typePrepend = "public const string ";
		string layersString = "";
		for (int i = 0; i < 32; i++)
		{
			string maskName = LayerMask.LayerToName(i);
			if (!string.IsNullOrEmpty(maskName))
			{
				string maskNameNoWhitespace = Regex.Replace(maskName, @"\s+", ""); // remove white space
				layersString += typePrepend + maskNameNoWhitespace + " = " + "\"" + maskName + "\";\r\n";
			}
		}

		return layersString;
	}
}
