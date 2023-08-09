using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : UIScreen
{
	public void Button_GameOver()
	{
		GameManager.Instance.GameOver();
	}

	public void Button_LevelComplete()
	{
		GameManager.Instance.LevelComplete();
	}
}
