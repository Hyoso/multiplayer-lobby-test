using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Globalization;
using UnityEngine.Networking;

public class UnityWebRequestChecker : MonoBehaviour
{
	public TMP_InputField input;
	public Text output;
	public TextMeshProUGUI outputTMP;

	void Start()
	{
		input.onEndEdit.AddListener(x => StartCoroutine(GetUniversalTime(x)));

	}

	IEnumerator GetUniversalTime(string url)
	{
		Debug.Log("Starting request");
		using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
		{
			//webRequest.downloadHandler = new DownloadHandlerBuffer();

			// Request and wait for the desired page.
			var response = webRequest.SendWebRequest();
			//var response = webRequest.Send();
			Debug.Log(response.progress);
			yield return response;

			string[] pages = url.Split('/');
			int page = pages.Length - 1;
			//Debug.Log("finished recieving request");

			if (webRequest.isNetworkError)
			{
				//Debug.Log("found error");
				Debug.Log(pages[page] + ": Error: " + webRequest.error);
			}
			else
			{
				//Debug.Log("Found web page");
				//Debug.Log("text: " + webRequest.downloadHandler.text);
				//Debug.Log("data: " + webRequest.downloadHandler.data.ToString());
				output.text = webRequest.downloadHandler.text;

				outputTMP.text = webRequest.downloadHandler.text;
			}

			Debug.Log("Finished request");

			webRequest.Dispose();
		}
	}
}
