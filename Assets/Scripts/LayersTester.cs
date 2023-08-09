using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersTester : MonoBehaviour
{
    void Start()
    {
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag(Tags.WEOWOAS))
		{
			Debug.LogError("HIT");
		}
	}
}






