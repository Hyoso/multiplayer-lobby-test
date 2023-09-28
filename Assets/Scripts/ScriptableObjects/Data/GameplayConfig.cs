using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameplayConfig : GenericConfig<GameplayConfig>
{
	[Header("Game")]
	public float gameFinishCooldown = 1f;
	public PlayerStats maxPlayerStats;
	
	[Header("Ads")]
	public int minAdFrequency = 3;
    public int maxAdFrequency = 5;

	[Header("Player Target System")]
	public float retargetCooldown = 3f;

	[Header("Enemy Target System")]
	public float checklayerInRangeCooldown = 0.1f;
}
