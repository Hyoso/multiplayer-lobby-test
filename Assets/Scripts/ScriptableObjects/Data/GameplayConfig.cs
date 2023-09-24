using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameplayConfig : GenericConfig<GameplayConfig>
{
	[Header("Game")]
	public float gameFinishCooldown = 1f;
	
	[Header("Ads")]
	public int minAdFrequency = 3;
    public int maxAdFrequency = 5;

	[Header("Target System")]
	public float retargetCooldown = 3f;
}
