using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : UIScreen
{
	public void Button_Reset()
	{
		GameManager.Instance.RestartGame();
	}
}
