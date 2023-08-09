using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScreen : UIScreen
{
	public void Button_Reset()
	{
		GameManager.Instance.RestartGame();
	}
}
