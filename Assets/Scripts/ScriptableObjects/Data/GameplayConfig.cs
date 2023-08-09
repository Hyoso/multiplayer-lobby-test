using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameplayConfig : GenericConfig<GameplayConfig>
{
	public float testFloat;

	// game
	public float gameFinishCooldown = 1f;
	
	// move this to ads config later
	public int minAdFrequency = 3;
	public int maxAdFrequency = 5;

	// ball spawning
	public float ballSpawnRadius = 2f;
	public float ballSpawnFrequency = 0.05f;
}
