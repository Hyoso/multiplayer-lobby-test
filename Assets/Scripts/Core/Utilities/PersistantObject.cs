using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantObject : MonoBehaviour
{
    void Start()
    {
		DontDestroyOnLoad(this.gameObject);
	}
}
