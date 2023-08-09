using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class Compression
{
	public static string DecompressStringGZip(string compressedString)
	{
		byte[] inputBytes = Convert.FromBase64String(compressedString);
		MemoryStream inputStream = new MemoryStream(inputBytes);

		GZipStream gZipStream = new GZipStream(inputStream, CompressionMode.Decompress);
		StreamReader reader = new StreamReader(gZipStream);

		string decompressString = reader.ReadToEnd();
		return decompressString;
	}

	public static string CompressStringGZip(string inputString)
	{
		byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
		MemoryStream outputStream = new MemoryStream();

		GZipStream gZipStream = new GZipStream(outputStream, CompressionMode.Compress, true);
		gZipStream.Write(inputBytes, 0, inputBytes.Length);
		gZipStream.Close();

		string compressedString = Convert.ToBase64String(outputStream.ToArray());
		return compressedString;
	}
}
