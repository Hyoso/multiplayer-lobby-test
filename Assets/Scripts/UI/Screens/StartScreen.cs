using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : UIScreen
{
	public void Button_Start()
	{
		GameManager.Instance.StartGame();
	}
}
